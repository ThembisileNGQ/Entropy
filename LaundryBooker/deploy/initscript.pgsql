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
   id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
   name TEXT,
   normalized_name TEXT,
   location jsonb
);

CREATE TABLE IF NOT EXISTS laundry.bookings
(
	id uuid not null constraint bookings_pkey primary key,
	booking jsonb not null
);

-- hydrate

INSERT INTO laundry.users(
   id,
   "name",
   normalized_name)
	VALUES(
      gen_random_uuid(),
      'mrblue',
      'MRBLUE');
