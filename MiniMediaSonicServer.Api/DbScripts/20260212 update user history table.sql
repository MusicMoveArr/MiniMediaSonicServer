alter table sonicserver_user_playhistory add column if not exists ImportedBy text default null;
alter table sonicserver_user_playhistory add column if not exists PlayOffset bigint default 0 not null;
