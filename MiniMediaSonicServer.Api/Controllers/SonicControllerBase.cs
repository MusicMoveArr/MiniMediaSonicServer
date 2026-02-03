using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;

namespace MiniMediaSonicServer.Api.Controllers;

public abstract class SonicControllerBase : ControllerBase
{
    protected UserModel User => HttpContext.Items["user"] as UserModel;
    
    protected User GetUserModel()
    {
        return GetUserModel(this.User);
    }
    protected User GetUserModel(UserModel user)
    {
        return new User
        {
            Username = user.Username,
            Email = user.Email,
            AdminRole = user.AdminRole,
            SettingsRole = user.SettingsRole,
            StreamRole = user.StreamRole,
            JukeboxRole = user.JukeboxRole,
            DownloadRole = user.DownloadRole,
            UploadRole = user.UploadRole,
            CoverArtRole = user.CoverArtRole,
            CommentRole = user.CommentRole,
            PodcastRole = user.PodcastRole,
            ShareRole = user.ShareRole,
            VideoConversionRole = user.VideoConversionRole,
            Folder = [user.MusicFolderId],
            MaxBitRate = user.MaxBitRate
        };
    }
}