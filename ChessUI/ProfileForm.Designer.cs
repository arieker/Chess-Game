namespace ChessUI
{
    partial class ProfileForm
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
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.openingDateLabel = new System.Windows.Forms.Label();
            this.openingDateTextBox = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Wins = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Losses = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Draws = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(16, 11);
            this.usernameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(73, 16);
            this.usernameLabel.TabIndex = 0;
            this.usernameLabel.Text = "Username:";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(20, 31);
            this.usernameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.ReadOnly = true;
            this.usernameTextBox.Size = new System.Drawing.Size(239, 22);
            this.usernameTextBox.TabIndex = 2;
            this.usernameTextBox.TabStop = false;
            // 
            // openingDateLabel
            // 
            this.openingDateLabel.AutoSize = true;
            this.openingDateLabel.Location = new System.Drawing.Point(16, 59);
            this.openingDateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.openingDateLabel.Name = "openingDateLabel";
            this.openingDateLabel.Size = new System.Drawing.Size(144, 16);
            this.openingDateLabel.TabIndex = 5;
            this.openingDateLabel.Text = "Account Opening Date:";
            // 
            // openingDateTextBox
            // 
            this.openingDateTextBox.Location = new System.Drawing.Point(20, 79);
            this.openingDateTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.openingDateTextBox.Name = "openingDateTextBox";
            this.openingDateTextBox.ReadOnly = true;
            this.openingDateTextBox.Size = new System.Drawing.Size(239, 22);
            this.openingDateTextBox.TabIndex = 4;
            this.openingDateTextBox.TabStop = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Wins,
            this.Losses,
            this.Draws});
            this.dataGridView1.Location = new System.Drawing.Point(16, 122);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView1.Size = new System.Drawing.Size(460, 55);
            this.dataGridView1.TabIndex = 6;
            this.dataGridView1.TabStop = false;
            // 
            // Wins
            // 
            this.Wins.HeaderText = "Wins";
            this.Wins.MinimumWidth = 6;
            this.Wins.Name = "Wins";
            this.Wins.ReadOnly = true;
            this.Wins.Width = 125;
            // 
            // Losses
            // 
            this.Losses.HeaderText = "Losses";
            this.Losses.MinimumWidth = 6;
            this.Losses.Name = "Losses";
            this.Losses.ReadOnly = true;
            this.Losses.Width = 125;
            // 
            // Draws
            // 
            this.Draws.HeaderText = "Draws";
            this.Draws.MinimumWidth = 6;
            this.Draws.Name = "Draws";
            this.Draws.ReadOnly = true;
            this.Draws.Width = 125;
            // 
            // ProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 198);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.openingDateLabel);
            this.Controls.Add(this.openingDateTextBox);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.usernameLabel);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ProfileForm";
            this.Text = "Account Information";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label openingDateLabel;
        private System.Windows.Forms.TextBox openingDateTextBox;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Wins;
        private System.Windows.Forms.DataGridViewTextBoxColumn Losses;
        private System.Windows.Forms.DataGridViewTextBoxColumn Draws;
    }
}