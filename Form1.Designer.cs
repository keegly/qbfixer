namespace qbfixer
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
            this.load_spreadsheet_btn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.excel_file_lbl = new System.Windows.Forms.Label();
            this.quit_btn = new System.Windows.Forms.Button();
            this.import_cheque_btn = new System.Windows.Forms.Button();
            this.customer_listbox = new System.Windows.Forms.ListBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.invoices_listview = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // load_spreadsheet_btn
            // 
            this.load_spreadsheet_btn.Location = new System.Drawing.Point(12, 12);
            this.load_spreadsheet_btn.Name = "load_spreadsheet_btn";
            this.load_spreadsheet_btn.Size = new System.Drawing.Size(81, 43);
            this.load_spreadsheet_btn.TabIndex = 0;
            this.load_spreadsheet_btn.Text = "Pick .xls";
            this.load_spreadsheet_btn.UseVisualStyleBackColor = true;
            this.load_spreadsheet_btn.Click += new System.EventHandler(this.load_spreadsheet_btn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Excel Worksheets|*.xls;*.xlsx|All Files|*.*";
            this.openFileDialog1.InitialDirectory = "C:\\Users\\Keeg\\Desktop";
            this.openFileDialog1.Title = "Select a Spreadsheet";
            // 
            // excel_file_lbl
            // 
            this.excel_file_lbl.Location = new System.Drawing.Point(108, 12);
            this.excel_file_lbl.Name = "excel_file_lbl";
            this.excel_file_lbl.Size = new System.Drawing.Size(100, 43);
            this.excel_file_lbl.TabIndex = 1;
            this.excel_file_lbl.Text = "*.xls";
            this.excel_file_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // quit_btn
            // 
            this.quit_btn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.quit_btn.Location = new System.Drawing.Point(13, 403);
            this.quit_btn.Name = "quit_btn";
            this.quit_btn.Size = new System.Drawing.Size(365, 27);
            this.quit_btn.TabIndex = 4;
            this.quit_btn.Text = "Exit";
            this.quit_btn.UseVisualStyleBackColor = true;
            this.quit_btn.Click += new System.EventHandler(this.quit_btn_Click);
            // 
            // import_cheque_btn
            // 
            this.import_cheque_btn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.import_cheque_btn.Location = new System.Drawing.Point(12, 340);
            this.import_cheque_btn.Name = "import_cheque_btn";
            this.import_cheque_btn.Size = new System.Drawing.Size(366, 43);
            this.import_cheque_btn.TabIndex = 6;
            this.import_cheque_btn.Text = "Process Payment";
            this.import_cheque_btn.UseVisualStyleBackColor = true;
            this.import_cheque_btn.Click += new System.EventHandler(this.import_cheque_btn_Click);
            // 
            // customer_listbox
            // 
            this.customer_listbox.FormattingEnabled = true;
            this.customer_listbox.Location = new System.Drawing.Point(214, 12);
            this.customer_listbox.Name = "customer_listbox";
            this.customer_listbox.ScrollAlwaysVisible = true;
            this.customer_listbox.Size = new System.Drawing.Size(164, 56);
            this.customer_listbox.TabIndex = 9;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 291);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(366, 43);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 11;
            // 
            // invoices_listview
            // 
            this.invoices_listview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.invoices_listview.Location = new System.Drawing.Point(12, 74);
            this.invoices_listview.Name = "invoices_listview";
            this.invoices_listview.Size = new System.Drawing.Size(365, 211);
            this.invoices_listview.TabIndex = 12;
            this.invoices_listview.UseCompatibleStateImageBehavior = false;
            this.invoices_listview.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Invoice #";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Total";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 451);
            this.Controls.Add(this.invoices_listview);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.customer_listbox);
            this.Controls.Add(this.import_cheque_btn);
            this.Controls.Add(this.quit_btn);
            this.Controls.Add(this.excel_file_lbl);
            this.Controls.Add(this.load_spreadsheet_btn);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Bluefire Invoicing Assistant";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button load_spreadsheet_btn;
        private System.Windows.Forms.Label excel_file_lbl;
        private System.Windows.Forms.Button quit_btn;
        private System.Windows.Forms.Button import_cheque_btn;
        private System.Windows.Forms.ListBox customer_listbox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListView invoices_listview;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;

    }
}

