using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.Playlists.Application.Models.Database;
using MiniMediaSonicServer.WebJob.Playlists.Application.Repositories;
using PlaylistsNET.Content;
using PlaylistsNET.Models;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Services;

public class PlaylistImportService
{
    private readonly UserRepository _userRepository;
    private readonly PlaylistRepository _playlistRepository;
    private readonly PlaylistImportRepository _playlistImportRepository;
    private readonly TrackRepository _trackRepository;
    
    public PlaylistImportService(UserRepository userRepository,
        PlaylistRepository playlistRepository,
        PlaylistImportRepository playlistImportRepository,
        TrackRepository trackRepository)
    {
        _userRepository = userRepository;
        _playlistRepository = playlistRepository;
        _playlistImportRepository = playlistImportRepository;
        _trackRepository = trackRepository;
    }

    public async Task ImportPlaylistPathAsync(string playlistPath)
    {
        FileInfo fileInfo = new FileInfo(playlistPath);
        using FileStream stream = fileInfo.OpenRead();
        var parser = PlaylistParserFactory.GetPlaylistParser(fileInfo.Extension);
        IBasePlaylist playlist = parser.GetFromStream(stream);
        List<string> paths = playlist.GetTracksPaths();
        string playlistName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));

        var importedPlaylist = await _playlistImportRepository.GetImportedPlaylistByPathAsync(playlistPath);
        Guid importId = importedPlaylist?.ImportId ??
                        await _playlistImportRepository.InsertPlaylistImportAsync(
                            playlistPath, 
                            true, 
                            playlistName,
                            fileInfo.LastWriteTime);

        var userIds = await _userRepository.GetAllUserIdsAsync();
        foreach (var userId in userIds)
        {
            PlaylistImportUserModel? userPlaylistImport = await _playlistImportRepository.GetPlaylistImportUserAsync(importId, userId);
            Guid? userPlaylistId = userPlaylistImport?.PlaylistId;
            if (!userPlaylistId.HasValue)
            {
                userPlaylistId = await _playlistRepository.CreatePlaylistAsync(userId, playlistName);
                await _playlistImportRepository.InsertPlaylistImportUserAsync(importId, userId, userPlaylistId.Value);
            }

            foreach (string path in paths)
            {
                Guid? trackId = await _trackRepository.GetTrackIdByPathAsync(path);
                if (!trackId.HasValue)
                {
                    continue;
                }
                bool alreadyExists = await _playlistRepository.TrackExistsInPlaylistAsync(userPlaylistId.Value, trackId.Value);
                if (alreadyExists)
                {
                    continue;
                }

                await _playlistRepository.AddTrackToPlaylistAsync(userPlaylistId.Value, trackId.Value);
            }
        }
    }
}