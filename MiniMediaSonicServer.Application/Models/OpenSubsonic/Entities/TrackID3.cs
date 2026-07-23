using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

public class TrackID3
{
    [XmlAttribute("id")]
    [JsonPropertyName("id")]
    public required Guid TrackId { get; set; }
    
    [XmlAttribute("parent")]
    [JsonPropertyName("parent")]
    public required Guid Parent { get; set; }

    [XmlAttribute("isDir")]
    [JsonPropertyName("isDir")]
    public required bool IsDir { get; set; } = false;

    [XmlAttribute("title")]
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [XmlAttribute("album")]
    [JsonPropertyName("album")]
    public required string Album { get; set; }

    [XmlAttribute("artist")]
    [JsonPropertyName("artist")]
    public required string Artist { get; set; }

    [XmlAttribute("track")]
    [JsonPropertyName("track")]
    public required int TrackNumber { get; set; }

    [XmlElement("year")]
    [JsonPropertyName("year")]
    public required int? Year { get; set; }

    [XmlAttribute("genre")]
    [JsonPropertyName("genre")]
    public required string Genre { get; set; }

    [XmlAttribute("coverArt")]
    [JsonPropertyName("coverArt")]
    public required string CoverArt { get; set; }

    [XmlAttribute("size")]
    [JsonPropertyName("size")]
    public required long Size { get; set; }

    [XmlAttribute("contentType")]
    [JsonPropertyName("contentType")]
    public required string ContentType { get; set; }

    [XmlAttribute("suffix")]
    [JsonPropertyName("suffix")]
    public required string Suffix { get; set; }

    [XmlAttribute("transcodedContentType")]
    [JsonPropertyName("transcodedContentType")]
    public required string TranscodedContentType { get; set; }

    [XmlAttribute("transcodedSuffix")]
    [JsonPropertyName("transcodedSuffix")]
    public required string TranscodedSuffix { get; set; }

    [XmlAttribute("duration")]
    [JsonPropertyName("duration")]
    public required int Duration { get; set; }

    [XmlAttribute("bitRate")]
    [JsonPropertyName("bitRate")]
    public required int BitRate { get; set; }

    [XmlAttribute("bitDepth")]
    [JsonPropertyName("bitDepth")]
    public required int BitDepth { get; set; }

    [XmlAttribute("samplingRate")]
    [JsonPropertyName("samplingRate")]
    public required int SamplingRate { get; set; }

    [XmlAttribute("channelCount")]
    [JsonPropertyName("channelCount")]
    public required int ChannelCount { get; set; }

    [XmlAttribute("path")]
    [JsonPropertyName("path")]
    public required string Path { get; set; }

    [XmlAttribute("isVideo")]
    [JsonPropertyName("isVideo")]
    public required bool IsVideo { get; set; }

    [XmlAttribute("userRating")]
    [JsonPropertyName("userRating")]
    public required int UserRating { get; set; }

    [XmlAttribute("averageRating")]
    [JsonPropertyName("averageRating")]
    public required int AverageRating { get; set; }

    [XmlAttribute("playCount")]
    [JsonPropertyName("playCount")]
    public required long PlayCount { get; set; }

    [XmlAttribute("discNumber")]
    [JsonPropertyName("discNumber")]
    public required int DiscNumber { get; set; }

    [XmlAttribute("created")]
    [JsonPropertyName("created")]
    public required DateTime Created { get; set; }

    [XmlElement("starred")]
    [JsonPropertyName("starred")]
    public required DateTime? Starred { get; set; }

    [XmlAttribute("albumId")]
    [JsonPropertyName("albumId")]
    public required Guid AlbumId { get; set; }

    [XmlAttribute("artistId")]
    [JsonPropertyName("artistId")]
    public required Guid ArtistId { get; set; }

    [XmlAttribute("type")]
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [XmlAttribute("mediaType")]
    [JsonPropertyName("mediaType")]
    public required string MediaType { get; set; }

    [XmlAttribute("bookmarkPosition")]
    [JsonPropertyName("bookmarkPosition")]
    public required long BookmarkPosition { get; set; }

    [XmlAttribute("originalWidth")]
    [JsonPropertyName("originalWidth")]
    public required int OriginalWidth { get; set; }

    [XmlAttribute("originalHeight")]
    [JsonPropertyName("originalHeight")]
    public required int OriginalHeight { get; set; }

    [XmlElement("played")]
    [JsonPropertyName("played")]
    public required DateTime? Played { get; set; }

    [XmlAttribute("bpm")]
    [JsonPropertyName("bpm")]
    public required int Bpm { get; set; }

    [XmlAttribute("comment")]
    [JsonPropertyName("comment")]
    public required string Comment { get; set; }

    [XmlAttribute("sortName")]
    [JsonPropertyName("sortName")]
    public required string SortName { get; set; }

    [XmlElement("musicBrainzId")]
    [JsonPropertyName("musicBrainzId")]
    public required Guid? MusicBrainzId { get; set; }

    [XmlElement("isrc")]
    [JsonPropertyName("isrc")]
    public required List<string> Isrc { get; set; } = new List<string>();

    [XmlElement("genres")]
    [JsonPropertyName("genres")]
    public required List<NameEntity> Genres { get; set; } = new List<NameEntity>();

    [XmlElement("artists")]
    [JsonPropertyName("artists")]
    public required List<NameIdEntity> Artists { get; set; } = new List<NameIdEntity>();

    [XmlAttribute("displayArtist")]
    [JsonPropertyName("displayArtist")]
    public required string DisplayArtist { get; set; }

    [XmlElement("albumArtists")]
    [JsonPropertyName("albumArtists")]
    public required List<NameIdEntity> AlbumArtists { get; set; } = new List<NameIdEntity>();

    [XmlAttribute("displayAlbumArtist")]
    [JsonPropertyName("displayAlbumArtist")]
    public required string DisplayAlbumArtist { get; set; }

    [XmlAttribute("contributors")]
    [JsonPropertyName("contributors")]
    public required string Contributors { get; set; }

    [XmlAttribute("displayComposer")]
    [JsonPropertyName("displayComposer")]
    public required string DisplayComposer { get; set; }

    [XmlElement("moods")]
    [JsonPropertyName("moods")]
    public required List<string> Moods { get; set; } = new List<string>();

    [XmlElement("replayGain")]
    [JsonPropertyName("replayGain")]
    public required ReplayGain ReplayGain { get; set; }

    [XmlAttribute("explicitStatus")]
    [JsonPropertyName("explicitStatus")]
    public required string ExplicitStatus { get; set; }
    
    [JsonIgnore]
    [XmlIgnore]
    public string Isrc_Single { get; set; }
}