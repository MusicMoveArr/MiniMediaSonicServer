ALTER TABLE metadata drop COLUMN computed_genre;

ALTER TABLE metadata ADD COLUMN computed_genre text GENERATED ALWAYS AS (
    regexp_replace(tag_alljsontags->>'Genre', '[,/&|]', ';', 'g')
) STORED;

CREATE INDEX idx_metadata_computed_genre ON metadata USING btree (computed_genre);