using CUETools.Codecs.FLAKE;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using NAudio.CoreAudioApi;
using NAudio.Lame;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using Record.Recorder.Type;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

    public class RecorderUtil
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly MusicBrainzClient brainzClient = new MusicBrainzClient();
        WaveFileWriter writer = null;
        WaveInEvent recordingDevice = null;
        readonly Logger log = LogManager.GetLogger("fileLogger");

        private readonly string dataFolder;
        private readonly string recordingFilePath;
        private readonly string tempDataFolder;
        private readonly string tempFile;
        private readonly string tempFilePath;

        readonly Stopwatch stopwatch = new Stopwatch();

        public RecorderUtil() : this(Assembly.GetEntryAssembly()?.GetName().Name ?? "Vinyl Recorder")
        {                                                                                                    
        }

        /// <summary>
        /// Takes a project name to use for the apps data folder
        /// </summary>
        /// <param name="projectName"></param>
        public RecorderUtil(string projectName)
        {
            dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), projectName);
            recordingFilePath = Path.Combine(dataFolder, "recording.wav");
            tempDataFolder = Path.Combine(dataFolder, "temp");
            tempFile = Path.Combine(tempDataFolder, "temp");
            tempFilePath = Path.Combine(tempDataFolder, "temp.wav"); //Path.Combine(dataFolder, "part.wav");

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

            recordingDevice.DataAvailable += OnDataAvailable;

            recordingDevice.RecordingStopped += (s, a) =>
            {
                writer?.Dispose();
                writer = null;
                recordingDevice?.Dispose();
            };

            recordingDevice.StartRecording();
        }

        public void PauseRecording()
        {
            if (recordingDevice != null) recordingDevice.DataAvailable -= OnDataAvailable;
        }

        public void ResumeRecording()
        {
            var silenceBuffer = new SilenceProvider(recordingDevice.WaveFormat)
                                .ToSampleProvider()
                                .Take(TimeSpan.FromMilliseconds(RecorderConfig.SilenceMinDuration * 2));
            using (var wavStream = new MemoryStream())
            {
                WaveFileWriter.WriteWavFileToStream(wavStream, new SampleToWaveProvider(silenceBuffer));
                var buffer = new byte[wavStream.Length];
                wavStream.Position = 0;
                wavStream.Read(buffer, 0, (int)wavStream.Length);
                writer.Write(buffer, 0, buffer.Length);
            }          

            if (recordingDevice != null) recordingDevice.DataAvailable += OnDataAvailable;
        }

        public void StopRecording()
        {
            if (recordingDevice != null) recordingDevice.StopRecording();
        }
        private void OnDataAvailable(object sender, WaveInEventArgs a)
        {
            writer.Write(a.Buffer, 0, a.BytesRecorded);
        }

        public async void PlayRecordingDevice()
        {
            int recordingDeviceNumber = await GetRecordingDeviceNumberByName(Settings.RecordingDeviceName);

            try
            {
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
            } catch (Exception ex)
            {
                
                log.Error(ex);                         
            }
        }

        public bool DeviceCanBePlayed(int deviceNumber)
        {
            using (recordingDevice = new WaveInEvent() { DeviceNumber = deviceNumber })
            {
                recordingDevice.WaveFormat = new WaveFormat(44100, 2);
                try
                {
                    recordingDevice.StartRecording();
                } catch (Exception ex)
                {
                    
                    log.Error(ex);
                    return false;
                }
                recordingDevice.StopRecording();
                
                return true;
            }
        }

        public async Task<string> DetectAndSaveTracksAsync(string recordingPath = null)
        {
            if (recordingPath == null)
            {
                recordingPath = recordingFilePath;
            }
            var trackPositions = new TrackPositionCollection();

            using (AudioFileReader reader = new AudioFileReader(recordingPath))
            {                
                trackPositions = reader.GetTrackPositions(.83);
            }
            GC.Collect();

            var trackDataCollection = ExtractTrackData(trackPositions, recordingPath);
            string outputFolder = await SaveTracksAsync(trackDataCollection);

            // cleanup temp folder
            DirectoryInfo di = new DirectoryInfo(tempDataFolder);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            return outputFolder;
        }

        private TrackDataCollection ExtractTrackData(TrackPositionCollection trackPositions, string recordingPath)
        {
            double weight = .05;
            IoC.SavingProgressVM.Message = Text.FindingSongDataMessage;
            var trackDataCollection = new TrackDataCollection { Album = new AlbumData { }, AlbumThumbs = new Collection<AlbumThumb>()  };

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
                    Data = song,
                    Album = new AlbumData { }
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

                        case SongDetectionType.SHAZAM:
                            ISampleProvider sample = new AudioFileReader(recordingPath)
                                                                        .Skip(trackPosition.Start)
                                                                        .Take(end.Subtract(trackPosition.Start));
                            
                            PopulateTrackData(trackData, GetTrackData(sample));
                            break;
                    }


                }

                trackDataCollection.Add(trackData);
                IoC.SavingProgressVM.BGWorker.ReportProgress(trackPositions.Count, weight);
            }

            
            MapAlbumThumbs(trackDataCollection);


            return trackDataCollection;
        }

        private async Task<string> SaveTracksAsync(TrackDataCollection trackDataCollection)
        {
            var pattern = new Regex("[<>:\"/\\|?*]");
            string outputFolderName = DateTime.Now.ToString("dd.MM.yy-HH:mm");

            IoC.SavingProgressVM.Message = Text.SavingSongsMessage;

            if (trackDataCollection.AlbumThumbs.Count == 1)
            {
                outputFolderName = trackDataCollection.First().Album.Title;
            } else if (!string.IsNullOrEmpty(Settings.AlbumName))
            {
                outputFolderName = Settings.AlbumName;
            }
            outputFolderName = pattern.Replace(outputFolderName, "");

            Directory.CreateDirectory(Path.Combine(Settings.OutputFolderLocation, outputFolderName));

            var albumThumbLocationTasks = new List<Task<string>>();
            foreach (var albumThumb in trackDataCollection.AlbumThumbs)
            {
                albumThumbLocationTasks.Add(DownloadTempImageAsync(albumThumb.ThumbUrl ?? "", trackDataCollection.AlbumThumbs));
            }

            stopwatch.Start();
            Console.WriteLine(TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));
            Parallel.ForEach(trackDataCollection, trackData =>
            {
                trackData.Path = TrySave(trackData.Title, outputFolderName, trackData.Data, Settings.SaveFileType);
            });
            Console.WriteLine(TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));
            GC.Collect();
            IoC.SavingProgressVM.BGWorker.ReportProgress(1, .2);

            await Task.WhenAll(albumThumbLocationTasks);

            Parallel.ForEach(trackDataCollection, trackData =>
            {
                var albumThumbLocation = trackDataCollection.AlbumThumbs.Where(albumThumb => albumThumb.Title == trackData.Album.Title).FirstOrDefault()?.ThumbPath;
                var file = TagLib.File.Create(trackData.Path);

                file.Tag.Title = trackData.Title;
                file.Tag.Album = trackData.Album.Title;
                file.Tag.Track = (uint)trackData.Track;
                file.Tag.Performers = trackData.Performers;
                file.Tag.Genres = new[] { trackData.Album.Genre ?? "" };
                file.Tag.Year = (uint)trackData.Album.Year;

                if (!string.IsNullOrEmpty(albumThumbLocation))
                {
                    var pic = new TagLib.IPicture[1];
                    pic[0] = new TagLib.Picture(albumThumbLocation);
                    file.Tag.Pictures = pic;
                }

                file.Save();
            });
            IoC.SavingProgressVM.BGWorker.ReportProgress(1, .05);
            return Path.Combine(Settings.OutputFolderLocation, outputFolderName);
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

        private string SaveAsTempMono(ISampleProvider sample)
        {
            WaveFileWriter.CreateWaveFile16(tempFilePath, sample);
            string monoFilePath = Path.Combine(tempDataFolder, "mono.wav");
            using (var reader = new AudioFileReader(tempFilePath))
            {   
                var sampleLength = reader.TotalTime;
                var start = new TimeSpan(sampleLength.Ticks / 2);
                var trimmed = reader.Skip(start).Take(TimeSpan.FromSeconds(5));
                var mono = new StereoToMonoSampleProvider(trimmed);

                WaveFileWriter.CreateWaveFile16(monoFilePath, mono);
                return monoFilePath;
            }
        }

        private byte[] GetMonoSampleAsBytes(ISampleProvider sample)
        {
            return File.ReadAllBytes(SaveAsTempMono(sample));
        }

        private ShazamCoreModel GetTrackData(ISampleProvider sample)
        {
            var task = GetTrackDataAsync(sample);
            task.Wait();
            return task.Result;
        }

        private async Task<ShazamCoreModel> GetTrackDataAsync(ISampleProvider sample)
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://shazam-core.p.rapidapi.com/v1/tracks/recognize"),
                    Headers =
                {
                    { "x-rapidapi-host", "shazam-core.p.rapidapi.com" },
                    { "x-rapidapi-key", "80605243fcmsh7987f9cd1918fa5p141b05jsn0085912743db" },
                },
                    Content = new MultipartFormDataContent
            {
                new ByteArrayContent(GetMonoSampleAsBytes(sample))
                {
                    Headers = {
                        ContentType = new MediaTypeHeaderValue("audio/wav"),
                        ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "file",
                            FileName = "mono.wav",
                        }
                    }
                },
            },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<ShazamCoreModel>(body);
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }            
        }

        private static void MapAlbumThumbs(TrackDataCollection trackDataCollection)
        {
            var albums = new List<string>();
            var albumThumbs = new Collection<AlbumThumb>();

            foreach (var track in trackDataCollection)
            {
                albums.Add(track.Album.Title);
            }

            foreach (var album in albums.Distinct())
            {
                string thumbUrl = trackDataCollection.Where(t => t.Album.Title == album).First().Album.ThumbUrl;
                if (!string.IsNullOrEmpty(thumbUrl))
                {
                    albumThumbs.Add(new AlbumThumb
                    {
                        Title = album,
                        ThumbUrl = thumbUrl
                    });
                }
            }

            trackDataCollection.AlbumThumbs = albumThumbs;
        }

        private static void PopulateTrackData(TrackData trackData, ShazamCoreModel data)
        {
            var task = PopulateTrackDataAsync(trackData, data);
            task.Wait();
        }

        /*public void TestMBrainz(string isrc)
        {

            var q = new MetaBrainz.MusicBrainz.Query("Vinyl Recorder", "1.0.0.0", "mailto:rasheedabi97@hotmail.com");            
            
            var recordings = q.FindRecordings($"isrc{isrc}");
            Console.WriteLine(recordings);

        }*/

        private static async Task PopulateTrackDataAsync(TrackData trackData, ShazamCoreModel data)
        {
            if (data?.matches.Length > 0)
            {
                try
                {
                    string isrc = data.track.isrc;
                    var query = new QueryParameters<Recording>()
                    {
                        { "isrc", isrc }
                    };

                    var recordings = await brainzClient.Recordings.SearchAsync(query);

                    trackData.Title = recordings.Items[0].Title;
                    var albumName = data.track.sections[0].metadata.Where(m => m.title == "Album").FirstOrDefault();

                    var album = recordings.Items[0].Releases.Where(r => r.Title.ToLower() == albumName?.text.ToLower()).DefaultIfEmpty(recordings.Items[0].Releases.First()).FirstOrDefault();

                    trackData.Album = new AlbumData
                    {
                        Title = album.Title,
                        Year = Convert.ToInt32(album.Date.Substring(0, 4)),
                        Genre = data.track.genres.primary,
                        ThumbUrl = data.track.images.coverart
                    };
                    var performers = new string[recordings.Items[0].Credits.Count()];
                    int i = 0;
                    foreach (var artist in recordings.Items[0].Credits)
                    {
                        performers[i] = artist.Name;
                        i++;
                    }

                    trackData.Performers = new[] { recordings.Items[0].Credits.First().Name };
                    trackData.Track = Convert.ToInt32(album.Media.First().Tracks.First().Number);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }       

        private async Task<string> DownloadTempImageAsync(string url, Collection<AlbumThumb> albumThumbs)
        {
            var pattern = new Regex("[<>:\"/\\|?*]");
            string fileLocation = Path.Combine(tempDataFolder, pattern.Replace(url, ""));

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

            albumThumbs.Where(a => a.ThumbUrl == url).First().ThumbPath = fileLocation;
            return fileLocation;
        }

    }
}
