using reservation_winforms.DTO.reservation;
using reservation_winforms.DTO.waitList;
using reservation_winforms.Services;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcWaitlist : UserControl
    {
        private readonly WaitlistService _waitlistService;
        private readonly ReservationService _reservationService;
        private List<WaitlistResponse> _masterData = new List<WaitlistResponse>();

        public UcWaitlist()
        {
            InitializeComponent();
            _waitlistService = new WaitlistService();
            _reservationService = new ReservationService();

            this.Load += async (s, e) => await LoadDataAsync();
            btnReload.Click += async (s, e) => await LoadDataAsync();
            btnAdd.Click += async (s, e) => await AddNewWaitlistAsync();

            btnSearchMissing.Click += (s, e) => RenderMissingGrid();
            txtSearchMissing.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) RenderMissingGrid();
            };

            dgvWaiting.CellContentClick += DgvWaiting_CellContentClick;
            dgvMissing.CellContentClick += DgvMissing_CellContentClick;
        }

        private async Task LoadDataAsync()
        {
            btnReload.Enabled = false;
            btnReload.Text = "LOADING...";

            var res = await _waitlistService.GetWaitlistAsync();
            if (res.IsSuccess)
            {
                _masterData = res.Data;
                RenderWaitingGrid();
                RenderMissingGrid();
            }
            else
            {
                MessageBox.Show(res.Message, "Data Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnReload.Enabled = true;
            btnReload.Text = "RELOAD";
        }

        private void RenderWaitingGrid()
        {
            dgvWaiting.Rows.Clear();
            var waitingList = _masterData.Where(x => x.Status.ToString() == "WAITING").ToList();

            tabWaiting.Text = $"WAITING ({waitingList.Count})";

            for (int i = 0; i < waitingList.Count; i++)
            {
                var item = waitingList[i];
                int rowIndex = dgvWaiting.Rows.Add(
                    i + 1,
                    item.CustomerName,
                    item.CustomerPhone,
                    item.GuestCount,
                    item.JoinedAt.ToString("HH:mm"),
                    item.ReadyToSeat ? "Seat Table" : "No tables",
                    "Missed",
                    "Cancel"
                );

                dgvWaiting.Rows[rowIndex].Tag = item;

                if (item.ReadyToSeat)
                {
                    dgvWaiting.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(235, 248, 240);
                }

                var seatCell = (DataGridViewButtonCell)dgvWaiting.Rows[rowIndex].Cells[colWaitActionSeat.Index];
                seatCell.UseColumnTextForButtonValue = false;
                seatCell.FlatStyle = FlatStyle.Flat;

                if (!item.ReadyToSeat)
                {
                    seatCell.Style.BackColor = Color.FromArgb(224, 224, 224);
                    seatCell.Style.ForeColor = Color.Gray;
                }
                else
                {
                    seatCell.Style.BackColor = Color.MediumSeaGreen;
                    seatCell.Style.ForeColor = Color.White;
                }
            }
        }

        private void RenderMissingGrid()
        {
            dgvMissing.Rows.Clear();
            string keyword = txtSearchMissing.Text.Trim().ToLower();

            var missingList = _masterData
                .Where(x => x.Status.ToString() == "MISSING")
                .Where(x => string.IsNullOrEmpty(keyword) || x.CustomerPhone.Contains(keyword))
                .ToList();

            tabMissing.Text = $"MISSED ({missingList.Count})";

            for (int i = 0; i < missingList.Count; i++)
            {
                var item = missingList[i];
                int rowIndex = dgvMissing.Rows.Add(
                    i + 1,
                    item.CustomerName,
                    item.CustomerPhone,
                    item.GuestCount,
                    item.JoinedAt.ToString("HH:mm"),
                    item.ReadyToSeat ? "Seat Table" : "No tables",
                    "Re-wait",
                    "Cancel"
                );

                dgvMissing.Rows[rowIndex].Tag = item;

                var seatCell = (DataGridViewButtonCell)dgvMissing.Rows[rowIndex].Cells[colMissActionSeat.Index];
                seatCell.UseColumnTextForButtonValue = false;
                seatCell.FlatStyle = FlatStyle.Flat;

                if (!item.ReadyToSeat)
                {
                    seatCell.Style.BackColor = Color.FromArgb(224, 224, 224);
                    seatCell.Style.ForeColor = Color.Gray;
                }
                else
                {
                    seatCell.Style.BackColor = Color.MediumSeaGreen;
                    seatCell.Style.ForeColor = Color.White;
                }
            }
        }

        private async Task AddNewWaitlistAsync()
        {
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            int guests = (int)nudGuests.Value;
            bool allowShort = chkAllowShortSeating.Checked;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Please enter both Name and Phone number!", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(phone, @"^(0[3|5|7|8|9])+([0-9]{8})$"))
            {
                MessageBox.Show("Invalid phone number!\nPlease enter exactly 10 digits starting with 0 (E.g.: 0912345678).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            btnAdd.Enabled = false;
            var req = new WaitlistRequest { Name = name, Phone = phone, GuestCount = guests, AllowShortSeating = allowShort };

            var res = await _waitlistService.AddToWaitlistAsync(req);

            if (res.IsSuccess)
            {
                txtName.Clear();
                txtPhone.Clear();
                nudGuests.Value = 2;
                chkAllowShortSeating.Checked = false;

                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show(res.Message, "Error Adding Waitlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnAdd.Enabled = true;
        }

        private async void DgvWaiting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var item = (WaitlistResponse)dgvWaiting.Rows[e.RowIndex].Tag;

            if (e.ColumnIndex == colWaitActionSeat.Index)
            {
                if (!item.ReadyToSeat)
                {
                    MessageBox.Show("There are no available tables for this group size yet. Please click 'RELOAD' to check the system again.", "No Tables", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                await HandleSeatAction(item);
            }
            else if (e.ColumnIndex == colWaitActionMiss.Index)
            {
                await CallActionAsync(item.WaitlistId, "missing");
            }
            else if (e.ColumnIndex == colWaitActionSkip.Index)
            {
                if (MessageBox.Show("Are you sure you want to cancel and remove this customer from the waitlist?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    await CallActionAsync(item.WaitlistId, "skip");
                }
            }
        }

        private async void DgvMissing_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var item = (WaitlistResponse)dgvMissing.Rows[e.RowIndex].Tag;

            if (e.ColumnIndex == colMissActionSeat.Index)
            {
                if (!item.ReadyToSeat)
                {
                    MessageBox.Show("There are no available tables for this group size yet.", "No Tables", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                await HandleSeatAction(item);
            }
            else if (e.ColumnIndex == colMissActionReWait.Index)
            {
                await CallActionAsync(item.WaitlistId, "re-wait");
                txtSearchMissing.Clear();
            }
            else if (e.ColumnIndex == colMissActionSkip.Index)
            {
                if (MessageBox.Show("Are you sure you want to cancel this customer's request?", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    await CallActionAsync(item.WaitlistId, "skip");
                }
            }
        }

        private async Task CallActionAsync(long id, string action)
        {
            var res = await _waitlistService.MarkActionAsync(id, action);
            if (res.IsSuccess)
            {
                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show(res.Message, "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task HandleSeatAction(WaitlistResponse item)
        {
            var optionsRes = await _reservationService.GetWalkInOptionsAsync(item.GuestCount);
            if (!optionsRes.IsSuccess || optionsRes.Data.Groups.Count == 0)
            {
                MessageBox.Show("System returned an error or there are no actual tables available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedIds = ShowOptionsDialog(optionsRes.Data, item.GuestCount);
            if (selectedIds == null || selectedIds.Count == 0) return;

            var suggestReq = new WalkInRequest { GuestCount = item.GuestCount, TableId = selectedIds, MergeTables = selectedIds.Count > 1 };
            var suggestRes = await _reservationService.SuggestWalkInAsync(suggestReq);

            if (!suggestRes.IsSuccess)
            {
                MessageBox.Show(suggestRes.Message, "Table Lock Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirmRes = await _reservationService.ConfirmWalkInAsync(suggestRes.Data.SuggestionId);
            if (confirmRes.IsSuccess)
            {
                await _waitlistService.MarkActionAsync(item.WaitlistId, "seat");
                MessageBox.Show("Customer seated successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show(confirmRes.Message, "Confirmation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<long> ShowOptionsDialog(WalkInOptionResponse data, int guestCount)
        {
            List<long> selectedIds = null;

            Form popup = new Form
            {
                Text = $"Table suggestions for {guestCount} pax",
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