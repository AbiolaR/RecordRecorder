namespace Record.Recorder.Type
{
    public enum SongDetectionType
    {
        /// <summary>
        /// Uses The Audio DB to get album and song data per album
        /// </summary>
        TADB,
        /// <summary>
        /// Uses Spotify to get song and album data per song
        /// </summary>
        SHAZAM
    }
}
