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
    public partial class RegisterForm : Form
    {
        public static bool IsLoggedIn { get; set; } // Technically bad practice to be public but idrc feel free to change this

        public RegisterForm()
        {
            InitializeComponent();
            okButton.Enabled = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxes_TextChanged(object sender, EventArgs e)
        {
            if (usernameTextBox.Text.Length > 0 && passwordTextBox.Text.Length > 0 && confirmPasswordTextBox.Text.Length > 0)
            {
                okButton.Enabled = true;
            }
            else
            {
                okButton.Enabled = false;
            }
        }
    }
}
