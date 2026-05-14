alter table albums add column if not exists year int default 0 not null;

update albums al set (year) = (
    select coalesce(max(tag_year), 0)
    from metadata m
    where m.AlbumId = al.AlbumId
      and m.tag_year > 0
)
where al.year = 0;

CREATE INDEX if not exists idx_albums_year ON albums USING btree (year);