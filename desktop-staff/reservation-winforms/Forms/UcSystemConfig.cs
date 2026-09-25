using reservation_winforms.DTO.config;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcSystemConfig : UserControl
    {
        private readonly SystemConfigService _configService;
        private readonly TableService _tableService;
        private List<SystemConfigDTO> _configList;

        private DateTimePicker _timePicker;
        private int _totalTablesCount = 2;

        public UcSystemConfig()
        {
            InitializeComponent();
            _configService = new SystemConfigService();
            _tableService = new TableService();
            _configList = new List<SystemConfigDTO>();

            SetupTimePicker();

            this.Load += async (s, e) => await LoadConfigsAsync();

            dgvConfigs.CellContentClick += DgvConfigs_CellContentClick;
            dgvConfigs.CellClick += DgvConfigs_CellClick;
            dgvConfigs.Scroll += (s, e) => _timePicker.Visible = false;

            dgvConfigs.CurrentCellDirtyStateChanged += DgvConfigs_CurrentCellDirtyStateChanged;
            dgvConfigs.CellValueChanged += DgvConfigs_CellValueChanged;
        }

        private void SetButtonState(int rowIndex, bool isEnabled)
        {
            if (rowIndex < 0 || rowIndex >= dgvConfigs.Rows.Count) return;

            var btnCell = dgvConfigs.Rows[rowIndex].Cells[3] as DataGridViewButtonCell;
            if (btnCell == null) return;

            if (isEnabled)
            {
                btnCell.Style.BackColor = Color.FromArgb(39, 174, 96);
                btnCell.Style.ForeColor = Color.White;
                btnCell.Style.SelectionBackColor = Color.FromArgb(39, 174, 96);
                btnCell.Value = "SAVE CHANGES";
                btnCell.Tag = "ENABLED";
            }
            else
            {
                btnCell.Style.BackColor = Color.FromArgb(224, 224, 224);
                btnCell.Style.ForeColor = Color.Gray;
                btnCell.Style.SelectionBackColor = Color.FromArgb(224, 224, 224);
                btnCell.Value = "UNCHANGED";
                btnCell.Tag = "DISABLED";
            }
        }

        private void DgvConfigs_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvConfigs.IsCurrentCellDirty && dgvConfigs.CurrentCell.ColumnIndex == 2)
            {
                dgvConfigs.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvConfigs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 2)
            {
                string key = dgvConfigs.Rows[e.RowIndex].Cells[0].Value.ToString();
                string currentValue = dgvConfigs.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";

                var originalConfig = _configList.FirstOrDefault(c => c.ConfigKey == key);
                if (originalConfig != null)
                {
                    bool hasChanged = currentValue != originalConfig.ConfigValue;
                    SetButtonState(e.RowIndex, hasChanged);
                }
            }
        }

        private void SetupTimePicker()
        {
            _timePicker = new DateTimePicker();
            _timePicker.Format = DateTimePickerFormat.Custom;
            _timePicker.CustomFormat = "HH:mm";
            _timePicker.ShowUpDown = true;
            _timePicker.Visible = false;
            _timePicker.Font = new Font("Segoe UI", 12);

            _timePicker.Leave += TimePicker_Leave;
            dgvConfigs.Controls.Add(_timePicker);
        }

        private void TimePicker_Leave(object sender, EventArgs e)
        {
            _timePicker.Visible = false;
            if (dgvConfigs.CurrentCell != null && dgvConfigs.CurrentCell.ColumnIndex == 2)
            {
                dgvConfigs.CurrentCell.Value = _timePicker.Value.ToString("HH:mm");
            }
        }

        private void DgvConfigs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string key = dgvConfigs.Rows[e.RowIndex].Cells[0].Value.ToString();

            if (e.ColumnIndex == 2 && (key.Contains("opening-time") || key.Contains("closing-time")))
            {
                Rectangle displayRect = dgvConfigs.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                _timePicker.Size = new Size(displayRect.Width, displayRect.Height);
                _timePicker.Location = new Point(displayRect.X, displayRect.Y);

                string currentValue = dgvConfigs.Rows[e.RowIndex].Cells[2].Value?.ToString();
                if (DateTime.TryParse(currentValue, out DateTime parsedTime))
                {
                    _timePicker.Value = parsedTime;
                }
                else
                {
                    _timePicker.Value = DateTime.Now;
                }

                _timePicker.Visible = true;
                _timePicker.Focus();
            }
            else
            {
                _timePicker.Visible = false;
            }
        }

        private async Task LoadConfigsAsync()
        {
            lblStatus.Text = "Loading system configurations...";
            lblStatus.ForeColor = Color.Orange;

            var tableRes = await _tableService.GetFloorMapAsync();
            if (tableRes.IsSuccess && tableRes.Data != null)
            {
                _totalTablesCount = tableRes.Data.Count;
            }

            var response = await _configService.GetAllConfigsAsync();

            if (response.IsSuccess && response.Data != null)
            {
                _configList = response.Data;
                RenderConfigGrid();
                lblStatus.Text = "Data loaded successfully!";
                lblStatus.ForeColor = Color.MediumSeaGreen;
            }
            else
            {
                MessageBox.Show(response.Message, "Configuration Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Load failed!";
                lblStatus.ForeColor = Color.IndianRed;
            }
        }

        private void RenderConfigGrid()
        {
            dgvConfigs.Rows.Clear();

            foreach (var config in _configList)
            {
                int rowIndex = dgvConfigs.Rows.Add(
                    config.ConfigKey,
                    config.Description,
                    config.ConfigValue,
                    "UNCHANGED"
                );

                SetButtonState(rowIndex, false);
            }
        }

        private async void DgvConfigs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 3)
            {
                var btnCell = dgvConfigs.Rows[e.RowIndex].Cells[3];
                if (btnCell.Tag?.ToString() == "DISABLED")
                {
                    return;
                }

                if (_timePicker.Visible)
                {
                    TimePicker_Leave(null, null);
                }

                string key = dgvConfigs.Rows[e.RowIndex].Cells[0].Value.ToString();
                string description = dgvConfigs.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? key;
                string newValue = dgvConfigs.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";

                if (!ValidateInput(key, newValue, out string errorMsg))
                {
                    MessageBox.Show(errorMsg, "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to update:\n[{description}] to value [{newValue}]?",
                    "Confirm Update",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await UpdateConfigValue(key, newValue, e.RowIndex);
                }
            }
        }

        private string GetValueFromGrid(string targetKey)
        {
            foreach (DataGridViewRow row in dgvConfigs.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == targetKey)
                {
                    return row.Cells[2].Value?.ToString();
                }
            }
            return "";
        }

        private int GetParsedInt(string targetKey, string currentKey, string currentValue)
        {
            string strVal = (currentKey == targetKey) ? currentValue : GetValueFromGrid(targetKey);
            return int.TryParse(strVal, out int val) ? val : 0;
        }

        private TimeSpan GetParsedTime(string targetKey, string currentKey, string currentValue)
        {
            string strVal = (currentKey == targetKey) ? currentValue : GetValueFromGrid(targetKey);
            return TimeSpan.TryParse(strVal, out TimeSpan val) ? val : TimeSpan.Zero;
        }

        private bool ValidateInput(string key, string value, out string errorMessage)
        {
            errorMessage = "";

            if (key == "reservation.deposit-per-guest")
            {
                if (!double.TryParse(value, out double deposit) || deposit < 0)
                {
                    errorMessage = "The deposit fee must be a number greater than or equal to 0.";
                    return false;
                }
                return true;
            }

            if (key == "restaurant.opening-time" || key == "restaurant.closing-time")
            {
                if (!TimeSpan.TryParse(value, out TimeSpan dummy))
                {
                    errorMessage = "Invalid time format (HH:mm).";
                    return false;
                }

                TimeSpan openTime = GetParsedTime("restaurant.opening-time", key, value);
                TimeSpan closeTime = GetParsedTime("restaurant.closing-time", key, value);

                if (openTime >= closeTime)
                {
                    errorMessage = "Logic error: Opening time must be EARLIER than closing time!";
                    return false;
                }

                int duration = GetParsedInt("reservation.duration-minutes", key, value);
                int buffer = GetParsedInt("reservation.buffer-minutes", key, value);
                if ((closeTime - openTime).TotalMinutes < (duration + buffer))
                {
                    errorMessage = $"Opening hours are too short! Must be enough for at least 1 turn ({duration + buffer} minutes).";
                    return false;
                }
                return true;
            }

            if (!int.TryParse(value, out int intVal) || intVal < 0)
            {
                errorMessage = "This value must be an integer greater than or equal to 0.";
                return false;
            }

            switch (key)
            {
                case "reservation.duration-minutes":
                    if (intVal < 30) { errorMessage = "Standard seating duration must be at least 30 minutes."; return false; }
                    int grace1 = GetParsedInt("reservation.grace-period-minutes", key, value);
                    if (grace1 >= intVal) { errorMessage = "Grace Period cannot be greater than or equal to the total seating duration!"; return false; }
                    break;

                case "reservation.buffer-minutes":
                    if (intVal < 0 || intVal > 60) { errorMessage = "Buffer time should be between 0 and 60 minutes."; return false; }
                    break;

                case "reservation.grace-period-minutes":
                    if (intVal < 0) { errorMessage = "Grace period cannot be negative."; return false; }
                    int duration1 = GetParsedInt("reservation.duration-minutes", key, value);
                    if (intVal >= duration1) { errorMessage = "Grace period cannot be longer than the standard seating duration!"; return false; }
                    break;

                case "reservation.soft-lock-minutes":
                    if (intVal <= 0 || intVal > 15) { errorMessage = "Soft-lock time should be between 1 and 15 minutes to avoid locking the system."; return false; }
                    break;

                case "reservation.max-merge-tables":
                    if (intVal < 1) { errorMessage = "Maximum combinable tables must be at least 1."; return false; }
                    if (intVal > _totalTablesCount && _totalTablesCount > 0)
                    {
                        errorMessage = $"Invalid: Maximum combinable tables ({intVal}) exceeds TOTAL AVAILABLE TABLES in the restaurant ({_totalTablesCount} tables).";
                        return false;
                    }
                    break;

                case "reservation.max-capacity-overflow":
                    if (intVal < 0 || intVal > 10) { errorMessage = "Capacity overflow should be between 0 and a maximum of 10 seats per group."; return false; }
                    break;
            }

            return true;
        }
        
        private async Task UpdateConfigValue(string key, string newValue, int rowIndex)
        {
            var btnCell = dgvConfigs.Rows[rowIndex].Cells[3] as DataGridViewButtonCell;
            btnCell.Value = "SAVING...";
            dgvConfigs.Enabled = false;

            var response = await _configService.UpdateConfigAsync(key, newValue);

            if (response.IsSuccess)
            {
                MessageBox.Show("Configuration updated successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var configInList = _configList.FirstOrDefault(c => c.ConfigKey == key);
                if (configInList != null) configInList.ConfigValue = newValue;

                SetButtonState(rowIndex, false);
            }
            else
            {
                MessageBox.Show(response.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await LoadConfigsAsync();
            }

            dgvConfigs.Enabled = true;
        }
    }
}