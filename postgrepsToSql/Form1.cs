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


        private Thread reader;
        private Thread writer;
        private ManualResetEvent _thSig = new ManualResetEvent(false);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
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
            richTextBoxProgres.Text += connectionStr + "\r\n\r\n";

            reader = new Thread(() => StartReading(_thSig, connectionStr));
            writer = new Thread(() => StartWriting(_thSig));
         
            reader.Start();
            writer.Start();
            
            reader.Join();
            writer.Join();

        }
        private void StartReading(EventWaitHandle makeWait, string connectionStr)
        {
          
            // gets table name
            GetTableName(connectionStr);

            // reads all data
            ReadDb(connectionStr);

            // gets data types
            GetDataTypes(connectionStr);
            
            makeWait.Set();
        }

        private void StartWriting(EventWaitHandle waitForRead)
        {
            _thSig.WaitOne();
            Migrate();
        }

        private void ReadDb(string connectionStr)
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();        
                richTextBoxProgres.Text += "Connection is successfull\r\n";

                var f = false;
                const int limit = 100;
                var offset = 0;
                var isShown = false;
                var isFinished = false;

                richTextBoxProgres.Text += limit + " rows at a time will be read\t\n";
                do
                {
                    var queryStr = "SELECT * FROM public.\"" + _tableName + "\"" +
                               " Limit " + limit + " OFFSET " + offset;
                    //richTextBoxProgres.Text += queryStr + "\r\n";


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
                                    //richTextBoxProgres.Text += dr.GetName(i) + "\t";
                                    _columnNames.Add(dr.GetName(i));
                                }

                                //richTextBoxProgres.Text += "\r\n";
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


        private void GetTableName(string connectionStr)
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                richTextBoxProgres.Text += "\r\nGetting table name...\r\n";
                var queryTableName = "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE'";
                //richTextBoxProgres.Text += queryTableName + "\r\n";

                var cmd = new NpgsqlCommand(queryTableName, connection);
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _tableName = dr[0].ToString();
                    richTextBoxProgres.Text += "Table name=" + _tableName + "\r\n\r\n";
                }
                connection.Dispose();
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
                    //richTextBoxProgres.Text += "Getting data types...\r\n\r\n";
                    //richTextBoxProgres.Text += queryDataType + "\r\n";

                    var cmd = new NpgsqlCommand(queryDataType, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _dataTypes.Add(dr[0].ToString());
                        richTextBoxProgres.Text += "Data type=" + dr[0] + "\r\n";
                    }

                }
            }
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
                richTextBoxProgres.Text += "migration has started" + "\r\n\r\n";

                // first create a table
                var createTable = "CREATE TABLE " + _tableName + "(";
                for (var i = 0; i < _dataTypes.Count; ++i)
                {
                    createTable += _columnNames[i] + " " + _dataTypes[i] + ", ";
                }
                createTable += ");";
                //richTextBoxProgres.Text += createTable + " is run\r\n\r\n";
                var cmd = new SqlCommand(createTable, connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    richTextBoxProgres.Text += "***Table" + _tableName + " is already created\r\n\r\n";
                }

                connection.Dispose();
            }



            // add data
            using (var connection2 = new SqlConnection(connectionStrSql))
            {
                connection2.Open();

                for (var i = 0; i < _datas.Count / _columnNames.Count; ++i)
                {
                    var insertData = "INSERT INTO " + _tableName + " (";
                    for (var j = 0; j < _dataTypes.Count; ++j)
                    {
                        if (j + 1 != _dataTypes.Count)
                            insertData += _columnNames[j] + ", ";
                        else
                            insertData += _columnNames[j];
                    }

                    insertData += ") VALUES (";
                    for (var j = 0; j < _dataTypes.Count; ++j)
                    {
                        if ((string)_dataTypes[j] == "text")
                        {
                            if (j + 1 != _dataTypes.Count)
                                insertData += "'" + _datas[i * 2 + j] + "',";
                            else
                                insertData += "'" + _datas[i * 2 + j] + "'";
                        }
                        else
                        {
                            if (j + 1 != _dataTypes.Count)
                                insertData += _datas[i * 2 + j] + ",";
                            else
                                insertData += _datas[i * 2 + j];
                        }
                    }

                    insertData += ")";

                    //richTextBoxProgres.Text += insertData + "\r\n\r\n";

                    var cmd = new SqlCommand(insertData, connection2);
                    cmd.ExecuteNonQuery();
                }
            }

            richTextBoxProgres.Text += "Migration finished\r\n";

            richTextBoxProgres.Text += _datas.Count / _columnNames.Count + " rows and " +
                                       _columnNames.Count + " columns habe been copied";
        }
    }
}
