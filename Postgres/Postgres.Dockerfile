FROM postgres:latest
COPY initscript.psql /docker-entrypoint-initdb.d/01_initscript.sql

ENV POSTGRES_PASSWORD="lutando"
ENV POSTGRES_USER="lutando"
ENV POSTGRES_DB="entropy"

# docker build -t lutando/postgres-entropy:latest -f Postgres.Dockerfile .
# docker push lutando/postgres-entropy:latest