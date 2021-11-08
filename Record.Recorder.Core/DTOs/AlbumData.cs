﻿using System.Collections.Generic;

namespace Record.Recorder.Core
{
    public class AlbumData
    {
        public string Title { get; set; }
        public string ThumbUrl { get; set; }
        public string Year { get; set; }
        public string Genre { get; set; }
        public Dictionary<int, TrackData> Tracks { get; set; }

        public AlbumData()
        {
            Tracks = new Dictionary<int, TrackData>();
        }
    }
}