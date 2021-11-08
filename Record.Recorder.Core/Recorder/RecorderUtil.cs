using CUETools.Codecs.FLAKE;
using NAudio.CoreAudioApi;
using NAudio.Lame;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Record.Recorder.Core
{
    public delegate void Notify();
    public class RecorderUtil
    {
        public static event Notify ProgressIndeterminateStarted;
        private static HttpClient client = new HttpClient();
        WaveFileWriter writer = null;
        WaveInEvent recordingDevice = null;
        private static readonly string dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RecordRecorder"); //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
        private static readonly string recordingFilePath = Path.Combine(dataFolder, "recording.wav");
        //private static readonly string tempDataFolder = Path.Combine(dataFolder, "temp");
        private static readonly string tempFolder = Path.Combine(dataFolder, "temp");
        private static readonly string tempFile = Path.Combine(tempFolder, "temp");
        private static readonly string tempFilePath = Path.Combine(tempFolder, "temp.wav"); //Path.Combine(dataFolder, "part.wav");
        public string OutputFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); //@"C:\Users\rasheed_abiola\Desktop\NAudio\recorded3.wav";
        public string OutputFolder = "Test2";
        public string fileType = AudioFileType.WAV;
        public int RecordingDeviceNum { get; set; } = 999;

        public RecorderUtil()
        {
            Directory.CreateDirectory(tempFolder);
        }

        async public Task<SortedDictionary<int, string>> GetRecordingDevices()
        {
            var recordingDevices = new SortedDictionary<int, string>();
            await Task.Run(() =>
            {

                for (int n = -1; n < WaveIn.DeviceCount; n++)
                {
                    var caps = WaveIn.GetCapabilities(n);
                    recordingDevices.Add(n, caps.ProductName);
                }

                var enumerator = new MMDeviceEnumerator();
                MMDeviceCollection enumeratedDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

                foreach (var device in recordingDevices.ToList())
                {
                    for (int i = 0; i < enumeratedDevices.Count; i++)
                    {
                        string deviceFullName = enumeratedDevices[i].ToString();
                        if (deviceFullName.Contains(device.Value))
                        {
                            recordingDevices[device.Key] = deviceFullName;
                        }
                    }
                }

                foreach (var device in recordingDevices.ToList())
                {
                    if ("Microsoft Sound Mapper".Equals(device.Value))
                    {
                        recordingDevices.Remove(device.Key);
                        break;
                    }
                }
            });

            return recordingDevices;
        }

        public async Task<KeyValuePair<int, string>> GetRecordingDeviceByName(string deviceName)
        {
            var recordingDevice = new KeyValuePair<int, string>();

            if (deviceName.Length <= 30)
            {
                for (int n = -1; n < WaveIn.DeviceCount; n++)
                {
                    var caps = WaveIn.GetCapabilities(n);
                    if (deviceName.Equals(caps.ProductName))
                    {
                        recordingDevice = new KeyValuePair<int, string>(n, deviceName);
                    }
                }
            }
            else
            {
                SortedDictionary<int, string> recordingDevices = await GetRecordingDevices();
                recordingDevice = recordingDevices.FirstOrDefault(device => device.Value == deviceName);
            }


            return recordingDevice;
        }

        private async Task<int> GetRecordingDeviceNumberByName(string deviceName)
        {
            return (await GetRecordingDeviceByName(deviceName)).Key;
        }

        public async void StartRecording()
        {
            int recordingDeviceNumber = await GetRecordingDeviceNumberByName(Properties.Settings.Default["RecordingDevice"].ToString());
            recordingDevice = new WaveInEvent() { DeviceNumber = recordingDeviceNumber };
            recordingDevice.WaveFormat = new WaveFormat(44100, 2);

            writer = new WaveFileWriter(recordingFilePath, recordingDevice.WaveFormat);

            recordingDevice.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
            };

            recordingDevice.RecordingStopped += (s, a) =>
            {
                writer?.Dispose();
                writer = null;
                recordingDevice?.Dispose();
            };

            recordingDevice.StartRecording();
        }

        public void StopRecording()
        {
            if (recordingDevice != null) recordingDevice.StopRecording();
        }

        public void PlayRecording()
        {
            using (var output = new WaveOutEvent())
            using (var player = new AudioFileReader(recordingFilePath))
            {
                output.Init(player);
                output.Play();
                while (output.PlaybackState == PlaybackState.Playing)
                {
                }
            }
        }

        public async void PlayRecordingDevice()
        {
            int recordingDeviceNumber = await GetRecordingDeviceNumberByName(Properties.Settings.Default["RecordingDevice"].ToString());

            using (recordingDevice = new WaveInEvent() { DeviceNumber = recordingDeviceNumber })
                recordingDevice.WaveFormat = new WaveFormat(44100, 2);
            var waveInProvider = new WaveInProvider(recordingDevice);
            using (var output = new WaveOutEvent())
            {
                output.Init(waveInProvider);
                recordingDevice.StartRecording();
                output.Play();
                while (output.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(50);
                }
            }
        }

        public async Task DetectAndSaveTracks(string filePath)
        {
            Dictionary<string, TimeSpan> trackPositions = new Dictionary<string, TimeSpan>();
            Task<AlbumModel> albumTask = null;
            if (IsInternetConnected() && "TADB".Equals(IoC.Settings.GetSongDetectionType()))
            {
                albumTask = GetAlbumInfoById(IoC.Settings.GetAlbumName());
            }

            using (AudioFileReader reader = new AudioFileReader(filePath))
            {
                trackPositions = reader.GetTrackPositions();
                reader.Dispose();
                GC.Collect();
                /*Console.WriteLine("DONE");
                foreach (KeyValuePair<String, TimeSpan> keyValuePair in trackPositions)
                {
                    Console.WriteLine(keyValuePair);
                }*/
            }
            await ExtractAndSaveTracksAsync(trackPositions, filePath, albumTask);
        }

        private async Task ExtractAndSaveTracksAsync(Dictionary<string, TimeSpan> trackPositions, string recordingPath, Task<AlbumModel> albumTask)
        {
            int trackPositionsAmount = trackPositions.Count / 2;
            var songs = new Dictionary<SongModel, ISampleProvider>();
            var album = new AlbumModel();

            if (albumTask != null)
            {
                album = await albumTask;
            }

            for (int i = 1; i <= trackPositionsAmount; i++)
            {
                trackPositions.TryGetValue("Start: " + i, out TimeSpan start);
                trackPositions.TryGetValue("End: " + i, out TimeSpan end);
                end = TimeSpan.FromMilliseconds(end.TotalMilliseconds + 700);

                var song = new AudioFileReader(recordingPath)
                                                .Skip(start)
                                                .Take(end.Subtract(start));

                var songData = new SongModel()
                {
                    Album = IoC.Settings.GetAlbumName(),
                    Title = $"Track {i}",
                    Track = i,
                };

                if (IsInternetConnected())
                {
                    switch (IoC.Settings.GetSongDetectionType())
                    {
                        case "TADB":
                            SongModel tempSongData;
                            if (album.Tracks.TryGetValue(i, out tempSongData))
                            {
                                songData = tempSongData;
                            }
                            break;

                        case "Shazam":
                            var shazamModel = await GetTrackNameAsync(GetMonoSampleAsBytes(start, recordingPath));
                            if (shazamModel.matches.Length > 0)
                            {
                                songData.Title = shazamModel.track.title;
                                songData.Performers = new[] { shazamModel.track.subtitle };
                            }
                            break;
                    }


                }

                songs.Add(songData, song);
            }

            Directory.CreateDirectory(Path.Combine(IoC.Settings.GetOutputFolderLocation(), album.Title ?? IoC.Settings.GetAlbumName()));

            Task<string> albumThumbLocationTask = DownloadTempImageAsync(album.ThumbUrl);
            
            Parallel.ForEach(songs, song =>
            {
                var songData = song.Key;
                songData.Path = TrySave(songData.Title, songData.Album, song.Value, IoC.Settings.GetFileType());
            });

            /*foreach (var song in songs)
            {
                var songData = song.Key;
                songData.Path = TrySave(songData.Title, songData.Album, song.Value, IoC.Settings.GetFileType());
            }*/

            string albumThumbLocation = await albumThumbLocationTask;

            Parallel.ForEach(songs, song =>
            {
                var songData = song.Key;

                var file = TagLib.File.Create(songData.Path);

                file.Tag.Title = songData.Title;
                file.Tag.Album = songData.Album;// ?? "Music To Be Murdered By"; //IoC.Settings.GetAlbumName();
                file.Tag.Track = (uint)songData.Track;
                file.Tag.Performers = songData.Performers;

                if (!string.IsNullOrEmpty(albumThumbLocation))
                {                    
                    var pic = new TagLib.IPicture[1];
                    pic[0] = new TagLib.Picture(albumThumbLocation);
                    file.Tag.Pictures = pic;
                }

                file.Save();
            });
        }


        private async Task SlowExtractAndSaveTracksAsync(Dictionary<string, TimeSpan> trackPositions, string recordingPath)
        {
            int trackPositionsAmount = trackPositions.Count / 2;



            Parallel.For(1, trackPositionsAmount + 1, i =>
            {
                //for (int i = 1; i <= trackPositionsAmount; i++)
                {
                    trackPositions.TryGetValue("Start: " + i, out TimeSpan start);
                    trackPositions.TryGetValue("End: " + i, out TimeSpan end);
                    end = TimeSpan.FromMilliseconds(end.TotalMilliseconds + 700);

                    var song = new AudioFileReader(recordingPath)
                                                    .Skip(start)
                                                    .Take(end.Subtract(start));

                    if (IsInternetConnected())
                    {
                        var shazamModel = new ShazamModel();//await GetTrackNameAsync(GetMonoSampleAsBytes(start, recordingPath));
                        if (shazamModel.matches.Length > 0)
                        {
                            //string filePath = Path.Combine(IoC.Settings.GetOutputFolderLocation(), IoC.Settings.GetAlbumName(), $"{shazamModel.track.title}{fileType}");


                            string filePath = TrySave(shazamModel.track.title, "", song, AudioFileType.MP3);

                            // setting the metadata
                            var file = TagLib.File.Create(filePath);
                            file.Tag.Title = shazamModel.track.title;
                            file.Tag.Track = (uint)i;
                            file.Tag.Performers = new[] { shazamModel.track.subtitle };
                            file.Save();
                            //file.Tag.Genres = shazamModel.track.genres.primary;
                        }
                        else
                        {
                            TrySave($"Track{i}", "", song, AudioFileType.MP3);
                        }

                    }
                    else
                    {
                        string path = TrySave($"Track{i}", "", song, AudioFileType.MP3);




                        var file = TagLib.File.Create(path);
                        file.Tag.Track = (uint)i;
                        file.Save();

                        /*
                var file1 = TagLib.File.Create(@"C:\Users\rasheed_abiola\Music\Test\Track 1.wav");
                var file2 = TagLib.File.Create(@"C:\Users\rasheed_abiola\Music\Test\Track 2.wav");

                Console.WriteLine(file1.ToString());
                Console.WriteLine(file2.ToString());*/
                        /*file.GetTag(TagLib.TagTypes.RiffInfo).Album = "Testttt";


                        //new TagLib.Riff.InfoTag()
                        Console.WriteLine(file.GetTag(TagLib.TagTypes.RiffInfo).Track);*/


                        /*
                        var file = TagLib.File.Create(path);



                        var tag = (TagLib.Riff.InfoTag)file.GetTag(TagLib.TagTypes.RiffInfo, true); ////// */


                        /*
                        tag.AlbumArtists = new[] { "Eminem" };
                        tag.Performers = songData.Performers;

                        tag.Album = "Test1";
                        tag.Track = (uint)song.Key.Track;
                        tag.Title = song.Key.Title;


                        var movieIdTag = (TagLib.Riff.MovieIdTag)file.GetTag(TagLib.TagTypes.MovieId, true);
                        movieIdTag.Track = (uint)songData.Track;
                        movieIdTag.Album = "Test2";*/

                        /*
                        var coll = new TagLib.ByteVectorCollection();
                        coll.Add(TagLib.ByteVector.FromString("Music To Be Murdered By", 23));

                        tag.SetValue(new TagLib.ByteVector("IPRD"), coll);

                        coll = new TagLib.ByteVectorCollection();
                        coll.Add(TagLib.ByteVector.FromUInt((uint)songData.Track));
                        tag.SetValue(new TagLib.ByteVector("ITRK"), coll);       ///// */

                        //var tag = (TagLib.Id3v2.Tag)file.GetTag(TagLib.TagTypes.Id3v2, true);                    
                        //tag.SetTextFrame("TRCK", i.ToString());

                        /*file.Tag.Title = "Test1";
                        file.Tag.Pictures
                        file.Tag.Performers = new[] { "Test 2" };
                        file.Tag.Artists = new[] { "Test 3" };
                        file.Tag.AlbumArtists = new[] { "Test 4" };
                        file.Save();*/
                    }
                }
            });
        }

        public virtual bool IsInternetConnected()
        {
            return true;
        }


        private string TrySave(string fileName, string albumName, ISampleProvider song, string audioFileType)
        {
            string filePathNoType = Path.Combine(IoC.Settings.GetOutputFolderLocation(), albumName, fileName);
            string filePathWithType = GetValidPath(filePathNoType, audioFileType);

            switch (audioFileType)
            {
                case AudioFileType.WAV:
                    WaveFileWriter.CreateWaveFile16(filePathWithType, song);
                    break;

                case AudioFileType.MP3:
                    SaveAsMp3(filePathWithType, song, fileName);
                    break;

                case AudioFileType.FLAC:
                    SaveAsFlac(filePathWithType, song, fileName);
                    break;
            }

            return filePathWithType;
        }

        private string GetValidPath(string filePathNoType, string fileType)
        {
            string filePathWithType = $"{filePathNoType}{fileType}";

            if (File.Exists(filePathWithType))
            {
                int i = 1;
                while (File.Exists($"{filePathNoType} ({i}){fileType}"))
                {
                    i++;
                }
                filePathWithType = $"{filePathNoType} ({i}){fileType}";
            }

            return filePathWithType;
        }

        private void SaveAsMp3(string fileName, ISampleProvider song, string songName)
        {

            string tempPath = $"{tempFile}{songName}{AudioFileType.WAV}";
            WaveFileWriter.CreateWaveFile16(tempPath, song);

            using (var reader = new AudioFileReader(tempPath))
            using (var writer = new LameMP3FileWriter(fileName, song.WaveFormat, LAMEPreset.MEDIUM_FAST))
                reader.CopyTo(writer);

        }

        private void SaveAsFlac(string fileName, ISampleProvider song, string songName)
        {
            string tempPath = $"{tempFile}{songName}{AudioFileType.WAV}";
            WaveFileWriter.CreateWaveFile16(tempPath, song);

            using (var audioSource = new CUETools.Codecs.WAVReader(tempPath, null))
            using (var flakeWriter = new FlakeWriter(fileName, audioSource.PCM) { CompressionLevel = 0 })
            {
                var bufferVal = Convert.ToInt32(audioSource.PCM.SampleRate * 4);
                var buffer = new CUETools.Codecs.AudioBuffer(audioSource, bufferVal);

                while (audioSource.Read(buffer, -1) != 0)
                {
                    flakeWriter.Write(buffer);
                }
            }

        }

        private void SaveAsFlac2(string fileName, ISampleProvider song, string songName)
        {
            string tempPath = $"{tempFile}{songName}{AudioFileType.WAV}";
            WaveFileWriter.CreateWaveFile16(tempPath, song);

            Stream OutFlacStream = new MemoryStream();
            CUETools.Codecs.IAudioSource audioSource = new CUETools.Codecs.WAVReader(tempPath, null);

            CUETools.Codecs.AudioBuffer buff = new CUETools.Codecs.AudioBuffer(audioSource, 0x10000);
            FlakeWriter flakeWriter = new FlakeWriter(fileName, OutFlacStream, audioSource.PCM);
            flakeWriter.CompressionLevel = 8;
            while (audioSource.Read(buff, -1) != 0)
            {
                flakeWriter.Write(buff);
            }
        }

        private static byte[] GetMonoSampleAsBytes(TimeSpan start, string recordingPath)
        {
            Directory.CreateDirectory(dataFolder);
            Directory.CreateDirectory(Path.Combine(dataFolder, "parts"));

            start = TimeSpan.FromMilliseconds(start.TotalMilliseconds + 10000);
            TimeSpan end = TimeSpan.FromMilliseconds(start.TotalMilliseconds + 5000);

            var trimmed = new AudioFileReader(recordingPath)
                                            .Skip(start)
                                            .Take(end.Subtract(start));

            var mono = new StereoToMonoSampleProvider(trimmed);
            WaveFileWriter.CreateWaveFile16(tempFilePath, mono); //$@"C:\Users\rasheed_abiola\Desktop\NAudio\parts\part.wav", mono);
            return File.ReadAllBytes(tempFilePath);
        }

        private static async Task<ShazamModel> GetTrackNameAsync(byte[] bytes)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://shazam.p.rapidapi.com/songs/detect"),
                Headers =
            {
                { "x-rapidapi-key", "80605243fcmsh7987f9cd1918fa5p141b05jsn0085912743db" },
                { "x-rapidapi-host", "shazam.p.rapidapi.com" },
            },
                Content = new StringContent(Convert.ToBase64String(bytes))
                {
                    Headers =
                {
                    ContentType = new MediaTypeHeaderValue("text/plain")
                }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var shazamModel = JsonConvert.DeserializeObject<ShazamModel>(body);
                return shazamModel;
            }
        }

        private static async Task<AlbumModel> GetAlbumInfoById(string id)
        {
            var tadbAlbumTracksTask = GetAlbumTracksObjectById(id);
            var tadbAlbumInfoModel = await GetAlbumInfoObjectById(id);
            AlbumModel album;

            if (tadbAlbumInfoModel != null) {
                album = new AlbumModel()
                {
                    Title = tadbAlbumInfoModel.album[0].strAlbum,
                    Year = tadbAlbumInfoModel.album[0].intYearReleased,
                    Genre = tadbAlbumInfoModel.album[0].strGenre,
                    ThumbUrl = tadbAlbumInfoModel.album[0].strAlbumThumb
                };
            } else { album = new AlbumModel(); }

            var tadbAlbumTracksModel = await tadbAlbumTracksTask;

            if (tadbAlbumTracksModel == null) { return album; }

            foreach (var track in tadbAlbumTracksModel.track)
            {
                var trackNum = Convert.ToInt32(track.intTrackNumber);

                album.Tracks.Add(trackNum, new SongModel
                {
                    Title = track.strTrack,
                    Album = track.strAlbum,
                    Track = trackNum,
                    Performers = new[] { track.strArtist }
                });
            }

            return album;
        }

        private static async Task<TADBAlbumTracksModel> GetAlbumTracksObjectById(string id)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://theaudiodb.p.rapidapi.com/track.php?m={id}"),
                Headers =
                {
                    { "x-rapidapi-host", "theaudiodb.p.rapidapi.com" },
                    { "x-rapidapi-key", "80605243fcmsh7987f9cd1918fa5p141b05jsn0085912743db" },
                },
            };
            try
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TADBAlbumTracksModel>(body);
                }
            } catch
            {
                return null;
            }
        }

        private static async Task<TADBAlbumInfoModel> GetAlbumInfoObjectById(string id)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://theaudiodb.p.rapidapi.com/album.php?m={id}"),
                Headers =
                {
                    { "x-rapidapi-host", "theaudiodb.p.rapidapi.com" },
                    { "x-rapidapi-key", "80605243fcmsh7987f9cd1918fa5p141b05jsn0085912743db" },
                },
            };
            try
            {
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TADBAlbumInfoModel>(body);
                }
            } catch
            {
                return null;
            }
        }

        private async Task<string> DownloadTempImageAsync(string url)
        {
            string fileExtension = Path.GetExtension(@url);
            string fileLocation = Path.Combine(tempFolder, $"albumcover{fileExtension}");

            try
            {
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(url), fileLocation);
                }
            } catch
            {
                return "";
            }
            
            return fileLocation;           
        }

    }
}
