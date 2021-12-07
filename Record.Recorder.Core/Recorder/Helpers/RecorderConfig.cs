namespace Record.Recorder.Core
{
    internal static class RecorderConfig
    {
        public static int SilenceMinDuration { get; } = 2000;
        public static int SampleRate { get; } = 44100;
        public static int Channels { get; } = 2;
    }
}
