using CUETools.Codecs.FLAKE;
using NAudio.CoreAudioApi;
using NAudio.Lame;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Newtonsoft.Json;
using Record.Recorder.Type;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Record.Recorder.Core.IoC;

namespace Record.Recorder.Core
{
    public delegate void Notify();
    public class RecorderUtil
    {
        private static readonly HttpClient client = new HttpClient();
        WaveFileWriter writer = null;
        WaveInEvent recordingDevice = null;

        private static readonly string dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetCallingAssembly().GetName().Name); //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
        private static readonly string recordingFilePath = Path.Combine(dataFolder, "recording.wav");
        private static readonly string tempDataFolder = Path.Combine(dataFolder, "temp");
        private static readonly string tempFile = Path.Combine(tempDataFolder, "temp");
        private static readonly string tempFilePath = Path.Combine(tempDataFolder, "temp.wav"); //Path.Combine(dataFolder, "part.wav");

        public RecorderUtil()
        {
            Directory.CreateDirectory(tempDataFolder);
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
            int recordingDeviceNumber = await GetRecordingDeviceNumberByName(Settings.RecordingDeviceName);
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
            int recordingDeviceNumber = await GetRecordingDeviceNumberByName(Settings.RecordingDeviceName);

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

        public async Task DetectAndSaveTracksAsync(string recordingPath)
        {
            var trackPositions = new TrackPositionCollection();
            Task<AlbumData> albumTask = null;
            if (IsInternetConnected() && SongDetectionType.TADB.Equals(Settings.SongDetectionType))
            {
                albumTask = GetAlbumInfoById(Settings.AlbumName);
            }

            using (AudioFileReader reader = new AudioFileReader(recordingPath))
            {
                trackPositions = reader.GetTrackPositions(.83);
            }
            GC.Collect();

            var trackDataCollection = await ExtractTrackDataAsync(trackPositions, recordingPath, albumTask);
            await SaveTracksAsync(trackDataCollection);
        }

        private async Task<TrackDataCollection> ExtractTrackDataAsync(TrackPositionCollection trackPositions, string recordingPath, Task<AlbumData> albumTask)
        {
            double weight = .05;
            IoC.SavingProgressVM.Message = "Song data is being found...";
            var trackDataCollection = new TrackDataCollection { Album = new AlbumData { Title = Settings.AlbumName } };

            if (albumTask != null)
            {
                trackDataCollection.Album = await albumTask;
            }

            foreach (var trackPosition in trackPositions)
            {
                var end = TimeSpan.FromMilliseconds(trackPosition.End.TotalMilliseconds + 700);

                var song = new AudioFileReader(recordingPath)
                                                .Skip(trackPosition.Start)
                                                .Take(end.Subtract(trackPosition.Start));

                var trackData = new TrackData()
                {
                    Title = $"Track {trackPosition.Number}",
                    Track = trackPosition.Number,
                    Data = song
                };

                if (IsInternetConnected())
                {
                    switch (Settings.SongDetectionType)
                    {
                        case SongDetectionType.TADB:
                            TrackData tempSongData;
                            if (trackDataCollection.Album.Tracks.TryGetValue(trackPosition.Number, out tempSongData))
                            {
                                trackData = tempSongData;
                                trackData.Data = song;
                            }
                            break;

                        case SongDetectionType.SPOTIFY:
                            var shazamModel = await GetTrackNameAsync(GetMonoSampleAsBytes(trackPosition.Start, recordingPath));
                            if (shazamModel.matches.Length > 0)
                            {
                                trackData.Title = shazamModel.track.title;
                                trackData.Performers = new[] { shazamModel.track.subtitle };
                            }
                            break;
                    }


                }

                trackDataCollection.Add(trackData);
                IoC.SavingProgressVM.BGWorker.ReportProgress(trackPositions.Count, weight);
            }

            return trackDataCollection;
        }

        private async Task SaveTracksAsync(TrackDataCollection trackDataCollection)
        {
            Directory.CreateDirectory(Path.Combine(Settings.OutputFolderLocation, trackDataCollection.Album.Title ?? Settings.AlbumName));

            Task<string> albumThumbLocationTask = DownloadTempImageAsync(trackDataCollection.Album.ThumbUrl ?? "");

            IoC.SavingProgressVM.Message = "Songs are being saved...";
            Parallel.ForEach(trackDataCollection, trackData =>
            {
                trackData.Path = TrySave(trackData.Title, trackDataCollection.Album.Title, trackData.Data, AudioFileType.MP3);//Settings.SaveFileType);
            });
            GC.Collect();
            IoC.SavingProgressVM.BGWorker.ReportProgress(1, .2);

            string albumThumbLocation = await albumThumbLocationTask;

            Parallel.ForEach(trackDataCollection, trackData =>
            {
                var file = TagLib.File.Create(trackData.Path);

                file.Tag.Title = trackData.Title;
                file.Tag.Album = trackDataCollection.Album.Title;
                file.Tag.Track = (uint)trackData.Track;
                file.Tag.Performers = trackData.Performers;

                if (!string.IsNullOrEmpty(albumThumbLocation))
                {
                    var pic = new TagLib.IPicture[1];
                    pic[0] = new TagLib.Picture(albumThumbLocation);
                    file.Tag.Pictures = pic;
                }

                file.Save();
            });
            IoC.SavingProgressVM.BGWorker.ReportProgress(1, .05);
        }

        public virtual bool IsInternetConnected()
        {
            return true;
        }

        private string TrySave(string fileName, string albumName, ISampleProvider song, string audioFileType)
        {
            string filePathNoType = Path.Combine(Settings.OutputFolderLocation, albumName, fileName);
            string filePathWithType = GetValidPath(filePathNoType, audioFileType);

            switch (audioFileType)
            {
                case AudioFileType.WAV:
                    WaveFileWriter.CreateWaveFile16(filePathWithType, song);
                    break;

                case AudioFileType.MP3:
                    SaveAsMp3(filePathWithType, song);
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

        private void SaveAsMp3(string fileName, ISampleProvider song)
        {
            using (var writer = new LameMP3FileWriter(fileName, song.WaveFormat, LAMEPreset.MEDIUM_FAST))
            using (var wavStream = new MemoryStream())
            {
                WaveFileWriter.WriteWavFileToStream(wavStream, new SampleToWaveProvider(song));
                wavStream.CopyTo(writer);
            }
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

        private static async Task<AlbumData> GetAlbumInfoById(string id)
        {
            var pattern = new Regex("[<>:\"/\\|?*]");
            var tadbAlbumTracksTask = GetAlbumTracksObjectById(id);
            var tadbAlbumInfoModel = await GetAlbumInfoObjectById(id);
            AlbumData album;

            if (tadbAlbumInfoModel != null)
            {
                album = new AlbumData()
                {
                    Title = tadbAlbumInfoModel.album[0].strAlbum,
                    Year = tadbAlbumInfoModel.album[0].intYearReleased,
                    Genre = tadbAlbumInfoModel.album[0].strGenre,
                    ThumbUrl = tadbAlbumInfoModel.album[0].strAlbumThumb
                };
            }
            else { album = new AlbumData(); }

            var tadbAlbumTracksModel = await tadbAlbumTracksTask;

            if (tadbAlbumTracksModel == null) { return album; }

            foreach (var track in tadbAlbumTracksModel.track)
            {
                var trackNum = Convert.ToInt32(track.intTrackNumber);

                album.Tracks.Add(trackNum, new TrackData
                {
                    Title = pattern.Replace(track.strTrack, ""),
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
            }
            catch
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
            }
            catch
            {
                return null;
            }
        }

        private async Task<string> DownloadTempImageAsync(string url)
        {
            string fileExtension = Path.GetExtension(@url);
            string fileLocation = Path.Combine(tempDataFolder, $"albumcover{fileExtension}");

            try
            {
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(url), fileLocation);
                }
            }
            catch
            {
                return "";
            }

            return fileLocation;
        }

    }
}
