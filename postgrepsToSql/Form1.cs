using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace postgrepsToSql
{
    public partial class Form1 : Form
    {
        private ArrayList _columnNames = new ArrayList();
        private ArrayList _dataTypes = new ArrayList();
        private ArrayList _datas = new ArrayList();
        private string _tableNameSource = "";
        private string _tableNameTarget = "";
        private int _numberOfColumns = 0;
        private int _numberOfRows = 0;
        private int _offset = 0;
        private const int Limit = 50;
        
        
        /**
         * Constructor for inializeing visual components and textes they store
         */
        public Form1()
        {
            InitializeComponent();
            comboBoxSource.SelectedIndex = 0;
            comboBoxTarget.SelectedIndex = 1;
        }

        /**
         * It blocks entering inputs form the user and start reading source databases
         */
        private void Button1_Click(object sender, EventArgs e)
        {
            if (checkBoxDetailedLog.Checked)
            {
                richTextBoxProgress.Text += "Button is clicked\r\n";
            }
            
            this.Enabled = false;
            StartReading();
        }
 
        /**
         * Gets connection string of choice
         * Reads source database and store datas
         */
        private void StartReading()
        {
            try
            {
                var connectionStr = GetConnectionStrFromSource();
                if (checkBoxDetailedLog.Checked)
                {
                    richTextBoxProgress.Text += "StartReading = " + connectionStr + "\r\n";
                }
                _tableNameSource = textBoxSourceTable.Text;
                ReadDb(connectionStr);
            }
            catch (NullReferenceException ex)
            {
                richTextBoxProgress.Text += "Source: " + ex.Message + "\r\n";
            }
            finally
            {
                this.Enabled = true;
            }
           
        }

        /**
         * Get table names from a given database
         */
        private void GetTableName(string connectionStr)
        {
            if (comboBoxSource.SelectedIndex == 0)
            {
                using (var connection = new NpgsqlConnection(connectionStr))
                {
                    connection.Open();
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "\r\nGetting table name...\r\n";
                    }

                    const string queryTableName = "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE'";

                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += queryTableName + "\r\n";
                    }

                    var cmd = new NpgsqlCommand(queryTableName, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _tableNameSource = dr[0].ToString();
                        if (checkBoxDetailedLog.Checked)
                        {
                            richTextBoxProgress.Text += "Table name=" + _tableNameSource + "\r\n\r\n";
                        }
                        break;
                    }
                }
            }
            else
            {
                using (var connection = new SqlConnection(connectionStr))
                {
                    connection.Open();
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "\r\nGetting table name...\r\n";
                    }

                    var queryTableName = "USE " + textBoxDatabase.Text + ";  SELECT * FROM sys.Tables;";

                    var cmd = new SqlCommand(queryTableName, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _tableNameSource = dr[0].ToString();
                        if (checkBoxDetailedLog.Checked)
                        {
                            richTextBoxProgress.Text += "Table name=" + _tableNameSource + "\r\n\r\n";
                        }

                        break;
                    }
                }
            }
        }

        /**
         * Opens database
         * Reads data as many as limit sets
         * Gets column types and stores them in an arraylist
         * Stores number of rows and columns
         * Migrates datas to target database
         */
        private void ReadDb(string connectionStr)
        {
            if (comboBoxSource.SelectedIndex == 0)
            {       
                using (var connection = new NpgsqlConnection(connectionStr))
                {
                    connection.Open();
                    if (checkBoxDetailedLog.Checked)
                    {
                         richTextBoxProgress.Text += "Connection is successfull\r\n";
                    }   
     
                    var isShown = false;
                    var isFinished = false;
    
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += Limit + " rows at a time will be read\t\n";
                    }
                    
                    do
                    {
                        var queryStr = "SELECT * FROM public.\"" + _tableNameSource + "\"" +
                                   " Limit " + Limit + " OFFSET " + _offset;
                        if (checkBoxDetailedLog.Checked)
                        {
                             richTextBoxProgress.Text += queryStr + "\r\n";
                        }
                       
    
                        var cmd = new NpgsqlCommand(queryStr, connection);
                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.HasRows)
                            {
                                isFinished = true;
                            }
                            while (dr.Read())
                            {
                                if (!isShown)
                                {
                                    for (var i = 0; i < dr.FieldCount; ++i)
                                    {
                                        richTextBoxProgress.Text += dr.GetName(i) + "\t";
                                        _columnNames.Add(dr.GetName(i));
                                    }
    
                                    richTextBoxProgress.Text += "\r\n";
                                    isShown = true;
                                    GetDataTypes(connectionStr);
                                }
    
                                for (var i = 0; i < dr.FieldCount; ++i)
                                {
                                    _datas.Add(dr[i]);   
                                }
                            }
                        }
                        _offset += Limit;

                        // here we migrate as much data as read from the source db
                        _numberOfRows = _datas.Count / _numberOfColumns;
                        Migrate();
                        
                        //we need to remove migrated items form the arraylists
                        _datas.Clear();
                        
                    } while (!isFinished);
                } 
            }
            else
            {
                var columnName = "";
                using (var connection = new SqlConnection(connectionStr))
                {
                    connection.Open();
                    if (checkBoxDetailedLog.Checked)
                    {
                         richTextBoxProgress.Text += "Connection is successfull\r\n";
                    }   

                    var queryColumnName = "SELECT *" + " FROM " + _tableNameSource;
                                   
                    var cmd = new SqlCommand(queryColumnName, connection);
                    using (var dr = cmd.ExecuteReader())
                    { 
                        while (dr.Read())
                        {
                            for (var i = 0; i < dr.FieldCount; ++i)
                            {
                                columnName = dr.GetName(i);
                            }

                            break;
                        }

                        if (checkBoxDetailedLog.Checked)
                        {
                            richTextBoxProgress.Text += "Ordered by column named " + columnName + "\r\n";
                        }
                    }
                    const int limit = 100;
                    var offset = 0;
                    var isFinished = false;
                    var isShown = false;
                    
                    do
                    {
                        var queryStr = "SELECT *" + " FROM " + _tableNameSource +
                                       " ORDER BY " + columnName +
                                       " OFFSET " + offset + " FETCH NEXT " + limit + " ROWS ONLY;";
                         
                        if (checkBoxDetailedLog.Checked)
                        {
                             richTextBoxProgress.Text += queryStr + "\r\n";
                        }

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (!dr.HasRows)
                            {
                                isFinished = true;
                            }
                            while (dr.Read())
                            {
                                if (!isShown)
                                {
                                    for (var i = 0; i < dr.FieldCount; ++i)
                                    {
                                        richTextBoxProgress.Text += dr.GetName(i) + "\t";
                                        _columnNames.Add(dr.GetName(i));
                                    }
    
                                    richTextBoxProgress.Text += "\r\n";
                                    isShown = true;
                                }
    
                                for (var i = 0; i < dr.FieldCount; ++i)
                                {
                                    _datas.Add(dr[i]);
                                }
                            }
                        }
                        offset += limit;
                    } while (!isFinished);
                }
            }
            
        }

        /**
         * First it handles differences between data types of target and source databases
         * Creates a table in target database and inserts
         * Checks if datas need quotes around them
         */
        private void Migrate()
        {
            // POSTGRESQL to MSSQL
            if (comboBoxSource.SelectedIndex == 0)
            {
               HandleDifferences();
               var connectionStr = GetConnectionStrFromTarget();
               using (var connection = new SqlConnection(connectionStr))
               {
                   
                   // Connecting to the server
                   try
                   {
                       connection.Open();
                       if (checkBoxDetailedLog.Checked)
                       {
                           richTextBoxProgress.Text += "Connected to the sql server. Migration will be started\r\n";
                       }
                   }
                   catch (Exception ex)
                   {
                       if (checkBoxDetailedLog.Checked)
                       {
                           richTextBoxProgress.Text +=
                               "Could not connected to Sql Server with the following conmnection string below\r\n" + connectionStr + "\r\n";
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
                       if (checkBoxDetailedLog.Checked)
                       {
                           richTextBoxProgress.Text += "Table " + _tableNameSource + " is created successfully\r\n";
                           richTextBoxProgress.Text += createTable + "\r\n";
                       }
                   }
                   catch (Exception ex)
                   {
                       if (checkBoxDetailedLog.Checked)
                       {
                           richTextBoxProgress.Text += _tableNameSource + " is not created.Please check if it already exists\r\n";
                           richTextBoxProgress.Text += createTable + "\r\n";
                        }
                   }
                   
                   
                   // Adding datas to new new database
                   for (var i = 0; i < _numberOfRows; ++i)
                   {
                       var insertData = "INSERT INTO " + _tableNameSource + " VALUES ( ";
                       for (var j = 0; j < _numberOfColumns; ++j)
                       {
                           if (IsQuote((string)_dataTypes[j]))
                           {
                               if (j + 1 != _numberOfColumns)
                                   insertData += "'" + _datas[i * _numberOfColumns + j] + "',";
                               else
                                   insertData += "'" + _datas[i * _numberOfColumns + j] + "'";
                           }
                           else
                           {
                               if (j + 1 != _numberOfColumns)
                                   insertData += _datas[i * _numberOfColumns + j] + ",";
                               else
                                   insertData += _datas[i * _numberOfColumns + j];
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
                           if (checkBoxDetailedLog.Checked)
                           {
                               richTextBoxProgress.Text += "Could not insert data\r\n";
                               richTextBoxProgress.Text += insertData + "\r\n";
                           }
                       }
                   }
               } 
                
            }
            // MSSQL to POSTGRESQL
            else
            {
                // implement later
            }        
        }

        /**
         * Opens source databases and runs a query for getting data types which the columns store
         */
        private void GetDataTypes(string connectionStr)
        {
            foreach (var t in _columnNames)
            {
                var queryDataType = "SELECT data_type" +
                                    " FROM information_schema.columns   " +
                                    " WHERE table_schema = 'public'" +
                                    " AND table_name =" + "'" + _tableNameSource + "'" +
                                    " AND column_name =" + "'" + t + "'";

                using (var connection = new NpgsqlConnection(connectionStr))
                {
                    try
                    {
                        connection.Open();
                        if (checkBoxDetailedLog.Checked)
                        {
                            richTextBoxProgress.Text += "Connected to postgresql server for reading column types\r\n";
                            richTextBoxProgress.Text += queryDataType + "\r\n";
                        }
                    }
                    catch(Exception ex)
                    {
                        if (checkBoxDetailedLog.Checked)
                        {
                            richTextBoxProgress.Text += "Could not connect to postgresql server\r\n";
                        }
                    }
                    
                    var cmd = new NpgsqlCommand(queryDataType, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _dataTypes.Add(dr[0].ToString());
                        richTextBoxProgress.Text += dr[0].ToString() + "\r\n";
                    }
                }
            }
            _numberOfColumns = _dataTypes.Count;
        }
        
        /**
         * Already selected types will return false for quotes
         */
        private bool IsQuote(string typeStr)
        {
            switch (typeStr)
            {
                case "[PK] integer":
                case "smallint":
                case "integer":
                case "int":
                case "BIT":
                    return false;
                default:
                    return true;
            }
        }
        
        /**
         * There are some differences between source and target data bases
         * Some data types are represented different
         * Manually its being handled by this function and being converted to convenient type
         */
        private void HandleDifferences()
        {
            for (var j = 0; j < _numberOfRows; ++j)
            {
                for (var i = 0; i < _numberOfColumns; ++i)
                {
                  
                    // casting from string to integer representation of boolean values
                    if (_datas[j * _numberOfColumns + i].ToString() == "True")
                    {
                        _datas[j * _numberOfColumns + i] = "1";
                    }
                    else if (_datas[j * _numberOfColumns + i].ToString() == "False")
                    {
                        _datas[j * _numberOfColumns + i] = "0";
                    }
                }
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
                }else if (_dataTypes[i].ToString().IndexOf("char") >= 0)
                {
                    _dataTypes[i] = "text";
                }
            }
        }
        /**
         * Builds s connection string for postgresql server
         */
        private string GetPostgreSqlConnectionStr()
        {
            if (checkBoxDetailedLog.Checked)
            {
                richTextBoxProgress.Text += "GetConnectionStr is called\r\n";
            }

            var sw = textBoxServer.Text;
            var port = textBoxPort.Text;
            var user = textBoxUserId.Text;
            var pass = textBoxPassword.Text;
            var db = textBoxDatabase.Text;
            return string.Format("Server=" + sw +
                                 ";Port=" + port +
                                 ";Database=" + db +
                                 ";User Id=" + user +
                                 ";Password=" + pass +
                                 ";Integrated Security=true;"
            );
        }
        /**
         * Builds a connection string for connecting ms sql server
         */
        private string GetSqlConnectionStr()
        {
            if (checkBoxDetailedLog.Checked)
            {
                richTextBoxProgress.Text += "GetConnectionStrSql is called\r\n";
            }

            return string.Format("Data Source=" + textBoxTargetServer.Text +
                ";Initial Catalog=" + textBoxTargetDatabase.Text +
                ";Integrated Security=SSPI");
        }
        
        /**
         * Enable or disable textboxes corersponding to the database type
         */
        private string GetConnectionStrFromSource()
        { 
            switch (comboBoxSource.SelectedIndex)
            {
                case 0:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Source:" + comboBoxSource.Text + " is selected\r\n";
                    }
                    return GetPostgreSqlConnectionStr();
                case 1:
                    textBoxUserId.Enabled = false;
                    textBoxPassword.Enabled = false;
                    textBoxPort.Enabled = false;
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Source:" + comboBoxSource.Text + " is selected\r\n";
                    }
                    return GetSqlConnectionStr();
                case 2:
                    textBoxUserId.Enabled = false;
                    textBoxPassword.Enabled = false;
                    textBoxPort.Enabled = false;
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Source:" + comboBoxSource.Text + " is selected\r\n";
                    }

                    return GetSqlConnectionStr();
                case 3:
                    textBoxUserId.Enabled = false;
                    textBoxPassword.Enabled = false;
                    textBoxPort.Enabled = false;
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Source:" + comboBoxSource.Text + " is selected\r\n";
                    }

                    return GetSqlConnectionStr();
                default:
                    textBoxUserId.Enabled = false;
                    textBoxPassword.Enabled = false;
                    textBoxPort.Enabled = false;
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Source:Default-" + " is selected\r\n";
                    }
                    throw new ArgumentNullException("No Database is selected");                    
            }
            
        }

        /**
         * Enable or disable textboxes corersponding to the database type
         */
        private string GetConnectionStrFromTarget()
        {
            
            switch (comboBoxTarget.SelectedIndex)
            {
                case 0:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:" + comboBoxTarget.Text + " is selected\r\n";
                    }
                    return GetPostgreSqlConnectionStr();
                case 1:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:" + comboBoxTarget.Text + " is selected\r\n";
                    }
                    return GetSqlConnectionStr();
                default:
                    throw new ArgumentNullException("No Database is selected");
            }
            
        }

        /**
         * Enable or disable textboxes corersponding to the database type
         */
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxSource.SelectedIndex)
            {
                case 0:
                    textBoxUserId.Enabled = true;
                    textBoxPassword.Enabled = true;
                    textBoxPort.Enabled = true;
                    break;
                case 1:
                    textBoxUserId.Enabled = false;
                    textBoxPassword.Enabled = false;
                    textBoxPort.Enabled = false;
                    break;
                case 2:
                    textBoxUserId.Enabled = false;
                    textBoxPassword.Enabled = false;
                    textBoxPort.Enabled = false;
                    break;
                case 3:
                    textBoxUserId.Enabled = false;
                    textBoxPassword.Enabled = false;
                    textBoxPort.Enabled = false;
                    break;
                default:
                    textBoxUserId.Enabled = false;
                    textBoxPassword.Enabled = false;
                    textBoxPort.Enabled = false;
                    break;
            }
        }
        
        /**
         * Enable or disable textboxes corersponding to the database type
         */
        private void comboBoxTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
           switch (comboBoxTarget.SelectedIndex)
            {
                case 0:
                    textBoxTargetUserId.Enabled = true;
                    textBoxTargetPassword.Enabled = true;
                    textBoxTargetPort.Enabled = true;
                    break;
                case 1:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    break;
                case 2:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    break;
                case 3:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    break;
                default:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    break;
            }
        }
    }
}
