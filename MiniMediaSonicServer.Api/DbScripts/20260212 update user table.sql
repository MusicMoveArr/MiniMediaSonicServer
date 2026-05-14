alter table sonicserver_user DROP COLUMN if exists ldapauthenticated;
alter table sonicserver_user add column if not exists Timezone text default 'Etc/UTC' NOT NULL;
