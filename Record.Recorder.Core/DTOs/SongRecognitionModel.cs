namespace Record.Recorder.Core
{
    public class SongRecognitionModel
    {
        public Location location { get; set; }
        public Match[] matches { get; set; }
        public string tagid { get; set; }
        public int timestamp { get; set; }
        public string timezone { get; set; }
        public SongRecognitionTrack track { get; set; }
    }

    public class Location
    {
        public float altitude { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class SongRecognitionTrack
    {
        public string albumadamid { get; set; }
        public Artist[] artists { get; set; }
        public Genres genres { get; set; }
        public Highlightsurls highlightsurls { get; set; }
        public Hub hub { get; set; }
        public Images1 images { get; set; }
        public string isrc { get; set; }
        public string key { get; set; }
        public string layout { get; set; }
        public string relatedtracksurl { get; set; }
        public Section[] sections { get; set; }
        public Share share { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public Urlparams urlparams { get; set; }
    }

    public class Genres
    {
        public string primary { get; set; }
    }

    public class Highlightsurls
    {
        public string artisthighlightsurl { get; set; }
        public string relatedhighlightsurl { get; set; }
    }

    public class Hub
    {
        public Action[] actions { get; set; }
        public string displayname { get; set; }
        public bool _explicit { get; set; }
        public string image { get; set; }
        public Option[] options { get; set; }
        public Provider[] providers { get; set; }
        public string type { get; set; }
    }

    public class Action
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Option
    {
        public Action1[] actions { get; set; }
        public Beacondata beacondata { get; set; }
        public string caption { get; set; }
        public bool colouroverflowimage { get; set; }
        public string image { get; set; }
        public string listcaption { get; set; }
        public string overflowimage { get; set; }
        public string providername { get; set; }
        public string type { get; set; }
    }

    public class Beacondata
    {
        public string providername { get; set; }
        public string type { get; set; }
    }

    public class Action1
    {
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        public string id { get; set; }
    }

    public class Provider
    {
        public Action2[] actions { get; set; }
        public string caption { get; set; }
        public Images images { get; set; }
        public string type { get; set; }
    }

    public class Images
    {
        public string _default { get; set; }
        public string overflow { get; set; }
    }

    public class Action2
    {
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Images1
    {
        public string background { get; set; }
        public string coverart { get; set; }
        public string coverarthq { get; set; }
        public string joecolor { get; set; }
    }

    public class Share
    {
        public string avatar { get; set; }
        public string href { get; set; }
        public string html { get; set; }
        public string image { get; set; }
        public string snapchat { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string twitter { get; set; }
    }

    public class Urlparams
    {
        public string trackartist { get; set; }
        public string tracktitle { get; set; }
    }

    public class Artist
    {
        public string adamid { get; set; }
        public string id { get; set; }
    }

    public class Section
    {
        public Metadata[] metadata { get; set; }
        public Metapage[] metapages { get; set; }
        public string tabname { get; set; }
        public string type { get; set; }
        public Beacondata1 beacondata { get; set; }
        public string footer { get; set; }
        public string[] text { get; set; }
        public string url { get; set; }
        public string youtubeurl { get; set; }
        public Action3[] actions { get; set; }
        public string avatar { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public Toptracks toptracks { get; set; }
        public bool verified { get; set; }
    }

    public class Beacondata1
    {
        public string commontrackid { get; set; }
        public string lyricsid { get; set; }
        public string providername { get; set; }
    }

    public class Toptracks
    {
        public string url { get; set; }
    }

    public class Metadata
    {
        public string text { get; set; }
        public string title { get; set; }
    }

    public class Metapage
    {
        public string caption { get; set; }
        public string image { get; set; }
    }

    public class Action3
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Match
    {
        public string channel { get; set; }
        public float frequencyskew { get; set; }
        public string id { get; set; }
        public float offset { get; set; }
        public float timeskew { get; set; }
    }

}

