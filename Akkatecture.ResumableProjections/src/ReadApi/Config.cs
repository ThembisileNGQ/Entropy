namespace ReadApi
{
    public static class Config
    {
        
        public static string Postgres =
@"
akka.persistence{
	journal {
		plugin = ""akka.persistence.journal.postgresql""
		postgresql {
			# qualified type name of the PostgreSql persistence journal actor
			class = ""Akka.Persistence.PostgreSql.Journal.PostgreSqlJournal, Akka.Persistence.PostgreSql""

			# dispatcher used to drive journal actor
			plugin-dispatcher = ""akka.actor.default-dispatcher""

			# connection string used for database access
			connection-string = ""Server=localhost;Port=30700;User Id=lutando;Password=lutando;Database=entropy;""

			# default SQL commands timeout
			connection-timeout = 30s

			# PostgreSql schema name to table corresponding with persistent journal
			schema-name = journal

			# PostgreSql table corresponding with persistent journal
			table-name = aggregate_events

			# should corresponding journal table be initialized automatically
			auto-initialize = on
			
			# timestamp provider used for generation of journal entries timestamps
			timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
		
			# metadata table
			metadata-table-name = metadata

			# defines column db type used to store payload. Available option: BYTEA (default), JSON, JSONB
			stored-as = JSONB
		}
	}
akka.persistence.query.journal.sql {
  # Implementation class of the SQL ReadJournalProvider
  class = ""Akka.Persistence.Query.Sql.SqlReadJournalProvider, Akka.Persistence.Query.Sql""
  
# Absolute path to the write journal plugin configuration entry that this 
# query journal will connect to. 
# If undefined (or "") it will connect to the default journal as specified by the
# akka.persistence.journal.plugin property.
	    write-plugin = ""
  
# The SQL write journal is notifying the query side as soon as things
# are persisted, but for efficiency reasons the query side retrieves the events 
# in batches that sometimes can be delayed up to the configured `refresh-interval`.
	    refresh-interval = 3s
  
# How many events to fetch in one query (replay) and keep buffered until they
# are delivered downstreams.
		    max-buffer-size = 100
    }
";
    }
}