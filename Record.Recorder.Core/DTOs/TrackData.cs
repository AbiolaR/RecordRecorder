using NAudio.Wave;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Record.Recorder.Core
{
    public class TrackData
    {
        /// <summary>
        /// The title of the track
        /// </summary>
        public string Title { get; set; }    

        /// <summary>
        /// The album data of the album the track was released in
        /// </summary>
        public AlbumData Album { get; set; }
        
        /// <summary>
        /// The number of the track in the album it was released in
        /// </summary>
        public int Track { get; set; }

        /// <summary>
        /// The artists featured in the track
        /// </summary>
        public string[] Performers { get; set; }
        
        /// <summary>
        /// The path this track was saved to
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The sample data of the track
        /// </summary>
        public ISampleProvider Data { get; set; }
    }

    public class TrackDataCollection : Collection<TrackData>
    {
        public AlbumData Album { get; set; }

        public Collection<AlbumThumb> AlbumThumbs { get; set; }
    }
}
