using System.Diagnostics;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Repositories;
using Index = MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities.Index;

namespace MiniMediaSonicServer.Application.Services;

public class ArtistService
{
    private readonly ArtistRepository _artistRepository;

    public ArtistService(ArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public async Task<ArtistID3> GetArtistByIdAsync(Guid artistId, Guid userId)
    {
        return await _artistRepository.GetArtistByIdAsync(artistId);
    }

    public async Task<ArtistsList> GetAllArtistsAsync(Guid userId)
    {
        var allArtists = await _artistRepository.GetAllArtistsAsync(userId);

        var artistsIndexes = allArtists
            .Select(artist => new
            {
                Artist = artist,
                Key = GetIndexKey(artist.Name)
            })
            .GroupBy(index => index.Key)
            .Select(index => new Index
            {
                Artist = index.Select(artist => artist.Artist).ToList(),
                Name = index.Key
            })
            .ToList();

        var artists = new ArtistsList
        {
            Index = artistsIndexes,
            IgnoredArticles = string.Join(' ', ArtistID3.IgnoredArticles)
        };
        return artists;
    }
    
    private string GetIndexKey(string name)
    {
        name = name.TrimStart();
        
        string? ignoreArticle = name.Length > 3 ? ArtistID3.IgnoredArticles.FirstOrDefault(n => name.ToLower().StartsWith(n.ToLower())) : string.Empty;
        if (ignoreArticle != null)
        {
            name = name.Substring(ignoreArticle.Length + 1);
        }
        
        if (string.IsNullOrWhiteSpace(name))
        {
            return "#";
        }
        
        var firstChar = char.ToUpperInvariant(name[0]);

        if (firstChar >= 'A' && firstChar <= 'Z')
        {
            return firstChar.ToString();
        }

        return "#";
    }
    public async Task<List<ArtistID3>> GetStarredArtistsAsync(Guid userId)
    {
        return await _artistRepository.GetStarredArtistsAsync(userId);
    }
}