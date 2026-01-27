ALTER TABLE metadata ADD COLUMN computed_genre text GENERATED ALWAYS AS (tag_alljsontags->>'Genre') STORED;
CREATE INDEX idx_metadata_computed_genre ON metadata USING btree (computed_genre);