namespace Record.Recorder.Core
{
    public class SongModel
    {
        public string Title { get; set; }
        public string Album { get; set; }
        public int Track { get; set; }
        public string[] Performers { get; set; }

        public string Path { get; set; }
    }
}
