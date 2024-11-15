CREATE SEQUENCE fileblazor;

CREATE TABLE "public"."BlazorApp" (
    "id" integer DEFAULT nextval('fileblazor') NOT NULL,
    "file_name" text NOT NULL,
    "unic_file_name" text NOT NULL,
    "path_file" text,
    "path_file_competed" text NOT NULL,
    "Status_sort" text,
    "Datetime" timestamp
) WITH (oids = false);
