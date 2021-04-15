
public class ShazamModel
{
    public Match[] matches { get; set; }
    public long timestamp { get; set; }
    public string timezone { get; set; }
    public string tagid { get; set; }
    public Track track { get; set; }
}

public class Track
{
    public string layout { get; set; }
    public string type { get; set; }
    public string key { get; set; }
    public string title { get; set; }
    public string subtitle { get; set; }
    public Images images { get; set; }
    public Share share { get; set; }
    public Hub hub { get; set; }
    public string url { get; set; }
    public Artist[] artists { get; set; }
    public string isrc { get; set; }
    public Genres genres { get; set; }
    public Urlparams urlparams { get; set; }
    public Myshazam myshazam { get; set; }
    public string albumadamid { get; set; }
    public Section[] sections { get; set; }
}

public class Images
{
    public string background { get; set; }
    public string coverart { get; set; }
    public string coverarthq { get; set; }
    public string joecolor { get; set; }
}

public class Share
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

public class Hub
{
    public string type { get; set; }
    public string image { get; set; }
    public Action[] actions { get; set; }
    public Option[] options { get; set; }
    public Provider[] providers { get; set; }
    public bool _explicit { get; set; }
    public string displayname { get; set; }
}

public class Action
{
    public string name { get; set; }
    public string type { get; set; }
    public string id { get; set; }
    public string uri { get; set; }
}

public class Option
{
    public string caption { get; set; }
    public Action1[] actions { get; set; }
    public Beacondata beacondata { get; set; }
    public string image { get; set; }
    public string type { get; set; }
    public string listcaption { get; set; }
    public string overflowimage { get; set; }
    public bool colouroverflowimage { get; set; }
    public string providername { get; set; }
}

public class Beacondata
{
    public string type { get; set; }
    public string providername { get; set; }
}

public class Action1
{
    public string name { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class Provider
{
    public string caption { get; set; }
    public Images1 images { get; set; }
    public Action2[] actions { get; set; }
    public string type { get; set; }
}

public class Images1
{
    public string overflow { get; set; }
    public string _default { get; set; }
}

public class Action2
{
    public string name { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class Genres
{
    public string primary { get; set; }
}

public class Urlparams
{
    public string tracktitle { get; set; }
    public string trackartist { get; set; }
}

public class Myshazam
{
    public Apple apple { get; set; }
}

public class Apple
{
    public Action3[] actions { get; set; }
}

public class Action3
{
    public string name { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
}

public class Artist
{
    public string id { get; set; }
    public string adamid { get; set; }
}

public class Section
{
    public string type { get; set; }
    public Metapage[] metapages { get; set; }
    public string tabname { get; set; }
    public Metadata[] metadata { get; set; }
    public string[] text { get; set; }
    public string footer { get; set; }
    public Beacondata1 beacondata { get; set; }
    public Youtubeurl youtubeurl { get; set; }
    public string avatar { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public bool verified { get; set; }
    public Action5[] actions { get; set; }
}

public class Beacondata1
{
    public string lyricsid { get; set; }
    public string providername { get; set; }
    public string commontrackid { get; set; }
}

public class Youtubeurl
{
    public string caption { get; set; }
    public Image image { get; set; }
    public Action4[] actions { get; set; }
}

public class Image
{
    public Dimensions dimensions { get; set; }
    public string url { get; set; }
}

public class Dimensions
{
    public int width { get; set; }
    public int height { get; set; }
}

public class Action4
{
    public string name { get; set; }
    public string type { get; set; }
    public Share1 share { get; set; }
    public string uri { get; set; }
}

public class Share1
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

public class Metapage
{
    public string image { get; set; }
    public string caption { get; set; }
}

public class Metadata
{
    public string title { get; set; }
    public string text { get; set; }
}

public class Action5
{
    public string type { get; set; }
    public string id { get; set; }
}

public class Match
{
    public string id { get; set; }
    public float offset { get; set; }
    public string channel { get; set; }
    public float timeskew { get; set; }
    public float frequencyskew { get; set; }
}
