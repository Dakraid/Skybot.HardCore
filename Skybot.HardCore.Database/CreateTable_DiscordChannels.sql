-- Table: public.discordChannels

DROP TABLE IF EXISTS public."discordChannels";

CREATE TABLE IF NOT EXISTS public."discordChannels"
(
    "ID" uuid NOT NULL,
    "ChannelId" numeric(20,0) NOT NULL,
    "ChannelName" text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "discordChannels_pkey" PRIMARY KEY ("ID")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."discordChannels"
    OWNER to postgres;