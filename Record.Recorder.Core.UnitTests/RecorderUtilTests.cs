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

            var mockSettings = new Mock<ISettingsManager>();

            mockSettings.Setup(settings => settings.OutputFolderLocation).Returns(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Test"));
            mockSettings.Setup(settings => settings.AlbumName).Returns("2334279");
            mockSettings.Setup(settings => settings.SaveFileType).Returns(AudioFileType.FLAC);
            mockSettings.Setup(settings => settings.SongDetectionType).Returns(SongDetectionType.TADB);
            IoC.Kernel.Bind<ISettingsManager>().ToConstant(mockSettings.Object);

            var mockMainVM = new Mock<MainViewModel>();
            var bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;
            mockMainVM.Setup(mainVM => mainVM.BGWorker).Returns(bgw);
            IoC.Kernel.Bind<MainViewModel>().ToConstant(mockMainVM.Object);

            var path = Path.Combine(TestContext.CurrentContext.WorkDirectory, @"Resources\Audio\full12min.wav");

            var mockRecorder = new Mock<RecorderUtil>();

            mockRecorder.Setup(recorder => recorder.IsInternetConnected()).Returns(false);



            DirectoryInfo di = new DirectoryInfo(@"C:\Users\rasheed_abiola\Music\Test");

            /*foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }*/


            Task.Run(async () =>
            {
                await mockRecorder.Object.DetectAndSaveTracksAsync(path);
            }).GetAwaiter().GetResult();

            Assert.Pass();
        }
    }
}