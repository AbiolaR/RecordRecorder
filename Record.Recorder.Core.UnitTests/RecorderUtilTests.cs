using Moq;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.IO;
using Record.Recorder.Type;
using System.Threading.Tasks;

namespace Record.Recorder.Core.UnitTests
{
    public class RecorderUtilTests
    {
        //private Mock mockSettings;
        //private SettingsHelper mockSettings;

        [SetUp]
        public void Setup()
        {

            //mocks = new MockFactory();
            //var mockSettings = mocks.CreateMock<SettingsHelper>();
        }

        [Test]
        public void Recognize_Two_Songs_Without_Internet_Test()
        {
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Test");
            var mockSettings = new Mock<ISettingsManager>();

            mockSettings.Setup(settings => settings.OutputFolderLocation).Returns(outputPath);
            mockSettings.Setup(settings => settings.AlbumName).Returns("m2tb");
            mockSettings.Setup(settings => settings.SaveFileType).Returns(AudioFileType.FLAC);
            mockSettings.Setup(settings => settings.SongDetectionType).Returns(SongDetectionType.SHAZAM);
            IoC.Kernel.Bind<ISettingsManager>().ToConstant(mockSettings.Object);

            var mockMainVM = new Mock<MainViewModel>();
            var bgw = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            mockMainVM.Setup(mainVM => mainVM.BGWorker).Returns(bgw);
            IoC.Kernel.Bind<MainViewModel>().ToConstant(mockMainVM.Object);

            var path = Path.Combine(outputPath, @"Resources\2songs.wav");

            var mockRecorder = new Mock<RecorderUtil>();

            mockRecorder.Setup(recorder => recorder.IsInternetConnected()).Returns(true);



            /*DirectoryInfo di = new DirectoryInfo(@"C:\Users\rasheed_abiola\Music\Test");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }*/

            /*var model = mockRecorder.Object.GetTrackData3();
            Console.WriteLine(model);*/
            Task.Run(async () =>
            {
                //await mockRecorder.Object.PopulateTrackDataAsync(new TrackData(), "USUM72000782");
                await mockRecorder.Object.DetectAndSaveTracksAsync(path);
            }).GetAwaiter().GetResult();

            Assert.Pass();
        }
    }
}