CREATE TABLE public.sonicserver_user_playqueue (
    UserId uuid NOT NULL,
    CurrentTrackId uuid,
    TrackPosition bigint NOT NULL,
    UpdatedByAppName text NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_user_playqueue_pkey PRIMARY KEY (UserId)
);

CREATE TABLE public.sonicserver_user_playqueue_track (
    UserId uuid NOT NULL,
    TrackId uuid,
    Index int,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_user_playqueue_track_pkey PRIMARY KEY (UserId, Index)
);
