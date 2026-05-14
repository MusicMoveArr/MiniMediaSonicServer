alter table sonicserver_user add column if not exists MalojaUrl text default null;
alter table sonicserver_user add column if not exists MalojaApiKey text default null;
