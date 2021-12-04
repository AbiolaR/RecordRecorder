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
            string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Vinyl Recorder");
            string resourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), @"Test\Resources");
            var mockSettings = new Mock<ISettingsManager>();

            mockSettings.Setup(settings => settings.OutputFolderLocation).Returns(outputPath);
            mockSettings.Setup(settings => settings.AlbumName).Returns("m2tb");
            mockSettings.Setup(settings => settings.SaveFileType).Returns(AudioFileType.MP3);
            mockSettings.Setup(settings => settings.SongDetectionType).Returns(SongDetectionType.SHAZAM);
            IoC.Kernel.Bind<ISettingsManager>().ToConstant(mockSettings.Object);

            var path = Path.Combine(resourcePath, "mgk.wav");

            var mockRecorder = new Mock<RecorderUtil>("Vinyl Recorder");

            
            mockRecorder.Setup(recorder => recorder.IsInternetConnected()).Returns(true);





            //mockRecorder.Object.TestMBrainz("USUM72000816");

            Task.Run(async () =>
            {
                await mockRecorder.Object.DetectAndSaveTracksAsync(path);
            }).GetAwaiter().GetResult();

            Assert.Pass();
        }
    }
}