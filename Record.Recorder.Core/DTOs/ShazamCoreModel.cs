namespace Record.Recorder.Core
{
#pragma warning disable IDE1006
    public class ShazamCoreModel
    {
        public ShazamCoreLocation location { get; set; }
        public ShazamCoreMatch[] matches { get; set; }
        public string tagid { get; set; }
        public long timestamp { get; set; }
        public string timezone { get; set; }
        public ShazamCoreTrack track { get; set; }
    }

    public class ShazamCoreLocation
    {
        public float altitude { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class ShazamCoreTrack
    {
        public string albumadamid { get; set; }
        public ShazamCoreArtist[] artists { get; set; }
        public ShazamCoreGenres genres { get; set; }
        public ShazamCoreHighlightsurls highlightsurls { get; set; }
        public ShazamCoreHub hub { get; set; }
        public ShazamCoreImages1 images { get; set; }
        public string isrc { get; set; }
        public string key { get; set; }
        public string layout { get; set; }
        public string relatedtracksurl { get; set; }
        public ShazamCoreSection[] sections { get; set; }
        public ShazamCoreShare share { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public ShazamCoreUrlparams urlparams { get; set; }
    }

    public class ShazamCoreGenres
    {
        public string primary { get; set; }
    }

    public class ShazamCoreHighlightsurls
    {
        public string artisthighlightsurl { get; set; }
        public string relatedhighlightsurl { get; set; }
    }

    public class ShazamCoreHub
    {
        public ShazamCoreAction[] actions { get; set; }
        public string displayname { get; set; }
        public bool _explicit { get; set; }
        public string image { get; set; }
        public ShazamCoreOption[] options { get; set; }
        public ShazamCoreProvider[] providers { get; set; }
        public string type { get; set; }
    }

    public class ShazamCoreAction
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ShazamCoreOption
    {
        public ShazamCoreAction1[] actions { get; set; }
        public ShazamCoreBeacondata beacondata { get; set; }
        public string caption { get; set; }
        public bool colouroverflowimage { get; set; }
        public string image { get; set; }
        public string listcaption { get; set; }
        public string overflowimage { get; set; }
        public string providername { get; set; }
        public string type { get; set; }
    }

    public class ShazamCoreBeacondata
    {
        public string providername { get; set; }
        public string type { get; set; }
    }

    public class ShazamCoreAction1
    {
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        public string id { get; set; }
    }

    public class ShazamCoreProvider
    {
        public ShazamCoreAction2[] actions { get; set; }
        public string caption { get; set; }
        public ShazamCoreImages images { get; set; }
        public string type { get; set; }
    }

    public class ShazamCoreImages
    {
        public string _default { get; set; }
        public string overflow { get; set; }
    }

    public class ShazamCoreAction2
    {
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ShazamCoreImages1
    {
        public string background { get; set; }
        public string coverart { get; set; }
        public string coverarthq { get; set; }
        public string joecolor { get; set; }
    }

    public class ShazamCoreShare
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

    public class ShazamCoreUrlparams
    {
        public string trackartist { get; set; }
        public string tracktitle { get; set; }
    }

    public class ShazamCoreArtist
    {
        public string adamid { get; set; }
        public string id { get; set; }
    }

    public class ShazamCoreSection
    {
        public ShazamCoreMetadata[] metadata { get; set; }
        public ShazamCoreMetapage[] metapages { get; set; }
        public string tabname { get; set; }
        public string type { get; set; }
        public ShazamCoreBeacondata1 beacondata { get; set; }
        public string footer { get; set; }
        public string[] text { get; set; }
        public string url { get; set; }
        public string youtubeurl { get; set; }
        public ShazamCoreAction3[] actions { get; set; }
        public string avatar { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public ShazamCoreToptracks toptracks { get; set; }
        public bool verified { get; set; }
    }

    public class ShazamCoreBeacondata1
    {
        public string commontrackid { get; set; }
        public string lyricsid { get; set; }
        public string providername { get; set; }
    }

    public class ShazamCoreToptracks
    {
        public string url { get; set; }
    }

    public class ShazamCoreMetadata
    {
        public string text { get; set; }
        public string title { get; set; }
    }

    public class ShazamCoreMetapage
    {
        public string caption { get; set; }
        public string image { get; set; }
    }

    public class ShazamCoreAction3
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class ShazamCoreMatch
    {
        public string channel { get; set; }
        public float frequencyskew { get; set; }
        public string id { get; set; }
        public float offset { get; set; }
        public float timeskew { get; set; }
    }

}
