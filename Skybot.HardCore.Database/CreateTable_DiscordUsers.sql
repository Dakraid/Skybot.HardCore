-- Table: public.discordUsers

DROP TABLE IF EXISTS public."discordUsers";

CREATE TABLE IF NOT EXISTS public."discordUsers"
(
    "ID" uuid NOT NULL,
    "UserId" numeric(20,0) NOT NULL,
    "UserDisplayName" text COLLATE pg_catalog."default" NOT NULL,
    "UserDiscriminator" numeric(5,0) NOT NULL,
    "IsBlocked" boolean NOT NULL DEFAULT false,
    CONSTRAINT "discordUsers_pkey" PRIMARY KEY ("ID")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."discordUsers"
    OWNER to postgres;