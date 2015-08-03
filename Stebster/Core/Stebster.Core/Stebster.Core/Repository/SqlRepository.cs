namespace Stebster.Core.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
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

        public void ExecuteProcedure(string procedureName, Dictionary<string, object> inputParameters = null, Dictionary<string, object> outputParameters = null)
        {
            RunStoredProcedure(false, procedureName, inputParameters, outputParameters);
        }

        public Dictionary<string, object>[] ExecuteProcedureGet(string procedureName, Dictionary<string, object> inputParameters = null, Dictionary<string, object> outputParameters = null)
        {
            return RunStoredProcedure(true, procedureName, inputParameters, outputParameters);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, object>[] RunStoredProcedure(bool executeReaderToGetResults, 
                                                                string procedureName,
                                                                Dictionary<string, object> inputParameters, 
                                                                IDictionary<string, object> outputParameters)
        {
            var results = new List<Dictionary<string, object>>();

            // Get Sql Connection
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                // Setup the Stored Procedure Command
                using (var sqlCommand = new SqlCommand(procedureName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    // Add any input parameters
                    if (inputParameters != null)
                    {
                        foreach (var kvp in inputParameters)
                        {
                            sqlCommand.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                    }

                    // Add any output parameters
                    if (outputParameters != null)
                    {
                        foreach (var kvp in outputParameters)
                        {
                            var outputParameter = new SqlParameter
                            {
                                ParameterName = kvp.Key,
                                Value = kvp.Value ?? DBNull.Value,
                                Direction = ParameterDirection.Output
                            };
                            sqlCommand.Parameters.Add(outputParameter);
                        }
                    }

                    // If we are getting results from reader
                    if (executeReaderToGetResults)
                    {
                        using (var sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            while (sqlDataReader.Read())
                            {
                                var row = new Dictionary<string, object>();

                                for (var i = 0; i < sqlDataReader.FieldCount; i++)
                                {
                                    row.Add(sqlDataReader.GetName(i), sqlDataReader.GetValue(i));
                                }

                                results.Add(row);
                            }
                        }
                    }
                    else // Otherwise we just execute the query
                    {
                        sqlCommand.ExecuteNonQuery();
                    }

                    // If we passed some output parameters
                    if (outputParameters != null)
                    {
                        // Loop sql output parameters replace our passed parameters with actual value we got
                        foreach (var parameter in sqlCommand.Parameters
                                                            .Cast<SqlParameter>()
                                                            .Where(x => x.Direction == ParameterDirection.Output && 
                                                                    outputParameters.ContainsKey(x.ParameterName)))
                        {
             
                            outputParameters[parameter.ParameterName] = parameter.Value;
                        }
                    }
                }
            }

            return results.ToArray();
        }

        #endregion
    }
}
