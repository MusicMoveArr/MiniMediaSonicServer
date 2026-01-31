CREATE UNIQUE INDEX idx_albums_record_id ON public.albums USING btree (record_id);
CREATE UNIQUE INDEX idx_artists_record_id ON public.artists USING btree (record_id);
CREATE UNIQUE INDEX idx_metadata_record_id ON public.metadata USING btree (record_id);
