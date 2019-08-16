FROM postgres:latest
COPY initscript.sql /docker-entrypoint-initdb.d/01_initscript.sql
COPY hydrationscript.sql /docker-entrypoint-initdb.d/02_hydrationscript.sql

ENV POSTGRES_PASSWORD="lutando"
ENV POSTGRES_USER="lutando"
ENV POSTGRES_DB="entropy"

# docker build -t lutando/postgres-entropy:hydrated -f Postgres.Hydrated.Dockerfile .
# docker push lutando/postgres-entropy:hydrated

# docker build -t lutando/postgres-entropy:hydrated -f Postgres.Hydrated.Dockerfile . && docker push lutando/postgres-entropy:hydrated