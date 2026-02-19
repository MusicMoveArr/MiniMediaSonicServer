alter table sonicserver_artist_rated add column StarredAt timestamp default null;
alter table sonicserver_album_rated add column StarredAt timestamp default null;
alter table sonicserver_track_rated add column StarredAt timestamp default null;

alter table sonicserver_artist_rated add column RatedAt timestamp default null;
alter table sonicserver_album_rated add column RatedAt timestamp default null;
alter table sonicserver_track_rated add column RatedAt timestamp default null;

update sonicserver_artist_rated 
set StarredAt = UpdatedAt
where Starred = true;

update sonicserver_album_rated
set StarredAt = UpdatedAt
where Starred = true;

update sonicserver_track_rated
set StarredAt = UpdatedAt
where Starred = true;

update sonicserver_artist_rated
set RatedAt = UpdatedAt
where Rating > 0;

update sonicserver_album_rated
set RatedAt = UpdatedAt
where Rating > 0;

update sonicserver_track_rated
set RatedAt = UpdatedAt
where Rating > 0;