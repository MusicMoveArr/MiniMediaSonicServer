using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Models.Database;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Services;

public class AutoRateAlbumsService
{
    private readonly UserRepository _userRepository;
    private readonly UserPropertyRepository _userPropertyRepository;
    private readonly AutoRateAlbumsRepository _autoRateAlbumsRepository;
    private readonly RatingRepository _ratingRepository;
    
    public AutoRateAlbumsService(UserRepository userRepository,
        UserPropertyRepository userPropertyRepository,
        AutoRateAlbumsRepository autoRateAlbumsRepository,
        RatingRepository ratingRepository)
    {
        _userRepository = userRepository;
        _userPropertyRepository = userPropertyRepository;
        _autoRateAlbumsRepository = autoRateAlbumsRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task RateAlbumsAsync()
    {
        var userIds = await _userRepository.GetAllUserIdsAsync();
        foreach (var userId in userIds)
        {
            await RateAlbumsAsync(userId);
        }
    }

    public async Task RateAlbumsAsync(Guid userId)
    {
        bool enabled = await _userPropertyRepository.GetUserPropertyBoolAsync(userId, "AutoRate_Albums_Enabled");
        if (!enabled)
        {
            return;
        }

        int.TryParse(await _userPropertyRepository.GetUserPropertyAsync(userId, "AutoRate_Albums_MinimumRating"), out int minRating);
        int.TryParse(await _userPropertyRepository.GetUserPropertyAsync(userId, "AutoRate_Albums_TracksRatedPercentage"), out int tracksRatedPercentage);

        if (minRating <= 0 || tracksRatedPercentage <= 0)
        {
            return;
        }

        List<AlbumRatingModel> albumRatings = await _autoRateAlbumsRepository.GetAlbumsToRateAsync(userId, minRating, tracksRatedPercentage);
        foreach (AlbumRatingModel rating in albumRatings)
        {
            await _ratingRepository.RateAlbumAsync(userId, rating.AlbumId, rating.AvgRating);
        }
    }
}