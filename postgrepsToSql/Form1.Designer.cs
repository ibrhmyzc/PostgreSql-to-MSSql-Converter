namespace postgrepsToSql
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelServer = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxDatabase = new System.Windows.Forms.TextBox();
            this.textBoxUserId = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.richTextBoxProgress = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxTargetServer = new System.Windows.Forms.TextBox();
            this.textBoxTargetDatabase = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxTargetPort = new System.Windows.Forms.TextBox();
            this.textBoxTargetUserId = new System.Windows.Forms.TextBox();
            this.textBoxTargetPassword = new System.Windows.Forms.TextBox();
            this.comboBoxSource = new System.Windows.Forms.ComboBox();
            this.comboBoxTarget = new System.Windows.Forms.ComboBox();
            this.checkBoxDetailedLog = new System.Windows.Forms.CheckBox();
            this.labelSourceTableName = new System.Windows.Forms.Label();
            this.labelTargetTableName = new System.Windows.Forms.Label();
            this.textBoxTargetTable = new System.Windows.Forms.TextBox();
            this.textBoxSourceTable = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelServer
            // 
            this.labelServer.AutoSize = true;
            this.labelServer.Location = new System.Drawing.Point(40, 54);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(38, 13);
            this.labelServer.TabIndex = 0;
            this.labelServer.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Database";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "User Id";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(40, 177);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Password";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(99, 51);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(100, 20);
            this.textBoxServer.TabIndex = 6;
            this.textBoxServer.Text = "127.0.0.1";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(99, 83);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxPort.TabIndex = 7;
            this.textBoxPort.Text = "5432";
            // 
            // textBoxDatabase
            // 
            this.textBoxDatabase.Location = new System.Drawing.Point(99, 113);
            this.textBoxDatabase.Name = "textBoxDatabase";
            this.textBoxDatabase.Size = new System.Drawing.Size(100, 20);
            this.textBoxDatabase.TabIndex = 8;
            this.textBoxDatabase.Text = "dvdrental";
            // 
            // textBoxUserId
            // 
            this.textBoxUserId.Location = new System.Drawing.Point(99, 143);
            this.textBoxUserId.Name = "textBoxUserId";
            this.textBoxUserId.Size = new System.Drawing.Size(100, 20);
            this.textBoxUserId.TabIndex = 9;
            this.textBoxUserId.Text = "postgres";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(99, 173);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(100, 20);
            this.textBoxPassword.TabIndex = 10;
            this.textBoxPassword.Text = "Logo1234";
            // 
            // buttonExecute
            // 
            this.buttonExecute.BackColor = System.Drawing.SystemColors.Highlight;
            this.buttonExecute.Location = new System.Drawing.Point(315, 373);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(100, 39);
            this.buttonExecute.TabIndex = 12;
            this.buttonExecute.Text = "Go";
            this.buttonExecute.UseVisualStyleBackColor = false;
            this.buttonExecute.Click += new System.EventHandler(this.Button1_Click);
            // 
            // richTextBoxProgress
            // 
            this.richTextBoxProgress.Location = new System.Drawing.Point(43, 228);
            this.richTextBoxProgress.Name = "richTextBoxProgress";
            this.richTextBoxProgress.Size = new System.Drawing.Size(372, 139);
            this.richTextBoxProgress.TabIndex = 13;
            this.richTextBoxProgress.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(256, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Server";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(256, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Database";
            // 
            // textBoxTargetServer
            // 
            this.textBoxTargetServer.Location = new System.Drawing.Point(315, 52);
            this.textBoxTargetServer.Name = "textBoxTargetServer";
            this.textBoxTargetServer.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetServer.TabIndex = 17;
            this.textBoxTargetServer.Text = "JAG-FRIA\\SQLEXPRESS";
            // 
            // textBoxTargetDatabase
            // 
            this.textBoxTargetDatabase.Location = new System.Drawing.Point(315, 111);
            this.textBoxTargetDatabase.Name = "textBoxTargetDatabase";
            this.textBoxTargetDatabase.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetDatabase.TabIndex = 18;
            this.textBoxTargetDatabase.Text = "STAJDB";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(256, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "TARGET";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "SOURCE";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(256, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Port";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(256, 146);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "User Id";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(256, 177);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Password";
            // 
            // textBoxTargetPort
            // 
            this.textBoxTargetPort.Location = new System.Drawing.Point(315, 80);
            this.textBoxTargetPort.Name = "textBoxTargetPort";
            this.textBoxTargetPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetPort.TabIndex = 24;
            // 
            // textBoxTargetUserId
            // 
            this.textBoxTargetUserId.Location = new System.Drawing.Point(315, 143);
            this.textBoxTargetUserId.Name = "textBoxTargetUserId";
            this.textBoxTargetUserId.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetUserId.TabIndex = 25;
            // 
            // textBoxTargetPassword
            // 
            this.textBoxTargetPassword.Location = new System.Drawing.Point(315, 173);
            this.textBoxTargetPassword.Name = "textBoxTargetPassword";
            this.textBoxTargetPassword.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetPassword.TabIndex = 26;
            // 
            // comboBoxSource
            // 
            this.comboBoxSource.FormattingEnabled = true;
            this.comboBoxSource.Items.AddRange(new object[] {
            "PostgreSql",
            "MSSql"});
            this.comboBoxSource.Location = new System.Drawing.Point(98, 15);
            this.comboBoxSource.Name = "comboBoxSource";
            this.comboBoxSource.Size = new System.Drawing.Size(101, 21);
            this.comboBoxSource.TabIndex = 27;
            this.comboBoxSource.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBoxTarget
            // 
            this.comboBoxTarget.FormattingEnabled = true;
            this.comboBoxTarget.Items.AddRange(new object[] {
            "PostgreSql",
            "MSSql"});
            this.comboBoxTarget.Location = new System.Drawing.Point(315, 12);
            this.comboBoxTarget.Name = "comboBoxTarget";
            this.comboBoxTarget.Size = new System.Drawing.Size(100, 21);
            this.comboBoxTarget.TabIndex = 28;
            this.comboBoxTarget.SelectedIndexChanged += new System.EventHandler(this.comboBoxTarget_SelectedIndexChanged);
            // 
            // checkBoxDetailedLog
            // 
            this.checkBoxDetailedLog.AutoSize = true;
            this.checkBoxDetailedLog.Location = new System.Drawing.Point(43, 382);
            this.checkBoxDetailedLog.Name = "checkBoxDetailedLog";
            this.checkBoxDetailedLog.Size = new System.Drawing.Size(220, 30);
            this.checkBoxDetailedLog.TabIndex = 29;
            this.checkBoxDetailedLog.Text = "Would you like to see detailed logs?\r\nIt will significantly slow down the progres" +
    "s";
            this.checkBoxDetailedLog.UseVisualStyleBackColor = true;
            // 
            // labelSourceTableName
            // 
            this.labelSourceTableName.AutoSize = true;
            this.labelSourceTableName.Location = new System.Drawing.Point(43, 209);
            this.labelSourceTableName.Name = "labelSourceTableName";
            this.labelSourceTableName.Size = new System.Drawing.Size(34, 13);
            this.labelSourceTableName.TabIndex = 30;
            this.labelSourceTableName.Text = "Table";
            // 
            // labelTargetTableName
            // 
            this.labelTargetTableName.AutoSize = true;
            this.labelTargetTableName.Location = new System.Drawing.Point(259, 209);
            this.labelTargetTableName.Name = "labelTargetTableName";
            this.labelTargetTableName.Size = new System.Drawing.Size(34, 13);
            this.labelTargetTableName.TabIndex = 31;
            this.labelTargetTableName.Text = "Table";
            // 
            // textBoxTargetTable
            // 
            this.textBoxTargetTable.Location = new System.Drawing.Point(315, 206);
            this.textBoxTargetTable.Name = "textBoxTargetTable";
            this.textBoxTargetTable.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetTable.TabIndex = 32;
            // 
            // textBoxSourceTable
            // 
            this.textBoxSourceTable.Location = new System.Drawing.Point(98, 206);
            this.textBoxSourceTable.Name = "textBoxSourceTable";
            this.textBoxSourceTable.Size = new System.Drawing.Size(100, 20);
            this.textBoxSourceTable.TabIndex = 33;
            this.textBoxSourceTable.Text = "actor";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 430);
            this.Controls.Add(this.textBoxSourceTable);
            this.Controls.Add(this.textBoxTargetTable);
            this.Controls.Add(this.labelTargetTableName);
            this.Controls.Add(this.labelSourceTableName);
            this.Controls.Add(this.checkBoxDetailedLog);
            this.Controls.Add(this.comboBoxTarget);
            this.Controls.Add(this.comboBoxSource);
            this.Controls.Add(this.textBoxTargetPassword);
            this.Controls.Add(this.textBoxTargetUserId);
            this.Controls.Add(this.textBoxTargetPort);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxTargetDatabase);
            this.Controls.Add(this.textBoxTargetServer);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.richTextBoxProgress);
            this.Controls.Add(this.buttonExecute);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUserId);
            this.Controls.Add(this.textBoxDatabase);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxServer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelServer);
            this.Name = "Form1";
            this.Text = "Postgre to Sql";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxDatabase;
        private System.Windows.Forms.TextBox textBoxUserId;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.RichTextBox richTextBoxProgress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxTargetServer;
        private System.Windows.Forms.TextBox textBoxTargetDatabase;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxTargetPort;
        private System.Windows.Forms.TextBox textBoxTargetUserId;
        private System.Windows.Forms.TextBox textBoxTargetPassword;
        private System.Windows.Forms.ComboBox comboBoxSource;
        private System.Windows.Forms.ComboBox comboBoxTarget;
        private System.Windows.Forms.CheckBox checkBoxDetailedLog;
        private System.Windows.Forms.Label labelSourceTableName;
        private System.Windows.Forms.Label labelTargetTableName;
        private System.Windows.Forms.TextBox textBoxTargetTable;
        private System.Windows.Forms.TextBox textBoxSourceTable;
    }
}

