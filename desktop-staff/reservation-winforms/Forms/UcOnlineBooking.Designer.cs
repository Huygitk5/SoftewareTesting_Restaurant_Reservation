namespace reservation_winforms.Forms
{
    partial class UcOnlineBooking
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearchHint = new System.Windows.Forms.Label();
            this.flpReservations = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.lblValTimeStatus = new System.Windows.Forms.Label();
            this.lblValTableStatus = new System.Windows.Forms.Label();
            this.lblTitleTableStatus = new System.Windows.Forms.Label();
            this.lblValTable = new System.Windows.Forms.Label();
            this.lblTitleTable = new System.Windows.Forms.Label();
            this.lblValGuests = new System.Windows.Forms.Label();
            this.lblTitleGuests = new System.Windows.Forms.Label();
            this.lblValTime = new System.Windows.Forms.Label();
            this.lblTitleTime = new System.Windows.Forms.Label();
            this.lblValPhone = new System.Windows.Forms.Label();
            this.lblTitlePhone = new System.Windows.Forms.Label();
            this.lblValName = new System.Windows.Forms.Label();
            this.lblTitleName = new System.Windows.Forms.Label();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.lblTitleNote = new System.Windows.Forms.Label();
            this.lblValNote = new System.Windows.Forms.Label();
            this.btnCheckIn = new System.Windows.Forms.Button();
            this.btnChangeTable = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1530, 92);
            this.pnlHeader.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(593, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ONLINE BOOKING CONFIRMATION";
            // 
            // pnlSearch
            // 
            this.pnlSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearch.Controls.Add(this.btnReload);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.lblSearchHint);
            this.pnlSearch.Location = new System.Drawing.Point(39, 110);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(1451, 100);
            this.pnlSearch.TabIndex = 2;
            // 
            // lblSearchHint
            // 
            this.lblSearchHint.AutoSize = true;
            this.lblSearchHint.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            this.lblSearchHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSearchHint.Location = new System.Drawing.Point(30, 30);
            this.lblSearchHint.Name = "lblSearchHint";
            this.lblSearchHint.Size = new System.Drawing.Size(319, 38);
            this.lblSearchHint.TabIndex = 0;
            this.lblSearchHint.Text = "Phone No / Booking ID:";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.txtSearch.Location = new System.Drawing.Point(360, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(400, 50);
            this.txtSearch.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(780, 24);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(180, 52);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "SEARCH";
            this.btnSearch.UseVisualStyleBackColor = false;
            // 
            // btnReload
            // 
            this.btnReload.BackColor = System.Drawing.Color.White;
            this.btnReload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReload.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnReload.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnReload.Location = new System.Drawing.Point(980, 24);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(180, 52);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "RELOAD";
            this.btnReload.UseVisualStyleBackColor = false;
            // 
            // flpReservations
            // 
            this.flpReservations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.flpReservations.AutoScroll = true;
            this.flpReservations.BackColor = System.Drawing.Color.White;
            this.flpReservations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpReservations.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpReservations.Location = new System.Drawing.Point(39, 230);
            this.flpReservations.Name = "flpReservations";
            this.flpReservations.Padding = new System.Windows.Forms.Padding(10);
            this.flpReservations.Size = new System.Drawing.Size(650, 800);
            this.flpReservations.TabIndex = 4;
            this.flpReservations.WrapContents = false;
            // 
            // pnlDetails
            // 
            this.pnlDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDetails.BackColor = System.Drawing.Color.White;
            this.pnlDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDetails.Controls.Add(this.lblValTimeStatus);
            this.pnlDetails.Controls.Add(this.lblValTableStatus);
            this.pnlDetails.Controls.Add(this.lblTitleTableStatus);
            this.pnlDetails.Controls.Add(this.lblValTable);
            this.pnlDetails.Controls.Add(this.lblTitleTable);
            this.pnlDetails.Controls.Add(this.lblValGuests);
            this.pnlDetails.Controls.Add(this.lblTitleGuests);
            this.pnlDetails.Controls.Add(this.lblValTime);
            this.pnlDetails.Controls.Add(this.lblTitleTime);
            this.pnlDetails.Controls.Add(this.lblValPhone);
            this.pnlDetails.Controls.Add(this.lblTitlePhone);
            this.pnlDetails.Controls.Add(this.lblValName);
            this.pnlDetails.Controls.Add(this.lblTitleName);
            this.pnlDetails.Controls.Add(this.lblDetailTitle);
            this.pnlDetails.Controls.Add(this.lblTitleNote);
            this.pnlDetails.Controls.Add(this.lblValNote);
            this.pnlDetails.Location = new System.Drawing.Point(710, 230);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(780, 600);
            this.pnlDetails.TabIndex = 5;
            // 
            // lblDetailTitle
            // 
            this.lblDetailTitle.AutoSize = true;
            this.lblDetailTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblDetailTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblDetailTitle.Location = new System.Drawing.Point(30, 25);
            this.lblDetailTitle.Name = "lblDetailTitle";
            this.lblDetailTitle.Size = new System.Drawing.Size(351, 45);
            this.lblDetailTitle.TabIndex = 0;
            this.lblDetailTitle.Text = "RESERVATION INFO:";
            // 
            // lblTitleName
            // 
            this.lblTitleName.AutoSize = true;
            this.lblTitleName.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleName.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleName.Location = new System.Drawing.Point(40, 100);
            this.lblTitleName.Name = "lblTitleName";
            this.lblTitleName.Size = new System.Drawing.Size(169, 38);
            this.lblTitleName.TabIndex = 1;
            this.lblTitleName.Text = "Customer:";
            // 
            // lblValName
            // 
            this.lblValName.AutoSize = true;
            this.lblValName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblValName.Location = new System.Drawing.Point(230, 95);
            this.lblValName.Name = "lblValName";
            this.lblValName.Size = new System.Drawing.Size(56, 45);
            this.lblValName.TabIndex = 2;
            this.lblValName.Text = "---";
            // 
            // lblTitlePhone
            // 
            this.lblTitlePhone.AutoSize = true;
            this.lblTitlePhone.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitlePhone.ForeColor = System.Drawing.Color.Gray;
            this.lblTitlePhone.Location = new System.Drawing.Point(40, 170);
            this.lblTitlePhone.Name = "lblTitlePhone";
            this.lblTitlePhone.Size = new System.Drawing.Size(151, 38);
            this.lblTitlePhone.TabIndex = 3;
            this.lblTitlePhone.Text = "Phone:";
            // 
            // lblValPhone
            // 
            this.lblValPhone.AutoSize = true;
            this.lblValPhone.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValPhone.Location = new System.Drawing.Point(230, 170);
            this.lblValPhone.Name = "lblValPhone";
            this.lblValPhone.Size = new System.Drawing.Size(49, 38);
            this.lblValPhone.TabIndex = 4;
            this.lblValPhone.Text = "---";
            // 
            // lblTitleTime
            // 
            this.lblTitleTime.AutoSize = true;
            this.lblTitleTime.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleTime.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTime.Location = new System.Drawing.Point(40, 240);
            this.lblTitleTime.Name = "lblTitleTime";
            this.lblTitleTime.Size = new System.Drawing.Size(107, 38);
            this.lblTitleTime.TabIndex = 5;
            this.lblTitleTime.Text = "Arrival:";
            // 
            // lblValTime
            // 
            this.lblValTime.AutoSize = true;
            this.lblValTime.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblValTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblValTime.Location = new System.Drawing.Point(230, 235);
            this.lblValTime.Name = "lblValTime";
            this.lblValTime.Size = new System.Drawing.Size(84, 45);
            this.lblValTime.TabIndex = 6;
            this.lblValTime.Text = "--:--";
            // 
            // lblTitleGuests
            // 
            this.lblTitleGuests.AutoSize = true;
            this.lblTitleGuests.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleGuests.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleGuests.Location = new System.Drawing.Point(40, 310);
            this.lblTitleGuests.Name = "lblTitleGuests";
            this.lblTitleGuests.Size = new System.Drawing.Size(135, 38);
            this.lblTitleGuests.TabIndex = 7;
            this.lblTitleGuests.Text = "Guests:";
            // 
            // lblValGuests
            // 
            this.lblValGuests.AutoSize = true;
            this.lblValGuests.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValGuests.Location = new System.Drawing.Point(230, 310);
            this.lblValGuests.Name = "lblValGuests";
            this.lblValGuests.Size = new System.Drawing.Size(49, 38);
            this.lblValGuests.TabIndex = 8;
            this.lblValGuests.Text = "---";
            // 
            // lblTitleTable
            // 
            this.lblTitleTable.AutoSize = true;
            this.lblTitleTable.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleTable.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTable.Location = new System.Drawing.Point(40, 380);
            this.lblTitleTable.Name = "lblTitleTable";
            this.lblTitleTable.Size = new System.Drawing.Size(162, 38);
            this.lblTitleTable.TabIndex = 9;
            this.lblTitleTable.Text = "Table:";
            // 
            // lblValTable
            // 
            this.lblValTable.AutoSize = true;
            this.lblValTable.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblValTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblValTable.Location = new System.Drawing.Point(230, 375);
            this.lblValTable.Name = "lblValTable";
            this.lblValTable.Size = new System.Drawing.Size(56, 45);
            this.lblValTable.TabIndex = 10;
            this.lblValTable.Text = "---";
            // 
            // lblTitleTableStatus
            // 
            this.lblTitleTableStatus.AutoSize = true;
            this.lblTitleTableStatus.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleTableStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTableStatus.Location = new System.Drawing.Point(40, 450);
            this.lblTitleTableStatus.Name = "lblTitleTableStatus";
            this.lblTitleTableStatus.Size = new System.Drawing.Size(150, 38);
            this.lblTitleTableStatus.TabIndex = 11;
            this.lblTitleTableStatus.Text = "Status:";
            // 
            // lblValTableStatus
            // 
            this.lblValTableStatus.AutoSize = true;
            this.lblValTableStatus.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValTableStatus.Location = new System.Drawing.Point(230, 450);
            this.lblValTableStatus.Name = "lblValTableStatus";
            this.lblValTableStatus.Size = new System.Drawing.Size(49, 38);
            this.lblValTableStatus.TabIndex = 12;
            this.lblValTableStatus.Text = "---";
            // 
            // lblValTimeStatus
            // 
            this.lblValTimeStatus.AutoSize = true;
            this.lblValTimeStatus.Font = new System.Drawing.Font("Segoe UI", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblValTimeStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.lblValTimeStatus.Location = new System.Drawing.Point(340, 242);
            this.lblValTimeStatus.Name = "lblValTimeStatus";
            this.lblValTimeStatus.Size = new System.Drawing.Size(0, 32);
            this.lblValTimeStatus.TabIndex = 13;
            // 
            // lblTitleNote
            // 
            this.lblTitleNote.AutoSize = true;
            this.lblTitleNote.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleNote.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleNote.Location = new System.Drawing.Point(40, 520);
            this.lblTitleNote.Name = "lblTitleNote";
            this.lblTitleNote.Size = new System.Drawing.Size(117, 38);
            this.lblTitleNote.TabIndex = 14;
            this.lblTitleNote.Text = "Note:";
            // 
            // lblValNote
            // 
            this.lblValNote.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic);
            this.lblValNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblValNote.Location = new System.Drawing.Point(230, 525);
            this.lblValNote.Name = "lblValNote";
            this.lblValNote.Size = new System.Drawing.Size(500, 70);
            this.lblValNote.TabIndex = 15;
            this.lblValNote.Text = "---";
            // 
            // btnChangeTable
            // 
            this.btnChangeTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeTable.BackColor = System.Drawing.Color.White;
            this.btnChangeTable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChangeTable.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnChangeTable.FlatAppearance.BorderSize = 2;
            this.btnChangeTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeTable.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnChangeTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnChangeTable.Location = new System.Drawing.Point(710, 850);
            this.btnChangeTable.Name = "btnChangeTable";
            this.btnChangeTable.Size = new System.Drawing.Size(350, 80);
            this.btnChangeTable.TabIndex = 6;
            this.btnChangeTable.Text = "CHANGE TABLE";
            this.btnChangeTable.UseVisualStyleBackColor = false;
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnCheckIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckIn.FlatAppearance.BorderSize = 0;
            this.btnCheckIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckIn.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnCheckIn.ForeColor = System.Drawing.Color.White;
            this.btnCheckIn.Location = new System.Drawing.Point(1080, 850);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(410, 80);
            this.btnCheckIn.TabIndex = 7;
            this.btnCheckIn.Text = "CHECK-IN";
            this.btnCheckIn.UseVisualStyleBackColor = false;
            // 
            // UcOnlineBooking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.btnChangeTable);
            this.Controls.Add(this.btnCheckIn);
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.flpReservations);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UcOnlineBooking";
            this.Size = new System.Drawing.Size(1530, 1122);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearchHint;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.FlowLayoutPanel flpReservations;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Label lblTitleName;
        private System.Windows.Forms.Label lblValName;
        private System.Windows.Forms.Label lblTitlePhone;
        private System.Windows.Forms.Label lblValPhone;
        private System.Windows.Forms.Label lblTitleTime;
        private System.Windows.Forms.Label lblValTime;
        private System.Windows.Forms.Label lblTitleGuests;
        private System.Windows.Forms.Label lblValGuests;
        private System.Windows.Forms.Label lblTitleTable;
        private System.Windows.Forms.Label lblValTable;
        private System.Windows.Forms.Label lblTitleTableStatus;
        private System.Windows.Forms.Label lblValTableStatus;
        private System.Windows.Forms.Label lblValTimeStatus;
        private System.Windows.Forms.Label lblTitleNote;
        private System.Windows.Forms.Label lblValNote;
        private System.Windows.Forms.Button btnCheckIn;
        private System.Windows.Forms.Button btnChangeTable;
    }
}