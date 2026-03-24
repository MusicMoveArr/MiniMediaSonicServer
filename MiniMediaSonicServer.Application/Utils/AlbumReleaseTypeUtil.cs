using FuzzySharp;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Utils;

public static class AlbumReleaseTypeUtil
{
    private static readonly string[] LiveKeywords = ["live", "concert", "in concert", "at the"];
    private static readonly string[] RemixKeywords = ["remix", "remixed", "edit", "extended mix", "club mix", "radio edit"];
    private static readonly string[] CompilationKeywords = ["greatest hits", "best of", "collection"];
    private static readonly string[] DemoKeywords = ["demo", "rehearsal"];
    private static readonly string[] AcousticKeywords = ["acoustic"];
    private static readonly string[] InstrumentalKeywords = ["instrumental"];
    
    public static void SetAlbumReleaseTypes(List<AlbumID3>? albums)
    {
        foreach (var album in albums ?? [])
        {
            SetAlbumReleaseTypes(album);
        }
    }
    
    public static void SetAlbumReleaseTypes(AlbumID3? album)
    {
        if (album != null)
        {
            album.releaseTypes = GetAlbumReleaseTypes(album);
        }
    }
    
    public static List<string> GetAlbumReleaseTypes(AlbumID3 album)
    {
        var releaseTypes = new List<string>();
        
        if (IsSingle(album))
        {
            releaseTypes.Add("single");
        }
        else if (CompilationKeywords.Any(keyword => album.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
            album.Song.Select(track => track.Artist).Distinct().Count() > 3 ||
            album.Song.SelectMany(track => track.AlbumArtists).Distinct().Count() > 3)
        {
            releaseTypes.Add("compilation");
        }
        else
        {
            //find it hard atm to say what "defines" what "is" an album
            releaseTypes.Add("album");
        }
        
        if (LiveKeywords.Any(keyword => album.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
            LiveKeywords.Any(keyword => album.Song.Any(track => track.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))))
        {
            releaseTypes.Add("live");
        }
        if (RemixKeywords.Any(keyword => album.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
            RemixKeywords.Any(keyword => album.Song.Any(track => track.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))))
        {
            releaseTypes.Add("remix");
        }
        if (DemoKeywords.Any(keyword => album.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
            DemoKeywords.Any(keyword => album.Song.Any(track => track.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))))
        {
            releaseTypes.Add("demo");
        }
        if (AcousticKeywords.Any(keyword => album.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
            AcousticKeywords.Any(keyword => album.Song.Any(track => track.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))))
        {
            releaseTypes.Add("acoustic");
        }
        if (InstrumentalKeywords.Any(keyword => album.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
            InstrumentalKeywords.Any(keyword => album.Song.Any(track => track.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))))
        {
            releaseTypes.Add("instrumental");
        }
        
        return releaseTypes;
    }

    private static bool IsSingle(AlbumID3 album)
    {
        if (!album.Song.Any())
        {
            return false;
        }
        if (album.Song.Count == 1)
        {
            return true;
        }
        
        TrackID3 firstTrack = album.Song.First();
        string trackTitle = firstTrack.Title;
        
        if (trackTitle.Contains("(") || trackTitle.Contains("["))
        {
            int index = Math.Max(trackTitle.IndexOf('('), trackTitle.IndexOf('['));
            trackTitle = trackTitle.Substring(0, index);
        }

        int percentageSum = album.Song
            .Skip(1)
            .Sum(song => Fuzz.PartialRatio(trackTitle, song.Title));

        return percentageSum / (album.Song.Count - 1) > 90;
    }
}