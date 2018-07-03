using System;
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
                        }
                        richTextBox1.Text += "\r\n";
                        isShown = true;
                    }
                    
                    for (var i = 0; i < dr.FieldCount; ++i)
                    {
                        richTextBox1.Text += dr[i] + "\t";
                    }
                    
                    richTextBox1.Text += "\r\n";
                }

            }

        }
    }
}
