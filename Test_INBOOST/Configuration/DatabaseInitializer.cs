using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Test_INBOOST.Configuration
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly string _masterConnectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;

            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };
            _masterConnectionString = builder.ToString();
        }

        public void Initialize()
        {
            CreateDatabaseIfNotExists();
            CreateTables();
        }

        private void CreateDatabaseIfNotExists()
        {
            using (IDbConnection db = new SqlConnection(_masterConnectionString))
            {
                var databaseName = new SqlConnectionStringBuilder(_connectionString).InitialCatalog;

                var createDbQuery = $@"
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
                BEGIN
                    CREATE DATABASE [{databaseName}]
                END";

                db.Execute(createDbQuery);
            }
        }

        private void CreateTables()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var usersTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
        CREATE TABLE Users (
            Id UNIQUEIDENTIFIER PRIMARY KEY, 
            UserId BIGINT NOT NULL,          
            UserName NVARCHAR( 100) NOT NULL,
            FirstName NVARCHAR(100),
            LastName NVARCHAR(100),
            Email NVARCHAR(100),
            Role INT NOT NULL,
            CreationDate DATETIME NOT NULL,
            Deleted BIT NOT NULL DEFAULT 0,
            DeletionDate DATETIME
        )";

                db.Execute(usersTableQuery);

                var weatherHistoryTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WeatherHistorys' and xtype='U')
        CREATE TABLE WeatherHistorys (
            Id UNIQUEIDENTIFIER PRIMARY KEY,
            UserId BIGINT NOT NULL,       
            City NVARCHAR(100) NOT NULL,
            WeatherData NVARCHAR(MAX) NOT NULL,
            CreationDate DATETIME NOT NULL,
            Deleted BIT NOT NULL DEFAULT 0,
            DeletionDate DATETIME
        )";

                db.Execute(weatherHistoryTableQuery);
            }
        }
    }
}