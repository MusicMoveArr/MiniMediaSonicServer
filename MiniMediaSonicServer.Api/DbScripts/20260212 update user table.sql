alter table sonicserver_user DROP COLUMN ldapauthenticated;
alter table sonicserver_user add column Timezone text default 'Etc/UTC' NOT NULL;
