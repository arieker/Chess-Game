﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessUI
{
    public partial class ProfileForm : Form
    {
        public ProfileForm(User user)
        {
            Debug.Write(user.getWins());
            Debug.Write(user.getLosses());
            Debug.Write(""+ user.getDraws());
            InitializeComponent();

            nicknameTextLabel.Text = user.getNickname();
            usernameTextLabel.Text = user.getUsername();
            openingDateTextLabel.Text = user.getDate();
            if (Program.currentUser == user)
            {
                changeNicknameButton.Enabled = true;
            }
            else
            {
                sendMatchRequestButton.Enabled = true;
                sendMatchRequestButton.Visible = true;
                changeNicknameButton.Visible = false;
            }
            dataGridView1.Rows[0].Cells[0].Value = user.getWins();
            dataGridView1.Rows[0].Cells[1].Value = user.getLosses();
            dataGridView1.Rows[0].Cells[2].Value = user.getDraws();

        }

        private void changeNicknameButton_Click(object sender, EventArgs e)
        {
            ChangeNicknameForm change_nickname_form = new ChangeNicknameForm();

            change_nickname_form.ShowDialog();
            nicknameTextLabel.Text = Program.currentUser.getNickname();
        }

        private void sendMatchRequestButton_Click(object sender, EventArgs e)
        {
            // send match request to someone
        }
    }
}
