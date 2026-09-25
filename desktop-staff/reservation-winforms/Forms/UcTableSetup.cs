using reservation_winforms.DTO.table;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcTableSetup : UserControl
    {
        private readonly TableService _tableService;
        private List<TableResponse> _tables = new List<TableResponse>();
        private int _currentTableVersion = 0;

        public UcTableSetup()
        {
            InitializeComponent();
            _tableService = new TableService();

            SetupDataGridView();

            this.Load += async (s, e) => await LoadDataAsync();
            dgvTables.CellClick += DgvTables_CellClick;

            btnSave.Click += BtnSave_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
        }

        private void SetupDataGridView()
        {
            dgvTables.Columns.Add("TableId", "Table ID");
            dgvTables.Columns.Add("Capacity", "Capacity (Pax)");
            dgvTables.Columns.Add("Status", "Usage Status");
            dgvTables.Columns.Add("IsActive", "Operating Status");

            dgvTables.EnableHeadersVisualStyles = false;
            dgvTables.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgvTables.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTables.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            dgvTables.DefaultCellStyle.Font = new Font("Segoe UI", 12F);
            dgvTables.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            var res = await _tableService.GetAllTablesAsync();
            if (res.IsSuccess)
            {
                _tables = res.Data;
                dgvTables.Rows.Clear();
                foreach (var t in _tables)
                {
                    string activeText = t.IsActive ? "Open" : "Disabled (Inactive)";
                    dgvTables.Rows.Add(t.TableId, t.Capacity, t.Status, activeText);

                    if (!t.IsActive)
                    {
                        dgvTables.Rows[dgvTables.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkGray;
                    }
                }
                BtnClear_Click(null, null);
            }
            else
            {
                MessageBox.Show(res.Message, "Data Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvTables_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                long id = Convert.ToInt64(dgvTables.Rows[e.RowIndex].Cells["TableId"].Value);
                var table = _tables.Find(t => t.TableId == id);
                if (table != null)
                {
                    txtTableId.Text = table.TableId.ToString();
                    numCapacity.Value = table.Capacity;
                    chkIsActive.Checked = table.IsActive;
                    _currentTableVersion = table.Version;

                    btnSave.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = table.IsActive;
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtTableId.Text = "";
            numCapacity.Value = 4; // Default to 4 pax
            chkIsActive.Checked = true;
            _currentTableVersion = 0;

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            dgvTables.ClearSelection();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            var req = new TableRequest { Capacity = (int)numCapacity.Value, IsActive = chkIsActive.Checked };
            var res = await _tableService.CreateTableAsync(req);

            if (res.IsSuccess)
            {
                MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            else MessageBox.Show(res.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            long id = Convert.ToInt64(txtTableId.Text);
            var req = new TableRequest
            {
                Capacity = (int)numCapacity.Value,
                IsActive = chkIsActive.Checked,
                Version = _currentTableVersion
            };

            var res = await _tableService.UpdateTableAsync(id, req);

            if (res.IsSuccess)
            {
                MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            else MessageBox.Show(res.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            long id = Convert.ToInt64(txtTableId.Text);
            var confirm = MessageBox.Show($"Are you sure you want to DISABLE Table #{id}?\nCustomers will no longer be able to book this table.",
                                          "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var res = await _tableService.DeleteTableAsync(id);
                if (res.IsSuccess)
                {
                    MessageBox.Show(res.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDataAsync();
                }
                else MessageBox.Show(res.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}