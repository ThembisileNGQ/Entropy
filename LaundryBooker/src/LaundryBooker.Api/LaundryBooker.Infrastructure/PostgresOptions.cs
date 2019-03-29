using Microsoft.Extensions.Configuration;

namespace LaundryBooker.Infrastructure
{
    public class PostgresOptions
    {
        public string ConnectionString { get; }

        public PostgresOptions(IConfiguration configuration)
        {
            ConnectionString = configuration["POSTGRES_CONNECTIONSTRING"];
        }
    }
}