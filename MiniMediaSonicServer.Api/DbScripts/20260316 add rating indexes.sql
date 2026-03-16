CREATE INDEX idx_sonicserver_track_rated_userid ON public.sonicserver_track_rated (userid,rating);
CREATE INDEX idx_sonicserver_artist_rated_userid ON public.sonicserver_artist_rated (userid);
