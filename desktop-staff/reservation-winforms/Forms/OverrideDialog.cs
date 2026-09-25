using System;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class OverrideDialog : Form
    {
        public string Reason { get; private set; }

        public OverrideDialog()
        {
            InitializeComponent();

            btnConfirm.Click += BtnConfirm_Click;
            btnCancel.Click += BtnCancel_Click;

            this.AcceptButton = btnConfirm;
            this.CancelButton = btnCancel;
        }

        public void SetWarningMessage(string message)
        {
            lblWarning.Text = message;
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            string inputReason = txtReason.Text.Trim();

            if (string.IsNullOrEmpty(inputReason))
            {
                MessageBox.Show("Please enter the reason for override!", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReason.Focus();
                return;
            }

            this.Reason = inputReason;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}