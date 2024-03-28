namespace DropTool
{
    partial class SearchDialogForm
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
            this.TextBox_SearchInput = new System.Windows.Forms.MaskedTextBox();
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Label_MainText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TextBox_SearchInput
            // 
            this.TextBox_SearchInput.Location = new System.Drawing.Point(12, 39);
            this.TextBox_SearchInput.Name = "TextBox_SearchInput";
            this.TextBox_SearchInput.Size = new System.Drawing.Size(273, 20);
            this.TextBox_SearchInput.TabIndex = 0;
            this.TextBox_SearchInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.Location = new System.Drawing.Point(68, 69);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.Btn_Cancel.TabIndex = 1;
            this.Btn_Cancel.Text = "Cancel";
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            this.Btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
            // 
            // Btn_Search
            // 
            this.Btn_Search.Location = new System.Drawing.Point(149, 69);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(75, 23);
            this.Btn_Search.TabIndex = 2;
            this.Btn_Search.Text = "Search";
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Label_MainText
            // 
            this.Label_MainText.Location = new System.Drawing.Point(12, 9);
            this.Label_MainText.Name = "Label_MainText";
            this.Label_MainText.Size = new System.Drawing.Size(273, 23);
            this.Label_MainText.TabIndex = 3;
            this.Label_MainText.Text = "Provide here Monster ID";
            this.Label_MainText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SearchDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 104);
            this.Controls.Add(this.Label_MainText);
            this.Controls.Add(this.Btn_Search);
            this.Controls.Add(this.Btn_Cancel);
            this.Controls.Add(this.TextBox_SearchInput);
            this.Name = "SearchDialogForm";
            this.Text = "SearchDialogForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox TextBox_SearchInput;
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.Label Label_MainText;
    }
}