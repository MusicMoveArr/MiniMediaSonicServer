CREATE TABLE public.sonicserver_user_playhistory (
    HistoryId uuid NOT NULL,
    UserId uuid NOT NULL,
    TrackId uuid NOT NULL,
    Scrobble bool NOT NULL,
    ScrobbleAt timestamp,
    Artist text NOT NULL,
    Albumartist text NOT NULL,
    Artists text NOT NULL,
    Album text NOT NULL,
    Title text NOT NULL,
    Isrc text NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_user_playhistory_pkey PRIMARY KEY (HistoryId)
);
CREATE INDEX idx_sonicserver_user_playhistory_userid ON public.sonicserver_user_playhistory USING btree (UserId);
