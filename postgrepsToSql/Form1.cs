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
            string sw = textBox1.Text;
            string port = textBox2.Text;
            string user = textBox4.Text;
            string pass = textBox5.Text;
            string db = textBox3.Text;

            string connectionStr = String.Format("Server=" + sw +
                                                 ";Port=" + port +
                                                 ";Database=" + db +
                                                 ";User Id=" + user +
                                                 ";Password=" + pass +
                                                 ";Integrated Security=true;"
                                                 );
            richTextBox1.Text += connectionStr + "\r\n";

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
                richTextBox1.Text += "Connection is successfull\r\n";

                var queryStr = "SELECT * FROM public.\"Customer\"";


                richTextBox1.Text += queryStr + "\r\n";

                var cmd = new NpgsqlCommand(queryStr, connection);
                var dr = cmd.ExecuteReader();

                bool isShown = false;

                while (dr.Read())
                {
                    if (!isShown)
                    {
                        for (var i = 0; i < dr.FieldCount; ++i)
                        {
                            richTextBox1.Text += dr.GetName(i) + "\t";
                            _columnNames.Add(dr.GetName(i));
                        }
                        richTextBox1.Text += "\r\n";
                        isShown = true;
                    }

                    for (var i = 0; i < dr.FieldCount; ++i)
                    {
                        _datas.Add(dr[i]);
                        richTextBox1.Text += dr[i] + "\t";
                    }

                    richTextBox1.Text += "\r\n";
                }
                connection.Dispose();
            }
        }


        private void GetTableName(string connectionStr)
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                connection.Open();
                richTextBox1.Text += "Getting table name...\r\n";
                var queryTableName = "SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE'";
                richTextBox1.Text += queryTableName + "\r\n";

                var cmd = new NpgsqlCommand(queryTableName, connection);
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _tableName = dr[0].ToString();
                    richTextBox1.Text += "Table name=" +  _tableName +  "\r\n";
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
                    richTextBox1.Text += "Getting data types...\r\n"; 
                    richTextBox1.Text += queryDataType + "\r\n";

                    var cmd = new NpgsqlCommand(queryDataType, connection);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _dataTypes.Add(dr[0].ToString());
                        richTextBox1.Text += "Data type=" + dr[0] + "\r\n";
                    }
                    connection.Dispose();
                }
            }
        }

        private void Migrate()
        {
            string server = textBox6.Text;
            string db = textBox7.Text;

            // connect to sql sevrver
            string connectionStrSql = "Data Source=" + server + 
                                      ";Initial Catalog=" + db + 
                                      ";Integrated Security=SSPI";
            richTextBox1.Text += connectionStrSql + "\r\n";
            
            using (SqlConnection connection = new SqlConnection(connectionStrSql))
            {
                connection.Open();
                richTextBox1.Text += "migration has started" + "\t\n";

                // first create a table
                var CreateTable = "CREATE TABLE " + _tableName + "(";
                for (int i = 0; i < _dataTypes.Count; ++i)
                {
                    CreateTable += _columnNames[i] + " " + _dataTypes[i] + ", ";
                }
                CreateTable += ");";
                richTextBox1.Text += CreateTable + " is run\r\n";
               // var cmd = new SqlCommand(CreateTable, connection);
                connection.Dispose();
            }


            // add data
            for (int i = 0; i < _datas.Count / _dataTypes.Count; ++i)
            {
                string insertData = "INSERT INTO " + _tableName + "(";
                for (int j = 0; j < _dataTypes.Count; ++j)
                {
                    insertData += _columnNames[j] + ", ";
                }

                insertData += ") VALUES (";
                for (int j = 0; j < _dataTypes.Count; ++j)
                {
                    if ((string) _dataTypes[j] == "text")
                    {
                        insertData += "'" + _datas[i * 2 + j] + "',";
                    }
                    else
                    {
                        insertData +=_datas[i * 2 + j] + ",";
                    }
                }

                insertData += ")";

                richTextBox1.Text += insertData + "\r\n";
                SqlConnection connection = new SqlConnection(connectionStrSql);
                //var cmd = new SqlCommand(insertData, connection);
            }
        }
    }
}
