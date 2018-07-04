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

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sw = textBoxServer.Text;
            string port = textBoxPort.Text;
            string user = textBoxUserId.Text;
            string pass = textBoxPassword.Text;
            string db = textBoxDatabase.Text;

            string connectionStr = String.Format("Server=" + sw +
                                                 ";Port=" + port +
                                                 ";Database=" + db +
                                                 ";User Id=" + user +
                                                 ";Password=" + pass +
                                                 ";Integrated Security=true;"
                                                 );
            richTextBoxProgres.Text += connectionStr + "\r\n\r\n";

            // reads all data
            ReadDb(connectionStr);

            // gets table name
            GetTableName(connectionStr);

            // gets data types
            GetDataTypes(connectionStr);

            // copies db
            Migrate();

        }

        private void ReadDb(string connectionStr)
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                richTextBoxProgres.Text += "Connection is successfull\r\n";

                var queryStr = "SELECT * FROM public.\"Customer\"";


                richTextBoxProgres.Text += queryStr + "\r\n";

                var cmd = new NpgsqlCommand(queryStr, connection);
                var dr = cmd.ExecuteReader();

                bool isShown = false;

                while (dr.Read())
                {
                    if (!isShown)
                    {
                        for (var i = 0; i < dr.FieldCount; ++i)
                        {
                            richTextBoxProgres.Text += dr.GetName(i) + "\t";
                            _columnNames.Add(dr.GetName(i));
                        }
                        richTextBoxProgres.Text += "\r\n";
                        isShown = true;
                    }

                    for (var i = 0; i < dr.FieldCount; ++i)
                    {
                        _datas.Add(dr[i]);
                        richTextBoxProgres.Text += dr[i] + "\t";
                    }

                    richTextBoxProgres.Text += "\r\n";
                }
                connection.Dispose();
            }
        }


        private void GetTableName(string connectionStr)
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                richTextBoxProgres.Text += "\r\nGetting table name...\r\n";
                var queryTableName = "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE'";
                richTextBoxProgres.Text += queryTableName + "\r\n";

                var cmd = new NpgsqlCommand(queryTableName, connection);
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _tableName = dr[0].ToString();
                    richTextBoxProgres.Text += "Table name=" +  _tableName + "\r\n\r\n";
                }
                connection.Dispose();
            }
        }

        private void GetDataTypes(string connectionStr)
        {
            for (int i = 0; i < _columnNames.Count; ++i)
            {
                var queryDataType = "SELECT data_type" +
                                    " FROM information_schema.columns   " +
                                    " WHERE table_schema = 'public'" +
                                    " AND table_name =" + "'" + _tableName + "'" +
                                    " AND column_name =" + "'" + _columnNames[i] + "'";

                using (var connection = new NpgsqlConnection(connectionStr))
                {
                    connection.Open();
                    richTextBoxProgres.Text += "Getting data types...\r\n\r\n"; 
                    richTextBoxProgres.Text += queryDataType + "\r\n";

                    var cmd = new NpgsqlCommand(queryDataType, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _dataTypes.Add(dr[0].ToString());
                        richTextBoxProgres.Text += "Data type=" + dr[0] + "\r\n";
                    }
                    connection.Dispose();
                }
            }
        }

        private void Migrate()
        {
            string server = textBoxTargetServer.Text;
            string db = textBoxTargetDatabase.Text;

            // connect to sql sevrver
            string connectionStrSql = "Data Source=" + server + 
                                      ";Initial Catalog=" + db + 
                                      ";Integrated Security=SSPI";
            richTextBoxProgres.Text += connectionStrSql + "\r\n\r\n";
            
            using (SqlConnection connection = new SqlConnection(connectionStrSql))
            {
                connection.Open();
                richTextBoxProgres.Text += "migration has started" + "\r\n\r\n";

                // first create a table
                var CreateTable = "CREATE TABLE " + _tableName + "(";
                for (int i = 0; i < _dataTypes.Count; ++i)
                {
                    CreateTable += _columnNames[i] + " " + _dataTypes[i] + ", ";
                }
                CreateTable += ");";
                richTextBoxProgres.Text += CreateTable + " is run\r\n\r\n";
                var cmd = new SqlCommand(CreateTable, connection);
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

            SqlConnection connection2 = new SqlConnection(connectionStrSql);
            connection2.Open();
            // add data
            for (int i = 0; i < _datas.Count / _dataTypes.Count; ++i)
            {
                string insertData = "INSERT INTO " + _tableName + "(";
                for (int j = 0; j < _dataTypes.Count; ++j)
                {
                    if(j + 1 != _dataTypes.Count)
                        insertData += _columnNames[j] + ", ";
                    else
                        insertData += _columnNames[j] ;
                }

                insertData += ") VALUES (";
                for (int j = 0; j < _dataTypes.Count; ++j)
                {
                    if ((string) _dataTypes[j] == "text")
                    {
                        if(j + 1 != _dataTypes.Count)
                            insertData += "'" + _datas[i * 2 + j] + "',";
                        else
                            insertData += "'" + _datas[i * 2 + j] + "'";
                    }
                    else
                    {
                        if (j + 1 != _dataTypes.Count)
                            insertData +=_datas[i * 2 + j] + ",";
                        else
                            insertData += _datas[i * 2 + j];
                    }
                }

                insertData += ");";

                richTextBoxProgres.Text += insertData + "\r\n\r\n";
               
                
                var cmd = new SqlCommand(insertData, connection2);
                cmd.ExecuteNonQuery();
            }
            connection2.Dispose();
            richTextBoxProgres.Text += "Migration finished\r\n";
        }
    }
}
