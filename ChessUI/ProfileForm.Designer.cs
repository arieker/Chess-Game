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
            this.openingDateLabel = new System.Windows.Forms.Label();
            this.Draws = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Losses = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Wins = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.nicknameLabel = new System.Windows.Forms.Label();
            this.changeNicknameButton = new System.Windows.Forms.Button();
            this.usernameTextLabel = new System.Windows.Forms.Label();
            this.openingDateTextLabel = new System.Windows.Forms.Label();
            this.nicknameTextLabel = new System.Windows.Forms.Label();
            this.sendMatchRequestButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(9, 48);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(58, 13);
            this.usernameLabel.TabIndex = 0;
            this.usernameLabel.Text = "Username:";
            // 
            // openingDateLabel
            // 
            this.openingDateLabel.AutoSize = true;
            this.openingDateLabel.Location = new System.Drawing.Point(9, 87);
            this.openingDateLabel.Name = "openingDateLabel";
            this.openingDateLabel.Size = new System.Drawing.Size(119, 13);
            this.openingDateLabel.TabIndex = 5;
            this.openingDateLabel.Text = "Account Opening Date:";
            // 
            // Draws
            // 
            this.Draws.HeaderText = "Draws";
            this.Draws.MinimumWidth = 6;
            this.Draws.Name = "Draws";
            this.Draws.ReadOnly = true;
            this.Draws.Width = 125;
            // 
            // Losses
            // 
            this.Losses.HeaderText = "Losses";
            this.Losses.MinimumWidth = 6;
            this.Losses.Name = "Losses";
            this.Losses.ReadOnly = true;
            this.Losses.Width = 125;
            // 
            // Wins
            // 
            this.Wins.HeaderText = "Wins";
            this.Wins.MinimumWidth = 6;
            this.Wins.Name = "Wins";
            this.Wins.ReadOnly = true;
            this.Wins.Width = 125;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Wins,
            this.Losses,
            this.Draws});
            this.dataGridView1.Location = new System.Drawing.Point(12, 140);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView1.Size = new System.Drawing.Size(345, 45);
            this.dataGridView1.TabIndex = 6;
            this.dataGridView1.TabStop = false;
            // 
            // nicknameLabel
            // 
            this.nicknameLabel.AutoSize = true;
            this.nicknameLabel.Location = new System.Drawing.Point(9, 9);
            this.nicknameLabel.Name = "nicknameLabel";
            this.nicknameLabel.Size = new System.Drawing.Size(58, 13);
            this.nicknameLabel.TabIndex = 7;
            this.nicknameLabel.Text = "Nickname:";
            // 
            // changeNicknameButton
            // 
            this.changeNicknameButton.Enabled = false;
            this.changeNicknameButton.Location = new System.Drawing.Point(208, 22);
            this.changeNicknameButton.Name = "changeNicknameButton";
            this.changeNicknameButton.Size = new System.Drawing.Size(149, 23);
            this.changeNicknameButton.TabIndex = 9;
            this.changeNicknameButton.Text = "Change Nickname";
            this.changeNicknameButton.UseVisualStyleBackColor = true;
            this.changeNicknameButton.Click += new System.EventHandler(this.changeNicknameButton_Click);
            // 
            // usernameTextLabel
            // 
            this.usernameTextLabel.AutoSize = true;
            this.usernameTextLabel.Location = new System.Drawing.Point(17, 68);
            this.usernameTextLabel.Name = "usernameTextLabel";
            this.usernameTextLabel.Size = new System.Drawing.Size(100, 13);
            this.usernameTextLabel.TabIndex = 10;
            this.usernameTextLabel.Text = "usernameTextLabel";
            // 
            // openingDateTextLabel
            // 
            this.openingDateTextLabel.AutoSize = true;
            this.openingDateTextLabel.Location = new System.Drawing.Point(17, 107);
            this.openingDateTextLabel.Name = "openingDateTextLabel";
            this.openingDateTextLabel.Size = new System.Drawing.Size(115, 13);
            this.openingDateTextLabel.TabIndex = 11;
            this.openingDateTextLabel.Text = "openingDateTextLabel";
            // 
            // nicknameTextLabel
            // 
            this.nicknameTextLabel.AutoSize = true;
            this.nicknameTextLabel.Location = new System.Drawing.Point(17, 29);
            this.nicknameTextLabel.Name = "nicknameTextLabel";
            this.nicknameTextLabel.Size = new System.Drawing.Size(100, 13);
            this.nicknameTextLabel.TabIndex = 12;
            this.nicknameTextLabel.Text = "nicknameTextLabel";
            // 
            // sendMatchRequestButton
            // 
            this.sendMatchRequestButton.Enabled = false;
            this.sendMatchRequestButton.Location = new System.Drawing.Point(208, 82);
            this.sendMatchRequestButton.Name = "sendMatchRequestButton";
            this.sendMatchRequestButton.Size = new System.Drawing.Size(149, 23);
            this.sendMatchRequestButton.TabIndex = 13;
            this.sendMatchRequestButton.Text = "Send Match Request";
            this.sendMatchRequestButton.UseVisualStyleBackColor = true;
            this.sendMatchRequestButton.Visible = false;
            this.sendMatchRequestButton.Click += new System.EventHandler(this.sendMatchRequestButton_Click);
            // 
            // ProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 206);
            this.Controls.Add(this.sendMatchRequestButton);
            this.Controls.Add(this.nicknameTextLabel);
            this.Controls.Add(this.openingDateTextLabel);
            this.Controls.Add(this.usernameTextLabel);
            this.Controls.Add(this.changeNicknameButton);
            this.Controls.Add(this.nicknameLabel);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.openingDateLabel);
            this.Controls.Add(this.usernameLabel);
            this.Name = "ProfileForm";
            this.Text = "Account Information";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label openingDateLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Draws;
        private System.Windows.Forms.DataGridViewTextBoxColumn Losses;
        private System.Windows.Forms.DataGridViewTextBoxColumn Wins;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label nicknameLabel;
        private System.Windows.Forms.Button changeNicknameButton;
        private System.Windows.Forms.Label usernameTextLabel;
        private System.Windows.Forms.Label openingDateTextLabel;
        private System.Windows.Forms.Label nicknameTextLabel;
        private System.Windows.Forms.Button sendMatchRequestButton;
    }
}