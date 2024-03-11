using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessUI
{
    public partial class ChangeNicknameForm : Form
    {
        public ChangeNicknameForm()
        {
            // optionally auto populate the textbox from the start with existing username, delete comment if you don't want to do this
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Change nickname to what is found in textbox
        }
    }
}
