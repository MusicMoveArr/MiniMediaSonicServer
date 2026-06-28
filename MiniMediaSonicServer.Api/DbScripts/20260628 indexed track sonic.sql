CREATE TABLE if not exists public.sonicserver_indexed_track_sonic (
    TrackId uuid NOT NULL,
    RelatedTrackId uuid NOT NULL,
    Distance float NOT NULL,
    IndexedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_indexed_track_sonic_pkey PRIMARY KEY (TrackId, RelatedTrackId)
);
CREATE INDEX if not exists idx_sonicserver_indexed_track_sonic_trackid ON sonicserver_indexed_track_sonic (TrackId);