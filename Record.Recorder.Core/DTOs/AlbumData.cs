using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Record.Recorder.Core
{
    public class AlbumData
    {
        public string Title { get; set; }
        public string ThumbUrl { get; set; }        
        public int Year { get; set; }
        public string Genre { get; set; }
        public Dictionary<int, TrackData> Tracks { get; set; }

        public AlbumData()
        {
            Tracks = new Dictionary<int, TrackData>();
        }
    }

    public class AlbumThumb
    {
        public string Title { get; set; }
        public string ThumbUrl { get; set; }
        public string ThumbPath { get; set; }
    }
}
