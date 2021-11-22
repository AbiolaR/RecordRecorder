
public class ShazamModel
{
    public ShazamMatch[] matches { get; set; }
    public long timestamp { get; set; }
    public string timezone { get; set; }
    public string tagid { get; set; }
    public ShazamTrack track { get; set; }
}

public class ShazamTrack
{
    public string layout { get; set; }
    public string type { get; set; }
    public string key { get; set; }
    public string title { get; set; }
    public string subtitle { get; set; }
    public ShazamImages images { get; set; }
    public ShazamShare share { get; set; }
    public ShazamHub hub { get; set; }
    public string url { get; set; }
    public ShazamArtist[] artists { get; set; }
    public string isrc { get; set; }
    public ShazamGenres genres { get; set; }
    public ShazamUrlparams urlparams { get; set; }
    public ShazamMyshazam myshazam { get; set; }
    public string albumadamid { get; set; }
    public ShazamSection[] sections { get; set; }
}

public class ShazamImages
{
    public string background { get; set; }
    public string coverart { get; set; }
    public string coverarthq { get; set; }
    public string joecolor { get; set; }
}

public class ShazamShare
{
    public string subject { get; set; }
    public string text { get; set; }
    public string href { get; set; }
    public string image { get; set; }
    public string twitter { get; set; }
    public string html { get; set; }
    public string avatar { get; set; }
    public string snapchat { get; set; }
}

public class ShazamHub
{
    public string type { get; set; }
    public string image { get; set; }
    public ShazamAction[] actions { get; set; }
    public ShazamOption[] options { get; set; }
    public ShazamProvider[] providers { get; set; }
    public bool _explicit { get; set; }
    public string displayname { get; set; }
}

public class ShazamAction
{
    public string name { get; set; }
    public string type { get; set; }
    public string id { get; set; }
    public string uri { get; set; }
}

public class ShazamOption
{
    public string caption { get; set; }
    public ShazamAction1[] actions { get; set; }
    public ShazamBeacondata beacondata { get; set; }
    public string image { get; set; }
    public string type { get; set; }
    public string listcaption { get; set; }
    public string overflowimage { get; set; }
    public bool colouroverflowimage { get; set; }
    public string providername { get; set; }
}

public class ShazamBeacondata
{
    public string type { get; set; }
    public string providername { get; set; }
}

public class ShazamAction1
{
    public string name { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class ShazamProvider
{
    public string caption { get; set; }
    public ShazamImages1 images { get; set; }
    public ShazamAction2[] actions { get; set; }
    public string type { get; set; }
}

public class ShazamImages1
{
    public string overflow { get; set; }
    public string _default { get; set; }
}

public class ShazamAction2
{
    public string name { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class ShazamGenres
{
    public string primary { get; set; }
}

public class ShazamUrlparams
{
    public string tracktitle { get; set; }
    public string trackartist { get; set; }
}

public class ShazamMyshazam
{
    public ShazamApple apple { get; set; }
}

public class ShazamApple
{
    public ShazamAction3[] actions { get; set; }
}

public class ShazamAction3
{
    public string name { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class ShazamArtist
{
    public string id { get; set; }
    public string adamid { get; set; }
}

public class ShazamSection
{
    public string type { get; set; }
    public ShazamMetapage[] metapages { get; set; }
    public string tabname { get; set; }
    public ShazamMetadata[] metadata { get; set; }
    public string[] text { get; set; }
    public string footer { get; set; }
    public ShazamBeacondata1 beacondata { get; set; }
    public ShazamYoutubeurl youtubeurl { get; set; }
    public string avatar { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public bool verified { get; set; }
    public ShazamAction5[] actions { get; set; }
}

public class ShazamBeacondata1
{
    public string lyricsid { get; set; }
    public string providername { get; set; }
    public string commontrackid { get; set; }
}

public class ShazamYoutubeurl
{
    public string caption { get; set; }
    public ShazamImage image { get; set; }
    public ShazamAction4[] actions { get; set; }
}

public class ShazamImage
{
    public ShazamDimensions dimensions { get; set; }
    public string url { get; set; }
}

public class ShazamDimensions
{
    public int width { get; set; }
    public int height { get; set; }
}

public class ShazamAction4
{
    public string name { get; set; }
    public string type { get; set; }
    public ShazamShare1 share { get; set; }
    public string uri { get; set; }
}

public class ShazamShare1
{
    public string subject { get; set; }
    public string text { get; set; }
    public string href { get; set; }
    public string image { get; set; }
    public string twitter { get; set; }
    public string html { get; set; }
    public string avatar { get; set; }
    public string snapchat { get; set; }
}

public class ShazamMetapage
{
    public string image { get; set; }
    public string caption { get; set; }
}

public class ShazamMetadata
{
    public string title { get; set; }
    public string text { get; set; }
}

public class ShazamAction5
{
    public string type { get; set; }
    public string id { get; set; }
}

public class ShazamMatch
{
    public string id { get; set; }
    public float offset { get; set; }
    public string channel { get; set; }
    public float timeskew { get; set; }
    public float frequencyskew { get; set; }
}
