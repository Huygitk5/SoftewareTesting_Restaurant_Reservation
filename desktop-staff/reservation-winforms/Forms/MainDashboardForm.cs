using System;
using System.Windows.Forms;
using reservation_winforms.Services;

namespace reservation_winforms.Forms
{
    public partial class MainDashboardForm : Form
    {
        public MainDashboardForm()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            clockTimer.Tick += (s, e) => {
                lblRealTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            };

            ApplyRolePermissions();

            this.Load += MainDashboardForm_Load;
            this.FormClosing += MainDashboardForm_FormClosing;

            LoadUserControl(new UcTableMap());

            btnTableMap.Click += BtnTableMap_Click;
            btnOnlineBooking.Click += BtnOnlineBooking_Click;
            btnWaitlist.Click += BtnWaitlist_Click;
            btnCheckOut.Click += BtnCheckOut_Click; 

            btnTableSetup.Click += BtnTableSetup_Click;
            btnReports.Click += BtnReports_Click;
            btnSystemLogs.Click += BtnSystemLogs_Click;
            btnRegisterStaff.Click += btnRegisterStaff_Click;
            btnSystemConfig.Click += BtnSystemConfig_Click;

            btnLogout.Click += BtnLogout_Click;
        }

        private async void MainDashboardForm_Load(object sender, EventArgs e)
        {
            LoadUserControl(new UcTableMap());
            await WebSocketService.Instance.ConnectAsync();
        }

        private async void MainDashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            await WebSocketService.Instance.DisconnectAsync();
        }

        private void ApplyRolePermissions()
        {
            if (GlobalState.Role != "MANAGER")
            {
                lblAdminSection.Visible = false;
                btnTableSetup.Visible = false;
                btnReports.Visible = false;
                btnSystemLogs.Visible = false;
                btnRegisterStaff.Visible = false;
                btnSystemConfig.Visible = false;
            }
        }

        private void LoadUserControl(UserControl uc)
        {
            pnlMainContent.Controls.Clear();

            uc.Dock = DockStyle.Fill;

            pnlMainContent.Controls.Add(uc);
            uc.BringToFront();
        }


        private void BtnTableMap_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcTableMap());
        }

        private void BtnOnlineBooking_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcOnlineBooking());
        }

        private void BtnWaitlist_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcWaitlist());
        }

        private void BtnCheckOut_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcCheckOut());
        }

        private void BtnTableSetup_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcTableSetup());
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcReports());
        }

        private void BtnSystemLogs_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcSystemLogs());
        }

        private void btnRegisterStaff_Click(object sender, EventArgs e)
        {
            pnlMainContent.Controls.Clear();

            UcRegister registerCtrl = new UcRegister();

            registerCtrl.Location = new System.Drawing.Point(
                (pnlMainContent.Width - registerCtrl.Width) / 2,
                (pnlMainContent.Height - registerCtrl.Height) / 2
            );

            pnlMainContent.Controls.Add(registerCtrl);
        }
        private void BtnSystemConfig_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcSystemConfig());
        }
        private async void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out of the system?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                await WebSocketService.Instance.DisconnectAsync();

                GlobalState.StaffToken = "";
                GlobalState.CurrentUsername = "";
                GlobalState.Role = "";

                this.Close();
            }
        }
    }
}