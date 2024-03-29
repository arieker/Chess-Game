﻿namespace ChessUI
{
    partial class FindMatchForm
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
            this.onlineUsersListBox = new System.Windows.Forms.ListBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.onlineUsersListBoxLabel = new System.Windows.Forms.Label();
            this.viewProfileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // onlineUsersListBox
            // 
            this.onlineUsersListBox.FormattingEnabled = true;
            this.onlineUsersListBox.Location = new System.Drawing.Point(12, 25);
            this.onlineUsersListBox.Name = "onlineUsersListBox";
            this.onlineUsersListBox.Size = new System.Drawing.Size(189, 225);
            this.onlineUsersListBox.TabIndex = 0;
            this.onlineUsersListBox.SelectedIndexChanged += new System.EventHandler(this.onlineUsersListBox_SelectedIndexChanged);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(207, 25);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // onlineUsersListBoxLabel
            // 
            this.onlineUsersListBoxLabel.AutoSize = true;
            this.onlineUsersListBoxLabel.Location = new System.Drawing.Point(12, 9);
            this.onlineUsersListBoxLabel.Name = "onlineUsersListBoxLabel";
            this.onlineUsersListBoxLabel.Size = new System.Drawing.Size(70, 13);
            this.onlineUsersListBoxLabel.TabIndex = 2;
            this.onlineUsersListBoxLabel.Text = "Online Users:";
            // 
            // viewProfileButton
            // 
            this.viewProfileButton.Enabled = false;
            this.viewProfileButton.Location = new System.Drawing.Point(12, 256);
            this.viewProfileButton.Name = "viewProfileButton";
            this.viewProfileButton.Size = new System.Drawing.Size(84, 23);
            this.viewProfileButton.TabIndex = 3;
            this.viewProfileButton.Text = "View Profile";
            this.viewProfileButton.UseVisualStyleBackColor = true;
            this.viewProfileButton.Click += new System.EventHandler(this.sendMatchRequestButton_Click);
            // 
            // FindMatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 286);
            this.Controls.Add(this.viewProfileButton);
            this.Controls.Add(this.onlineUsersListBoxLabel);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.onlineUsersListBox);
            this.Name = "FindMatchForm";
            this.Text = "Find a Match";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox onlineUsersListBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Label onlineUsersListBoxLabel;
        private System.Windows.Forms.Button viewProfileButton;
    }
}