using System;
using System.Windows.Forms;
using reservation_winforms.Services;

namespace reservation_winforms.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;

        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthService();

            btnLogin.Click += BtnLogin_Click;
            btnExit.Click += btnExit_Click;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblMessage.Text = "Please enter username and password!";
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Checking...";
            lblMessage.Text = "";

            var response = await _authService.LoginAsync(user, pass);

            btnLogin.Enabled = true;
            btnLogin.Text = "SIGN IN";

            if (response.IsSuccess)
            {
                txtPassword.Clear();
                lblMessage.Text = "";

                this.Hide();
                MainDashboardForm dashboard = new MainDashboardForm();
                dashboard.ShowDialog();

                this.Show();
            }
            else
            {
                lblMessage.Text = response.Message;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}