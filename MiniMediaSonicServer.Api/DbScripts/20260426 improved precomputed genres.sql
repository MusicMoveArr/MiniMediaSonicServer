ALTER TABLE metadata if exists drop COLUMN computed_genre;

ALTER TABLE metadata if not exists ADD COLUMN if not exists computed_genre text GENERATED ALWAYS AS (
    regexp_replace(tag_alljsontags->>'Genre', '[,/&|]', ';', 'g')
) STORED;

CREATE INDEX if not exists idx_metadata_computed_genre ON metadata USING btree (computed_genre);