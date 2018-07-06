using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
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
        private string _tableName;
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
                GetTableName(connectionStr);

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
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                if (checkBoxDetailedLog.Checked)
                {
                     richTextBoxProgress.Text += "Connection is successfull\r\n";
                }
                

                var f = false;
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
                                if (checkBoxDetailedLog.Checked)
                                {
                                    richTextBoxProgress.Text += dr[i] + " is added\r\n";
                                }
                                
                            }
                        }
                    }

                    offset += limit;

                } while (!isFinished);

            }
        }


        private void GetTableName(string connectionStr)
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                richTextBoxProgress.Text += "\r\nGetting table name...\r\n";
                var queryTableName = "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE'";
                
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

            //richTextBoxProgres.Text += connectionStrSql + "\r\n\r\n";

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
                
                var cmd = new SqlCommand(createTable, connection);
                try
                {
                    cmd.ExecuteNonQuery();
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
                    var insertData = "INSERT INTO " + _tableName + " (";
                    for (var j = 0; j < _numberOfColumns; ++j)
                    {
                        if (j + 1 != _numberOfColumns)
                            insertData += _columnNames[j] + ", ";
                        else
                            insertData += _columnNames[j];
                    }

                    insertData += ") VALUES (";
                    for (var j = 0; j < _numberOfColumns; ++j)
                    {
                        if ((string)_dataTypes[j] != "integer")
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
                    cmd.ExecuteNonQuery();
                }
            }


            if (checkBoxDetailedLog.Checked)
            {
               richTextBoxProgress.Text += "Migration finished\r\n";
               
               richTextBoxProgress.Text += _datas.Count / _columnNames.Count + " rows and " +
                                           _columnNames.Count + " columns have been copied";
            }
            
        }

        private string GetConnectionStr()
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

        private string GetConnectionStrSql()
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
                    return GetConnectionStr();
                case 1:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Source:" + comboBoxSource.Text + " is selected\r\n";
                    }
                    return GetConnectionStrSql();
                case 2:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Source:" + comboBoxSource.Text + " is selected\r\n";
                    }

                    return GetConnectionStrSql();
                case 3:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Source:" + comboBoxSource.Text + " is selected\r\n";
                    }

                    return GetConnectionStrSql();
                default:
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
                    return GetConnectionStr();
                case 1:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:" + comboBoxTarget.Text + " is selected\r\n";
                    }

                    return GetConnectionStrSql();
                case 2:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:" + comboBoxTarget.Text + " is selected\r\n";
                    }

                    return GetConnectionStrSql();
                case 3:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:" + comboBoxTarget.Text + " is selected\r\n";
                    }

                    return GetConnectionStrSql();
                default:
                    if (checkBoxDetailedLog.Checked)
                    {
                        richTextBoxProgress.Text += "Target:Default-" +  " is selected\r\n";
                    }
                    throw new ArgumentNullException("No Database is selected");
            }
            

           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void comboBoxTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
    }
}
