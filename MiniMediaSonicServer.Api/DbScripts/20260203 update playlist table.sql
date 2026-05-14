alter table sonicserver_playlist add column if not exists DeletedAt timestamp default null;
alter table sonicserver_playlist add column if not exists IsDeleted bool default false;
