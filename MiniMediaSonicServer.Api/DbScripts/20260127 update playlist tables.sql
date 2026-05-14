ALTER TABLE sonicserver_playlist_track ADD COLUMN if not exists artist text;
ALTER TABLE sonicserver_playlist_track ADD COLUMN if not exists albumartist text;
ALTER TABLE sonicserver_playlist_track ADD COLUMN if not exists artists text;
ALTER TABLE sonicserver_playlist_track ADD COLUMN if not exists album text;
ALTER TABLE sonicserver_playlist_track ADD COLUMN if not exists title text;
ALTER TABLE sonicserver_playlist_track ADD COLUMN if not exists isrc text;
