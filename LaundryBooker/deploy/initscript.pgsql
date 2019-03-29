-- init
DO
$do$
DECLARE
  _db TEXT := 'mrwhite';
  _user TEXT := 'lutando';
  _password TEXT := 'lutando';
BEGIN
CREATE EXTENSION IF NOT EXISTS dblink;
   IF EXISTS (SELECT 1 FROM pg_database WHERE datname = _db) THEN
   RAISE NOTICE 'Database already exists';
   ELSE
   PERFORM dblink_connect('host=localhost user=' || _user || ' password=' || _password || ' dbname=' || current_database());
   PERFORM dblink_exec('CREATE DATABASE ' || _db);
   END IF;
END
$do$;

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE SCHEMA IF NOT EXISTS laundry;

CREATE TABLE IF NOT EXISTS laundry.users
(
   id uuid primary key not null,
   display_name TEXT,
   normalized_name TEXT
);

CREATE TABLE IF NOT EXISTS laundry.bookings
(
	id varchar(12) not null constraint bookings_pkey primary key,
	booking jsonb not null
);

-- hydrate

INSERT INTO laundry.users(
   id,
   display_name,
   normalized_name)
VALUES(
   '41983a34-2bc7-4d2c-bf28-206387e83693',
   'mrblue',
   'MRBLUE');

INSERT INTO laundry.users(
   id,
   display_name,
   normalized_name)
VALUES(
   '169d4425-15b2-4c83-9174-6ba91f2625b2',
   'mrred',
   'MRRED');

INSERT INTO laundry.users(
   id,
   display_name,
   normalized_name)
VALUES(
   'f1c791fe-a85a-475e-b221-c4581352f981',
   'mrpurple',
   'MRPURPLE');

INSERT INTO laundry.users(
   id,
   display_name,
   normalized_name)
VALUES(
   'b91207cd-8f0f-402e-9c02-7dde37394837',
   'mrblack',
   'MRBLACK');

INSERT INTO laundry.users(
   id,
   display_name,
   normalized_name)
VALUES(
   '45954e08-8795-4c33-ae20-1161e729ddd9',
   'mrcyan',
   'MRCYAN');



