using Moq;
using NUnit.Framework;
using System.IO;
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

            mockSettings.Setup(settings => settings.GetOutputFolderLocation()).Returns(@"C:\Users\rasheed_abiola\Music\Test");
            mockSettings.Setup(settings => settings.GetAlbumName()).Returns("2334279");
            mockSettings.Setup(settings => settings.GetFileType()).Returns(AudioFileType.FLAC);
            mockSettings.Setup(settings => settings.GetSongDetectionType()).Returns("TADB");

            IoC.Kernel.Bind<ISettingsManager>().ToConstant(mockSettings.Object);

            var path = Path.Combine(TestContext.CurrentContext.WorkDirectory, @"Resources\Audio\MusicToBeMurderedBy.wav");

            var mockRecorder = new Mock<RecorderUtil>();

            mockRecorder.Setup(recorder => recorder.IsInternetConnected()).Returns(true);

            DirectoryInfo di = new DirectoryInfo(@"C:\Users\rasheed_abiola\Music\Test");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }


            Task.Run(async () =>
            {
                await mockRecorder.Object.DetectAndSaveTracks(path);
            }).GetAwaiter().GetResult();

            Assert.Pass();
        }
    }
}