CREATE TABLE public.sonicserver_user_share (
    ShareId uuid NOT NULL,
    UserId uuid NOT NULL,
    ShareName text NOT NULL,
    Description text NOT NULL,
    ExpiresAt timestamp,
    VisitCount int default 0,
    Type text NOT NULL,
    MediaId uuid NOT NULL,
    IsDeleted bool default false,
    DeletedAt timestamp,
    LastVisitedAt timestamp,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_user_share_pkey PRIMARY KEY (ShareId, UserId, ShareName)
);
