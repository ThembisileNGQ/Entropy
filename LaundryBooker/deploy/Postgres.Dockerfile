FROM postgres:latest
COPY initscript.pgsql /docker-entrypoint-initdb.d/01_initscript.sql

ENV POSTGRES_PASSWORD="lutando"
ENV POSTGRES_USER="lutando"
ENV POSTGRES_DB="mrwhite"

# docker build -t lutando/mrwhite-postgres:latest . -f Postgres.Dockerfile .
# docker push lutando/mrwhite-postgres:latest