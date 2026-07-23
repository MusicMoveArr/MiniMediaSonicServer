using FuzzySharp;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Models.Database;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Services;

public class AutoRateDuplicateTracksService
{
    private readonly UserRepository _userRepository;
    private readonly UserPropertyRepository _userPropertyRepository;
    private readonly AutoRateDuplicateTracksRepository _autoRateDuplicateTracksRepository;
    private readonly RatingRepository _ratingRepository;
    private readonly FingerPrintService _fingerPrintService;
    
    public AutoRateDuplicateTracksService(UserRepository userRepository,
        UserPropertyRepository userPropertyRepository,
        AutoRateDuplicateTracksRepository autoRateDuplicateTracksRepository,
        RatingRepository ratingRepository,
        FingerPrintService fingerPrintService)
    {
        _userRepository = userRepository;
        _userPropertyRepository = userPropertyRepository;
        _autoRateDuplicateTracksRepository = autoRateDuplicateTracksRepository;
        _ratingRepository = ratingRepository;
        _fingerPrintService = fingerPrintService;
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
        bool enabled = await _userPropertyRepository.GetUserPropertyBoolAsync(userId, "AutoRate_DuplicateTracks_Enabled");
        if (!enabled)
        {
            return;
        }

        List<Guid> alreadyProcessedTrack = new List<Guid>();
        List<RateDuplicateTrackModel> trackRatings = await _autoRateDuplicateTracksRepository.GetRatedTracksAsync(userId);
        var grouped = trackRatings.GroupBy(grouped => grouped.ArtistName);
        
        foreach (var artistRatedTracks in grouped)
        {
            var tracks = await _autoRateDuplicateTracksRepository.TracksByArtistNameAsync(userId, artistRatedTracks.Key);

            foreach (var ratedTrack in artistRatedTracks)
            {
                var similarTracks = tracks
                    .AsParallel()
                    .Where(track => track.MetadataId != ratedTrack.MetadataId)
                    .Where(track => !alreadyProcessedTrack.Contains(track.MetadataId))
                    .Where(track => Fuzz.Ratio(track.TrackTitle.ToLower(), ratedTrack.TrackTitle.ToLower()) > 70)
                    .Select(track => new
                    {
                        Track = track,
                        similarity = _fingerPrintService.DTWSimilarity(
                            ratedTrack.AcoustIdFingerprintData,
                            track.AcoustIdFingerprintData)
                    })
                    .Where(track => track.similarity >= 0.99)
                    .ToList();

                if (similarTracks.Any())
                {
                    var lastUpdatedRatings = similarTracks
                        .Select(t => new
                        {
                            TrackId = t.Track.MetadataId,
                            Rating = t.Track.Rating,
                            Title = t.Track.TrackTitle,
                            LastUpdatedAt = t.Track.RatedAt
                        })
                        .Union([
                            new
                            {
                                TrackId = ratedTrack.MetadataId,
                                Rating = ratedTrack.Rating,
                                Title = ratedTrack.TrackTitle,
                                LastUpdatedAt = ratedTrack.RatedAt
                            }
                        ])
                        .OrderByDescending(t => t.LastUpdatedAt)
                        .ToList();

                    var masterRating = lastUpdatedRatings.FirstOrDefault();

                    if (masterRating == null || masterRating?.LastUpdatedAt.HasValue == false)
                    {
                        continue;
                    }
                    var duplicateTracks = lastUpdatedRatings.Skip(1).ToList();
                    
                    foreach (var rating in duplicateTracks.Where(t => t.Rating != masterRating!.Rating))
                    {
                        await _ratingRepository.RateTrackAsync(userId, rating.TrackId, masterRating!.Rating);
                    }
                    
                    alreadyProcessedTrack.Add(ratedTrack.MetadataId);
                    alreadyProcessedTrack.AddRange(similarTracks.Select(t => t.Track.MetadataId));
                }
            }
        }
    }
}