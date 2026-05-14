CREATE UNIQUE INDEX if not exists idx_albums_record_id ON public.albums USING btree (record_id);
CREATE UNIQUE INDEX if not exists idx_artists_record_id ON public.artists USING btree (record_id);
CREATE UNIQUE INDEX if not exists idx_metadata_record_id ON public.metadata USING btree (record_id);
