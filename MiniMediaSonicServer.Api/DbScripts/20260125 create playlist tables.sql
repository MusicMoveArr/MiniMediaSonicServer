CREATE TABLE if not exists public.sonicserver_playlist (
    PlaylistId uuid NOT NULL,
    UserId uuid NOT NULL,
    Name text NOT NULL,
    Public bool NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_playlist_pkey PRIMARY KEY (PlaylistId)
);

CREATE TABLE if not exists public.sonicserver_playlist_track (
    PlaylistId uuid NOT NULL,
    TrackId uuid NOT NULL,
    TrackOrder int NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_playlist_track_pkey PRIMARY KEY (PlaylistId, TrackId)
);
CREATE INDEX if not exists  idx_sonicserver_playlist_userid ON public.sonicserver_playlist (UserId);