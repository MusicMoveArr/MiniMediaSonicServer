CREATE TABLE public.sonicserver_user_bookmark_track (
    UserId uuid NOT NULL,
    TrackId uuid NOT NULL,
    Position bigint NOT NULL,
    Comment text NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_user_bookmark_track_pkey PRIMARY KEY (UserId, TrackId)
);

