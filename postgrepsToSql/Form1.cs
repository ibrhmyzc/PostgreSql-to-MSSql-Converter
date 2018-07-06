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
        private int _number_of_columns = 0;
        private int _number_of_rows = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeThreads(GetConnectionStr());
        }

        private void InitializeThreads(string connectionStr)
        {
            StartReading(connectionStr);
            StartWriting();
        }

        private string GetConnectionStr()
        {
            var sw = textBoxServer.Text;
            var port = textBoxPort.Text;
            var user = textBoxUserId.Text;
            var pass = textBoxPassword.Text;
            var db = textBoxDatabase.Text;
            var connectionStr = string.Format("Server=" + sw +
                                              ";Port=" + port +
                                              ";Database=" + db +
                                              ";User Id=" + user +
                                              ";Password=" + pass +
                                              ";Integrated Security=true;"
            );
            return connectionStr;
        }
        
        private void StartReading(string connectionStr)
        {
          
            // gets table name
            GetTableName(connectionStr);

            // reads all data
            ReadDb(connectionStr);

            // gets data types
            GetDataTypes(connectionStr);
           
        }

        private void StartWriting()
        {
            Migrate();
        }

        private void ReadDb(string connectionStr)
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();        
                //richTextBoxProgres.Text += "Connection is successfull\r\n";

                var f = false;
                const int limit = 100;
                var offset = 0;
                var isShown = false;
                var isFinished = false;

               // richTextBoxProgres.Text += limit + " rows at a time will be read\t\n";
                do
                {
                    var queryStr = "SELECT * FROM public.\"" + _tableName + "\"" +
                               " Limit " + limit + " OFFSET " + offset;
                    richTextBoxProgress.Text += queryStr + "\r\n";

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
                                //richTextBoxProgress.Text += dr[i] + " is added\r\n";
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
                //richTextBoxProgress.Text += queryTableName + "\r\n";

                var cmd = new NpgsqlCommand(queryTableName, connection);
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _tableName = dr[0].ToString();
                    richTextBoxProgress.Text += "Table name=" + _tableName + "\r\n\r\n";
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
                    richTextBoxProgress.Text += "Getting data types...\r\n\r\n";
                    //richTextBoxProgress.Text += queryDataType + "\r\n";

                    var cmd = new NpgsqlCommand(queryDataType, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _dataTypes.Add(dr[0].ToString());
                       // richTextBoxProgres.Text += "Data type=" + dr[0] + "\r\n";
                    }
                }
            }

            _number_of_columns = _dataTypes.Count;
            _number_of_rows = _datas.Count / _number_of_columns;
        }

        private void Migrate()
        {      
            var server = textBoxTargetServer.Text;
            var db = textBoxTargetDatabase.Text;

            // connect to sql sevrver
            var connectionStrSql = "Data Source=" + server +
                                      ";Initial Catalog=" + db +
                                      ";Integrated Security=SSPI";
            //richTextBoxProgres.Text += connectionStrSql + "\r\n\r\n";

            using (var connection = new SqlConnection(connectionStrSql))
            {
                connection.Open();
                richTextBoxProgress.Text += "migration has started" + "\r\n\r\n";

                var createTable = "CREATE TABLE " + _tableName + "(";
                for (var i = 0; i < _number_of_columns; ++i)
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
                    richTextBoxProgress.Text += "***Table" + _tableName + " is already created\r\n\r\n";
                }
            }

            // add data
            using (var connection = new SqlConnection(connectionStrSql))
            {
                connection.Open();

                for (var i = 0; i < _number_of_rows; ++i)
                {
                    var insertData = "INSERT INTO " + _tableName + " (";
                    for (var j = 0; j < _number_of_columns; ++j)
                    {
                        if (j + 1 != _number_of_columns)
                            insertData += _columnNames[j] + ", ";
                        else
                            insertData += _columnNames[j];
                    }

                    insertData += ") VALUES (";
                    for (var j = 0; j < _number_of_columns; ++j)
                    {
                        if ((string)_dataTypes[j] != "integer")
                        {
                            if (j + 1 != _number_of_columns)
                                insertData += "'" + _datas[i * _number_of_columns + j] + "',";
                            else
                                insertData += "'" + _datas[i * _number_of_columns + j] + "'";
                        }
                        else
                        {
                            if (j + 1 != _number_of_columns)
                                insertData += _datas[i * _number_of_columns + j] + ",";
                            else
                                insertData += _datas[i * _number_of_columns + j];
                        }
                    }
                    insertData += ")";

                    //richTextBoxProgress.Text += insertData + "\r\n\r\n";

                    var cmd = new SqlCommand(insertData, connection);
                    cmd.ExecuteNonQuery();
                }
            }


            richTextBoxProgress.Text += "Migration finished\r\n";

            richTextBoxProgress.Text += _datas.Count / _columnNames.Count + " rows and " +
                                       _columnNames.Count + " columns have been copied";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
