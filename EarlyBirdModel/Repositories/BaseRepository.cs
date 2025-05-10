using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace EarlyBirdAPI.Model.Repositories
{
    public class BaseRepository
    {
        protected string ConnectionString { get; }

        public BaseRepository(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("EBDatabase") 
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        // Retrieve data from the database
        protected NpgsqlDataReader GetData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();

            return cmd.ExecuteReader();
        }

        // Insert data into the database
        protected bool InsertData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();

            var result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        // Update data in the database
        protected bool UpdateData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();

            cmd.ExecuteNonQuery();
            return true;
        }

        // Delete data from the database
        protected bool DeleteData(NpgsqlConnection conn, NpgsqlCommand cmd)
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();

            cmd.ExecuteNonQuery();
            return true;
        }
    }
}
