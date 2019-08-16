
namespace WriteApi
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

			event-adapters {
        
				aggregate-event-tagger  = ""Akkatecture.Events.AggregateEventTagger, Akkatecture""
    
			}
    
			event-adapter-bindings = {
    
				""Akkatecture.Aggregates.ICommittedEvent, Akkatecture"" = aggregate-event-tagger
    
			}
		}
	}

	snapshot-store {
		plugin = ""akka.persistence.snapshot-store.postgresql""
		postgresql {
			# qualified type name of the PostgreSql persistence journal actor
			class = ""Akka.Persistence.PostgreSql.Snapshot.PostgreSqlSnapshotStore, Akka.Persistence.PostgreSql""

			# dispatcher used to drive journal actor
			plugin-dispatcher = ""akka.actor.default-dispatcher""

			# connection string used for database access
			connection-string = ""Server=localhost;Port=30700;User Id=lutando;Password=lutando;Database=entropy;""

			# default SQL commands timeout
			connection-timeout = 30s

			# PostgreSql schema name to table corresponding with persistent journal
			schema-name = store

			# PostgreSql table corresponding with persistent journal
			table-name = aggregate_snapshots

			# should corresponding journal table be initialized automatically
			auto-initialize = on
			
			# defines column db type used to store payload. Available option: BYTEA (default), JSON, JSONB
			stored-as = JSONB
		}
	}
}
";
    }
}
