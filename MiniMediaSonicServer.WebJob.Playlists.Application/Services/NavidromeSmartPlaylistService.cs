using System.Text;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.Playlists.Application.Models.Database;
using MiniMediaSonicServer.WebJob.Playlists.Application.Models.Navidrome.SmartPlaylist;
using MiniMediaSonicServer.WebJob.Playlists.Application.Repositories;
using Newtonsoft.Json;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Services;

public class NavidromeSmartPlaylistService
{
    private readonly NavidromeSmartPlaylistRepository _navidromeSmartPlaylistRepository;
    private readonly PlaylistImportRepository _playlistImportRepository;
    private readonly UserRepository _userRepository;
    private readonly PlaylistRepository _playlistRepository;

    private readonly Dictionary<string, object> _parameters;
    
    public NavidromeSmartPlaylistService(
        NavidromeSmartPlaylistRepository navidromeSmartPlaylistRepository,
        PlaylistImportRepository playlistImportRepository,
        UserRepository userRepository,
        PlaylistRepository playlistRepository)
    {
        _navidromeSmartPlaylistRepository = navidromeSmartPlaylistRepository;
        _playlistImportRepository = playlistImportRepository;
        _userRepository = userRepository;
        _playlistRepository = playlistRepository;
        _parameters = new Dictionary<string, object>();
    }

    public async Task ProcessNavidromeSmartPlaylist(string playlistPath)
    {
        _parameters.Clear();
        FileInfo fileInfo = new FileInfo(playlistPath);
        var nsp = JsonConvert.DeserializeObject<SmartPlaylistModel>(File.ReadAllText(playlistPath));
        
        StringBuilder filter = new StringBuilder();
        
        foreach(var op in nsp.All)
        {
            if (op.Any?.Any() == true)
            {
                bool addQueryOperator = false;
                filter.Append("(");
                foreach (var anyOp in op.Any)
                {
                    ProcessOperator(filter, anyOp, true, addQueryOperator);
                    addQueryOperator = true;
                }
                filter.Append(")");
            }
            else
            {
                ProcessOperator(filter, op, false, false);
            }
        }
        
        var importedPlaylist = await _playlistImportRepository.GetImportedPlaylistByPathAsync(playlistPath);
        Guid importId = importedPlaylist?.ImportId ??
                        await _playlistImportRepository.InsertPlaylistImportAsync(
                            playlistPath, 
                            true, 
                            nsp.Name,
                            fileInfo.LastWriteTime);
        
        
        var userIds = await _userRepository.GetAllUserIdsAsync();
        foreach (var userId in userIds)
        {
            PlaylistImportUserModel? userPlaylistImport = await _playlistImportRepository.GetPlaylistImportUserAsync(importId, userId);
            Guid? userPlaylistId = userPlaylistImport?.PlaylistId;
            if (!userPlaylistId.HasValue)
            {
                userPlaylistId = await _playlistRepository.CreatePlaylistAsync(userId, nsp.Name);
                await _playlistImportRepository.InsertPlaylistImportUserAsync(importId, userId, userPlaylistId.Value);
            }
            
            string sortBy = nsp.Sort == "random" ? "random" :
                string.Join(',', nsp.Sort
                .Replace("-", string.Empty)
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(order => GetDbColumn(order))
                .Where(order => !string.IsNullOrWhiteSpace(order)));
            
            var trackIds = await _navidromeSmartPlaylistRepository.GetTrackIdsForPlaylistAsync(
                userId,
                filter.ToString(), 
                _parameters, 
                sortBy,
                nsp.Order, 
                nsp.Limit);

            await _playlistRepository.DeleteAllTracksFromPlaylistAsync(userPlaylistId.Value);
            
            foreach (var trackId in trackIds.Distinct())
            {
                await _playlistRepository.AddTrackToPlaylistAsync(userPlaylistId.Value, trackId);
            }

            await _playlistRepository.UpdatePlaylistUpdatedAtAsync(userPlaylistId.Value, DateTime.Now);
        }
    }

    private void ProcessOperator(
        StringBuilder filter, 
        Operator op,
        bool isAnyOperator,
        bool addQueryOperator)
    {
        if (_parameters.Count > 0 && !isAnyOperator)
        {
            filter.Append(" and ");
        }
        
        foreach(var opField in op.ActiveOperatorFields)
        {
            foreach (var activeField in opField.ActiveFields)
            {
                string queryFilter = GetOperatorFilter(activeField.Key, activeField.Value, op.OperatorType);
                if (!string.IsNullOrEmpty(queryFilter))
                {
                    if (addQueryOperator)
                    {
                        filter.Append($" {(isAnyOperator ? "or" : "and")} ");
                    }
                            
                    filter.Append(queryFilter);
                    addQueryOperator = true;
                }
            }
        }
    }

    private string AddParameter(object value)
    {
        string paramName = $"@param{_parameters.Count}";
        _parameters.Add(paramName, value);
        return paramName;
    }

    private string GetDbColumn(string propertyName)
    {
        switch (propertyName.ToLower())
        {
            case "title": return "m.Title";
            case "album": return "al.Title";
            case "hascoverart": return string.Empty;
            case "tracknumber": return "m.Tag_Track";
            case "discnumber": return "m.Tag_Disc";
            case "year": return "m.Tag_Year";
            case "date": return string.Empty;
            case "originalyear": return "m.Tag_Year";
            case "originaldate": return string.Empty;
            case "releaseyear": return "m.Tag_Year";
            case "releasedate": return string.Empty;
            case "size": return string.Empty;
            case "compilation": return string.Empty;
            case "dateadded": return "m.File_CreationTime";
            case "datemodified": return "m.File_LastWriteTime";
            case "discsubtitle": return string.Empty;
            case "comment": return "t.tags->>'comment'";
            case "lyrics": return "t.tags->>'lyrics'";
            case "sorttitle": return "COALESCE(t.tags->>'sorttitle', t.tags->>'titlesort')";
            case "sortalbum": return "COALESCE(t.tags->>'sortalbum', t.tags->>'albumsort')";
            case "sortartist": return "COALESCE(t.tags->>'sortartist', t.tags->>'artistsort')";
            case "sortalbumartist": return "t.tags->>'sortalbumartist'";
            case "albumtype": return "t.tags->>'musicbrainz_albumtype'";
            case "albumcomment": return string.Empty;
            case "catalognumber": return "t.tags->>'catalognumber'";
            case "filepath": return "m.Path";
            case "filetype": return string.Empty;
            case "grouping": return string.Empty;
            case "duration": return "m.Tag_Length";
            case "bitrate": return "COALESCE((t.tags->>'bitrate')::numeric, 0)";
            case "bitdepth": return "COALESCE((t.tags->>'bitdepth')::numeric, 0)";
            case "bpm": return "COALESCE((t.tags->>'bpm')::numeric, 0)";
            case "channels": return string.Empty;
            case "loved": return "rated.Starred";
            case "dateloved": return string.Empty;
            case "lastplayed": return "(current_timestamp::date - history.CreatedAt::date)";
            case "daterated": return string.Empty;
            case "playcount": return "COALESCE(playhistory.TrackPlaycount, 0)";
            case "rating": return "rated.Rating";
            case "mbz_album_id": return "t.tags->>'musicbrainz album id'";
            case "mbz_album_artist_id": return "t.tags->>'musicbrainz album artist id'";
            case "mbz_artist_id": return "t.tags->>'musicbrainz artist id'";
            case "mbz_recording_id": return "t.tags->>'musicbrainz track id'";
            case "mbz_release_track_id": return "t.tags->>'musicbrainz track id'";
            case "mbz_release_group_id": return "t.tags->>'musicbrainz release group id'";
            case "library_id": return string.Empty;
            //extra - Not officially supported by Navidrome's documentation
            case "genre": return "m.Computed_Genre";
            case "artistloved": return "artist_rated.Starred";
            case "albumloved": return "album_rated.Starred";
            default:
                return string.Empty;
        }
    }
    
    private string GetOperatorFilter(
        string propertyName,
        object propertyValue,
        OperatorType operatorType)
    {
        string dbColumn = GetDbColumn(propertyName);
        string parameter = string.Empty;
        
        switch (operatorType)
        {
            case OperatorType.Is:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn}={parameter}";
            case OperatorType.IsNot:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn}!={parameter}";
            case OperatorType.GreaterThan:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn}>{parameter}";
            case OperatorType.LessThan:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn}<{parameter}";
            case OperatorType.Contains:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn} ilike '%'||{parameter}||'%'";
            case OperatorType.NotContains:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn} not ilike '%'||{parameter}||'%'";
            case OperatorType.StartsWith:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn} ilike {parameter}||'%'";
            case OperatorType.EndsWith:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn} ilike '%'||{parameter}";
            case OperatorType.InTheRange:
                List<int> range = propertyValue as List<int>;
                if (range?.Count != 2)
                {
                    return string.Empty;
                }
                parameter = AddParameter(range.First());
                string parameter2 = AddParameter(range.Last());
                return $"{dbColumn} BETWEEN {parameter} and {parameter2}";
            case OperatorType.Before:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn} < {parameter}";
            case OperatorType.After:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn} > {parameter}";
            case OperatorType.InTheLast:
                parameter = AddParameter(propertyValue);
                return $"{dbColumn} <= {parameter}";
            default:
                return string.Empty;
        }
    }
}