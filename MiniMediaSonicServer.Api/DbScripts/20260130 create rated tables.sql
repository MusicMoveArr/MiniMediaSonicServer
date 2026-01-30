CREATE TABLE public.sonicserver_track_rated (
    UserId uuid NOT NULL,
    TrackId uuid NOT NULL,
    Rating int NOT NULL,
    Starred bool NOT NULL,
    Artist text NOT NULL,
    Albumartist text NOT NULL,
    Artists text NOT NULL,
    Album text NOT NULL,
    Title text NOT NULL,
    Isrc text NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_track_rated_pkey PRIMARY KEY (UserId, TrackId)
);

CREATE TABLE public.sonicserver_artist_rated (
    UserId uuid NOT NULL,
    ArtistId uuid NOT NULL,
    Rating int NOT NULL,
    Starred bool NOT NULL,
    Artist text NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_artist_rated_pkey PRIMARY KEY (UserId, ArtistId)
);

CREATE TABLE public.sonicserver_album_rated (
    UserId uuid NOT NULL,
    AlbumId uuid NOT NULL,
    Rating int NOT NULL,
    Starred bool NOT NULL,
    Artist text NOT NULL,
    Album text NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_album_rated_pkey PRIMARY KEY (UserId, AlbumId)
);