CREATE TABLE public.sonicserver_user (
    UserId uuid NOT NULL,
    Username text NOT NULL,
    Password text NOT NULL,
    Name text NOT NULL,
    Email text NOT NULL,
    CreationDateTime timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_user_name_key UNIQUE (Username),
    CONSTRAINT sonicserver_user_pkey PRIMARY KEY (UserId)
);

CREATE TABLE public.sonicserver_usertokens (
    UserId uuid NOT NULL,
    Token text NOT NULL,
    ValidTill timestamp NOT NULL,
    CreationDateTime timestamp DEFAULT current_timestamp,
    CONSTRAINT sonicserver_usertokens_name_key UNIQUE (UserId, Token),
    CONSTRAINT sonicserver_usertokens_pkey PRIMARY KEY (UserId)
);