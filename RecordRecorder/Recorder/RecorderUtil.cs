using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecordRecorder
{
    public delegate void Notify();
    class RecorderUtil
    {
        public static event Notify ProgressIndeterminateStarted;
        private static HttpClient client = new HttpClient();
        WaveFileWriter writer = null;
        WaveInEvent recordingDevice = null;
        private static readonly string dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
        private static readonly string dataFilePath = Path.Combine(dataFolder, "recording1.wav"); 
        private static readonly string dataPartPath = Path.Combine(dataFolder, "part.wav");
        public string OutputFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); //@"C:\Users\rasheed_abiola\Desktop\NAudio\recorded3.wav";
        public string OutputFolder = "";
        public string fileType = AudioFileType.WAV;
        public int RecordingDeviceNum { get; set; } = 999;

        public Dictionary<int, string> GetRecordingDevices()
        {
            Dictionary<int, string> recordingDevices = new Dictionary<int, string>();
            recordingDevices.Add(-2, "Bitte Aufnahmegerät auswählen"); // placeholder


            for (int n = -1; n < WaveIn.DeviceCount; n++)
            {
                var caps = WaveIn.GetCapabilities(n);
                recordingDevices.Add(n, caps.ProductName);
            }
            return recordingDevices;
        }

        public void StartRecording()
        {
            recordingDevice = new WaveInEvent() { DeviceNumber = RecordingDeviceNum };
            recordingDevice.WaveFormat = new WaveFormat(44100, 2);

            writer = new WaveFileWriter(dataFilePath, recordingDevice.WaveFormat);

            recordingDevice.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
            };

            recordingDevice.RecordingStopped += (s, a) =>
            {
                writer?.Dispose();
                writer = null;
                recordingDevice.Dispose();
            };

            recordingDevice.StartRecording();
        }

        public void StopRecording()
        {
            if (recordingDevice != null) recordingDevice.StopRecording();
        }

        public async Task DetectAndSaveTracks()
        {
            Dictionary<String, TimeSpan> trackPositions = new Dictionary<string, TimeSpan>();

            /*var task = Task.Run(() => {
                AudioFileReader reader = new AudioFileReader(dataFilePath);
                trackPositions = reader.GetTrackPositions();
                reader.Dispose();
                GC.Collect();

                Console.WriteLine("DONE");
                foreach (KeyValuePair<String, TimeSpan> keyValuePair in trackPositions)
                {
                    Console.WriteLine(keyValuePair);
                }
            });*/

            /*using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Bitte den Ordner auswählen in dem die Lieder gespeichert werden sollen.";

                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (!task.IsCompleted)
                    {
                        ProgressIndeterminateStarted?.Invoke();
                        await task;
                    }
                    await ExtractAndSaveTracksAsync(trackPositions, task);
                }
            }*/

            using (AudioFileReader reader = new AudioFileReader(dataFilePath))
            {
                trackPositions = reader.GetTrackPositions();
                reader.Dispose();
                GC.Collect();
                Console.WriteLine("DONE");
                foreach (KeyValuePair<String, TimeSpan> keyValuePair in trackPositions)
                {
                    Console.WriteLine(keyValuePair);
                }
            }

            await ExtractAndSaveTracksAsync(trackPositions);
        }


        private async Task ExtractAndSaveTracksAsync(Dictionary<String, TimeSpan> trackPositions)
        {
            int trackPositionsAmount = trackPositions.Count / 2;

            for (int i = 1; i <= trackPositionsAmount; i++)
            {
                trackPositions.TryGetValue("Start: " + i, out TimeSpan start);
                trackPositions.TryGetValue("End: " + i, out TimeSpan end);
                end = TimeSpan.FromMilliseconds(end.TotalMilliseconds + 500);

                var trimmed = new AudioFileReader(dataFilePath)
                                                .Skip(start)
                                                .Take(end.Subtract(start));
                var shazamModel = await GetTrackNameAsync(GetMonoSampleAsBytes(start));
                string filePath = Path.Combine(OutputFolderPath, OutputFolder, $"{shazamModel.track.title}{fileType}");

                WaveFileWriter.CreateWaveFile16(filePath, trimmed);
                var file = TagLib.File.Create(filePath);
                // setting the metadata
                file.Tag.Title = shazamModel.track.title;
                file.Save();
                //file.Tag.Genres = shazamModel.track.genres.primary;
            }
        }

        private static byte[] GetMonoSampleAsBytes(TimeSpan start)
        {
            Directory.CreateDirectory(dataFolder);
            Directory.CreateDirectory(Path.Combine(dataFolder, "parts"));

            start = TimeSpan.FromMilliseconds(start.TotalMilliseconds + 10000);
            TimeSpan end = TimeSpan.FromMilliseconds(start.TotalMilliseconds + 5000);

            var trimmed = new AudioFileReader(dataFilePath)
                                            .Skip(start)
                                            .Take(end.Subtract(start));

            var mono = new StereoToMonoSampleProvider(trimmed);
            WaveFileWriter.CreateWaveFile16(dataPartPath, mono); //$@"C:\Users\rasheed_abiola\Desktop\NAudio\parts\part.wav", mono);
            return File.ReadAllBytes(dataPartPath);
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
                var shazamModel = JsonSerializer.Deserialize<ShazamModel>(body);
                return shazamModel;
            }
        }
        
    }
}
