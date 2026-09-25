namespace reservation_winforms.Forms
{
    partial class UcWaitlist
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnReload = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlAddWaitlist = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.chkAllowShortSeating = new System.Windows.Forms.CheckBox();
            this.nudGuests = new System.Windows.Forms.NumericUpDown();
            this.lblGuests = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAddTitle = new System.Windows.Forms.Label();
            this.pnlList = new System.Windows.Forms.Panel();
            this.tabControlWaitlist = new System.Windows.Forms.TabControl();
            this.tabWaiting = new System.Windows.Forms.TabPage();
            this.dgvWaiting = new System.Windows.Forms.DataGridView();
            this.colWaitStt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWaitName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWaitPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWaitGuests = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWaitTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWaitActionSeat = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colWaitActionMiss = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colWaitActionSkip = new System.Windows.Forms.DataGridViewButtonColumn();
            this.tabMissing = new System.Windows.Forms.TabPage();
            this.dgvMissing = new System.Windows.Forms.DataGridView();
            this.colMissStt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMissName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMissPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMissGuests = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMissTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMissActionSeat = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colMissActionReWait = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colMissActionSkip = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.btnSearchMissing = new System.Windows.Forms.Button();
            this.txtSearchMissing = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlAddWaitlist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGuests)).BeginInit();
            this.pnlList.SuspendLayout();
            this.tabControlWaitlist.SuspendLayout();
            this.tabWaiting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWaiting)).BeginInit();
            this.tabMissing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMissing)).BeginInit();
            this.pnlSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.btnReload);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1530, 92);
            this.pnlHeader.TabIndex = 2;
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnReload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReload.FlatAppearance.BorderSize = 0;
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnReload.ForeColor = System.Drawing.Color.White;
            this.btnReload.Location = new System.Drawing.Point(1280, 22);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(220, 50);
            this.btnReload.TabIndex = 1;
            this.btnReload.Text = "RELOAD";
            this.btnReload.UseVisualStyleBackColor = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(434, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "WAITLIST MANAGEMENT";
            // 
            // pnlAddWaitlist
            // 
            this.pnlAddWaitlist.BackColor = System.Drawing.Color.White;
            this.pnlAddWaitlist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAddWaitlist.Controls.Add(this.btnAdd);
            this.pnlAddWaitlist.Controls.Add(this.chkAllowShortSeating);
            this.pnlAddWaitlist.Controls.Add(this.nudGuests);
            this.pnlAddWaitlist.Controls.Add(this.lblGuests);
            this.pnlAddWaitlist.Controls.Add(this.txtPhone);
            this.pnlAddWaitlist.Controls.Add(this.lblPhone);
            this.pnlAddWaitlist.Controls.Add(this.txtName);
            this.pnlAddWaitlist.Controls.Add(this.lblName);
            this.pnlAddWaitlist.Controls.Add(this.lblAddTitle);
            this.pnlAddWaitlist.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlAddWaitlist.Location = new System.Drawing.Point(0, 92);
            this.pnlAddWaitlist.Name = "pnlAddWaitlist";
            this.pnlAddWaitlist.Padding = new System.Windows.Forms.Padding(30);
            this.pnlAddWaitlist.Size = new System.Drawing.Size(347, 1030);
            this.pnlAddWaitlist.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(30, 462);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(286, 60);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "ADD TO WAITLIST";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // chkAllowShortSeating
            // 
            this.chkAllowShortSeating.AutoSize = true;
            this.chkAllowShortSeating.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.chkAllowShortSeating.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.chkAllowShortSeating.Location = new System.Drawing.Point(30, 420);
            this.chkAllowShortSeating.Name = "chkAllowShortSeating";
            this.chkAllowShortSeating.Size = new System.Drawing.Size(243, 36);
            this.chkAllowShortSeating.TabIndex = 8;
            this.chkAllowShortSeating.Text = "Allow short seating";
            this.chkAllowShortSeating.UseVisualStyleBackColor = true;
            // 
            // nudGuests
            // 
            this.nudGuests.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudGuests.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.nudGuests.Location = new System.Drawing.Point(30, 360);
            this.nudGuests.Name = "nudGuests";
            this.nudGuests.Size = new System.Drawing.Size(286, 45);
            this.nudGuests.TabIndex = 1;
            this.nudGuests.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lblGuests
            // 
            this.lblGuests.AutoSize = true;
            this.lblGuests.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblGuests.Location = new System.Drawing.Point(30, 320);
            this.lblGuests.Name = "lblGuests";
            this.lblGuests.Size = new System.Drawing.Size(193, 32);
            this.lblGuests.TabIndex = 2;
            this.lblGuests.Text = "No. of Guests:";
            // 
            // txtPhone
            // 
            this.txtPhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtPhone.Location = new System.Drawing.Point(30, 250);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(284, 45);
            this.txtPhone.TabIndex = 3;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblPhone.Location = new System.Drawing.Point(30, 210);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(126, 32);
            this.lblPhone.TabIndex = 4;
            this.lblPhone.Text = "Phone (*):";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtName.Location = new System.Drawing.Point(30, 140);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(284, 45);
            this.txtName.TabIndex = 5;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblName.Location = new System.Drawing.Point(30, 100);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(123, 32);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "Name (*):";
            // 
            // lblAddTitle
            // 
            this.lblAddTitle.AutoSize = true;
            this.lblAddTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblAddTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblAddTitle.Location = new System.Drawing.Point(30, 30);
            this.lblAddTitle.Name = "lblAddTitle";
            this.lblAddTitle.Size = new System.Drawing.Size(234, 38);
            this.lblAddTitle.TabIndex = 7;
            this.lblAddTitle.Text = "NEW CUSTOMER";
            // 
            // pnlList
            // 
            this.pnlList.BackColor = System.Drawing.Color.White;
            this.pnlList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlList.Controls.Add(this.tabControlWaitlist);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlList.Location = new System.Drawing.Point(347, 92);
            this.pnlList.Name = "pnlList";
            this.pnlList.Padding = new System.Windows.Forms.Padding(10);
            this.pnlList.Size = new System.Drawing.Size(1183, 1030);
            this.pnlList.TabIndex = 4;
            // 
            // tabControlWaitlist
            // 
            this.tabControlWaitlist.Controls.Add(this.tabWaiting);
            this.tabControlWaitlist.Controls.Add(this.tabMissing);
            this.tabControlWaitlist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlWaitlist.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.tabControlWaitlist.Location = new System.Drawing.Point(10, 10);
            this.tabControlWaitlist.Name = "tabControlWaitlist";
            this.tabControlWaitlist.SelectedIndex = 0;
            this.tabControlWaitlist.Size = new System.Drawing.Size(1161, 1008);
            this.tabControlWaitlist.TabIndex = 0;
            // 
            // tabWaiting
            // 
            this.tabWaiting.Controls.Add(this.dgvWaiting);
            this.tabWaiting.Location = new System.Drawing.Point(4, 47);
            this.tabWaiting.Name = "tabWaiting";
            this.tabWaiting.Padding = new System.Windows.Forms.Padding(3);
            this.tabWaiting.Size = new System.Drawing.Size(1153, 957);
            this.tabWaiting.TabIndex = 0;
            this.tabWaiting.Text = "WAITING";
            this.tabWaiting.UseVisualStyleBackColor = true;
            // 
            // dgvWaiting
            // 
            this.dgvWaiting.AllowUserToAddRows = false;
            this.dgvWaiting.AllowUserToDeleteRows = false;
            this.dgvWaiting.AllowUserToResizeRows = false;
            this.dgvWaiting.BackgroundColor = System.Drawing.Color.White;
            this.dgvWaiting.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvWaiting.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvWaiting.ColumnHeadersHeight = 40;
            this.dgvWaiting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colWaitStt,
            this.colWaitName,
            this.colWaitPhone,
            this.colWaitGuests,
            this.colWaitTime,
            this.colWaitActionSeat,
            this.colWaitActionMiss,
            this.colWaitActionSkip});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(245)))), ((int)(((byte)(251)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvWaiting.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvWaiting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvWaiting.EnableHeadersVisualStyles = false;
            this.dgvWaiting.Location = new System.Drawing.Point(3, 3);
            this.dgvWaiting.Name = "dgvWaiting";
            this.dgvWaiting.ReadOnly = true;
            this.dgvWaiting.RowHeadersVisible = false;
            this.dgvWaiting.RowHeadersWidth = 62;
            this.dgvWaiting.RowTemplate.Height = 50;
            this.dgvWaiting.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWaiting.Size = new System.Drawing.Size(1147, 951);
            this.dgvWaiting.TabIndex = 0;
            // 
            // colWaitStt
            // 
            this.colWaitStt.HeaderText = "No.";
            this.colWaitStt.MinimumWidth = 8;
            this.colWaitStt.Name = "colWaitStt";
            this.colWaitStt.ReadOnly = true;
            this.colWaitStt.Width = 60;
            // 
            // colWaitName
            // 
            this.colWaitName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colWaitName.HeaderText = "Customer Name";
            this.colWaitName.MinimumWidth = 8;
            this.colWaitName.Name = "colWaitName";
            this.colWaitName.ReadOnly = true;
            // 
            // colWaitPhone
            // 
            this.colWaitPhone.HeaderText = "Phone";
            this.colWaitPhone.MinimumWidth = 8;
            this.colWaitPhone.Name = "colWaitPhone";
            this.colWaitPhone.ReadOnly = true;
            this.colWaitPhone.Width = 150;
            // 
            // colWaitGuests
            // 
            this.colWaitGuests.HeaderText = "Pax";
            this.colWaitGuests.MinimumWidth = 8;
            this.colWaitGuests.Name = "colWaitGuests";
            this.colWaitGuests.ReadOnly = true;
            this.colWaitGuests.Width = 120;
            // 
            // colWaitTime
            // 
            this.colWaitTime.HeaderText = "Arrival";
            this.colWaitTime.MinimumWidth = 8;
            this.colWaitTime.Name = "colWaitTime";
            this.colWaitTime.ReadOnly = true;
            this.colWaitTime.Width = 120;
            // 
            // colWaitActionSeat
            // 
            this.colWaitActionSeat.HeaderText = "Seat Table";
            this.colWaitActionSeat.MinimumWidth = 8;
            this.colWaitActionSeat.Name = "colWaitActionSeat";
            this.colWaitActionSeat.ReadOnly = true;
            this.colWaitActionSeat.Text = "Seat Table";
            this.colWaitActionSeat.UseColumnTextForButtonValue = true;
            this.colWaitActionSeat.Width = 110;
            // 
            // colWaitActionMiss
            // 
            this.colWaitActionMiss.HeaderText = "Missed";
            this.colWaitActionMiss.MinimumWidth = 8;
            this.colWaitActionMiss.Name = "colWaitActionMiss";
            this.colWaitActionMiss.ReadOnly = true;
            this.colWaitActionMiss.Text = "Missed";
            this.colWaitActionMiss.UseColumnTextForButtonValue = true;
            this.colWaitActionMiss.Width = 110;
            // 
            // colWaitActionSkip
            // 
            this.colWaitActionSkip.HeaderText = "Skip";
            this.colWaitActionSkip.MinimumWidth = 8;
            this.colWaitActionSkip.Name = "colWaitActionSkip";
            this.colWaitActionSkip.ReadOnly = true;
            this.colWaitActionSkip.Text = "Cancel";
            this.colWaitActionSkip.UseColumnTextForButtonValue = true;
            this.colWaitActionSkip.Width = 110;
            // 
            // tabMissing
            // 
            this.tabMissing.Controls.Add(this.dgvMissing);
            this.tabMissing.Controls.Add(this.pnlSearch);
            this.tabMissing.Location = new System.Drawing.Point(4, 47);
            this.tabMissing.Name = "tabMissing";
            this.tabMissing.Padding = new System.Windows.Forms.Padding(3);
            this.tabMissing.Size = new System.Drawing.Size(1153, 957);
            this.tabMissing.TabIndex = 1;
            this.tabMissing.Text = "MISSED";
            this.tabMissing.UseVisualStyleBackColor = true;
            // 
            // dgvMissing
            // 
            this.dgvMissing.AllowUserToAddRows = false;
            this.dgvMissing.AllowUserToDeleteRows = false;
            this.dgvMissing.AllowUserToResizeRows = false;
            this.dgvMissing.BackgroundColor = System.Drawing.Color.White;
            this.dgvMissing.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvMissing.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvMissing.ColumnHeadersHeight = 40;
            this.dgvMissing.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMissStt,
            this.colMissName,
            this.colMissPhone,
            this.colMissGuests,
            this.colMissTime,
            this.colMissActionSeat,
            this.colMissActionReWait,
            this.colMissActionSkip});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(245)))), ((int)(((byte)(251)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMissing.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvMissing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMissing.EnableHeadersVisualStyles = false;
            this.dgvMissing.Location = new System.Drawing.Point(3, 73);
            this.dgvMissing.Name = "dgvMissing";
            this.dgvMissing.ReadOnly = true;
            this.dgvMissing.RowHeadersVisible = false;
            this.dgvMissing.RowHeadersWidth = 62;
            this.dgvMissing.RowTemplate.Height = 50;
            this.dgvMissing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMissing.Size = new System.Drawing.Size(1147, 881);
            this.dgvMissing.TabIndex = 0;
            // 
            // colMissStt
            // 
            this.colMissStt.HeaderText = "No.";
            this.colMissStt.MinimumWidth = 8;
            this.colMissStt.Name = "colMissStt";
            this.colMissStt.ReadOnly = true;
            this.colMissStt.Width = 60;
            // 
            // colMissName
            // 
            this.colMissName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMissName.HeaderText = "Customer Name";
            this.colMissName.MinimumWidth = 8;
            this.colMissName.Name = "colMissName";
            this.colMissName.ReadOnly = true;
            // 
            // colMissPhone
            // 
            this.colMissPhone.HeaderText = "Phone";
            this.colMissPhone.MinimumWidth = 8;
            this.colMissPhone.Name = "colMissPhone";
            this.colMissPhone.ReadOnly = true;
            this.colMissPhone.Width = 150;
            // 
            // colMissGuests
            // 
            this.colMissGuests.HeaderText = "Pax";
            this.colMissGuests.MinimumWidth = 8;
            this.colMissGuests.Name = "colMissGuests";
            this.colMissGuests.ReadOnly = true;
            this.colMissGuests.Width = 120;
            // 
            // colMissTime
            // 
            this.colMissTime.HeaderText = "Arrival";
            this.colMissTime.MinimumWidth = 8;
            this.colMissTime.Name = "colMissTime";
            this.colMissTime.ReadOnly = true;
            this.colMissTime.Width = 120;
            // 
            // colMissActionSeat
            // 
            this.colMissActionSeat.HeaderText = "Seat Table";
            this.colMissActionSeat.MinimumWidth = 8;
            this.colMissActionSeat.Name = "colMissActionSeat";
            this.colMissActionSeat.ReadOnly = true;
            this.colMissActionSeat.Text = "Seat Table";
            this.colMissActionSeat.UseColumnTextForButtonValue = true;
            this.colMissActionSeat.Width = 110;
            // 
            // colMissActionReWait
            // 
            this.colMissActionReWait.HeaderText = "Return";
            this.colMissActionReWait.MinimumWidth = 8;
            this.colMissActionReWait.Name = "colMissActionReWait";
            this.colMissActionReWait.ReadOnly = true;
            this.colMissActionReWait.Text = "Re-wait";
            this.colMissActionReWait.UseColumnTextForButtonValue = true;
            this.colMissActionReWait.Width = 110;
            // 
            // colMissActionSkip
            // 
            this.colMissActionSkip.HeaderText = "Skip/Cancel";
            this.colMissActionSkip.MinimumWidth = 8;
            this.colMissActionSkip.Name = "colMissActionSkip";
            this.colMissActionSkip.ReadOnly = true;
            this.colMissActionSkip.Text = "Cancel";
            this.colMissActionSkip.UseColumnTextForButtonValue = true;
            this.colMissActionSkip.Width = 110;
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlSearch.Controls.Add(this.btnSearchMissing);
            this.pnlSearch.Controls.Add(this.txtSearchMissing);
            this.pnlSearch.Controls.Add(this.lblSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(3, 3);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(1147, 70);
            this.pnlSearch.TabIndex = 1;
            // 
            // btnSearchMissing
            // 
            this.btnSearchMissing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearchMissing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnSearchMissing.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchMissing.FlatAppearance.BorderSize = 0;
            this.btnSearchMissing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchMissing.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSearchMissing.ForeColor = System.Drawing.Color.White;
            this.btnSearchMissing.Location = new System.Drawing.Point(985, 12);
            this.btnSearchMissing.Name = "btnSearchMissing";
            this.btnSearchMissing.Size = new System.Drawing.Size(150, 47);
            this.btnSearchMissing.TabIndex = 2;
            this.btnSearchMissing.Text = "SEARCH";
            this.btnSearchMissing.UseVisualStyleBackColor = false;
            // 
            // txtSearchMissing
            // 
            this.txtSearchMissing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchMissing.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtSearchMissing.Location = new System.Drawing.Point(245, 14);
            this.txtSearchMissing.Name = "txtSearchMissing";
            this.txtSearchMissing.Size = new System.Drawing.Size(712, 45);
            this.txtSearchMissing.TabIndex = 0;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSearch.Location = new System.Drawing.Point(20, 20);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(176, 32);
            this.lblSearch.TabIndex = 1;
            this.lblSearch.Text = "Search Phone:";
            // 
            // UcWaitlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.pnlAddWaitlist);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UcWaitlist";
            this.Size = new System.Drawing.Size(1530, 1122);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlAddWaitlist.ResumeLayout(false);
            this.pnlAddWaitlist.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGuests)).EndInit();
            this.pnlList.ResumeLayout(false);
            this.tabControlWaitlist.ResumeLayout(false);
            this.tabWaiting.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWaiting)).EndInit();
            this.tabMissing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMissing)).EndInit();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnReload;

        private System.Windows.Forms.Panel pnlAddWaitlist;
        private System.Windows.Forms.Label lblAddTitle;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.NumericUpDown nudGuests;
        private System.Windows.Forms.Label lblGuests;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.CheckBox chkAllowShortSeating;

        private System.Windows.Forms.Panel pnlList;
        private System.Windows.Forms.TabControl tabControlWaitlist;

        // Waiting Tab
        private System.Windows.Forms.TabPage tabWaiting;
        private System.Windows.Forms.DataGridView dgvWaiting;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWaitStt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWaitName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWaitPhone;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWaitGuests;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWaitTime;
        private System.Windows.Forms.DataGridViewButtonColumn colWaitActionSeat;
        private System.Windows.Forms.DataGridViewButtonColumn colWaitActionMiss;
        private System.Windows.Forms.DataGridViewButtonColumn colWaitActionSkip;

        // Missing Tab
        private System.Windows.Forms.TabPage tabMissing;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.TextBox txtSearchMissing;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.DataGridView dgvMissing;
        private System.Windows.Forms.Button btnSearchMissing;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMissStt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMissName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMissPhone;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMissGuests;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMissTime;
        private System.Windows.Forms.DataGridViewButtonColumn colMissActionSeat;
        private System.Windows.Forms.DataGridViewButtonColumn colMissActionReWait;
        private System.Windows.Forms.DataGridViewButtonColumn colMissActionSkip;
    }
}