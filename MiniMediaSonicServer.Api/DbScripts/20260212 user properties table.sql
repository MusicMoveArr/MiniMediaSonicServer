CREATE TABLE public.sonicserver_user_property (
    UserId uuid NOT NULL,
    Name text NOT NULL,
    Value text NOT NULL,
    CreatedAt timestamp DEFAULT current_timestamp,
    UpdatedAt timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_user_property_pkey PRIMARY KEY (UserId, Name)
);