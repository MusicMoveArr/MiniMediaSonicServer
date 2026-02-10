namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Navidrome.SmartPlaylist;

public class OperatorField
{
    public string? Title { get; set; }
    public string? Album { get; set; }
    public bool? HasCoverAart { get; set; }
    public int? TrackNumber { get; set; }
    public int? DiscNumber { get; set; }
    public List<int>? Year { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? OriginalYear { get; set; }
    public DateTime? OriginalDate { get; set; }
    public DateTime? ReleaseYear { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public long? Size { get; set; }
    public bool? Compilation { get; set; }
    public DateTime? DateAdded { get; set; }
    public DateTime? DateModified { get; set; }
    public string? DiscSubtitle { get; set; }
    public string? Comment { get; set; }
    public string? Lyrics { get; set; }
    public string? SortTitle { get; set; }
    public string? SortAlbum { get; set; }
    public string? SortArtist { get; set; }
    public string? SortAlbumArtist { get; set; }
    public string? AlbumType { get; set; }
    public string? AlbumComment { get; set; }
    public string? CatalogNumber { get; set; }
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
    public string? Grouping { get; set; }
    public int? Duration { get; set; }
    public int? Bitrate { get; set; }
    public int? BitDepth { get; set; }
    public int? BPM { get; set; }
    public int? Channels { get; set; }
    public bool? Loved { get; set; }
    public DateTime? DateLoved { get; set; }
    public int? LastPlayed { get; set; }
    public DateTime? DateRated { get; set; }
    public int? PlayCount { get; set; }
    public int? Rating { get; set; }
    public Guid? Mbz_Album_Id { get; set; }
    public Guid? Mbz_Album_Artist_Id { get; set; }
    public Guid? Mbz_Artist_Id { get; set; }
    public Guid? Mbz_Recording_Id { get; set; }
    public Guid? Mbz_Release_Track_Id { get; set; }
    public Guid? Mbz_Release_Group_Id { get; set; }
    public int? Library_Id { get; set; }
    
    //extra - Not officially supported by Navidrome's documentation
    public string? Genre { get; set; }
    public bool? ArtistLoved { get; set; }
    public bool? AlbumLoved { get; set; }
    
    
    private Dictionary<string, object?> _activeFields;
    public Dictionary<string, object?> ActiveFields
    {
        get
        {
            if (_activeFields != null)
            {
                return _activeFields;
            }

            var hmm = this.GetType()
                .GetProperties();

            _activeFields = this.GetType()
                .GetProperties()
                .Where(prop => prop.Name != "ActiveFields")
                .Select(prop => new
                {
                    Name = prop.Name,
                    Value = prop.GetValue(this)
                })
                .Where(val => val.Value != null)
                .ToDictionary(key => key.Name, val => val.Value);
            return _activeFields;
        }
    }
}