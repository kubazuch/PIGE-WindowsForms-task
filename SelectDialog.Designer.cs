namespace DiskSpaceAnalyzer
{
    partial class SelectDialog
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
            this.allRadio = new System.Windows.Forms.RadioButton();
            this.individualRadio = new System.Windows.Forms.RadioButton();
            this.modalCancel = new System.Windows.Forms.Button();
            this.modalOK = new System.Windows.Forms.Button();
            this.folderRadio = new System.Windows.Forms.RadioButton();
            this.folderChosen = new System.Windows.Forms.TextBox();
            this.selectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // allRadio
            // 
            this.allRadio.AutoSize = true;
            this.allRadio.Location = new System.Drawing.Point(12, 12);
            this.allRadio.Name = "allRadio";
            this.allRadio.Size = new System.Drawing.Size(105, 19);
            this.allRadio.TabIndex = 0;
            this.allRadio.TabStop = true;
            this.allRadio.Text = "&All Local Drives";
            this.allRadio.UseVisualStyleBackColor = true;
            // 
            // individualRadio
            // 
            this.individualRadio.AutoSize = true;
            this.individualRadio.Location = new System.Drawing.Point(12, 37);
            this.individualRadio.Name = "individualRadio";
            this.individualRadio.Size = new System.Drawing.Size(112, 19);
            this.individualRadio.TabIndex = 1;
            this.individualRadio.TabStop = true;
            this.individualRadio.Text = "&Individual Drives";
            this.individualRadio.UseVisualStyleBackColor = true;
            // 
            // modalCancel
            // 
            this.modalCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modalCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.modalCancel.Location = new System.Drawing.Point(397, 226);
            this.modalCancel.Name = "modalCancel";
            this.modalCancel.Size = new System.Drawing.Size(75, 23);
            this.modalCancel.TabIndex = 3;
            this.modalCancel.Text = "Cancel";
            this.modalCancel.UseVisualStyleBackColor = true;
            // 
            // modalOK
            // 
            this.modalOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modalOK.Location = new System.Drawing.Point(316, 226);
            this.modalOK.Name = "modalOK";
            this.modalOK.Size = new System.Drawing.Size(75, 23);
            this.modalOK.TabIndex = 4;
            this.modalOK.Text = "OK";
            this.modalOK.UseVisualStyleBackColor = true;
            this.modalOK.Click += new System.EventHandler(this.modalOK_Click);
            // 
            // folderRadio
            // 
            this.folderRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.folderRadio.AutoSize = true;
            this.folderRadio.Location = new System.Drawing.Point(12, 172);
            this.folderRadio.Name = "folderRadio";
            this.folderRadio.Size = new System.Drawing.Size(69, 19);
            this.folderRadio.TabIndex = 5;
            this.folderRadio.TabStop = true;
            this.folderRadio.Text = "A &Folder";
            this.folderRadio.UseVisualStyleBackColor = true;
            // 
            // folderChosen
            // 
            this.folderChosen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folderChosen.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.folderChosen.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.folderChosen.Location = new System.Drawing.Point(12, 197);
            this.folderChosen.Name = "folderChosen";
            this.folderChosen.Size = new System.Drawing.Size(379, 23);
            this.folderChosen.TabIndex = 6;
            this.folderChosen.TextChanged += new System.EventHandler(this.folderChosen_TextChanged);
            // 
            // selectButton
            // 
            this.selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectButton.Location = new System.Drawing.Point(397, 197);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(75, 23);
            this.selectButton.TabIndex = 7;
            this.selectButton.Text = "...";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // SelectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.folderChosen);
            this.Controls.Add(this.folderRadio);
            this.Controls.Add(this.modalOK);
            this.Controls.Add(this.modalCancel);
            this.Controls.Add(this.individualRadio);
            this.Controls.Add(this.allRadio);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "SelectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Disk or Folder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RadioButton allRadio;
        private RadioButton individualRadio;
        private Button modalCancel;
        private Button modalOK;
        private RadioButton folderRadio;
        private TextBox folderChosen;
        private Button selectButton;
    }
}