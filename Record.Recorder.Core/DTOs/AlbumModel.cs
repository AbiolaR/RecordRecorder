using System.Collections.Generic;

namespace Record.Recorder.Core
{
    public class AlbumModel
    {
        public string Title { get; set; }
        public string ThumbUrl { get; set; }
        public string Year { get; set; }
        public Dictionary<int, SongModel> Tracks { get; set; }

        public AlbumModel()
        {
            Tracks = new Dictionary<int, SongModel>();
        }
    }
}
