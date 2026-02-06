CREATE TABLE public.sonicserver_indexed_search (
    SearchId uuid NOT NULL,
    Type text NOT NULL,
    Id uuid NOT NULL,
    SearchTerm text NOT NULL,
    CONSTRAINT sonicserver_indexed_search_pkey PRIMARY KEY (SearchId)
);

CREATE INDEX idx_sonicserver_indexed_search_type ON public.sonicserver_indexed_search (Type);
CREATE INDEX idx_sonicserver_indexed_search_searchterm_trgm ON sonicserver_indexed_search USING gin (SearchTerm gin_trgm_ops);


--tracks
insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
select gen_random_uuid(), 'track', m.MetadataId, lower(m.Title)
FROM artists a
JOIN albums al ON al.artistid = a.artistid
JOIN metadata m on m.albumid = al.albumid;

insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
select gen_random_uuid(), 'track', m.MetadataId, lower(a.Name) || ' ' || lower(m.Title)
FROM artists a
JOIN albums al ON al.artistid = a.artistid
JOIN metadata m on m.albumid = al.albumid;

insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
select gen_random_uuid(), 'track', m.MetadataId, lower(a.Name) || ' ' || lower(al.Title) || ' ' || lower(m.Title)
FROM artists a
JOIN albums al ON al.artistid = a.artistid
JOIN metadata m on m.albumid = al.albumid;


--albums
insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
select gen_random_uuid(), 'album', al.AlbumId, lower(al.Title)
FROM artists a
JOIN albums al ON al.artistid = a.artistid;

insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
select gen_random_uuid(), 'album', al.AlbumId, lower(a.Name) || ' ' || lower(al.Title)
FROM artists a
JOIN albums al ON al.artistid = a.artistid;

--artists
insert into sonicserver_indexed_search (SearchId, type, Id, SearchTerm)
select gen_random_uuid(), 'artist', a.ArtistId, lower(a.Name)
FROM artists a;
