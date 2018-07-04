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
            this.richTextBoxProgres = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxTargetServer = new System.Windows.Forms.TextBox();
            this.textBoxTargetDatabase = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelServer
            // 
            this.labelServer.AutoSize = true;
            this.labelServer.Location = new System.Drawing.Point(40, 27);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(38, 13);
            this.labelServer.TabIndex = 0;
            this.labelServer.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Database";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "User Id";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(40, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Password";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(99, 24);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(100, 20);
            this.textBoxServer.TabIndex = 6;
            this.textBoxServer.Text = "127.0.0.1";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(99, 56);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxPort.TabIndex = 7;
            this.textBoxPort.Text = "5432";
            // 
            // textBoxDatabase
            // 
            this.textBoxDatabase.Location = new System.Drawing.Point(99, 86);
            this.textBoxDatabase.Name = "textBoxDatabase";
            this.textBoxDatabase.Size = new System.Drawing.Size(100, 20);
            this.textBoxDatabase.TabIndex = 8;
            this.textBoxDatabase.Text = "postgres";
            // 
            // textBoxUserId
            // 
            this.textBoxUserId.Location = new System.Drawing.Point(99, 116);
            this.textBoxUserId.Name = "textBoxUserId";
            this.textBoxUserId.Size = new System.Drawing.Size(100, 20);
            this.textBoxUserId.TabIndex = 9;
            this.textBoxUserId.Text = "postgres";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(99, 146);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(100, 20);
            this.textBoxPassword.TabIndex = 10;
            this.textBoxPassword.Text = "Logo1234";
            // 
            // buttonExecute
            // 
            this.buttonExecute.Location = new System.Drawing.Point(43, 340);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(156, 49);
            this.buttonExecute.TabIndex = 12;
            this.buttonExecute.Text = "Go";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBoxProgres
            // 
            this.richTextBoxProgres.Location = new System.Drawing.Point(244, 24);
            this.richTextBoxProgres.Name = "richTextBoxProgres";
            this.richTextBoxProgres.Size = new System.Drawing.Size(489, 365);
            this.richTextBoxProgres.TabIndex = 13;
            this.richTextBoxProgres.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(40, 252);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Server";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(40, 292);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "DB Name";
            // 
            // textBoxTargetServer
            // 
            this.textBoxTargetServer.Location = new System.Drawing.Point(99, 249);
            this.textBoxTargetServer.Name = "textBoxTargetServer";
            this.textBoxTargetServer.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetServer.TabIndex = 17;
            this.textBoxTargetServer.Text = "GEBZEHAVUZ11";
            // 
            // textBoxTargetDatabase
            // 
            this.textBoxTargetDatabase.Location = new System.Drawing.Point(99, 289);
            this.textBoxTargetDatabase.Name = "textBoxTargetDatabase";
            this.textBoxTargetDatabase.Size = new System.Drawing.Size(100, 20);
            this.textBoxTargetDatabase.TabIndex = 18;
            this.textBoxTargetDatabase.Text = "STAJDB";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(96, 215);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "TARGET";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxTargetDatabase);
            this.Controls.Add(this.textBoxTargetServer);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.richTextBoxProgres);
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
        private System.Windows.Forms.RichTextBox richTextBoxProgres;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxTargetServer;
        private System.Windows.Forms.TextBox textBoxTargetDatabase;
        private System.Windows.Forms.Label label8;
    }
}

