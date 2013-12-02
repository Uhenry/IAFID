using System;
using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;
using QuickXmlReader.Functions;

namespace QuickXmlReader
{
    public partial class frmLogin : Form
    {
        private Point lastPos;

        //private int logAttempt = 0;
        private WindowsIdentity currentUser = WindowsIdentity.GetCurrent();

        private dbConnections dbcon = new dbConnections();

        [System.Runtime.InteropServices.DllImport("advapi32.dll")]
        public static extern bool LogonUser(string userName, string domainName, string password, int logonType,
            int LogonProvider, ref IntPtr phToken);

        public frmLogin()
        {
            InitializeComponent();
        }

        private void CancelLabel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void frmLogin_MouseDown(object sender, MouseEventArgs e)
        {
            lastPos = MousePosition;
        }

        private void frmLogin_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int xoffset = MousePosition.X - lastPos.X;
                int yoffset = MousePosition.Y - lastPos.Y;
                this.Left += xoffset;
                this.Top += yoffset;

                lastPos = MousePosition;
            }
        }

        public bool IsValidateCredentials(string userName, string password, string domain)
        {
            IntPtr tokenHandler = IntPtr.Zero;
            bool isValid = LogonUser(userName, domain, password, 2, 0, ref tokenHandler);
            return isValid;
        }

        public static string getDomainName(string usernameDomain)
        {
            if (string.IsNullOrEmpty(usernameDomain))
            {
                MessageBox.Show("Argument cannot be null or empty", "Error-Username Domain", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            if (usernameDomain.Contains("\\"))
            {
                int index = usernameDomain.IndexOf("\\");
                return usernameDomain.Substring(0, index);
            }
            else if (usernameDomain.Contains("@"))
            {
                int index = usernameDomain.IndexOf("@");
                return usernameDomain.Substring(index + 1);
            }
            else
            {
                return "";
            }
        }

        public void CheckUser(string username)
        {
            int state = 1;
            UserInfo.userName = username;
            state = dbcon.getUserInfo(username);

            switch (state)
            {
                case 1: // Success
                    if (UserInfo.IsEnable)
                    {
                        UserInfo.UserLoginTime = DateTime.Now;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                        "System Login Failed. Error Code-DA001\nPlease contact your Administrator",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                        // dbcon.errorLog("Account Disabled", this.UsernameTextBox.Text.ToString().Trim());
                        this.DialogResult = DialogResult.No;
                    }
                    break;

                case 2: //User Does not Exist
                    MessageBox.Show(
                           "System Login Failed. Error Code-DE001\nPlease contact your Administrator",
                           "Login Failed",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Warning);

                    //dbcon.errorLog("No user account", this.UsernameTextBox.Text.ToString().Trim());
                    this.DialogResult = DialogResult.No;
                    break;
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.UsernameTextBox.Text) || string.IsNullOrEmpty(this.PasswordTextBox.Text))
            {
                MessageBox.Show(
                        "User ID or Password cannot be empty. Please try again.\nIf the problem persists please contact your Administrator",
                        "Authentication Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
            }
            else
            {
                bool isValid = IsValidateCredentials(this.UsernameTextBox.Text.ToString(), this.PasswordTextBox.Text.ToString(), getDomainName(currentUser.Name));
                if (isValid)
                {
                    CheckUser(this.UsernameTextBox.Text.ToString());
                }
                else
                {
                    MessageBox.Show(
                            "You have entered an invalid User ID or Password. Please try again.\nIf the problem persists please contact your Administrator",
                            "Authentication Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.UsernameTextBox.Text = string.Empty;
            this.PasswordTextBox.Text = string.Empty;
            this.UsernameTextBox.Focus();
        }
    }
}