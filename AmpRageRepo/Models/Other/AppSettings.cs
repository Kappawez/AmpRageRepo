using Microsoft.Extensions.Configuration;

/// <summary>
/// Class for adding various settings for the solution via dependency injection
/// </summary>

namespace AmpRageRepo.Models
{
    public class ApplicationSettings
    {
        public ApplicationSettings(string dbConnection, string apiConnection)
        {
            DbConnection = dbConnection;
            ApiConnection = apiConnection;
        }
        public ApplicationSettings(IConfiguration configuration)
        {
            DbConnection = configuration.GetConnectionString("DbConnection");
            ApiConnection = configuration.GetConnectionString("ApiConnection");
        }

        public string DbConnection { get; private set; }
        public string ApiConnection { get; private set; }
    }
}