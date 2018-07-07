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
        private string _tableName = "customer";
        private int _numberOfColumns = 0;
        private int _numberOfRows = 0;

        public Form1()
        {
            InitializeComponent();    
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (checkBoxDetailedLog.Checked)
            {
                richTextBoxProgress.Text += "Button is clicked\r\n";
            }
            
            this.Enabled = false;
            StartReading();
            StartWriting();
        }
 
        private void StartReading()
        {
            try
            {
                var connectionStr = GetConnectionStrFromSource();
                if (checkBoxDetailedLog.Checked)
                {
                    richTextBoxProgress.Text += "StartReading = " + connectionStr + "\r\n";
                }

                // gets table name
                //GetTableName(connectionStr);

                // reads all data
                ReadDb(connectionStr);

                // gets data types
                GetDataTypes(connectionStr);
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

        private void StartWriting()
        {
            try
            {
                var connectionStrSql = GetConnectionStrFromTarget();
                if (checkBoxDetailedLog.Checked)
                {
                    richTextBoxProgress.Text += "StartWriting: " + connectionStrSql + "\r\n";
                }
                Migrate(connectionStrSql);
            }
            catch (NullReferenceException ex)
            {
                richTextBoxProgress.Text += "Target: " + ex.Message + "\r\n";
            }
            finally
            {
                this.Enabled = true;
            }
            
            
        }

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
     
                    const int limit = 100;
                    var offset = 0;
                    var isShown = false;
                    var isFinished = false;
    
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += limit + " rows at a time will be read\t\n";
                    }
                    
                    do
                    {
                        var queryStr = "SELECT * FROM public.\"" + _tableName + "\"" +
                                   " Limit " + limit + " OFFSET " + offset;
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
            else
            {
                string columnName = "";
                using (var connection = new SqlConnection(connectionStr))
                {
                    connection.Open();
                    if (checkBoxDetailedLog.Checked)
                    {
                         richTextBoxProgress.Text += "Connection is successfull\r\n";
                    }   

                    var queryColumnName = "SELECT *" + " FROM " + _tableName;
                                   
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
                        var queryStr = "SELECT *" + " FROM " + _tableName +
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
                        _tableName = dr[0].ToString();
                        if (checkBoxDetailedLog.Checked)
                        {
                            richTextBoxProgress.Text += "Table name=" + _tableName + "\r\n\r\n";
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
                        _tableName = dr[0].ToString();
                        if (checkBoxDetailedLog.Checked)
                        {
                            richTextBoxProgress.Text += "Table name=" + _tableName + "\r\n\r\n";
                        }

                        break;
                    }
                }
            }
            
        }
        
        private void GetDataTypes(string connectionStr)
        {
            foreach (var t in _columnNames)
            {
                var queryDataType = "SELECT data_type" +
                                    " FROM information_schema.columns   " +
                                    " WHERE table_schema = 'public'" +
                                    " AND table_name =" + "'" + _tableName + "'" +
                                    " AND column_name =" + "'" + t + "'";

                using (var connection = new NpgsqlConnection(connectionStr))
                {
                    connection.Open();
                    if (checkBoxDetailedLog.Checked)
                    {
                         richTextBoxProgress.Text += "Getting data types...\r\n\r\n";
                         richTextBoxProgress.Text += queryDataType + "\r\n";
                    }
                   
                    var cmd = new NpgsqlCommand(queryDataType, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _dataTypes.Add(dr[0].ToString());
                        if (checkBoxDetailedLog.Checked)
                        {
                             richTextBoxProgress.Text += "Data type=" + dr[0] + "\r\n";
                        }
                        
                    }
                }
            }

            _numberOfColumns = _dataTypes.Count;
            _numberOfRows = _datas.Count / _numberOfColumns;
        }

        private void Migrate(string connectionStrSql)
        {
            HandleDifferences();

            using (var connection = new SqlConnection(connectionStrSql))
            {
                connection.Open();
                if (checkBoxDetailedLog.Checked)
                {
                    richTextBoxProgress.Text += "migration has started" + "\r\n\r\n";
                }
                

                var createTable = "CREATE TABLE " + _tableName + "(";
                for (var i = 0; i < _numberOfColumns; ++i)
                {
                    createTable += _columnNames[i] + " " + _dataTypes[i] + ", ";
                }
                createTable += ");";

                if (checkBoxDetailedLog.Checked)
                {
                    richTextBoxProgress.Text += createTable + "\r\n";
                }
                
                var cmd = new SqlCommand(createTable, connection);
                try
                {
                    cmd.ExecuteNonQuery();
                    richTextBoxProgress.Text += createTable + "\r\n";
                }
                catch (Exception ex)
                {
                    if (checkBoxDetailedLog.Checked)
                    {
                         richTextBoxProgress.Text += "***Table" + _tableName + " is already created\r\n\r\n";
                    }
                }
            }

            // add data
            using (var connection = new SqlConnection(connectionStrSql))
            {
                connection.Open();

                for (var i = 0; i < _numberOfRows; ++i)
                {
                    var insertData = "INSERT INTO " + _tableName;
                   

                    insertData += " VALUES (";
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
                    insertData += ")";

                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += insertData + "\r\n\r\n";
                    }
                   
                    var cmd = new SqlCommand(insertData, connection);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        richTextBoxProgress.Text += "migration exe problem\r\n";
                    }
                    
                }
            }

            if (checkBoxDetailedLog.Checked)
            {
               richTextBoxProgress.Text += "Migration finished\r\n";
               
               richTextBoxProgress.Text += _datas.Count / _columnNames.Count + " rows and " +
                                           _columnNames.Count + " columns have been copied";
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
                    return false;
                default:
                    return true;
            }
        }

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
                case 2:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:" + comboBoxTarget.Text + " is selected\r\n";
                    }

                    return GetSqlConnectionStr();
                case 3:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:" + comboBoxTarget.Text + " is selected\r\n";
                    }

                    return GetSqlConnectionStr();
                default:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:Default-" +  " is selected\r\n";
                    }
                    throw new ArgumentNullException("No Database is selected");
            }
            
        }

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
