using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;

namespace Record.Recorder.Core
{
    static class WaveFileWriterExt
    {
        public static void WriteSilence(this WaveFileWriter writer)
        {
            var silenceBuffer = new SilenceProvider(new WaveFormat(RecorderConfig.SampleRate, RecorderConfig.Channels))
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
        }
    }
}
