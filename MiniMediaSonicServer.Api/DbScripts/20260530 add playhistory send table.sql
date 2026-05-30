CREATE TABLE if not exists public.sonicserver_user_playhistory_send (
    HistoryId uuid not null,
    ServiceName text not null,
    SendAt timestamp,
    CONSTRAINT sonicserver_user_playhistory_send_pkey PRIMARY KEY (HistoryId, ServiceName)
);
CREATE INDEX if not exists idx_sonicserver_user_playhistory_send_historyid ON public.sonicserver_user_playhistory_send USING btree (HistoryId, ServiceName);
