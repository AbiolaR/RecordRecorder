using Newtonsoft.Json;
using Record.Recorder.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace RecordRecorder
{
    class UpdateManager : IUpdateManager
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetEntryAssembly()?.GetName().Name, "temp");
        private static readonly string msiFileLocation = Path.Combine(tempFolder, "update.msi");
        public Task DownloadTask { get; set; }

        public async Task<bool> CheckForUpdateAvailableAsync()
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://media-server.casacam.net:9277/vinylrecorder/version"),

                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    
                    var update = JsonConvert.DeserializeObject<UpdateVersion>(body);
                    var newestVersion = new Version(update.Version);
                    if (App.Version < newestVersion)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;            
        }

        public string DownloadUpdate()
        {
            var task = DownloadUpdateAsync();
            task.Wait();
            return task.Result;
        }
        public async Task<string> DownloadUpdateAsync()
        {
            string url = IoC.Settings.ApplicationLanguage switch
            {
                "Deutsch" => "https://media-server.casacam.net:9277/vinylrecorder/download/de/VinylRecorderInstaller-De.msi",
                _ => "https://media-server.casacam.net:9277/vinylrecorder/download/en/VinylRecorderInstaller-En.msi"
            };
            

            string fileLocation = msiFileLocation;//Path.Combine(tempFolder, "update.msi");
            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(UpdateDownloadProgressChanged);                 
                    await client.DownloadFileTaskAsync(new Uri(url), fileLocation);
                }
            }
            catch
            {
                return "";
            }

            return fileLocation;
        }

        public void UpdateApplication()
        {
            string batchLocation = Path.Combine(tempFolder, "update.bat");
            string currentInstallLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (File.Exists(batchLocation))
            {
                File.Delete(batchLocation);
            }
            /*var fi = new FileInfo(batchLocation);
            var fileStream = fi.Create();
            fileStream.Close();*/

            using (StreamWriter writer = File.AppendText(batchLocation))
            {
                writer.WriteLine("timeout /t 3 /nobreak");
                writer.WriteLine($"msiexec /i \"{msiFileLocation}\" /qb /norestart TARGETDIR=\"{currentInstallLocation}\"");
                writer.WriteLine($"start \"\" \"{currentInstallLocation}\\Vinyl Recorder.exe\"");
            }

            var p = new Process
            {
                StartInfo = new ProcessStartInfo("cmd.exe", $"/c \"{batchLocation}\"")
            };
            p.StartInfo.CreateNoWindow = true;
            p.Start();
        }

        private void UpdateDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs eventArgs)
        {
            double bytesReceived = double.Parse(eventArgs.BytesReceived.ToString());
            int totalBytes = Convert.ToInt32(eventArgs.TotalBytesToReceive.ToString());

            IoC.SavingProgressVM.BGWorker.ReportProgress(totalBytes, bytesReceived - IoC.SavingProgressVM.ProgressValue);
        }
    }
}
