CREATE SEQUENCE fileblazor;

CREATE TABLE "public"."BlazorApp" (
    "id" integer DEFAULT nextval('fileblazor') NOT NULL,
    "file_name" text NOT NULL,
    "promt_sort" text,
    "path_file" text,
    "path_file_itg" text NOT NULL,
    "Status_sort" text
) WITH (oids = false);
