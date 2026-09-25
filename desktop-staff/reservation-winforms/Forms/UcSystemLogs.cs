using reservation_winforms.DTO.overrides;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reservation_winforms.Forms
{
    public partial class UcSystemLogs : UserControl
    {
        private readonly OverrideService _overrideService;
        private List<OverrideLogResponse> _allLogs = new List<OverrideLogResponse>();

        public UcSystemLogs()
        {
            InitializeComponent();
            _overrideService = new OverrideService();

            dtpFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpTo.Value = DateTime.Now;

            dtpFrom.ValueChanged += DtpFrom_ValueChanged;
            DtpFrom_ValueChanged(null, null);

            btnFilter.Click += BtnFilter_Click;

            txtSearch.Text = "";
            txtSearch.ForeColor = Color.Black;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            dgvLogs.RowTemplate.Height = 50;

            btnExportExcel.Click += BtnExportExcel_Click;

            this.Load += async (s, e) => await LoadLogsAsync();
        }

        private void DtpFrom_ValueChanged(object sender, EventArgs e)
        {
            dtpTo.MinDate = dtpFrom.Value.Date;
        }

        private async void BtnFilter_Click(object sender, EventArgs e)
        {
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            btnFilter.Enabled = false;
            btnFilter.Text = "LOADING...";

            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date.AddDays(1).AddTicks(-1);

            var res = await _overrideService.GetLogsAsync(fromDate, toDate);

            if (res.IsSuccess && res.Data != null)
            {
                _allLogs = res.Data;

                // Nếu ô tìm kiếm đang có chữ thì tự động lọc lại luôn
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    TxtSearch_TextChanged(null, null);
                }
                else
                {
                    RenderLogs(_allLogs);
                }
            }
            else
            {
                MessageBox.Show(res.Message, "Data Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            btnFilter.Enabled = true;
            btnFilter.Text = "FILTER DATA";
        }

        private void RenderLogs(List<OverrideLogResponse> list)
        {
            dgvLogs.Rows.Clear();

            foreach (var log in list)
            {
                dgvLogs.Rows.Add(
                    log.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                    log.AccountUsername,
                    $"#{log.ReservationId}",
                    log.Reason
                );
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                RenderLogs(_allLogs);
                return;
            }

            var filteredList = _allLogs.Where(log =>
                (log.AccountUsername != null && log.AccountUsername.ToLower().Contains(keyword)) ||
                log.ReservationId.ToString().Contains(keyword)
            ).ToList();

            RenderLogs(filteredList);
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvLogs.Rows.Count == 0 || (dgvLogs.Rows.Count == 1 && dgvLogs.Rows[0].IsNewRow))
            {
                MessageBox.Show("No log data to export!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string defaultFileName = $"system_logs_{DateTime.Now:yyyyMMdd_HHmm}.xls";
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Spreadsheet (*.xls)|*.xls", FileName = defaultFileName })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder xml = new StringBuilder();
                        xml.AppendLine("<?xml version=\"1.0\"?>");
                        xml.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
                        xml.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                        xml.AppendLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
                        xml.AppendLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
                        xml.AppendLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                        xml.AppendLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");

                        // 1. STYLES
                        xml.AppendLine(" <Styles>");
                        xml.AppendLine("  <Style ss:ID=\"Default\" ss:Name=\"Normal\"><Alignment ss:Vertical=\"Bottom\"/><Borders/><Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\"/></Style>");
                        xml.AppendLine("  <Style ss:ID=\"sTitle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"16\" ss:Bold=\"1\" ss:Color=\"#2C3E50\"/></Style>");
                        xml.AppendLine("  <Style ss:ID=\"sHeader\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"12\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#273C75\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"sData\"><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine(" </Styles>");

                        // 2. WORKSHEET
                        xml.AppendLine(" <Worksheet ss:Name=\"System Logs\">");
                        xml.AppendLine("  <Table>");
                        xml.AppendLine("   <Column ss:Width=\"150\"/><Column ss:Width=\"120\"/><Column ss:Width=\"80\"/><Column ss:Width=\"400\"/>");

                        // 3. TITLE
                        xml.AppendLine("   <Row ss:Height=\"30\"><Cell ss:StyleID=\"sTitle\"><Data ss:Type=\"String\">SYSTEM OVERRIDE LOGS REPORT</Data></Cell></Row>");
                        xml.AppendLine("   <Row><Cell><Data ss:Type=\"String\">Time Range: " + dtpFrom.Value.ToString("dd/MM/yyyy") + " - " + dtpTo.Value.ToString("dd/MM/yyyy") + "</Data></Cell></Row>");
                        xml.AppendLine("   <Row><Cell><Data ss:Type=\"String\">Exported At: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "</Data></Cell></Row>");
                        xml.AppendLine("   <Row ss:Index=\"5\"/>"); // Blank row

                        // 4. HEADERS
                        xml.AppendLine("   <Row ss:Height=\"20\">");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">Recorded Time</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">Staff/Account</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">Booking ID</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">Reason / Description</Data></Cell>");
                        xml.AppendLine("   </Row>");

                        // 5. DATA ROWS
                        int recordCount = 0;
                        foreach (DataGridViewRow row in dgvLogs.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string time = row.Cells[0].Value?.ToString() ?? "";
                            string staff = row.Cells[1].Value?.ToString() ?? "";
                            string id = row.Cells[2].Value?.ToString() ?? "";
                            string reason = row.Cells[3].Value?.ToString() ?? "";

                            xml.AppendLine("   <Row>");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"String\">" + EscapeXml(time) + "</Data></Cell>");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"String\">" + EscapeXml(staff) + "</Data></Cell>");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"String\">" + EscapeXml(id) + "</Data></Cell>");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"String\">" + EscapeXml(reason) + "</Data></Cell>");
                            xml.AppendLine("   </Row>");
                            recordCount++;
                        }

                        // 6. SUMMARY
                        xml.AppendLine("   <Row ss:Index=\"" + (xml.ToString().Split('\n').Length + 1) + "\"/>");
                        xml.AppendLine("   <Row><Cell><Data ss:Type=\"String\">Total Log Records:</Data></Cell><Cell><Data ss:Type=\"Number\">" + recordCount + "</Data></Cell></Row>");

                        xml.AppendLine("  </Table>");
                        xml.AppendLine(" </Worksheet>");
                        xml.AppendLine("</Workbook>");

                        File.WriteAllText(sfd.FileName, xml.ToString(), Encoding.UTF8);
                        MessageBox.Show("System logs exported to Excel successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string EscapeXml(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&apos;");
        }
    }
}