namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Navidrome.SmartPlaylist;

public enum OperatorType
{
    Unknown,
    Is,
    IsNot,
    GreaterThan,
    LessThan,
    Contains,
    NotContains,
    StartsWith,
    EndsWith,
    InTheRange,
    Before,
    After,
    InTheLast,
    NotInTheLastIs,
    InPlaylist,
    NotInPlaylist
}