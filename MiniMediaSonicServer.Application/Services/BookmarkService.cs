using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class BookmarkService
{
    private readonly BookmarkRepository _bookmarkRepository;
    private readonly SearchService _searchService;
    private readonly TrackService _trackService;
    public BookmarkService(BookmarkRepository bookmarkRepository,
        SearchService searchService,
        TrackService trackService)
    {
        _bookmarkRepository = bookmarkRepository;
        _searchService = searchService;
        _trackService = trackService;
    }

    public async Task<bool> CreateBookmarkAsync(Guid userId, Guid trackId, long position, string comment)
    {
        ID3Type? type = await _searchService.GetID3TypeAsync(trackId);
        if (type != ID3Type.Track)
        {
            return false;
        }
        await _bookmarkRepository.UpsertBookmarkAsync(userId, trackId, position, comment);
        
        return true;
    }

    public async Task<List<Bookmark>> GetBookmarksAsync(Guid userId)
    {
        var bookmarkModels = await _bookmarkRepository.GetBookmarksByUserIdAsync(userId);
        var trackIds = bookmarkModels
            .Select(b => b.TrackId)
            .Distinct()
            .ToList();
        var tracks = await _trackService.GetTrackByIdAsync(trackIds, userId);

        var bookmarks = bookmarkModels.Select(bookmark => 
            new Bookmark
            {
                Track = tracks.FirstOrDefault(t => t.TrackId == bookmark.TrackId),
                Position = bookmark.Position,
                Comment = bookmark.Comment,
                Username = bookmark.Username,
                CreatedAt = bookmark.CreatedAt,
                UpdatedAt = bookmark.UpdatedAt
            })
            .Where(bookmark => bookmark.Track != null)
            .ToList();
        return bookmarks;
    }

    public async Task DeleteBookmarkAsync(Guid userId, Guid trackId)
    {
        await _bookmarkRepository.DeleteBookmarkAsync(userId, trackId);
    }
}