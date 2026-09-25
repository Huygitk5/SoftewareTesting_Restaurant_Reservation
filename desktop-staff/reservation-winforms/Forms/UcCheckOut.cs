using reservation_winforms.DTO.reservation;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcCheckOut : UserControl
    {
        private readonly ReservationService _reservationService;
        private readonly OverrideService _overrideService;
        private readonly SystemConfigService _configService;

        private ReservationResponse _currentSelectedRes = null;

        private int _durationMinutes = 120;

        public UcCheckOut()
        {
            InitializeComponent();
            _reservationService = new ReservationService();
            _overrideService = new OverrideService();
            _configService = new SystemConfigService();

            btnRefresh.Click += BtnRefresh_Click;
            btnCheckOut.Click += BtnCheckOut_Click;

            this.Load += async (s, e) =>
            {
                await LoadSystemConfigsAsync();
                await LoadActiveTablesAsync();
            };

            btnChangeTable.Click += async (s, e) => await HandleChangeTableAsync();

            ResetDetailsPanel();
        }

        private async Task LoadSystemConfigsAsync()
        {
            var res = await _configService.GetAllConfigsAsync();
            if (res.IsSuccess && res.Data != null)
            {
                var durationConfig = res.Data.FirstOrDefault(c => c.ConfigKey == "reservation.duration-minutes");
                if (durationConfig != null && int.TryParse(durationConfig.ConfigValue, out int parsedDuration))
                {
                    _durationMinutes = parsedDuration;
                }
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadActiveTablesAsync();
        }

        private async Task LoadActiveTablesAsync()
        {
            btnRefresh.Enabled = false;
            btnRefresh.Text = "LOADING...";

            var res = await _reservationService.GetActiveReservationsAsync();

            btnRefresh.Enabled = true;
            btnRefresh.Text = "REFRESH";

            if (!res.IsSuccess || res.Data == null)
            {
                MessageBox.Show("Unable to load the list of active tables.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RenderActiveTables(res.Data);
        }

        private void RenderActiveTables(List<ReservationResponse> activeReservations)
        {
            flpActiveTables.Controls.Clear();
            ResetDetailsPanel();

            if (activeReservations.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "There are currently no active tables.",
                    Font = new Font("Segoe UI", 14, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                flpActiveTables.Controls.Add(lblEmpty);
                return;
            }

            foreach (var r in activeReservations)
            {
                string tableStr = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "N/A";
                TimeSpan duration = DateTime.Now - r.StartTime;
                int durationMins = (int)duration.TotalMinutes;

                string durationText = durationMins > _durationMinutes ? $"⚠️ Overdue ({durationMins}min)" : $"{durationMins} minutes";

                Button btnCard = new Button
                {
                    Text = $"🪑 TABLE {tableStr}\n\n👥 Customer: {r.CustomerName ?? "Walk-in"}\n🕒 Seat: {durationText}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Width = 260,
                    Height = 150,
                    Margin = new Padding(10),
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    BackColor = Color.White,
                    Tag = r
                };

                btnCard.FlatAppearance.BorderColor = durationMins > _durationMinutes ? Color.FromArgb(231, 76, 60) : Color.FromArgb(41, 128, 185);
                btnCard.FlatAppearance.BorderSize = 2;

                btnCard.Click += (s, e) =>
                {
                    foreach (Control c in flpActiveTables.Controls)
                        if (c is Button b) { b.BackColor = Color.White; b.ForeColor = Color.Black; }

                    btnCard.BackColor = Color.FromArgb(41, 128, 185);
                    btnCard.ForeColor = Color.White;

                    _currentSelectedRes = (ReservationResponse)btnCard.Tag;
                    ShowReservationDetails(_currentSelectedRes);
                };

                flpActiveTables.Controls.Add(btnCard);
            }
        }

        private void ShowReservationDetails(ReservationResponse r)
        {
            pnlDetails.Visible = true;
            btnCheckOut.Visible = true;
            btnChangeTable.Visible = true;

            lblValTable.Text = (r.TableIds != null && r.TableIds.Count > 0) ? string.Join(", ", r.TableIds) : "N/A";
            lblValName.Text = r.CustomerName ?? "Walk-in customer ";
            lblValTime.Text = r.StartTime.ToString("HH:mm");

            TimeSpan duration = DateTime.Now - r.StartTime;
            int mins = (int)duration.TotalMinutes;
            if (mins < 2)
            {
                lblValDuration.Text = $"{mins} minute";
            }
            else
            {
                lblValDuration.Text = $"{mins} minutes";
            }

            lblValDuration.ForeColor = mins > _durationMinutes ? Color.Red : Color.FromArgb(243, 156, 18);

            lblValDeposit.Text = (r.DepositAmount != null && r.DepositAmount > 0) ? $"{r.DepositAmount:N0}đ" : "0đ";

            chkOverride.Checked = false;
            chkOverride.Visible = (mins > _durationMinutes);
        }

        private async void BtnCheckOut_Click(object sender, EventArgs e)
        {
            if (_currentSelectedRes == null) return;

            if (chkOverride.Checked)
            {
                using (var frm = new OverrideDialog())
                {
                    frm.SetWarningMessage($"The table for {_currentSelectedRes.CustomerName} has exceeded its allotted time (Overstay).\nPlease enter a resolution reason to override the system.");

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            btnCheckOut.Enabled = false;
                            btnCheckOut.Text = "OVERRIDING...";

                            var res = await _overrideService.OverrideReservationAsync(_currentSelectedRes.ReservationId, frm.Reason);

                            if (res.IsSuccess)
                            {
                                MessageBox.Show("Forced payment processed successfully. The log has been saved and the table has been released!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadActiveTablesAsync();
                            }
                            else
                            {
                                MessageBox.Show(res.Message, "Override error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        finally
                        {
                            btnCheckOut.Enabled = true;
                            btnCheckOut.Text = "CHECK OUT";
                        }
                    }
                }
                return;
            }

            string confirmMsg = $"Confirm payment and release the table for {_currentSelectedRes.CustomerName}?\n";
            if (_currentSelectedRes.DepositAmount > 0)
            {
                confirmMsg += $"\n💰 Warning: This customer has already made a deposit. Please deduct {_currentSelectedRes.DepositAmount:N0} VND from the bill.";
            }

            var confirm = MessageBox.Show(confirmMsg, "Checkout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                btnCheckOut.Enabled = false;
                btnCheckOut.Text = "PROCESSING...";

                var result = await _reservationService.CheckOutAsync(_currentSelectedRes.ReservationId);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Table successfully released! These tables are now available for the next guests..", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadActiveTablesAsync();
                }
                else
                {
                    MessageBox.Show(result.Message, "Checkout error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnCheckOut.Enabled = true;
                btnCheckOut.Text = "CHECK OUT";
            }
        }

        private async Task HandleChangeTableAsync()
        {
            if (_currentSelectedRes == null)
            {
                MessageBox.Show("Please select an occupied table to change!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnChangeTable.Enabled = false;
                btnChangeTable.Text = "LOADING...";

                int guestCount = _currentSelectedRes.GuestCount;

                var optionsRes = await _reservationService.GetWalkInOptionsAsync(guestCount);

                if (!optionsRes.IsSuccess || optionsRes.Data == null || optionsRes.Data.Groups == null || optionsRes.Data.Groups.Count == 0)
                {
                    MessageBox.Show("There are currently no suitable available table combinations for this party size!", "No Tables Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedIds = ShowOptionsDialog(optionsRes.Data, guestCount);
                if (selectedIds == null || selectedIds.Count == 0) return;

                var changeReq = new ChangeTableRequest
                {
                    TableIds = selectedIds
                };

                btnChangeTable.Text = "CHANGING...";
                var res = await _reservationService.ChangeTableAsync(_currentSelectedRes.ReservationId, changeReq);

                if (res.IsSuccess)
                {
                    MessageBox.Show("Table transfer completed successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    long savedResId = _currentSelectedRes.ReservationId;
                    await LoadActiveTablesAsync();

                    foreach (Control c in flpActiveTables.Controls)
                    {
                        if (c is Button btnCard && btnCard.Tag is ReservationResponse rObj && rObj.ReservationId == savedResId)
                        {
                            btnCard.PerformClick();
                            flpActiveTables.ScrollControlIntoView(btnCard);
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(res.Message, "Cannot Change Table", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                btnChangeTable.Enabled = true;
                btnChangeTable.Text = "CHANGE TABLE";
            }
        }

        private List<long> ShowOptionsDialog(WalkInOptionResponse data, int guestCount)
        {
            List<long> selectedIds = null;

            Form popup = new Form
            {
                Text = $"Suggested tables for {guestCount} guests",
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
                    string typeStr = opt.TableIds.Count > 1 ? "Combined table" : "Single table";
                    string limitStr = opt.AvailableUntil.HasValue ? $" | Must release table at: {opt.AvailableUntil.Value:HH:mm}" : "";

                    string btnText = $"Table {tableStr} ({opt.TotalCapacity} seats) - {typeStr}{limitStr}";

                    Button btnOpt = new Button
                    {
                        Text = btnText,
                        Width = 490,
                        Height = 45,
                        Margin = new Padding(5),
                        TextAlign = ContentAlignment.MiddleLeft,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular)
                    };

                    btnOpt.FlatAppearance.BorderColor = opt.AvailableUntil.HasValue ? Color.Orange : Color.LightGray;

                    btnOpt.Click += (s, e) =>
                    {
                        var confirmResult = MessageBox.Show(
                            $"Are you sure you want to switch to Table {tableStr} ?",
                            "Confirm table change",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (confirmResult == DialogResult.Yes)
                        {
                            selectedIds = opt.TableIds;
                            popup.DialogResult = DialogResult.OK;
                            popup.Close();
                        }
                    };

                    flp.Controls.Add(btnOpt);
                }
            }

            popup.ShowDialog();
            return selectedIds;
        }

        private void ResetDetailsPanel()
        {
            pnlDetails.Visible = false;
            btnCheckOut.Visible = false;
            btnChangeTable.Visible = false;
            _currentSelectedRes = null;
            chkOverride.Visible = false;
            chkOverride.Checked = false;
        }
    }
}