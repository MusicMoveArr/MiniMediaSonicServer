using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class RatingService
{
    private readonly RatingRepository _ratingRepository;

    public RatingService(RatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task RateTrackAsync(Guid userUd, Guid trackId, int rating)
    {
        await _ratingRepository.RateTrackAsync(userUd, trackId, rating);
    }

    public async Task StarTrackAsync(Guid userId, Guid trackId, bool star)
    {
        await _ratingRepository.StarTrackAsync(userId, trackId, star);
    }

    public async Task RateArtistAsync(Guid userUd, Guid artistId, int rating)
    {
        await _ratingRepository.RateArtistAsync(userUd, artistId, rating);
    }

    public async Task StarArtistAsync(Guid userId, Guid artistId, bool star)
    {
        await _ratingRepository.StarArtistAsync(userId, artistId, star);
    }

    public async Task RateAlbumAsync(Guid userUd, Guid albumId, int rating)
    {
        await _ratingRepository.RateAlbumAsync(userUd, albumId, rating);
    }

    public async Task StarAlbumAsync(Guid userId, Guid albumId, bool star)
    {
        await _ratingRepository.StarAlbumAsync(userId, albumId, star);
    }
}