using System;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Microsoft.Reporting.WinForms;
using System.Data.SqlClient;
namespace database_proj
{
    public partial class PlatformAnalyticsForm : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private DataGridView dgvAnalytics;
        private Button[] analyticsButtons;
        private Button printButton;
        private Button btnGenerateGraphs;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
        // Store last report data for graph generation
        private List<(string Title, DataTable Data, string ChartType)> lastReportData = new List<(string, DataTable, string)>();

        public PlatformAnalyticsForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildAnalyticsUI();
        }
        private void ShowDemographicsReport(DataTable dt)
        {
            reportViewer1.Reset();
            reportViewer1.LocalReport.ReportPath = "Report1.rdlc";
            reportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("Report1.rdlc", dt); // Must match name in RDLC
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.RefreshReport();
            reportViewer1.Visible = true;
        }
        private void BuildAnalyticsUI()
        {
            this.Controls.Clear();
            this.Text = "Platform Analytics";
            this.BackColor = ColorTranslator.FromHtml("#fddbc7");
            this.Size = new Size(1000, 647);
            this.StartPosition = FormStartPosition.CenterScreen;

            // === Custom Title Bar ===
            Panel titleBar = new Panel()
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = ColorTranslator.FromHtml("#006c83")
            };
            titleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            };
            this.Controls.Add(titleBar);

            Button closeButton = new Button()
            {
                Text = "Close",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 53, 69),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(70, 30),
                Location = new Point(this.Width - 80, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();
            titleBar.Controls.Add(closeButton);

            // === Left Panel with 3 Buttons ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(250, 500),
                Location = new Point(30, 80),
                BackColor = Color.Transparent
            };
            this.Controls.Add(leftPanel);

            Color yellow = Color.FromArgb(255, 193, 7);
            Button btnDemographics = new Button()
            {
                Size = new Size(220, 40),
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = yellow,
                ForeColor = Color.Black,
                Text = "Traveler Demographics and Preferences Report"
            };
            leftPanel.Controls.Add(btnDemographics);
            btnDemographics.Click += BtnDemographics_Click;

            Button btnGrowth = new Button()
            {
                Size = new Size(220, 40),
                Location = new Point(10, 70),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = yellow,
                ForeColor = Color.Black,
                Text = "Platform Growth Report"
            };
            leftPanel.Controls.Add(btnGrowth);
            btnGrowth.Click += BtnGrowth_Click;

            Button btnPayments = new Button()
            {
                Size = new Size(220, 40),
                Location = new Point(10, 130),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = yellow,
                ForeColor = Color.Black,
                Text = "Payment Transaction"
            };
            leftPanel.Controls.Add(btnPayments);
            btnPayments.Click += BtnPayments_Click;



            // === Print Button ===
            printButton = new Button()
            {
                Text = "Print",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Size = new Size(120, 40),
                Location = new Point(830, 500),
                FlatStyle = FlatStyle.Flat
            };
            printButton.FlatAppearance.BorderSize = 0;
            printButton.Click += PrintButton_Click;
            this.Controls.Add(printButton);

            // === Generate Graphs Button ===
            btnGenerateGraphs = new Button()
            {
                Text = "Generate Graphs",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Size = new Size(160, 40),
                Location = new Point(650, 500),
                FlatStyle = FlatStyle.Flat
            };
            btnGenerateGraphs.FlatAppearance.BorderSize = 0;
            btnGenerateGraphs.Click += BtnGenerateGraphs_Click;
            reportViewer1.Dock = DockStyle.Fill;
            reportViewer1.Visible = false;
            this.Controls.Add(reportViewer1);
            this.Controls.Add(btnGenerateGraphs);
        }

        private void BtnDemographics_Click(object sender, EventArgs e)
        {
            string connStr = "Data Source=LAIQ-VICTUS\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = @"SELECT DATEDIFF(YEAR, u.DateOfBirth, GETDATE()) AS Label, COUNT(*) AS Value
                         FROM Travelers t JOIN Users u ON t.TravelerID = u.UserID
                         GROUP BY DATEDIFF(YEAR, u.DateOfBirth, GETDATE())";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                ShowDemographicsReport(dt);
            }
        }


        private void BtnGrowth_Click(object sender, EventArgs e)
        {
            lastReportData.Clear();
            string connectionString = "Data Source=LAIQ-VICTUS\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string newUserQuery = @"SELECT YEAR(CreatedAt) AS Year, MONTH(CreatedAt) AS Month, COUNT(*) AS NewUsers FROM Users GROUP BY YEAR(CreatedAt), MONTH(CreatedAt) ORDER BY Year, Month";
                string activeUserQuery = @"SELECT YEAR(Timestamp) AS Year, MONTH(Timestamp) AS Month, COUNT(DISTINCT UserID) AS ActiveUsers FROM AuditTrail GROUP BY YEAR(Timestamp), MONTH(Timestamp) ORDER BY Year, Month";
                string providerGrowthQuery = @"SELECT YEAR(CreatedAt) AS Year, MONTH(CreatedAt) AS Month, COUNT(*) AS NewProviders FROM Users WHERE Role = 'service provider' GROUP BY YEAR(CreatedAt), MONTH(CreatedAt) ORDER BY Year, Month";
                string regionalQuery = @"SELECT DestinationName AS Region, MIN(StartDate) AS FirstTrip FROM Trips GROUP BY DestinationName ORDER BY FirstTrip";
                string yoyQuery = @"SELECT YEAR(CreatedAt) AS Year, COUNT(*) AS NewUsers FROM Users GROUP BY YEAR(CreatedAt) ORDER BY Year";

                var dt = new DataTable();
                using (var cmd = new SqlCommand(newUserQuery, conn))
                using (var adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(dt);
                ShowReport("GrowthReport", "Report1.rdlc", dt);
                lastReportData.Add(("Monthly New User Registrations", dt, "Line"));
            }
        }

        private void BtnPayments_Click(object sender, EventArgs e)
        {
            lastReportData.Clear();
            string connectionString = "Data Source=LAIQ-VICTUS\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT PaymentMethod, COUNT(*) AS Count FROM Payments GROUP BY PaymentMethod";

                var dt = new DataTable();
                using (var cmd = new SqlCommand(query, conn))
                using (var adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(dt);
                ShowReport("PaymentsReport", "Report1.rdlc", dt);
                lastReportData.Add(("Payment Methods Used", dt, "Pie"));
            }
        }
        private void ShowReport(string reportName, string rdlcFile, DataTable dt)
        {
            reportViewer1.Reset();
            reportViewer1.LocalReport.ReportPath = rdlcFile;
            reportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource(reportName, dt);
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.RefreshReport();
            reportViewer1.Visible = true;
        }


        private void AddSectionToGrid(string sectionTitle, DataTable dt)
        {
            // Add the data columns if not already present
            foreach (DataColumn col in dt.Columns)
            {
                if (!dgvAnalytics.Columns.Contains(col.ColumnName))
                    dgvAnalytics.Columns.Add(col.ColumnName, col.ColumnName);
            }

            // Add a section header row (after columns exist)
            int rowIndex = dgvAnalytics.Rows.Add();
            dgvAnalytics.Rows[rowIndex].DefaultCellStyle.Font = new System.Drawing.Font(dgvAnalytics.Font, System.Drawing.FontStyle.Bold);
            dgvAnalytics.Rows[rowIndex].Cells[0].Value = sectionTitle;

            // Add the data
            foreach (DataRow dr in dt.Rows)
            {
                object[] values = new object[dgvAnalytics.Columns.Count];
                for (int i = 0; i < dgvAnalytics.Columns.Count; i++)
                {
                    if (dt.Columns.Contains(dgvAnalytics.Columns[i].Name))
                        values[i] = dr[dgvAnalytics.Columns[i].Name];
                    else
                        values[i] = null;
                }
                dgvAnalytics.Rows.Add(values);
            }
            // Add a blank row for spacing
            dgvAnalytics.Rows.Add();
        }

        private void BtnGenerateGraphs_Click(object sender, EventArgs e)
        {
            if (lastReportData.Count == 0)
            {
                MessageBox.Show("Please load a report first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "TravelEase_Analytics_Charts");
            Directory.CreateDirectory(folderPath);
            foreach (var (Title, Data, ChartType) in lastReportData)
            {
                if (Data.Columns.Count < 2 || Data.Rows.Count == 0) continue;
                Chart chart = new Chart();
                chart.Size = new Size(600, 400);
                ChartArea area = new ChartArea();
                chart.ChartAreas.Add(area);
                Series series = new Series
                {
                    ChartType = ChartType == "Pie" ? SeriesChartType.Pie :
                                ChartType == "Line" ? SeriesChartType.Line :
                                SeriesChartType.Column
                };
                chart.Series.Add(series);
                // X and Y
                string xCol = Data.Columns[0].ColumnName;
                string yCol = Data.Columns[1].ColumnName;
                foreach (DataRow row in Data.Rows)
                {
                    series.Points.AddXY(row[xCol], row[yCol]);
                }
                chart.Titles.Add(Title);
                string fileName = Path.Combine(folderPath, Title.Replace(" ", "_") + ".png");
                chart.SaveImage(fileName, ChartImageFormat.Png);
            }
            GeneratePdfReport(folderPath);
        }

        private void GeneratePdfReport(string folderPath)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF Files (.pdf)|.pdf";
                sfd.Title = "Save Analytics PDF Report";
                sfd.FileName = "Analytics_Report.pdf";
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                string pdfPath = sfd.FileName;

                PdfDocument document = new PdfDocument();
                document.Info.Title = "TravelEase Analytics Report";
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                int y = 40;

                PdfSharp.Drawing.XFont titleFont = new PdfSharp.Drawing.XFont("Arial", 18);
                PdfSharp.Drawing.XFont headerFont = new PdfSharp.Drawing.XFont("Arial", 12);
                PdfSharp.Drawing.XFont textFont = new PdfSharp.Drawing.XFont("Arial", 10);

                gfx.DrawString("TravelEase Analytics Report", titleFont, XBrushes.Black, new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
                y += 40;

                foreach (var (Title, Data, ChartType) in lastReportData)
                {
                    // Section header
                    gfx.DrawString(Title, headerFont, XBrushes.DarkBlue, 40, y);
                    y += 25;

                    // Table headers
                    double x = 40;
                    double colSpacing = 120; // Adjust as needed for your data
                    foreach (DataColumn col in Data.Columns)
                    {
                        gfx.DrawString(col.ColumnName, textFont, XBrushes.Black, x, y);
                        x += colSpacing;
                    }
                    y += 18;

                    // Table rows
                    foreach (DataRow row in Data.Rows)
                    {
                        x = 40;
                        foreach (DataColumn col in Data.Columns)
                        {
                            string value = row[col]?.ToString() ?? "";
                            gfx.DrawString(value, textFont, XBrushes.Black, x, y);
                            x += colSpacing;
                        }
                        y += 16;
                        if (y > page.Height - 60)
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            y = 40;
                        }
                    }
                    y += 20; // Extra space between sections

                    // Add new page if needed
                    if (y > page.Height - 100)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }

                gfx.DrawString("Report generated on: " + DateTime.Now, textFont, XBrushes.Gray, 40, y + 20);
                document.Save(pdfPath);
                MessageBox.Show($"PDF report saved to {pdfPath}", "PDF Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            byte[] bytes = reportViewer1.LocalReport.Render(
                "PDF", null, out mimeType, out encoding,
                out extension, out streamIds, out warnings);

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "PDF File|*.pdf";
            save.Title = "Save report as PDF";
            save.FileName = "Analytics_Report.pdf";
            if (save.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(save.FileName, bytes);
                MessageBox.Show("PDF Exported Successfully!", "Done");
            }
        }
        private void PrintButton_Click(object sender, EventArgs e)
        {
            if (lastReportData.Count == 0)
            {
                MessageBox.Show("Please load a report first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "TravelEase_Analytics_Charts");
            Directory.CreateDirectory(folderPath);
            GeneratePdfReport(folderPath);
        }
    }
}