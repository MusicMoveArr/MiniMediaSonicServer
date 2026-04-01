using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Services;

public class AutoLikeService
{
    private readonly UserRepository _userRepository;
    private readonly UserPropertyRepository _userPropertyRepository;
    private readonly AutoLikeArtistRepository _autoLikeArtistRepository;
    private readonly RatingRepository _ratingRepository;
    
    public AutoLikeService(UserRepository userRepository,
        UserPropertyRepository userPropertyRepository,
        AutoLikeArtistRepository autoLikeArtistRepository,
        RatingRepository ratingRepository)
    {
        _userRepository = userRepository;
        _userPropertyRepository = userPropertyRepository;
        _autoLikeArtistRepository = autoLikeArtistRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task FavoriteArtistsAsync()
    {
        var userIds = await _userRepository.GetAllUserIdsAsync();
        foreach (var userId in userIds)
        {
            await FavoriteArtistsAsync(userId);
        }
    }

    public async Task FavoriteArtistsAsync(Guid userId)
    {
        bool enabled = await _userPropertyRepository.GetUserPropertyBoolAsync(userId, "AutoLike_Artists_Enabled");
        if (!enabled)
        {
            return;
        }

        int daysRecent = 0;
        int listenCount = 0;

        int.TryParse(await _userPropertyRepository.GetUserPropertyAsync(userId, "AutoLike_Artists_DaysRecent"), out daysRecent);
        int.TryParse(await _userPropertyRepository.GetUserPropertyAsync(userId, "AutoLike_Artists_ListenCount"), out listenCount);

        if (daysRecent <= 0 || listenCount <= 0)
        {
            return;
        }

        List<Guid> artistIds = await _autoLikeArtistRepository.GetArtistIdToLikeAsync(userId, daysRecent, listenCount);
        foreach (Guid artistId in artistIds)
        {
            await _ratingRepository.StarArtistAsync(userId, artistId, true);
        }
    }
}