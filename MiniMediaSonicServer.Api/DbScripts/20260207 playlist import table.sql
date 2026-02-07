CREATE TABLE public.sonicserver_playlist_import (
    ImportId uuid NOT NULL,
    Path text NOT NULL,
    IsGlobal bool NOT NULL,
    Name text NOT NULL,
    FileModifiedDate timestamp NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_playlist_import_path_key UNIQUE (Path),
    CONSTRAINT sonicserver_playlist_import_pkey PRIMARY KEY (ImportId)
);
CREATE TABLE public.sonicserver_playlist_import_user (
    ImportId uuid NOT NULL,
    UserId uuid NOT NULL,
    PlaylistId uuid NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_playlist_import_user_key UNIQUE (ImportId, UserId),
    CONSTRAINT sonicserver_playlist_import_user_pkey PRIMARY KEY (ImportId, UserId)
);