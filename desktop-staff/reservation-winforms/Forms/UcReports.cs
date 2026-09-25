using reservation_winforms.DTO.report;
using reservation_winforms.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace reservation_winforms.Forms
{
    public partial class UcReports : UserControl
    {
        private readonly ReportService _reportService;

        private List<DailyReservationReport> _currentReportData;
        private NoShowRateResponse _currentRateData;

        public UcReports()
        {
            InitializeComponent();
            _reportService = new ReportService();

            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            dtpFromMonth.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpToMonth.Value = DateTime.Now;

            numFromYear.Value = DateTime.Now.Year - 3;
            numToYear.Value = DateTime.Now.Year;

            chartReservations.Series.Clear();
            dtpFromDate.ValueChanged += DtpFromDate_ValueChanged;
            dtpFromMonth.ValueChanged += DtpFromMonth_ValueChanged;
            numFromYear.ValueChanged += NumFromYear_ValueChanged;

            DtpFromDate_ValueChanged(null, null);
            DtpFromMonth_ValueChanged(null, null);
            NumFromYear_ValueChanged(null, null);

            Series sWalkIn = new Series("Walk-in Reservations");
            sWalkIn.ChartType = SeriesChartType.StackedColumn;
            sWalkIn.Color = Color.FromArgb(39, 174, 96);
            sWalkIn.IsValueShownAsLabel = true;

            Series sOnline = new Series("Online Reservations (Success)");
            sOnline.ChartType = SeriesChartType.StackedColumn;
            sOnline.Color = Color.FromArgb(41, 128, 185);
            sOnline.IsValueShownAsLabel = true;

            Series sNoShow = new Series("No-Show Guests");
            sNoShow.ChartType = SeriesChartType.StackedColumn;
            sNoShow.Color = Color.FromArgb(243, 156, 18);
            sNoShow.IsValueShownAsLabel = true;

            chartReservations.Series.Add(sWalkIn);
            chartReservations.Series.Add(sOnline);
            chartReservations.Series.Add(sNoShow);

            chartReservations.Series[0].Points.Clear();
            chartReservations.ChartAreas[0].AxisX.Title = "Time";
            chartReservations.ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
            chartReservations.ChartAreas[0].AxisY.Title = "Number of Reservations";
            chartReservations.ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);

            btnFilter.Click += BtnFilter_Click;
            tabFilter.SizeMode = TabSizeMode.Fixed;
            tabFilter.ItemSize = new Size((tabFilter.Width / tabFilter.TabCount) - 2, 35);

            if (btnExportExcel != null)
            {
                btnExportExcel.Click += BtnExportExcel_Click;
            }

            this.Load += async (s, e) => await ExecuteFilterAsync();
        }

        private void DtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            DateTime from = dtpFromDate.Value;
            dtpToDate.MinDate = new DateTime(1753, 1, 1);
            dtpToDate.MaxDate = new DateTime(9998, 12, 31);
            dtpToDate.MinDate = from;
            dtpToDate.MaxDate = new DateTime(from.Year, from.Month, DateTime.DaysInMonth(from.Year, from.Month));
        }

        private void DtpFromMonth_ValueChanged(object sender, EventArgs e)
        {
            DateTime from = dtpFromMonth.Value;
            dtpToMonth.MinDate = new DateTime(1753, 1, 1);
            dtpToMonth.MaxDate = new DateTime(9998, 12, 31);
            dtpToMonth.MinDate = from;
            dtpToMonth.MaxDate = new DateTime(from.Year, 12, 31);
        }

        private void NumFromYear_ValueChanged(object sender, EventArgs e)
        {
            numToYear.Minimum = numFromYear.Value;
        }

        private async void BtnFilter_Click(object sender, EventArgs e)
        {
            await ExecuteFilterAsync();
        }

        private async System.Threading.Tasks.Task ExecuteFilterAsync()
        {
            try
            {
                btnFilter.Enabled = false;
                btnFilter.Text = "LOADING...";

                _currentReportData = null;
                _currentRateData = null;

                (bool IsSuccess, List<DailyReservationReport> Data, string Message) chartRes = (false, null, "");
                (bool IsSuccess, NoShowRateResponse Data, string Message) rateRes = (false, null, "");

                string xLabelFormat = "dd/MM";

                if (tabFilter.SelectedTab == tabPageDate)
                {
                    string fromStr = dtpFromDate.Value.ToString("yyyy-MM-dd");
                    string toStr = dtpToDate.Value.ToString("yyyy-MM-dd");

                    chartRes = await _reportService.GetReservationsByDateAsync(fromStr, toStr);
                    rateRes = await _reportService.GetNoShowRateAsync(fromStr, toStr);
                    xLabelFormat = "dd/MM";
                }
                else if (tabFilter.SelectedTab == tabPageMonth)
                {
                    string fromStr = dtpFromMonth.Value.ToString("yyyy-MM");
                    string toStr = dtpToMonth.Value.ToString("yyyy-MM");

                    chartRes = await _reportService.GetReservationsByMonthAsync(fromStr, toStr);
                    rateRes = await _reportService.GetNoShowRateByMonthAsync(fromStr, toStr);
                    xLabelFormat = "MM/yyyy";
                }
                else if (tabFilter.SelectedTab == tabPageYear)
                {
                    string fromStr = numFromYear.Value.ToString();
                    string toStr = numToYear.Value.ToString();

                    chartRes = await _reportService.GetReservationsByYearAsync(fromStr, toStr);
                    rateRes = await _reportService.GetNoShowRateByYearAsync(fromStr, toStr);
                    xLabelFormat = "yyyy";
                }

                if (rateRes.IsSuccess && rateRes.Data != null)
                {
                    _currentRateData = rateRes.Data;
                    lblTotalAll.Text = rateRes.Data.TotalAll.ToString();
                    lblTotalOnline.Text = rateRes.Data.TotalOnline.ToString();
                    lblTotalWalkIn.Text = rateRes.Data.TotalWalkIn.ToString();
                    lblNoShow.Text = rateRes.Data.NoShowCount.ToString();
                    lblRate.Text = $"{rateRes.Data.NoShowRate}%";

                    lblRate.ForeColor = rateRes.Data.NoShowRate > 15 ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113);
                }
                else MessageBox.Show(rateRes.Message, "Error fetching No-Show Rate", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (chartRes.IsSuccess && chartRes.Data != null)
                {
                    _currentReportData = chartRes.Data;
                    chartReservations.Series["Walk-in Reservations"].Points.Clear();
                    chartReservations.Series["Online Reservations (Success)"].Points.Clear();
                    chartReservations.Series["No-Show Guests"].Points.Clear();

                    foreach (var item in chartRes.Data)
                    {
                        string displayLabel = item.Date;
                        if (item.Date.Length == 10) displayLabel = DateTime.Parse(item.Date).ToString(xLabelFormat);
                        else if (item.Date.Length == 7) displayLabel = DateTime.Parse(item.Date + "-01").ToString(xLabelFormat);

                        long walkIn = item.TotalWalkIn;
                        long noShow = item.NoShowCount;
                        long onlineSuccess = item.TotalOnline - noShow;

                        chartReservations.Series["Walk-in Reservations"].Points.AddXY(displayLabel, walkIn);
                        chartReservations.Series["Online Reservations (Success)"].Points.AddXY(displayLabel, onlineSuccess);
                        chartReservations.Series["No-Show Guests"].Points.AddXY(displayLabel, noShow);

                        int lastIdx = chartReservations.Series["Walk-in Reservations"].Points.Count - 1;
                        chartReservations.Series["Walk-in Reservations"].Points[lastIdx].IsValueShownAsLabel = (walkIn > 0);
                        chartReservations.Series["Online Reservations (Success)"].Points[lastIdx].IsValueShownAsLabel = (onlineSuccess > 0);
                        chartReservations.Series["No-Show Guests"].Points[lastIdx].IsValueShownAsLabel = (noShow > 0);
                    }
                }
                else MessageBox.Show(chartRes.Message, "Error rendering chart", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFilter.Enabled = true;
                btnFilter.Text = "FILTER DATA";
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (_currentReportData == null || _currentReportData.Count == 0 || _currentRateData == null)
            {
                MessageBox.Show("There is no report data to export!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string defaultFileName = $"reports_{DateTime.Now:yyyyMMdd_HHmm}.xls";
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
                        xml.AppendLine("  <Style ss:ID=\"sTitle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"18\" ss:Bold=\"1\" ss:Color=\"#2C3E50\"/></Style>");
                        
                        // Card Header Styles (Matching UI Colors)
                        xml.AppendLine("  <Style ss:ID=\"cTotalHead\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#34495E\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"cOnlineHead\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#2980B9\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"cWalkInHead\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#27AE60\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"cNoShowHead\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#F39C12\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"cRateHead\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#E74C3C\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");

                        // Card Value Styles (Matching UI Colors)
                        xml.AppendLine("  <Style ss:ID=\"cTotalVal\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"18\" ss:Bold=\"1\" ss:Color=\"#34495E\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"cOnlineVal\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"18\" ss:Bold=\"1\" ss:Color=\"#2980B9\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"cWalkInVal\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"18\" ss:Bold=\"1\" ss:Color=\"#27AE60\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"cNoShowVal\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"18\" ss:Bold=\"1\" ss:Color=\"#F39C12\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"cRateVal\"><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/><Font ss:FontName=\"Segoe UI\" ss:Size=\"18\" ss:Bold=\"1\" ss:Color=\"#E74C3C\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");

                        xml.AppendLine("  <Style ss:ID=\"sHeader\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"11\" ss:Bold=\"1\" ss:Color=\"#FFFFFF\"/><Interior ss:Color=\"#2980B9\" ss:Pattern=\"Solid\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine("  <Style ss:ID=\"sData\"><Alignment ss:Horizontal=\"Center\"/><Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/><Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/></Borders></Style>");
                        xml.AppendLine(" </Styles>");

                        // 2. WORKSHEET
                        xml.AppendLine(" <Worksheet ss:Name=\"Reservations Report\">");
                        xml.AppendLine("  <Table ss:ExpandedColumnCount=\"10\">");
                        xml.AppendLine("   <Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/>");

                        // 3. TITLE
                        string reportTitle = "RESERVATIONS STATISTICS REPORT";
                        if (tabFilter.SelectedTab == tabPageDate) reportTitle += $" ({dtpFromDate.Value:dd/MM/yyyy} - {dtpToDate.Value:dd/MM/yyyy})";
                        else if (tabFilter.SelectedTab == tabPageMonth) reportTitle += $" ({dtpFromMonth.Value:MM/yyyy} - {dtpToMonth.Value:MM/yyyy})";
                        else if (tabFilter.SelectedTab == tabPageYear) reportTitle += $" ({numFromYear.Value} - {numToYear.Value})";

                        xml.AppendLine("   <Row ss:Height=\"35\"><Cell ss:MergeAcross=\"4\" ss:StyleID=\"sTitle\"><Data ss:Type=\"String\">" + EscapeXml(reportTitle) + "</Data></Cell></Row>");
                        xml.AppendLine("   <Row><Cell ss:MergeAcross=\"4\"><Data ss:Type=\"String\">Exported At: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "</Data></Cell></Row>");
                        xml.AppendLine("   <Row ss:Index=\"4\"/>"); // Blank row

                        // 4. SUMMARY CARDS (Top Section)
                        xml.AppendLine("   <Row ss:Height=\"20\">");
                        xml.AppendLine("    <Cell ss:StyleID=\"cTotalHead\"><Data ss:Type=\"String\">TOTAL RESERVED</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"cOnlineHead\"><Data ss:Type=\"String\">ONLINE BOOKINGS</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"cWalkInHead\"><Data ss:Type=\"String\">WALK-IN GUESTS</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"cNoShowHead\"><Data ss:Type=\"String\">NO-SHOW GUESTS</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"cRateHead\"><Data ss:Type=\"String\">NO-SHOW RATE</Data></Cell>");
                        xml.AppendLine("   </Row>");
                        xml.AppendLine("   <Row ss:Height=\"45\">");
                        xml.AppendLine("    <Cell ss:StyleID=\"cTotalVal\"><Data ss:Type=\"Number\">" + _currentRateData.TotalAll + "</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"cOnlineVal\"><Data ss:Type=\"Number\">" + _currentRateData.TotalOnline + "</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"cWalkInVal\"><Data ss:Type=\"Number\">" + _currentRateData.TotalWalkIn + "</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"cNoShowVal\"><Data ss:Type=\"Number\">" + _currentRateData.NoShowCount + "</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"cRateVal\"><Data ss:Type=\"String\">" + _currentRateData.NoShowRate + "%</Data></Cell>");
                        xml.AppendLine("   </Row>");
                        xml.AppendLine("   <Row ss:Index=\"7\"/>"); // Blank row

                        // 5. DETAIL TABLE HEADERS
                        xml.AppendLine("   <Row ss:Height=\"22\">");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">Time</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">Total</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">Walk-in</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">Online</Data></Cell>");
                        xml.AppendLine("    <Cell ss:StyleID=\"sHeader\"><Data ss:Type=\"String\">No-Show</Data></Cell>");
                        xml.AppendLine("   </Row>");

                        // 6. DATA ROWS
                        foreach (var item in _currentReportData)
                        {
                            long walkIn = item.TotalWalkIn;
                            long noShow = item.NoShowCount;
                            long online = item.TotalOnline;
                            long total = walkIn + online;

                            string timeLabel = item.Date;
                            if (item.Date.Length == 10) timeLabel = DateTime.Parse(item.Date).ToString("dd/MM/yyyy");
                            else if (item.Date.Length == 7) timeLabel = DateTime.Parse(item.Date + "-01").ToString("MM/yyyy");

                            xml.AppendLine("   <Row ss:Height=\"18\">");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"String\">" + EscapeXml(timeLabel) + "</Data></Cell>");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"Number\">" + total + "</Data></Cell>");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"Number\">" + walkIn + "</Data></Cell>");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"Number\">" + online + "</Data></Cell>");
                            xml.AppendLine("    <Cell ss:StyleID=\"sData\"><Data ss:Type=\"Number\">" + noShow + "</Data></Cell>");
                            xml.AppendLine("   </Row>");
                        }

                        xml.AppendLine("  </Table>");
                        xml.AppendLine(" </Worksheet>");
                        xml.AppendLine("</Workbook>");

                        File.WriteAllText(sfd.FileName, xml.ToString(), Encoding.UTF8);
                        MessageBox.Show("Professional report with overview cards exported successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
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