using reservation_winforms.DTO;
using reservation_winforms.DTO.reservation;
using reservation_winforms.DTO.table;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcTableMap : UserControl
    {
        private readonly TableService _tableService;
        private readonly ReservationService _reservationService;
        private readonly SystemConfigService _configService;

        private List<FloorMapTableResponse> _allTables = new List<FloorMapTableResponse>();
        private List<FloorMapTableResponse> _selectedTables = new List<FloorMapTableResponse>();

        private Timer _blinkTimer;
        private bool _blinkToggle = false;
        private List<Button> _blinkingButtons = new List<Button>();

        private int _softLockMinutes = 5;
        
        public UcTableMap()
        {
            InitializeComponent();
            _tableService = new TableService();
            _reservationService = new ReservationService();
            _configService = new SystemConfigService();

            InitializeBlinkTimer();

            this.Load += UcTableMap_Load;
            this.HandleDestroyed += UcTableMap_HandleDestroyed;

            btnFilterAll.Click += (s, e) => ApplyFilter("ALL");
            btnFilterAvailable.Click += (s, e) => ApplyFilter("AVAILABLE");
            btnFilterOccupied.Click += (s, e) => ApplyFilter("OCCUPIED");
            btnFilterReserved.Click += (s, e) => ApplyFilter("RESERVED");
            btnFilterOverstay.Click += (s, e) => ApplyFilter("OVERSTAY");

            btnSeatWalkIn.Click += BtnSeatWalkIn_Click;
        }

        private void InitializeBlinkTimer()
        {
            _blinkTimer = new Timer();
            _blinkTimer.Interval = 500;
            _blinkTimer.Tick += BlinkTimer_Tick;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            _blinkToggle = !_blinkToggle;

            foreach (var btn in _blinkingButtons)
            {
                var table = btn.Tag as FloorMapTableResponse;
                if (table == null) continue;

                if (_blinkToggle)
                {
                    btn.BackColor = Color.Orange;
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.BackColor = table.Status == "OCCUPIED" ? Color.Gray : Color.IndianRed;
                    btn.ForeColor = Color.White;
                }
            }
        }

        private async void UcTableMap_Load(object sender, EventArgs e)
        {
            await LoadSystemConfigsAsync();
            await LoadTableData();
            WebSocketService.Instance.OnTableStatusChanged += OnTableStatusChanged;
            WebSocketService.Instance.OnTableAlertReceived += OnTableAlertReceived;
        }

        private void UcTableMap_HandleDestroyed(object sender, EventArgs e)
        {
            WebSocketService.Instance.OnTableStatusChanged -= OnTableStatusChanged;
            WebSocketService.Instance.OnTableAlertReceived -= OnTableAlertReceived;
        }

        private void OnTableStatusChanged(TableUpdate msg)
        {       
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke(new Action(async () => {

                    if (msg.Status != "AVAILABLE")
                    {
                        var selected = _selectedTables.FirstOrDefault(t => t.TableId == msg.TableId);
                        if (selected != null)
                        {
                            _selectedTables.Remove(selected);
                            UpdateSelectedTableLabel();
                        }
                    }

                    await LoadTableData();
                }));
            }
        }

        private void OnTableAlertReceived(TableAlertMessage msg)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke(new Action(() => {
                    var btn = GetButtonByTableId(msg.TableId);
                    if (btn != null)
                    {
                        if (msg.AlertType == "START_BLINK")
                        {
                            if (!_blinkingButtons.Contains(btn))
                            {
                                _blinkingButtons.Add(btn);
                                if (!_blinkTimer.Enabled) _blinkTimer.Start();
                            }
                        }
                        else if (msg.AlertType == "STOP_BLINK")
                        {
                            _blinkingButtons.Remove(btn);

                            var table = btn.Tag as FloorMapTableResponse;
                            if (table != null)
                            {
                                btn.BackColor = table.Status == "OCCUPIED" ? Color.Gray : Color.IndianRed;
                            }
                        }
                    }
                }));
            }
        }

        private Button GetButtonByTableId(long tableId)
        {
            foreach (Control ctrl in flpTableMap.Controls)
            {
                if (ctrl is Button btn && btn.Tag is FloorMapTableResponse t && t.TableId == tableId)
                {
                    return btn;
                }
            }
            return null;
        }

        private async Task LoadSystemConfigsAsync()
        {
            var res = await _configService.GetAllConfigsAsync();
            if (res.IsSuccess && res.Data != null)
            {
                var softLockConfig = res.Data.FirstOrDefault(c => c.ConfigKey == "reservation.soft-lock-minutes");
                if (softLockConfig != null && int.TryParse(softLockConfig.ConfigValue, out int parsedLock))
                {
                    _softLockMinutes = parsedLock;
                }
            }
        }

        private async Task LoadTableData()
        {
            Label lblLoading = new Label { Text = "Loading floor map from server...", AutoSize = true, Font = new Font("Segoe UI", 12) };

            flpTableMap.Controls.Add(lblLoading);

            var response = await _tableService.GetFloorMapAsync();

            flpTableMap.Controls.Clear();

            if (response.IsSuccess && response.Data != null)
            {
                _allTables = response.Data;
                _selectedTables.Clear();
                UpdateSelectedTableLabel();

                if (response.Data.Count == 0)
                {
                    Label lblEmpty = new Label
                    {
                        Text = "No tables found in the system!\nPlease ask the Manager to add new tables in the 'Table Setup' tab.",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Margin = new Padding(20)
                    };
                    flpTableMap.Controls.Add(lblEmpty);
                }
                else
                {
                    _allTables = response.Data;
                    DrawTables(_allTables);
                }
            }
            else
            {
                MessageBox.Show(response.Message, "Data Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilter(string filterType)
        {
            flpTableMap.Controls.Clear();

            if (_allTables == null || _allTables.Count == 0) return;

            List<FloorMapTableResponse> filteredList = new List<FloorMapTableResponse>();

            foreach (var table in _allTables)
            {
                if (!table.IsActive) continue;

                bool isReserved = table.CurrentReservationStatus == "RESERVED";

                switch (filterType)
                {
                    case "ALL":
                        filteredList.Add(table);
                        break;
                    case "AVAILABLE":
                        if (table.Status == "AVAILABLE" && !isReserved) filteredList.Add(table);
                        break;
                    case "RESERVED":
                        if (table.Status == "AVAILABLE" && isReserved) filteredList.Add(table);
                        break;
                    case "OCCUPIED":
                        if (table.Status == "OCCUPIED") filteredList.Add(table);
                        break;
                    case "OVERSTAY":
                        if (table.Status == "OVERSTAY") filteredList.Add(table);
                        break;
                }
            }

            DrawTables(filteredList);
        }

        private void DrawTables(List<FloorMapTableResponse> tables)
        {
            _blinkTimer.Stop();
            _blinkingButtons.Clear();
            bool hasBlinkingTables = false;

            foreach (var table in tables)
            {
                if (!table.IsActive) continue;

                Button btnTable = new Button
                {
                    Width = 120,
                    Height = 120,
                    Margin = new Padding(15),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Tag = table
                };

                btnTable.FlatAppearance.BorderSize = 0;
                string tableText = $"Table {table.TableId}\n({table.Capacity} pax)";

                bool shouldBlink = false;

                if ((table.Status == "OCCUPIED" || table.Status == "OVERSTAY") && table.NextReservationTime.HasValue)
                {
                    TimeSpan timeUntilNext = table.NextReservationTime.Value - DateTime.Now;

                    if (timeUntilNext.TotalMinutes > 0 && timeUntilNext.TotalMinutes <= 15)
                    {
                        shouldBlink = true;
                        hasBlinkingTables = true;
                        tableText += $"\n\n⏰ Next: {table.NextReservationTime.Value:HH:mm}";
                    }
                }

                switch (table.Status)
                {
                    case "AVAILABLE":
                        if (table.CurrentReservationStatus == "RESERVED")
                        {
                            btnTable.BackColor = Color.Orange;
                            btnTable.ForeColor = Color.White;

                            if (table.CurrentReservationTime.HasValue)
                            {
                                tableText += $"\n\nReserved: {table.CurrentReservationTime.Value.ToString("HH:mm")}";
                            }
                            else
                            {
                                tableText += $"\n\nReserved";
                            }
                        }
                        else
                        {
                            btnTable.BackColor = Color.MediumSeaGreen;
                            btnTable.ForeColor = Color.White;
                        }
                        break;

                    case "OCCUPIED":
                        btnTable.BackColor = Color.Gray;
                        btnTable.ForeColor = Color.White;
                        if (!shouldBlink) tableText += $"\n\nCust: {table.CurrentCustomerName}";
                        break;

                    case "OVERSTAY":
                        btnTable.BackColor = Color.IndianRed;
                        btnTable.ForeColor = Color.White;
                        if (!shouldBlink) tableText += $"\n\n⚠️ OVERSTAY";
                        break;

                    default:
                        btnTable.BackColor = Color.Gray;
                        btnTable.ForeColor = Color.White;
                        break;
                }

                if (_selectedTables.Any(t => t.TableId == table.TableId))
                {
                    btnTable.BackColor = Color.DodgerBlue;
                }

                btnTable.Text = tableText;
                btnTable.Click += BtnTable_Click;

                if (shouldBlink)
                {
                    _blinkingButtons.Add(btnTable);
                }

                flpTableMap.Controls.Add(btnTable);
            }

            if (hasBlinkingTables)
            {
                _blinkTimer.Start();
            }
        }

        private void BtnTable_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            FloorMapTableResponse tableData = clickedButton.Tag as FloorMapTableResponse;

            bool isAvailable = tableData.Status == "AVAILABLE";

            if (isAvailable)
            {
                var existingTable = _selectedTables.FirstOrDefault(t => t.TableId == tableData.TableId);

                if (existingTable != null)
                {
                    _selectedTables.Remove(existingTable);
                    clickedButton.BackColor = tableData.CurrentReservationStatus == "RESERVED" ? Color.Orange : Color.MediumSeaGreen;
                }
                else
                {
                    if (tableData.CurrentReservationStatus == "RESERVED")
                    {
                        string timeStr = tableData.CurrentReservationTime?.ToString("HH:mm") ?? "N/A";
                        DialogResult confirmResult = MessageBox.Show(
                            $"Note: Table {tableData.TableId} is reserved for {timeStr}.\n" +
                            "The system will automatically limit the seating duration for Walk-in guests to clear the table on time.\n\n" +
                            "Are you sure you want to select this table?",
                            "Temporarily Available",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Information);

                        if (confirmResult != DialogResult.OK) return;
                    }

                    _selectedTables.Add(tableData);
                    clickedButton.BackColor = Color.DodgerBlue;
                }

                UpdateSelectedTableLabel();
            }
            else
            {
                MessageBox.Show($"Table {tableData.TableId} is currently occupied/overstaying. Cannot be selected!",
                    "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateSelectedTableLabel()
        {
            if (_selectedTables.Count == 0)
            {
                lblSelectedTable.Text = "No table selected";
                lblSelectedTable.ForeColor = Color.FromArgb(41, 128, 185);
            }
            else
            {
                var selectedIds = _selectedTables.Select(t => t.TableId).OrderBy(id => id);
                lblSelectedTable.Text = $"Table: {string.Join(", ", selectedIds)}";
                lblSelectedTable.ForeColor = Color.MediumSeaGreen;
            }
        }

        private async void BtnSeatWalkIn_Click(object sender, EventArgs e)
        {
            try
            {
                btnSeatWalkIn.Enabled = false;
                btnSeatWalkIn.Text = "PROCESSING...";

                int guests = (int)nudGuestCount.Value;
                List<long> finalTableIdsToSuggest = null;

                if (_selectedTables.Count > 0)
                {
                    finalTableIdsToSuggest = _selectedTables.Select(t => t.TableId).ToList();
                }
                else
                {
                    var optionsRes = await _reservationService.GetWalkInOptionsAsync(guests);

                    if (!optionsRes.IsSuccess || optionsRes.Data.Groups == null || optionsRes.Data.Groups.Count == 0)
                    {
                        MessageBox.Show("There are no available tables or combinable tables suitable for this party size.", "No Tables", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    finalTableIdsToSuggest = ShowOptionsDialog(optionsRes.Data, guests);

                    if (finalTableIdsToSuggest == null) return;
                }

                var request = new WalkInRequest
                {
                    GuestCount = guests,
                    TableId = finalTableIdsToSuggest,
                    MergeTables = false
                };

                var suggestRes = await _reservationService.SuggestWalkInAsync(request);

                if (!suggestRes.IsSuccess)
                {
                    MessageBox.Show(suggestRes.Message, "Cannot hold table", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await LoadTableData();
                    return;
                }

                var suggestion = suggestRes.Data;
                string tableList = string.Join(", ", suggestion.SuggestedTables.Select(t => t.TableId));
                string typeText = suggestion.AvailabilityType.Contains("PARTIAL") ? "⚠️ TEMPORARILY AVAILABLE (Pending later reservation)" : "✅ FULLY AVAILABLE";

                string confirmMsg = $"[SEATING CONFIRMATION]\n\n" +
                                    $"Tables: {tableList} (Capacity: {suggestion.SuggestedTables.Sum(t => t.Capacity)} seats)\n" +
                                    $"Type: {typeText}\n" +
                                    $"Duration: From {suggestion.StartTime:HH:mm} to {suggestion.EndTime:HH:mm}\n\n" +
                                    $"*System is temporarily locking this table for {_softLockMinutes} mins (until {suggestion.LockExpiresAt:HH:mm:ss}).\n" +
                                    $"Does the customer agree?";

                DialogResult result = MessageBox.Show(confirmMsg, "Confirm Order", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    var confirmRes = await _reservationService.ConfirmWalkInAsync(suggestion.SuggestionId);

                    if (confirmRes.IsSuccess)
                    {
                        MessageBox.Show("Table seated successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(confirmRes.Message, "Confirmation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    await _reservationService.CancelWalkInSuggestionAsync(suggestion.SuggestionId);
                    MessageBox.Show("Seating suggestion canceled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                _selectedTables.Clear();
                UpdateSelectedTableLabel();
                await LoadTableData();
            }
            finally
            {
                btnSeatWalkIn.Enabled = true;
                btnSeatWalkIn.Text = "SEAT WALK-IN";
            }
        }

        private List<long> ShowOptionsDialog(WalkInOptionResponse data, int guestCount)
        {
            List<long> selectedIds = null;

            Form popup = new Form
            {
                Text = $"Table suggestions for {guestCount} guests",
                Size = new Size(550, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10)
            };
            popup.Controls.Add(flp);

            foreach (var group in data.Groups)
            {
                Label lblGroup = new Label
                {
                    Text = group.GroupName,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    AutoSize = true,
                    Margin = new Padding(0, 10, 0, 5),
                    ForeColor = group.GroupName.Contains("Priority") ? Color.MediumSeaGreen : Color.Orange
                };
                flp.Controls.Add(lblGroup);

                foreach (var opt in group.Options)
                {
                    string tableStr = string.Join(", ", opt.TableIds);
                    string typeStr = opt.TableIds.Count > 1 ? "Combined" : "Single";
                    string limitStr = opt.AvailableUntil.HasValue ? $" | Must leave by: {opt.AvailableUntil.Value:HH:mm}" : "";

                    string btnText = $"Table {tableStr} ({opt.TotalCapacity} pax) - {typeStr}{limitStr}";

                    Button btnOpt = new Button
                    {
                        Text = btnText,
                        Width = 490,
                        Height = 45,
                        Margin = new Padding(5, 5, 5, 5),
                        TextAlign = ContentAlignment.MiddleLeft,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular)
                    };

                    btnOpt.FlatAppearance.BorderColor = opt.AvailableUntil.HasValue ? Color.Orange : Color.LightGray;

                    btnOpt.Click += (s, e) =>
                    {
                        selectedIds = opt.TableIds;
                        popup.DialogResult = DialogResult.OK;
                        popup.Close();
                    };

                    flp.Controls.Add(btnOpt);
                }
            }

            popup.ShowDialog();
            return selectedIds;
        }

    }
}