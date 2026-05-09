using System.Globalization;
using MiniMediaSonicServer.Application.Helpers;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Models;

public class MusicCacheTrackModel
{
    public const string VariousArtistsName = "Various Artists";
    private const int MaxFilePartNameLength = 80;

    public string Artist
    {
        get => _track.Artist;
        set => _track.Artist = value;
    }
    
    public string SortArtist => _track.Artist;
    public string Title
    {
        get => _track.Title;
        set => _track.Title = value;
    }
    public string Album
    {
        get => _track.Album;
        set => _track.Album = value;
    }

    public double BitRate => _track.BitRate;
    public int Duration => _track.Duration;
    public int Year => _track.Year ?? 0;
    public string ISRC => _track.Isrc_Single;
    public int DiscNumber => _track.DiscNumber;
    public int TrackNumber => _track.TrackNumber;
    
    public string CleanArtist
    {
        get
        {
            string? artist = Artist;
            
            if (string.IsNullOrWhiteSpace(artist) ||
                (artist.Contains(VariousArtistsName) && !string.IsNullOrWhiteSpace(Artist)))
            {
                artist = Artist;
            }
        
            if (string.IsNullOrWhiteSpace(artist) ||
                (artist.Contains(VariousArtistsName) && !string.IsNullOrWhiteSpace(SortArtist)))
            {
                artist = SortArtist;
            }
            
            artist = ArtistHelper.GetUncoupledArtistName(artist);
            artist = ArtistHelper.GetShortWordVersion(artist, MaxFilePartNameLength);
            return artist
                .Replace('/', '+')
                .Replace('\\', '+');
        }
    }
    
    public string CleanAlbum
    {
        get
        {
            string albumName = ArtistHelper.GetShortWordVersion(Album, MaxFilePartNameLength);
            return albumName
                .Replace('/', '+')
                .Replace('\\', '+');
        }
    }
    public string CleanArtistUpper => CleanArtist.ToUpper();
    public string CleanAlbumUpper => CleanAlbum.ToUpper();
    public string ArtistUpper => Artist.ToUpper();
    public string AlbumUpper => Album.ToUpper();
    public string FileExtension { get; private set; }
    private readonly TrackID3 _track;

    public MusicCacheTrackModel(string sourceFilePath, TrackID3 track)
    {
        FileExtension = new FileInfo(sourceFilePath).Extension;
        _track = track;
    }
    
}