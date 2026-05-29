DROP INDEX IF EXISTS idx_albums_record_title_asc_id;
CREATE INDEX IF NOT EXISTS idx_albums_record_title_asc_id ON public.albums USING btree (record_title_asc_id);
