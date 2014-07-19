using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace StreamInsight.Demos.Twitter.Common
{
    public class DatabaseHelper
    {
        AzureDbConfig _config;

        public DatabaseHelper(AzureDbConfig config)
        {
            if (null == config || string.IsNullOrEmpty(config.ServerName) || string.IsNullOrEmpty(config.DatabaseName) ||
                string.IsNullOrEmpty(config.UserName) || string.IsNullOrEmpty(config.Password))
                throw new ArgumentNullException("Missing database connection settings.  Check app.config file.");

            _config = config;
        }

        private string CreateAdoConnectionString()
        {
            // create a new instance of the SQLConnectionStringBuilder
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = _config.ServerName,
                InitialCatalog = _config.DatabaseName,
                Encrypt = true,
                TrustServerCertificate = false,
                UserID = _config.UserName,
                Password = _config.Password
            };
            return connectionStringBuilder.ToString();
        }

        public DataSet FillDataset(
            string storedProcedureName,
            params SqlParameter[] parameters)
        {
            return FillDataset(storedProcedureName, CommandType.StoredProcedure, parameters);
        }

        public DataSet FillDataset(
            string sqlCommand, CommandType sqlCommandType,
            params SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(CreateAdoConnectionString());
            SqlDataAdapter adapter = new SqlDataAdapter();

            adapter.SelectCommand = new SqlCommand(sqlCommand, connection);
            adapter.SelectCommand.CommandType = sqlCommandType;

            // assign all parameters with its values
            for (int i = 0; i < parameters.Length; i++)
            {
                adapter.SelectCommand.Parameters.Add(parameters[i]);
            }

            DataSet ds = new DataSet();
            adapter.Fill(ds);

            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Close();

            return ds;
        }

        public void ExecuteNonQuery(
            string storedProcedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(CreateAdoConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(storedProcedureName, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.Add(parameters[i]);
                }

                connection.Open();
                cmd.ExecuteNonQuery();

                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }
        }

        public void SaveTweet(Tweet t)
        {
            if (t == null)
                return;

            try
            {

                ExecuteNonQuery(
                    "sp_InsertTweetInfo",
                    new SqlParameter
                    {
                        ParameterName = "@TweetID",
                        Direction = ParameterDirection.Input,
                        DbType = DbType.Int64,
                        Value = t.ID
                    },
                    new SqlParameter
                    {
                        ParameterName = "@Sentiment",
                        Direction = ParameterDirection.Input,
                        DbType = DbType.Int32,
                        Size = 20,
                        Value = t.SentimentScore
                    },
                    new SqlParameter
                    {
                        ParameterName = "@Topic",
                        Direction = ParameterDirection.Input,
                        DbType = DbType.String,
                        Size = 100,
                        Value = t.Topic
                    },
                    new SqlParameter
                    {
                        ParameterName = "@CreatedAt",
                        Direction = ParameterDirection.Input,
                        DbType = DbType.DateTime,
                        Value = t.CreatedAt
                    });
            }
            catch (Exception ex)
            
            { throw ex; }

        }

    }
}
