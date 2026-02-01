# MiniMedia Sonic Server

A work in progress Subsonic music server with as backend database, postgres

There is a lot of work to be done still, but the basics work (playing a track, seeing the artists/albums/tracks)

My main goal is to come as close as possible to a real service, so that means performance, features they provide (covers, similar tracks etc)

## Roadmap
This roadmap will be ongoing as the project keeps going
- [ ] Support for all OpenSubsonic API's
- [ ] Support for all Navidrome API's
- [x] Legacy Subsonic authentication (needs to be improved still)
- [x] Token-based Subsonic authentication
- [x] Get similar tracks from Tidal
- [ ] Redis caching

## Docker Compose
```
services:
  main_app:
    container_name: MiniMediaSonicServer
    deploy:
      resources:
        limits:
          memory: 256M
    hostname: MiniMediaSonicServer
    image: musicmovearr/minimediasonicserver:latest
    ports:
      - target: 8080
        published: "8080"
        protocol: tcp
    restart: always
    volumes:
      - type: bind
        source: /DATA/AppData/MiniMediaSonicServer/appsettings.json
        target: /app/appsettings.json
      - ~/Music:~/Music:ro
```

## Example Configuration
It's important you bind this file to /app/appsettings.json as above in the docker example

Change the connectionstring to your own postgres database

Change the "aaaaaaa" with a random 64character string, for example on linux you can run, openssl rand -hex 32

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "DatabaseConfiguration":{
    "ConnectionString": "Host=192.168.1.1;Username=postgres;Password=postgres;Database=minimedia;Pooling=true;MinPoolSize=5;MaxPoolSize=100;"
  },
  "EncryptionKeys": {
    "UserPasswordKey": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
  },
  "Redis": {
    "ConnectionString": ""
  }
}
```

## Implemented API's
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
| Download                   | Working |  |
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
| GetOpenSubsonicExtensions  | Working |  |
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
| GetSong                    | Working |  |
| GetSongsByGenre            | Not yet |  |
| GetStarred2                | Not yet |  |
| GetStarred                 | Working |  |
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
| Scrobble                   | Working | Need to implement Last.fm. history via database and ListenBrainz work |
| Search2                    | Not yet |  |
| Search3                    | Working |  |
| Search                     | Not yet |  |
| SetRating                  | Working |  |
| Star                       | Working |  |
| StartScan                  | Not yet |  |
| Stream                     | Working |  |
| TokenInfo                  | Not yet |  |
| Unstar                     | Working |  |
| UpdateInternetRadioStation | Not yet |  |
| UpdatePlaylist             | Working |  |
| UpdateShare                | Not yet |  |
| UpdateUser                 | Not yet |  |
                             
