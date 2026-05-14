alter table sonicserver_album_rated add column if not exists UnStarredAt timestamp;
alter table sonicserver_artist_rated add column if not exists UnStarredAt timestamp;
alter table sonicserver_track_rated add column if not exists  UnStarredAt timestamp;
