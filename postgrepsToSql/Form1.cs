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
        private const int ProgressBarMaxSize = 100;

        public Form1()
        {
            InitializeComponent();
            comboBoxSource.SelectedIndex = 0;
            comboBoxTarget.SelectedIndex = 1;
            progressBar.Maximum = ProgressBarMaxSize;
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            RunThread();
        }

        private void RunThread()
        {
            Debug.WriteLine("Run thread method");
            var connectionStrSource = GetConnectionStrFromSource();
            var connectionStrTarget = GetConnectionStrFromTarget();
            var type = comboBoxSource.SelectedIndex;
            var tableName = textBoxSourceTable.Text;

            var myTask = new DatabaseTask(connectionStrSource, connectionStrTarget, type, tableName);
            myTask.DoWithThreads();
        }
 

        private string GetPostgreSqlConnectionStr()
        {
            return string.Format("Server=" + textBoxServer.Text +
                                 ";Port=" + textBoxPort.Text +
                                 ";Database=" + textBoxDatabase.Text +
                                 ";User Id=" + textBoxUserId.Text +
                                 ";Password=" + textBoxPassword.Text +
                                 ";Integrated Security=true;"
            );
        }
        

        private string GetSqlConnectionStr()
        {
            //return "SERVER=172.16.1.25; DATABASE=diyalogo; uId=LOGO; Password=LOGO";

            return string.Format("Data Source=" + textBoxTargetServer.Text +
                ";Initial Catalog=" + textBoxTargetDatabase.Text +
                ";Integrated Security=SSPI");
        }
        
        private string GetConnectionStrFromSource()
        { 
            switch (comboBoxSource.SelectedIndex)
            {
                case 0:
                    return GetPostgreSqlConnectionStr();
                
                case 1:
                    return GetSqlConnectionStr();
                
                default:
                    throw new ArgumentNullException("No Database is selected");                    
            }       
        }
        

        private string GetConnectionStrFromTarget()
        {         
            switch (comboBoxTarget.SelectedIndex)
            {
                case 0:
                    return GetPostgreSqlConnectionStr();

                case 1:
                    return GetSqlConnectionStr();

                default:
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

                default:
                    textBoxTargetUserId.Enabled = false;
                    textBoxTargetPassword.Enabled = false;
                    textBoxTargetPort.Enabled = false;
                    break;
            }
        }
    }
}
