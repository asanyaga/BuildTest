namespace CussonsIntegrationWindowsFormsUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.labelSchedule = new System.Windows.Forms.Label();
            this.masterDatalistBox = new System.Windows.Forms.ListBox();
            this.buttonImport = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxTimeOptions = new System.Windows.Forms.ComboBox();
            this.labelScheduleDisplayer = new System.Windows.Forms.Label();
            this.buttonClearlogs = new System.Windows.Forms.Button();
            this.LogsListBox = new System.Windows.Forms.ListBox();
            this.SelectAllcheckBox = new System.Windows.Forms.CheckBox();
            this.progressPictureBox = new System.Windows.Forms.PictureBox();
            this.buttonViewLogs = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.searchOrderTextBox = new System.Windows.Forms.TextBox();
            this.buttonDwnload = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.wstextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.progressPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Activity Log";
            // 
            // labelSchedule
            // 
            this.labelSchedule.AutoSize = true;
            this.labelSchedule.Location = new System.Drawing.Point(30, 322);
            this.labelSchedule.Name = "labelSchedule";
            this.labelSchedule.Size = new System.Drawing.Size(52, 13);
            this.labelSchedule.TabIndex = 3;
            this.labelSchedule.Text = "Schedule";
            // 
            // masterDatalistBox
            // 
            this.masterDatalistBox.FormattingEnabled = true;
            this.masterDatalistBox.Location = new System.Drawing.Point(335, 29);
            this.masterDatalistBox.Name = "masterDatalistBox";
            this.masterDatalistBox.Size = new System.Drawing.Size(120, 160);
            this.masterDatalistBox.TabIndex = 4;
            this.masterDatalistBox.SelectedIndexChanged += new System.EventHandler(this.SelectedMasterDataChanged);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(388, 195);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(67, 23);
            this.buttonImport.TabIndex = 5;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(342, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Master Data";
            // 
            // comboBoxTimeOptions
            // 
            this.comboBoxTimeOptions.FormattingEnabled = true;
            this.comboBoxTimeOptions.Location = new System.Drawing.Point(33, 339);
            this.comboBoxTimeOptions.Name = "comboBoxTimeOptions";
            this.comboBoxTimeOptions.Size = new System.Drawing.Size(176, 21);
            this.comboBoxTimeOptions.TabIndex = 7;
            this.comboBoxTimeOptions.SelectedIndexChanged += new System.EventHandler(this.SelectedHourChanged);
            // 
            // labelScheduleDisplayer
            // 
            this.labelScheduleDisplayer.AutoSize = true;
            this.labelScheduleDisplayer.Location = new System.Drawing.Point(247, 339);
            this.labelScheduleDisplayer.Name = "labelScheduleDisplayer";
            this.labelScheduleDisplayer.Size = new System.Drawing.Size(89, 13);
            this.labelScheduleDisplayer.TabIndex = 8;
            this.labelScheduleDisplayer.Text = "Current Schedule";
            // 
            // buttonClearlogs
            // 
            this.buttonClearlogs.Location = new System.Drawing.Point(169, 299);
            this.buttonClearlogs.Name = "buttonClearlogs";
            this.buttonClearlogs.Size = new System.Drawing.Size(75, 23);
            this.buttonClearlogs.TabIndex = 9;
            this.buttonClearlogs.Text = "Clear Logs";
            this.buttonClearlogs.UseVisualStyleBackColor = true;
            this.buttonClearlogs.Click += new System.EventHandler(this.buttonClearlogs_Click);
            // 
            // LogsListBox
            // 
            this.LogsListBox.FormattingEnabled = true;
            this.LogsListBox.HorizontalScrollbar = true;
            this.LogsListBox.Location = new System.Drawing.Point(12, 68);
            this.LogsListBox.Name = "LogsListBox";
            this.LogsListBox.Size = new System.Drawing.Size(310, 225);
            this.LogsListBox.TabIndex = 10;
            // 
            // SelectAllcheckBox
            // 
            this.SelectAllcheckBox.AutoSize = true;
            this.SelectAllcheckBox.Location = new System.Drawing.Point(345, 200);
            this.SelectAllcheckBox.Name = "SelectAllcheckBox";
            this.SelectAllcheckBox.Size = new System.Drawing.Size(37, 17);
            this.SelectAllcheckBox.TabIndex = 12;
            this.SelectAllcheckBox.Text = "All";
            this.SelectAllcheckBox.UseVisualStyleBackColor = true;
            this.SelectAllcheckBox.Click += new System.EventHandler(this.SelectAllClicked);
            // 
            // progressPictureBox
            // 
            this.progressPictureBox.Location = new System.Drawing.Point(345, 235);
            this.progressPictureBox.Name = "progressPictureBox";
            this.progressPictureBox.Size = new System.Drawing.Size(100, 87);
            this.progressPictureBox.TabIndex = 13;
            this.progressPictureBox.TabStop = false;
            // 
            // buttonViewLogs
            // 
            this.buttonViewLogs.Location = new System.Drawing.Point(88, 299);
            this.buttonViewLogs.Name = "buttonViewLogs";
            this.buttonViewLogs.Size = new System.Drawing.Size(75, 23);
            this.buttonViewLogs.TabIndex = 14;
            this.buttonViewLogs.Text = "View Logs";
            this.buttonViewLogs.UseVisualStyleBackColor = true;
            this.buttonViewLogs.Click += new System.EventHandler(this.buttonViewLogs_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_CompletedWork);
            // 
            // searchOrderTextBox
            // 
            this.searchOrderTextBox.Location = new System.Drawing.Point(105, 29);
            this.searchOrderTextBox.Name = "searchOrderTextBox";
            this.searchOrderTextBox.Size = new System.Drawing.Size(153, 20);
            this.searchOrderTextBox.TabIndex = 15;
            // 
            // buttonDwnload
            // 
            this.buttonDwnload.Location = new System.Drawing.Point(264, 26);
            this.buttonDwnload.Name = "buttonDwnload";
            this.buttonDwnload.Size = new System.Drawing.Size(65, 23);
            this.buttonDwnload.TabIndex = 16;
            this.buttonDwnload.Text = "Download..";
            this.buttonDwnload.UseVisualStyleBackColor = true;
            this.buttonDwnload.Click += new System.EventHandler(this.buttonDwnload_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(104, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "(Enter order ref..e.g TX000001)";
            // 
            // wstextBox
            // 
            this.wstextBox.Location = new System.Drawing.Point(33, 390);
            this.wstextBox.Name = "wstextBox";
            this.wstextBox.Size = new System.Drawing.Size(257, 20);
            this.wstextBox.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 365);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "WebApi";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 422);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.wstextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonDwnload);
            this.Controls.Add(this.searchOrderTextBox);
            this.Controls.Add(this.buttonViewLogs);
            this.Controls.Add(this.progressPictureBox);
            this.Controls.Add(this.SelectAllcheckBox);
            this.Controls.Add(this.LogsListBox);
            this.Controls.Add(this.buttonClearlogs);
            this.Controls.Add(this.labelScheduleDisplayer);
            this.Controls.Add(this.comboBoxTimeOptions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.masterDatalistBox);
            this.Controls.Add(this.labelSchedule);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Distributr Integration Service";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.progressPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSchedule;
        private System.Windows.Forms.ListBox masterDatalistBox;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxTimeOptions;
        private System.Windows.Forms.Label labelScheduleDisplayer;
        private System.Windows.Forms.Button buttonClearlogs;
        private System.Windows.Forms.ListBox LogsListBox;
        private System.Windows.Forms.CheckBox SelectAllcheckBox;
        private System.Windows.Forms.PictureBox progressPictureBox;
        private System.Windows.Forms.Button buttonViewLogs;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox searchOrderTextBox;
        private System.Windows.Forms.Button buttonDwnload;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox wstextBox;
        private System.Windows.Forms.Label label4;
    }
}

