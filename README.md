# MiniMedia Sonic Server

A work in progress Subsonic music server with as backend database, postgres

There is a lot of work to be done still, but the basics work (playing a track, seeing the artists/albums/tracks)

My main goal is to come as close as possible to a real service, so that means performance, features they provide (covers, similar tracks etc)

# Roadmap
This roadmap will be ongoing as the project keeps going
- [ ] Support for all OpenSubsonic API's
- [ ] Support for all Navidrome API's
- [x] Legacy Subsonic authentication (needs to be improved still)
- [x] Token-based Subsonic authentication
- [x] Get similar tracks from Tidal
- [ ] Redis caching

# Implemented API's
A lot of Not yet/Partially but on iPhone the Arpeggi/Narjo apps are usable, mind you with missing API implementations

| API | Implemented | Status | 
| --- | --- | --- |
| AddChatMessage             | Not yet |  |
| ChangePassword             | Not yet |  |
| CreateBookmark             | Not yet |  |
| CreateInternetRadioStation | Not yet |  |
| CreatePlaylist             | Working |  |
| CreatePodcastChannel       | Not yet |  |
| CreateShare                | Not yet |  |
| CreateUser                 | Not yet |  |
| DeleteBookmark             | Not yet |  |
| DeleteInternetRadioStation | Not yet |  |
| DeletePlaylist             | Not yet |  |
| DeletePodcastChannel       | Not yet |  |
| DeletePodcastEpisode       | Not yet |  |
| DeleteShare                | Not yet |  |
| DeleteUser                 | Not yet |  |
| Download                   | Not yet |  |
| DownloadPodcastEpisode     | Not yet |  |
| GetAlbum                   | Working |  |
| GetAlbumList2              | Working |  |
| GetAlbumList               | Not yet |  |
| GetArtist                  | Working |  |
| GetArtistInfo2             | Not yet |  |
| GetArtistInfo              | Not yet |  |
| GetArtists                 | Working |  |
| GetAvatar                  | Not yet |  |
| GetBookmarks               | Not yet |  |
| GetCaptions                | Not yet |  |
| GetChatMessages            | Not yet |  |
| GetCoverArt                | Working |  |
| GetGenres                  | Working |  |
| GetIndexes                 | Not yet |  |
| GetInternetRadioStations   | Not yet |  |
| GetLicense                 | Not yet |  |
| GetLyricsBySongId          | Not yet |  |
| GetLyrics                  | Not yet |  |
| GetMusicDirectory          | Not yet |  |
| GetMusicFolders            | Partially |  |
| GetNewestPodcasts          | Not yet |  |
| GetNowPlaying              | Not yet |  |
| GetOpenSubsonicExtensions  | Not yet |  |
| GetPlaylist                | Working |  |
| GetPlaylists               | Working |  |
| GetPlayQueueByIndex        | Not yet |  |
| GetPlayQueue               | Not yet |  |
| GetPodcastEpisode          | Not yet |  |
| GetPodcasts                | Not yet |  |
| GetRandomSongs             | Not yet |  |
| GetScanStatus              | Not yet |  |
| GetShares                  | Not yet |  |
| GetSimilarSongs2           | Working |  |
| GetSimilarSongs            | Not yet |  |
| GetSong                    | Not yet |  |
| GetSongsByGenre            | Not yet |  |
| GetStarred2                | Not yet |  |
| GetStarred                 | Not yet |  |
| GetTopSongs                | Not yet |  |
| GetTranscodeDecision       | Not yet |  |
| GetTranscodeStream         | Not yet |  |
| GetUser                    | Not yet |  |
| GetUsers                   | Not yet |  |
| GetVideoInfo               | Not yet |  |
| GetVideos                  | Not yet |  |
| HLS                        | Not yet |  |
| JukeboxControl             | Not yet |  |
| Ping                       | Working |  |
| RefreshPodcasts            | Not yet |  |
| SavePlayQueueByIndex       | Not yet |  |
| SavePlayQueue              | Not yet |  |
| Scrobble                   | Not yet |  |
| Search2                    | Not yet |  |
| Search3                    | Working |  |
| Search                     | Not yet |  |
| SetRating                  | Not yet |  |
| Star                       | Not yet |  |
| StartScan                  | Not yet |  |
| Stream                     | Working |  |
| TokenInfo                  | Not yet |  |
| Unstar                     | Not yet |  |
| UpdateInternetRadioStation | Not yet |  |
| UpdatePlaylist             | Working |  |
| UpdateShare                | Not yet |  |
| UpdateUser                 | Not yet |  |
                             
