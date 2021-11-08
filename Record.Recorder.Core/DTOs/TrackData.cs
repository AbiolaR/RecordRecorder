using NAudio.Wave;
using System.Collections.ObjectModel;

namespace Record.Recorder.Core
{
    public class TrackData
    {
        public string Title { get; set; }
        public string Album { get; set; }
        public int Track { get; set; }
        public string[] Performers { get; set; }
        public string Path { get; set; }
        public ISampleProvider Data { get; set; }
    }

    public class TrackDataCollection : Collection<TrackData>
    {

    }
}
