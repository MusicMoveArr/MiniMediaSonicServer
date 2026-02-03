alter table sonicserver_playlist add column DeletedAt timestamp default null;
alter table sonicserver_playlist add column IsDeleted bool default false;
