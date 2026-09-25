namespace reservation_winforms.Forms
{
    partial class UcCheckOut
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

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.flpActiveTables = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.lblTitleTable = new System.Windows.Forms.Label();
            this.lblValTable = new System.Windows.Forms.Label();
            this.lblTitleName = new System.Windows.Forms.Label();
            this.lblValName = new System.Windows.Forms.Label();
            this.lblTitleTime = new System.Windows.Forms.Label();
            this.lblValTime = new System.Windows.Forms.Label();
            this.lblTitleDuration = new System.Windows.Forms.Label();
            this.lblValDuration = new System.Windows.Forms.Label();
            this.lblTitleDeposit = new System.Windows.Forms.Label();
            this.lblValDeposit = new System.Windows.Forms.Label();
            this.btnCheckOut = new System.Windows.Forms.Button();
            this.btnChangeTable = new System.Windows.Forms.Button();
            this.chkOverride = new System.Windows.Forms.CheckBox();
            this.pnlHeader.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.btnRefresh);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1530, 92);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(1296, 20);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(204, 50);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "RELOAD";
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(466, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "CHECK OUT & CHANGE TABLE";
            // 
            // flpActiveTables
            // 
            this.flpActiveTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpActiveTables.AutoScroll = true;
            this.flpActiveTables.Location = new System.Drawing.Point(30, 120);
            this.flpActiveTables.Name = "flpActiveTables";
            this.flpActiveTables.Size = new System.Drawing.Size(850, 950);
            this.flpActiveTables.TabIndex = 1;
            // 
            // pnlDetails
            // 
            this.pnlDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDetails.BackColor = System.Drawing.Color.White;
            this.pnlDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDetails.Controls.Add(this.lblDetailTitle);
            this.pnlDetails.Controls.Add(this.lblTitleTable);
            this.pnlDetails.Controls.Add(this.lblValTable);
            this.pnlDetails.Controls.Add(this.lblTitleName);
            this.pnlDetails.Controls.Add(this.lblValName);
            this.pnlDetails.Controls.Add(this.lblTitleTime);
            this.pnlDetails.Controls.Add(this.lblValTime);
            this.pnlDetails.Controls.Add(this.lblTitleDuration);
            this.pnlDetails.Controls.Add(this.lblValDuration);
            this.pnlDetails.Controls.Add(this.lblTitleDeposit);
            this.pnlDetails.Controls.Add(this.lblValDeposit);
            this.pnlDetails.Location = new System.Drawing.Point(920, 120);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(580, 500);
            this.pnlDetails.TabIndex = 2;
            // 
            // lblDetailTitle
            // 
            this.lblDetailTitle.AutoSize = true;
            this.lblDetailTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblDetailTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblDetailTitle.Location = new System.Drawing.Point(30, 30);
            this.lblDetailTitle.Name = "lblDetailTitle";
            this.lblDetailTitle.Size = new System.Drawing.Size(286, 45);
            this.lblDetailTitle.TabIndex = 0;
            this.lblDetailTitle.Text = "TABLE INFO:";
            // 
            // lblTitleTable
            // 
            this.lblTitleTable.AutoSize = true;
            this.lblTitleTable.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleTable.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTable.Location = new System.Drawing.Point(30, 110);
            this.lblTitleTable.Name = "lblTitleTable";
            this.lblTitleTable.Size = new System.Drawing.Size(105, 38);
            this.lblTitleTable.TabIndex = 1;
            this.lblTitleTable.Text = "Table:";
            // 
            // lblValTable
            // 
            this.lblValTable.AutoSize = true;
            this.lblValTable.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblValTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblValTable.Location = new System.Drawing.Point(220, 105);
            this.lblValTable.Name = "lblValTable";
            this.lblValTable.Size = new System.Drawing.Size(65, 48);
            this.lblValTable.TabIndex = 2;
            this.lblValTable.Text = "---";
            // 
            // lblTitleName
            // 
            this.lblTitleName.AutoSize = true;
            this.lblTitleName.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleName.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleName.Location = new System.Drawing.Point(30, 180);
            this.lblTitleName.Name = "lblTitleName";
            this.lblTitleName.Size = new System.Drawing.Size(168, 38);
            this.lblTitleName.TabIndex = 3;
            this.lblTitleName.Text = "Customer:";
            // 
            // lblValName
            // 
            this.lblValName.AutoSize = true;
            this.lblValName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValName.Location = new System.Drawing.Point(220, 180);
            this.lblValName.Name = "lblValName";
            this.lblValName.Size = new System.Drawing.Size(50, 38);
            this.lblValName.TabIndex = 4;
            this.lblValName.Text = "---";
            // 
            // lblTitleTime
            // 
            this.lblTitleTime.AutoSize = true;
            this.lblTitleTime.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleTime.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleTime.Location = new System.Drawing.Point(30, 250);
            this.lblTitleTime.Name = "lblTitleTime";
            this.lblTitleTime.Size = new System.Drawing.Size(117, 38);
            this.lblTitleTime.TabIndex = 5;
            this.lblTitleTime.Text = "Seated at:";
            // 
            // lblValTime
            // 
            this.lblValTime.AutoSize = true;
            this.lblValTime.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValTime.Location = new System.Drawing.Point(220, 250);
            this.lblValTime.Name = "lblValTime";
            this.lblValTime.Size = new System.Drawing.Size(69, 38);
            this.lblValTime.TabIndex = 6;
            this.lblValTime.Text = "--:--";
            // 
            // lblTitleDuration
            // 
            this.lblTitleDuration.AutoSize = true;
            this.lblTitleDuration.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleDuration.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleDuration.Location = new System.Drawing.Point(30, 320);
            this.lblTitleDuration.Name = "lblTitleDuration";
            this.lblTitleDuration.Size = new System.Drawing.Size(120, 38);
            this.lblTitleDuration.TabIndex = 7;
            this.lblTitleDuration.Text = "Duration:";
            // 
            // lblValDuration
            // 
            this.lblValDuration.AutoSize = true;
            this.lblValDuration.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblValDuration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.lblValDuration.Location = new System.Drawing.Point(220, 320);
            this.lblValDuration.Name = "lblValDuration";
            this.lblValDuration.Size = new System.Drawing.Size(109, 38);
            this.lblValDuration.TabIndex = 8;
            this.lblValDuration.Text = "-- mins";
            // 
            // lblTitleDeposit
            // 
            this.lblTitleDeposit.AutoSize = true;
            this.lblTitleDeposit.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblTitleDeposit.ForeColor = System.Drawing.Color.Gray;
            this.lblTitleDeposit.Location = new System.Drawing.Point(30, 390);
            this.lblTitleDeposit.Name = "lblTitleDeposit";
            this.lblTitleDeposit.Size = new System.Drawing.Size(126, 38);
            this.lblTitleDeposit.TabIndex = 9;
            this.lblTitleDeposit.Text = "Deposit:";
            // 
            // lblValDeposit
            // 
            this.lblValDeposit.AutoSize = true;
            this.lblValDeposit.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblValDeposit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblValDeposit.Location = new System.Drawing.Point(220, 385);
            this.lblValDeposit.Name = "lblValDeposit";
            this.lblValDeposit.Size = new System.Drawing.Size(58, 45);
            this.lblValDeposit.TabIndex = 10;
            this.lblValDeposit.Text = "0đ";
            // 
            // btnCheckOut
            // 
            this.btnCheckOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnCheckOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckOut.FlatAppearance.BorderSize = 0;
            this.btnCheckOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckOut.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.btnCheckOut.ForeColor = System.Drawing.Color.White;
            this.btnCheckOut.Location = new System.Drawing.Point(1220, 700);
            this.btnCheckOut.Name = "btnCheckOut";
            this.btnCheckOut.Size = new System.Drawing.Size(280, 100);
            this.btnCheckOut.TabIndex = 5;
            this.btnCheckOut.Text = "CHECK OUT";
            this.btnCheckOut.UseVisualStyleBackColor = false;
            // 
            // btnChangeTable
            // 
            this.btnChangeTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeTable.BackColor = System.Drawing.Color.White;
            this.btnChangeTable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChangeTable.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnChangeTable.FlatAppearance.BorderSize = 2;
            this.btnChangeTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeTable.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnChangeTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnChangeTable.Location = new System.Drawing.Point(920, 700);
            this.btnChangeTable.Name = "btnChangeTable";
            this.btnChangeTable.Size = new System.Drawing.Size(280, 100);
            this.btnChangeTable.TabIndex = 6;
            this.btnChangeTable.Text = "CHANGE TABLE";
            this.btnChangeTable.UseVisualStyleBackColor = false;
            // 
            // chkOverride
            // 
            this.chkOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkOverride.AutoSize = true;
            this.chkOverride.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkOverride.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.chkOverride.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.chkOverride.Location = new System.Drawing.Point(920, 640);
            this.chkOverride.Name = "chkOverride";
            this.chkOverride.Size = new System.Drawing.Size(537, 34);
            this.chkOverride.TabIndex = 4;
            this.chkOverride.Text = "⚠️ Check here to OVERRIDE Check Out";
            this.chkOverride.UseVisualStyleBackColor = true;
            this.chkOverride.Visible = false;
            // 
            // UcCheckOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.chkOverride);
            this.Controls.Add(this.btnCheckOut);
            this.Controls.Add(this.btnChangeTable);
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.flpActiveTables);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UcCheckOut";
            this.Size = new System.Drawing.Size(1530, 1122);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.FlowLayoutPanel flpActiveTables;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Label lblTitleTable;
        private System.Windows.Forms.Label lblValTable;
        private System.Windows.Forms.Label lblTitleName;
        private System.Windows.Forms.Label lblValName;
        private System.Windows.Forms.Label lblTitleTime;
        private System.Windows.Forms.Label lblValTime;
        private System.Windows.Forms.Label lblTitleDuration;
        private System.Windows.Forms.Label lblValDuration;
        private System.Windows.Forms.Label lblTitleDeposit;
        private System.Windows.Forms.Label lblValDeposit;
        private System.Windows.Forms.CheckBox chkOverride;
        private System.Windows.Forms.Button btnChangeTable;
        private System.Windows.Forms.Button btnCheckOut;
    }
}