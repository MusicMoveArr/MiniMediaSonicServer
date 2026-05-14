ALTER TABLE metadata drop COLUMN if exists computed_genre;

ALTER TABLE metadata ADD COLUMN if not exists computed_genre text GENERATED ALWAYS AS (
    regexp_replace(tag_alljsontags->>'Genre', '[,/&|]', ';', 'g')
) STORED;

CREATE INDEX if not exists idx_metadata_computed_genre ON metadata USING btree (computed_genre);