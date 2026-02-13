alter table sonicserver_user_playhistory add column ImportedBy text default null;
alter table sonicserver_user_playhistory add column PlayOffset bigint default 0 not null;
