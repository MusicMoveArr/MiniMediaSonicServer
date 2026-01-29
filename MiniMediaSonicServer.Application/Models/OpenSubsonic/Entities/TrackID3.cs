using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class TrackID3
{
    [XmlAttribute("id")]
    [JsonPropertyName("id")]
    public Guid TrackId { get; set; }
    
    [XmlAttribute("parent")]
    [JsonPropertyName("parent")]
    public Guid Parent { get; set; }

    [XmlAttribute("isDir")]
    [JsonPropertyName("isDir")]
    public bool IsDir { get; set; } = false;

    [XmlAttribute("title")]
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [XmlAttribute("album")]
    [JsonPropertyName("album")]
    public string Album { get; set; }

    [XmlAttribute("artist")]
    [JsonPropertyName("artist")]
    public string Artist { get; set; }

    [XmlAttribute("track")]
    [JsonPropertyName("track")]
    public int TrackNumber { get; set; }

    [XmlAttribute("year")]
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [XmlAttribute("genre")]
    [JsonPropertyName("genre")]
    public string Genre { get; set; }

    [XmlAttribute("coverArt")]
    [JsonPropertyName("coverArt")]
    public string CoverArt { get; set; }

    [XmlAttribute("size")]
    [JsonPropertyName("size")]
    public long Size { get; set; }

    [XmlAttribute("contentType")]
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }

    [XmlAttribute("suffix")]
    [JsonPropertyName("suffix")]
    public string Suffix { get; set; }

    [XmlAttribute("transcodedContentType")]
    [JsonPropertyName("transcodedContentType")]
    public string TranscodedContentType { get; set; }

    [XmlAttribute("transcodedSuffix")]
    [JsonPropertyName("transcodedSuffix")]
    public string TranscodedSuffix { get; set; }

    [XmlAttribute("duration")]
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [XmlAttribute("bitRate")]
    [JsonPropertyName("bitRate")]
    public int BitRate { get; set; }

    [XmlAttribute("bitDepth")]
    [JsonPropertyName("bitDepth")]
    public int BitDepth { get; set; }

    [XmlAttribute("samplingRate")]
    [JsonPropertyName("samplingRate")]
    public int SamplingRate { get; set; }

    [XmlAttribute("channelCount")]
    [JsonPropertyName("channelCount")]
    public int ChannelCount { get; set; }

    [XmlAttribute("path")]
    [JsonPropertyName("path")]
    public string Path { get; set; }

    [XmlAttribute("isVideo")]
    [JsonPropertyName("isVideo")]
    public bool IsVideo { get; set; }

    [XmlAttribute("userRating")]
    [JsonPropertyName("userRating")]
    public int UserRating { get; set; }

    [XmlAttribute("averageRating")]
    [JsonPropertyName("averageRating")]
    public int AverageRating { get; set; }

    [XmlAttribute("playCount")]
    [JsonPropertyName("playCount")]
    public long PlayCount { get; set; }

    [XmlAttribute("discNumber")]
    [JsonPropertyName("discNumber")]
    public int DiscNumber { get; set; }

    [XmlAttribute("created")]
    [JsonPropertyName("created")]
    public string Created { get; set; }

    [XmlAttribute("starred")]
    [JsonPropertyName("starred")]
    public string Starred { get; set; }

    [XmlAttribute("albumId")]
    [JsonPropertyName("albumId")]
    public Guid AlbumId { get; set; }

    [XmlAttribute("artistId")]
    [JsonPropertyName("artistId")]
    public Guid ArtistId { get; set; }

    [XmlAttribute("type")]
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [XmlAttribute("mediaType")]
    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; }

    [XmlAttribute("bookmarkPosition")]
    [JsonPropertyName("bookmarkPosition")]
    public long BookmarkPosition { get; set; }

    [XmlAttribute("originalWidth")]
    [JsonPropertyName("originalWidth")]
    public int OriginalWidth { get; set; }

    [XmlAttribute("originalHeight")]
    [JsonPropertyName("originalHeight")]
    public int OriginalHeight { get; set; }

    [XmlAttribute("played")]
    [JsonPropertyName("played")]
    public string Played { get; set; }

    [XmlAttribute("bpm")]
    [JsonPropertyName("bpm")]
    public int Bpm { get; set; }

    [XmlAttribute("comment")]
    [JsonPropertyName("comment")]
    public string Comment { get; set; }

    [XmlAttribute("sortName")]
    [JsonPropertyName("sortName")]
    public string SortName { get; set; }

    [XmlAttribute("musicBrainzId")]
    [JsonPropertyName("musicBrainzId")]
    public Guid MusicBrainzId { get; set; }

    [XmlElement("isrc")]
    [JsonPropertyName("isrc")]
    public List<string> Isrc { get; set; } = new List<string>();

    [XmlElement("genres")]
    [JsonPropertyName("genres")]
    public List<NameEntity> Genres { get; set; } = new List<NameEntity>();

    [XmlElement("artists")]
    [JsonPropertyName("artists")]
    public List<NameIdEntity> Artists { get; set; } = new List<NameIdEntity>();

    [XmlAttribute("displayArtist")]
    [JsonPropertyName("displayArtist")]
    public string DisplayArtist { get; set; }

    [XmlElement("albumArtists")]
    [JsonPropertyName("albumArtists")]
    public List<NameIdEntity> AlbumArtists { get; set; } = new List<NameIdEntity>();

    [XmlAttribute("displayAlbumArtist")]
    [JsonPropertyName("displayAlbumArtist")]
    public string DisplayAlbumArtist { get; set; }

    [XmlAttribute("contributors")]
    [JsonPropertyName("contributors")]
    public string Contributors { get; set; }

    [XmlAttribute("displayComposer")]
    [JsonPropertyName("displayComposer")]
    public string DisplayComposer { get; set; }

    [XmlElement("moods")]
    [JsonPropertyName("moods")]
    public List<string> Moods { get; set; } = new List<string>();

    [XmlElement("replayGain")]
    [JsonPropertyName("replayGain")]
    public ReplayGain ReplayGain { get; set; }

    [XmlAttribute("explicitStatus")]
    [JsonPropertyName("explicitStatus")]
    public string ExplicitStatus { get; set; }
    
    [JsonIgnore]
    public string Isrc_Single { get; set; }
}