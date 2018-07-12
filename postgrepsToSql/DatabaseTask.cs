using System;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Npgsql;

namespace postgrepsToSql
{
    public class DatabaseTask
    {
        private ArrayList _columnNames = new ArrayList();
        private ArrayList _dataTypes = new ArrayList();
        private ArrayList _queryList = new ArrayList();
        private int _count = 0;
        private string _tableNameSource = "";
        private int _numberOfColumns = 0;
        private int _numberOfRows = 0;
        private int _offset = 0;
        private const int Limit = 15000;
        private string _connectionStrSource = "";
        private string _connectionStrTarget = "";


        public DatabaseTask(string connectionStrSource, string connectionStrTarget, int type, string tableName)
        {
            Debug.WriteLine("Constructor for DataBaseTask");
            _tableNameSource = tableName;
            this._connectionStrSource = connectionStrSource;
            this._connectionStrTarget = connectionStrTarget;
            _numberOfRows = GetCount();
            GetDataName();
            GetDataTypes();
            CreateTable();
        }

        public void DoWithThreads()
        {
            GetDataFromDb();

            var waitHandles = new WaitHandle[_queryList.Count];
            for (var i = 0; i < _queryList.Count; i++)
            {
                waitHandles[i] = new ManualResetEvent(false);
                var i1 = i;
                new Thread(waitHandle =>
                {
                    Debug.WriteLine("Thread " + i1 + " is running-> " + _queryList[i1]);
                    ThreadFunction(_queryList[i1].ToString());
                    (waitHandle as ManualResetEvent)?.Set();
                }).Start(waitHandles[i]);
            }
        }

        private void GetDataFromDb()
        {
            var tmpOffset = 0;
            while (tmpOffset < _count + _offset)
            {
                var queryStr = "SELECT * FROM public.\"" + _tableNameSource + "\"" +
                               " Limit " + Limit + " OFFSET " + tmpOffset;
                _queryList.Add(queryStr);
                tmpOffset += Limit;
            }
        }

        private void ThreadFunction(string queryStr)
        {
            var localData = new ArrayList();

            using (var connection = new NpgsqlConnection(_connectionStrSource))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                var cmd = new NpgsqlCommand(queryStr, connection);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        for (var i = 0; i < dr.FieldCount; ++i)
                        {
                            localData.Add(dr[i]);
                        }
                    }
                    Debug.WriteLine(Thread.CurrentThread.ManagedThreadId + " - Number of local data = " + localData.Count);
                    Migrate(localData);
                }
            }
        }

        private void Migrate(IList localData)
        {
           HandleDifferences(localData);
           using (var connection = new SqlConnection(_connectionStrTarget))
           {  
               // Connecting to the server
               try
               {
                   connection.Open();
               }
               catch (Exception ex)
               {
                   Debug.WriteLine(ex.ToString());
               }    

               // Adding datas to new new database
               for (var i = 0; i < localData.Count / _numberOfColumns; ++i)
               {
                   var insertData = "INSERT INTO " + _tableNameSource + " VALUES ( ";
                   for (var j = 0; j < _numberOfColumns; ++j)
                   {
                       try
                       {
                           var dat = localData[i * _numberOfColumns + j].ToString();
                           dat = dat.Replace("'", "''");

                           if (dat == "")
                           {
                               dat = "null";
                           }

                           if (IsQuote((string) _dataTypes[j]) && dat != "null")
                           {
                               if (j + 1 != _numberOfColumns)
                                   insertData += "'" + dat + "',";
                               else
                                   insertData += "'" + dat + "'";
                           }
                           else
                           {

                               if (j + 1 != _numberOfColumns)
                                   insertData += dat + ",";
                               else
                                   insertData += dat;
                           }
                       }
                       catch (ArgumentOutOfRangeException ex)
                       {
                           Debug.WriteLine(ex.Message);
                       }
                   }
                   insertData += " )";
                   
                   var cmd2 = new SqlCommand(insertData, connection);
                   try
                   {
                       cmd2.ExecuteNonQuery();
                      
                   }
                   catch(Exception ex)
                   {
                        Debug.WriteLine(ex.ToString());
                        Debug.WriteLine(insertData);                    
                   }
               }
           }        
        }
                      
        private bool IsQuote(string typeStr)
        {
            switch (typeStr)
            {
                case "[PK] integer":
                case "smallint":
                case "integer":
                case "int":
                case "BIT":
                case "BIGINT":
                case "REAL":
                case "DOUBLE PRECISION":
                    return false;
                default:
                    return true;
            }
        }

        private void CreateTable()
        {
            using (var connection = new SqlConnection(_connectionStrTarget))
            {
                // Connecting to the server
                try
                {
                    connection.Open();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                for (var i = 0; i < _numberOfColumns; ++i)
                {
                    // ms sql does not have boolean type. instead, it has BIT
                    if (_dataTypes[i].ToString() == "boolean")
                    {
                        _dataTypes[i] = "BIT";
                    }
                    else if (_dataTypes[i].ToString() == "timestamp without time zone")
                    {
                        _dataTypes[i] = "datetime2";
                    }
                    else if (_dataTypes[i].ToString().IndexOf("char") >= 0)
                    {
                        _dataTypes[i] = "text";
                    }
                }

                //Creating the sql query for creating a table 
                var createTable = "CREATE TABLE " + _tableNameSource + " (";
                for (var i = 0; i < _numberOfColumns; ++i)
                {
                    createTable += _columnNames[i] + " " + _dataTypes[i] + ", ";
                }

                createTable += " );";

                var cmd = new SqlCommand(createTable, connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Debug.WriteLine(createTable);
                }
            }
        }

        private void HandleDifferences(IList localData)
        {
            for (var j = 0; j < localData.Count; ++j)
            {
                try
                {
                    // casting from string to integer representation of boolean values
                    if (localData[j].ToString() == "True")
                    {
                        localData[j] = "1";
                    }
                    else if (localData[j].ToString() == "False")
                    {
                        localData[j] = "0";
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Debug.WriteLine(j + " -> " + ex.Message);
                }
            }
        }

        public int GetCount()
        {
            using (var connection = new NpgsqlConnection(_connectionStrSource))
            {
                connection.Open();

                var queryTableName = "SELECT COUNT(*) FROM public.\"" + _tableNameSource + "\"";
                var cmd = new NpgsqlCommand(queryTableName, connection);

                try
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        _count = int.Parse(dr[0].ToString());
                        return _count;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            Debug.WriteLine("Count is " + _count);
            return 0;
        }

        private void GetDataName()
        {
            var queryColumnName = "SELECT * FROM public.\"" + _tableNameSource + "\"";

            using (var connection = new NpgsqlConnection(_connectionStrSource))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                var cmd = new NpgsqlCommand(queryColumnName, connection);
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    for (var i = 0; i < dr.FieldCount; ++i)
                    {
                        _columnNames.Add(dr.GetName(i));
                    }

                    break;
                }
            }
            _numberOfColumns = _columnNames.Count;

            Debug.WriteLine("Number of Columns is " + _numberOfColumns);
        }

        private void GetDataTypes()
        {
            foreach (var t in _columnNames)
            {
                var queryDataType = "SELECT data_type" +
                                    " FROM information_schema.columns   " +
                                    " WHERE table_schema = 'public'" +
                                    " AND table_name =" + "'" + _tableNameSource + "'" +
                                    " AND column_name =" + "'" + t + "'";

                using (var connection = new NpgsqlConnection(_connectionStrSource))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    var cmd = new NpgsqlCommand(queryDataType, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _dataTypes.Add(dr[0].ToString());
                    }
                }
            }
        }
    }
}