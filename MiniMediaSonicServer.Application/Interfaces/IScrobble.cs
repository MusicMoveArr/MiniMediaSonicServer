using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Application.Interfaces;

public interface IScrobble
{
    Task<bool> ScrobbleAsync(TrackID3 track, UserModel user, DateTime scrobbleAt);
}