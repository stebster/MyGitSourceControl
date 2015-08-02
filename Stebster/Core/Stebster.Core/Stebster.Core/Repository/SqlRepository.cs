namespace Stebster.Core.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using Interface;

    public class SqlRepository : ISqlRepository
    {
        #region Public Properties

        public string ServerName { get; private set; }
        public string DatabaseName { get; private set; }

        #endregion

        #region Private Properties

        private string ConnectionString
        {
            get { return string.Format("Server={0};DataBase={1};Integrated Security=SSPI", ServerName, DatabaseName); }
        }

        #endregion

        #region Constructor

        public SqlRepository(string serverName, string databaseName)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
        }

        #endregion

        #region Implement ISqlRepository Members

        public void ExecuteProcedure(string procedureName, IDictionary<string, object> inputParameters = null, IDictionary<string, object> outputParameters = null)
        {
            RunStoredProcedure(false, procedureName, inputParameters, outputParameters);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteProcedureGet(string procedureName, IDictionary<string, object> inputParameters = null, IDictionary<string, object> outputParameters = null)
        {
            return RunStoredProcedure(true, procedureName, inputParameters, outputParameters);
        }

        #endregion

        #region Private Methods

        private List<Dictionary<string, object>> RunStoredProcedure(bool get, string procedureName, IDictionary<string, object> inputParameters, IDictionary<string, object> outputParameters)
        {
            var all = new List<Dictionary<string, object>>();

            SqlConnection sqlConnection = null;
            SqlDataReader sqlDataReader  = null;

            try
            {
                sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                var sqlCommand = new SqlCommand(procedureName, sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                if (inputParameters != null)
                {
                    foreach (var kvp in inputParameters)
                    {
                        sqlCommand.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                    }
                }

                if (outputParameters != null)
                {
                    foreach (var kvp in outputParameters)
                    {
                        var outputParameter = new SqlParameter { ParameterName = kvp.Key, Value = kvp.Value ?? DBNull.Value, Direction = ParameterDirection.Output };
                        sqlCommand.Parameters.Add(outputParameter);
                    }
                }

                if (get)
                {
                    sqlDataReader = sqlCommand.ExecuteReader();

                    while (sqlDataReader.Read())
                    {
                        var row = new Dictionary<string, object>();

                        for (var i = 0; i < sqlDataReader.FieldCount; i++)
                        {
                            row.Add(sqlDataReader.GetName(i), sqlDataReader.GetValue(i));
                        }

                        all.Add(row);
                    }
                }
                else
                {
                    sqlCommand.ExecuteNonQuery();
                }

                if (outputParameters != null)
                {
                    // Loop all parameters and any output parameters replace with actual value we got
                    foreach (SqlParameter parameter in sqlCommand.Parameters)
                    {
                        if (parameter.Direction == ParameterDirection.Output)
                        {
                            if (outputParameters.ContainsKey(parameter.ParameterName))
                                outputParameters[parameter.ParameterName] = parameter.Value;
                        }
                    }
                }
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();

                if (sqlDataReader != null)
                    sqlDataReader.Close();
            }

            return all;
        }

        #endregion


     
    }
}
