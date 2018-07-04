using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            for (int i = 0; i < _datas.Count; ++i)
                richTextBox1.Text += _datas[i] + "\r\n";
        }

        private void GetDataTypes(string connectionStr)
        {
            for (int i = 0; i < _columnNames.Count; ++i)
            {
                var queryDataType = "SELECT data_type" +
                                    " FROM information_schema.columns   " +
                                    " WHERE table_schema = 'public'" +
                                    " AND table_name =" + "'" + "Customer" + "'" +
                                    " AND column_name =" + "'" + _columnNames[i] + "'";
            }
        }

        private void Migrate()
        {
            // connect to sql sevrver
            string connectionStrSql = "Data Source = ; Initial Catalog = STAJDB; Integrated Security = True";
        }
    }
}
