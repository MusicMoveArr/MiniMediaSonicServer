ALTER TABLE public.metadata
    ADD COLUMN if not exists computed_bitrate integer
    GENERATED ALWAYS AS (
    (tag_alljsontags ->> 'Bitrate')::integer
) STORED;

ALTER TABLE public.metadata
    ADD column if not exists computed_bpm integer
    GENERATED ALWAYS AS (
    NULLIF(
    regexp_replace(coalesce(tag_alljsontags->>'bpm', tag_alljsontags->>'BPM'), '[^0-9]', '', 'g'),
    ''
    )::integer
    ) STORED;

ALTER TABLE public.metadata
    ADD column if not exists computed_replaygain_track_gain numeric
    GENERATED ALWAYS AS (
    NULLIF(
    substring(coalesce(tag_alljsontags->>'replaygain_track_gain',
    tag_alljsontags->>'REPLAYGAIN_TRACK_GAIN') from '-?[0-9]+(\.[0-9]+)?'),
    ''
    )::numeric
    ) STORED;

ALTER TABLE public.metadata
    ADD column if not exists computed_replaygain_album_gain numeric
    GENERATED ALWAYS AS (
    NULLIF(
    substring(coalesce(tag_alljsontags->>'replaygain_album_gain',
    tag_alljsontags->>'REPLAYGAIN_ALBUM_GAIN') from '-?[0-9]+(\.[0-9]+)?'),
    ''
    )::numeric
    ) STORED;

ALTER TABLE public.metadata
    ADD column if not exists computed_replaygain_track_peak numeric
    GENERATED ALWAYS AS (
    NULLIF(
    substring(coalesce(tag_alljsontags->>'replaygain_track_peak',
    tag_alljsontags->>'REPLAYGAIN_TRACK_PEAK') from '-?[0-9]+(\.[0-9]+)?'),
    ''
    )::numeric
    ) STORED;

ALTER TABLE public.metadata
    ADD column if not exists computed_replaygain_album_peak numeric
    GENERATED ALWAYS AS (
    NULLIF(
    substring(coalesce(tag_alljsontags->>'replaygain_album_peak',
    tag_alljsontags->>'REPLAYGAIN_ALBUM_PEAK') from '-?[0-9]+(\.[0-9]+)?'),
    ''
    )::numeric
    ) STORED;


