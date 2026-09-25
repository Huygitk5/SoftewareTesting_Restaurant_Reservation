namespace reservation_winforms.Forms
{
    partial class MainDashboardForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.btnSystemConfig = new System.Windows.Forms.Button();
            this.btnRegisterStaff = new System.Windows.Forms.Button();
            this.btnSystemLogs = new System.Windows.Forms.Button();
            this.btnReports = new System.Windows.Forms.Button();
            this.btnTableSetup = new System.Windows.Forms.Button();
            this.lblAdminSection = new System.Windows.Forms.Label();
            this.btnCheckOut = new System.Windows.Forms.Button();
            this.btnWaitlist = new System.Windows.Forms.Button();
            this.btnOnlineBooking = new System.Windows.Forms.Button();
            this.btnTableMap = new System.Windows.Forms.Button();
            this.lblReceptionSection = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.pnlLogo = new System.Windows.Forms.Panel();
            this.lblLogo = new System.Windows.Forms.Label();
            this.lblRealTime = new System.Windows.Forms.Label();
            this.clockTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlMainContent = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.pnlSidebar.SuspendLayout();
            this.pnlLogo.SuspendLayout();
            this.pnlMainContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.pnlSidebar.Controls.Add(this.btnSystemConfig);
            this.pnlSidebar.Controls.Add(this.btnRegisterStaff);
            this.pnlSidebar.Controls.Add(this.btnSystemLogs);
            this.pnlSidebar.Controls.Add(this.btnReports);
            this.pnlSidebar.Controls.Add(this.btnTableSetup);
            this.pnlSidebar.Controls.Add(this.lblAdminSection);
            this.pnlSidebar.Controls.Add(this.btnCheckOut);
            this.pnlSidebar.Controls.Add(this.btnWaitlist);
            this.pnlSidebar.Controls.Add(this.btnOnlineBooking);
            this.pnlSidebar.Controls.Add(this.btnTableMap);
            this.pnlSidebar.Controls.Add(this.lblReceptionSection);
            this.pnlSidebar.Controls.Add(this.btnLogout);
            this.pnlSidebar.Controls.Add(this.pnlLogo);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(390, 1200);
            this.pnlSidebar.TabIndex = 0;
            // 
            // btnRegisterStaff
            // 
            this.btnRegisterStaff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegisterStaff.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRegisterStaff.FlatAppearance.BorderSize = 0;
            this.btnRegisterStaff.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnRegisterStaff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegisterStaff.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegisterStaff.ForeColor = System.Drawing.Color.White;
            this.btnRegisterStaff.Location = new System.Drawing.Point(0, 817);
            this.btnRegisterStaff.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRegisterStaff.Name = "btnRegisterStaff";
            this.btnRegisterStaff.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnRegisterStaff.Size = new System.Drawing.Size(390, 77);
            this.btnRegisterStaff.TabIndex = 10;
            this.btnRegisterStaff.Text = "👤 Register Reception";
            this.btnRegisterStaff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRegisterStaff.UseVisualStyleBackColor = true;
            // 
            // btnSystemConfig
            // 
            this.btnSystemConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSystemConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSystemConfig.FlatAppearance.BorderSize = 0;
            this.btnSystemConfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnSystemConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSystemConfig.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSystemConfig.ForeColor = System.Drawing.Color.White;
            this.btnSystemConfig.Location = new System.Drawing.Point(0, 894);
            this.btnSystemConfig.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSystemConfig.Name = "btnSystemConfig";
            this.btnSystemConfig.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnSystemConfig.Size = new System.Drawing.Size(390, 77);
            this.btnSystemConfig.TabIndex = 12;
            this.btnSystemConfig.Text = "🔧 System Config";
            this.btnSystemConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSystemConfig.UseVisualStyleBackColor = true;
            // 
            // btnSystemLogs
            // 
            this.btnSystemLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSystemLogs.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSystemLogs.FlatAppearance.BorderSize = 0;
            this.btnSystemLogs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnSystemLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSystemLogs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSystemLogs.ForeColor = System.Drawing.Color.White;
            this.btnSystemLogs.Location = new System.Drawing.Point(0, 740);
            this.btnSystemLogs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSystemLogs.Name = "btnSystemLogs";
            this.btnSystemLogs.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnSystemLogs.Size = new System.Drawing.Size(390, 77);
            this.btnSystemLogs.TabIndex = 8;
            this.btnSystemLogs.Text = "📝 System Log";
            this.btnSystemLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSystemLogs.UseVisualStyleBackColor = true;
            // 
            // btnReports
            // 
            this.btnReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReports.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnReports.FlatAppearance.BorderSize = 0;
            this.btnReports.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReports.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReports.ForeColor = System.Drawing.Color.White;
            this.btnReports.Location = new System.Drawing.Point(0, 663);
            this.btnReports.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReports.Name = "btnReports";
            this.btnReports.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnReports.Size = new System.Drawing.Size(390, 77);
            this.btnReports.TabIndex = 7;
            this.btnReports.Text = "📈 Report";
            this.btnReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReports.UseVisualStyleBackColor = true;
            // 
            // btnTableSetup
            // 
            this.btnTableSetup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTableSetup.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTableSetup.FlatAppearance.BorderSize = 0;
            this.btnTableSetup.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnTableSetup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTableSetup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTableSetup.ForeColor = System.Drawing.Color.White;
            this.btnTableSetup.Location = new System.Drawing.Point(0, 586);
            this.btnTableSetup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnTableSetup.Name = "btnTableSetup";
            this.btnTableSetup.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnTableSetup.Size = new System.Drawing.Size(390, 77);
            this.btnTableSetup.TabIndex = 6;
            this.btnTableSetup.Text = "⚙️ Manage Table Map";
            this.btnTableSetup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTableSetup.UseVisualStyleBackColor = true;
            // 
            // lblAdminSection
            // 
            this.lblAdminSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAdminSection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAdminSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.lblAdminSection.Location = new System.Drawing.Point(0, 524);
            this.lblAdminSection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAdminSection.Name = "lblAdminSection";
            this.lblAdminSection.Padding = new System.Windows.Forms.Padding(22, 0, 0, 0);
            this.lblAdminSection.Size = new System.Drawing.Size(390, 62);
            this.lblAdminSection.TabIndex = 5;
            this.lblAdminSection.Text = "MANAGER";
            this.lblAdminSection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCheckOut
            // 
            this.btnCheckOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckOut.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCheckOut.FlatAppearance.BorderSize = 0;
            this.btnCheckOut.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnCheckOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckOut.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckOut.ForeColor = System.Drawing.Color.White;
            this.btnCheckOut.Location = new System.Drawing.Point(0, 447);
            this.btnCheckOut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCheckOut.Name = "btnCheckOut";
            this.btnCheckOut.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnCheckOut.Size = new System.Drawing.Size(390, 77);
            this.btnCheckOut.TabIndex = 11;
            this.btnCheckOut.Text = "💳 Check-out && Change Table";
            this.btnCheckOut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCheckOut.UseVisualStyleBackColor = true;
            // 
            // btnWaitlist
            // 
            this.btnWaitlist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWaitlist.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnWaitlist.FlatAppearance.BorderSize = 0;
            this.btnWaitlist.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnWaitlist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWaitlist.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWaitlist.ForeColor = System.Drawing.Color.White;
            this.btnWaitlist.Location = new System.Drawing.Point(0, 370);
            this.btnWaitlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnWaitlist.Name = "btnWaitlist";
            this.btnWaitlist.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnWaitlist.Size = new System.Drawing.Size(390, 77);
            this.btnWaitlist.TabIndex = 4;
            this.btnWaitlist.Text = "⏳ Waitlist";
            this.btnWaitlist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWaitlist.UseVisualStyleBackColor = true;
            // 
            // btnOnlineBooking
            // 
            this.btnOnlineBooking.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOnlineBooking.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOnlineBooking.FlatAppearance.BorderSize = 0;
            this.btnOnlineBooking.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnOnlineBooking.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOnlineBooking.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOnlineBooking.ForeColor = System.Drawing.Color.White;
            this.btnOnlineBooking.Location = new System.Drawing.Point(0, 293);
            this.btnOnlineBooking.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOnlineBooking.Name = "btnOnlineBooking";
            this.btnOnlineBooking.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnOnlineBooking.Size = new System.Drawing.Size(390, 77);
            this.btnOnlineBooking.TabIndex = 3;
            this.btnOnlineBooking.Text = "🌐 Check-in Online";
            this.btnOnlineBooking.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOnlineBooking.UseVisualStyleBackColor = true;
            // 
            // btnTableMap
            // 
            this.btnTableMap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTableMap.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTableMap.FlatAppearance.BorderSize = 0;
            this.btnTableMap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnTableMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTableMap.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTableMap.ForeColor = System.Drawing.Color.White;
            this.btnTableMap.Location = new System.Drawing.Point(0, 216);
            this.btnTableMap.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnTableMap.Name = "btnTableMap";
            this.btnTableMap.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnTableMap.Size = new System.Drawing.Size(390, 77);
            this.btnTableMap.TabIndex = 2;
            this.btnTableMap.Text = "📊 Walk-in";
            this.btnTableMap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTableMap.UseVisualStyleBackColor = true;
            // 
            // lblReceptionSection
            // 
            this.lblReceptionSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblReceptionSection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReceptionSection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.lblReceptionSection.Location = new System.Drawing.Point(0, 154);
            this.lblReceptionSection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReceptionSection.Name = "lblReceptionSection";
            this.lblReceptionSection.Padding = new System.Windows.Forms.Padding(22, 0, 0, 0);
            this.lblReceptionSection.Size = new System.Drawing.Size(390, 62);
            this.lblReceptionSection.TabIndex = 1;
            this.lblReceptionSection.Text = "RECEPTION";
            this.lblReceptionSection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLogout
            // 
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(0, 1045);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnLogout.Size = new System.Drawing.Size(390, 77);
            this.btnLogout.TabIndex = 9;
            this.btnLogout.Text = "🚪 Đăng xuất";
            this.btnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.UseVisualStyleBackColor = true;
            // 
            // pnlLogo
            // 
            this.pnlLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(43)))), ((int)(((byte)(56)))));
            this.pnlLogo.Controls.Add(this.lblRealTime);
            this.pnlLogo.Controls.Add(this.lblLogo);
            this.pnlLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogo.Location = new System.Drawing.Point(0, 0);
            this.pnlLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(390, 154);
            this.pnlLogo.TabIndex = 0;
            // 
            // lblLogo
            // 
            this.lblLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogo.ForeColor = System.Drawing.Color.White;
            this.lblLogo.Location = new System.Drawing.Point(0, 0);
            this.lblLogo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(390, 154);
            this.lblLogo.TabIndex = 0;
            this.lblLogo.Text = "POS SYSTEM";
            this.lblLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRealTime
            // 
            this.lblRealTime.BackColor = System.Drawing.Color.Transparent;
            this.lblRealTime.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblRealTime.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            this.lblRealTime.ForeColor = System.Drawing.Color.White;
            this.lblRealTime.Location = new System.Drawing.Point(0, 100);
            this.lblRealTime.Name = "lblRealTime";
            this.lblRealTime.Size = new System.Drawing.Size(390, 54);
            this.lblRealTime.TabIndex = 1;
            this.lblRealTime.Text = "00/00/0000 00:00:00";
            this.lblRealTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // clockTimer
            // 
            this.clockTimer.Enabled = true;
            this.clockTimer.Interval = 1000;
            // 
            // pnlMainContent
            // 
            this.pnlMainContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.pnlMainContent.Controls.Add(this.lblWelcome);
            this.pnlMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainContent.Location = new System.Drawing.Point(390, 0);
            this.pnlMainContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlMainContent.Name = "pnlMainContent";
            this.pnlMainContent.Size = new System.Drawing.Size(1530, 1122);
            this.pnlMainContent.TabIndex = 1;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.Silver;
            this.lblWelcome.Location = new System.Drawing.Point(0, 0);
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(1530, 1122);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Please select a tab from the menu on the left.";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainDashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1122);
            this.Controls.Add(this.pnlMainContent);
            this.Controls.Add(this.pnlSidebar);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(1525, 1151);
            this.Name = "MainDashboardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "POS System - Reservation Management";
            this.pnlSidebar.ResumeLayout(false);
            this.pnlLogo.ResumeLayout(false);
            this.pnlMainContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlLogo;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Label lblRealTime;
        private System.Windows.Forms.Timer clockTimer;
        private System.Windows.Forms.Label lblReceptionSection;
        private System.Windows.Forms.Button btnTableMap;
        private System.Windows.Forms.Button btnWaitlist;
        private System.Windows.Forms.Button btnOnlineBooking;
        private System.Windows.Forms.Button btnCheckOut;
        private System.Windows.Forms.Button btnSystemConfig;
        private System.Windows.Forms.Label lblAdminSection;
        private System.Windows.Forms.Button btnSystemLogs;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.Button btnTableSetup;
        private System.Windows.Forms.Button btnRegisterStaff;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel pnlMainContent;
        private System.Windows.Forms.Label lblWelcome;
    }
}