using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;




//Ashher
namespace WinFormsApp1
{
    public class PerformanceAnalyticsForm : Form
    {
        private DataGridView grid;

        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public PerformanceAnalyticsForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;

            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Performance Analytics";
            this.BackColor = ColorTranslator.FromHtml("#fddbc7");
            this.Size = new Size(1200, 647); // Adjusted width for the form
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

            Label titleLabel = new Label()
            {
                Text = "Performance Analytics",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            titleBar.Controls.Add(titleLabel);

            // Close Button
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

            // === Left Panel (Logo/Image) ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 647), // Increased size of left panel
                Location = new Point(0, 0),
                BackColor = Color.LightGray
            };

            PictureBox logo = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\DELL\\Documents\\db_images\\left_logo.jpg"),
                //  Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"), // Replace with your path
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };

            leftPanel.Controls.Add(logo);
            this.Controls.Add(leftPanel);

            // === Right Panel (Main Content) ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(700, 647), // Adjusted size for the right panel to fill the remaining space
                Location = new Point(500, 0),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            Label title = new Label()
            {
                Text = "Performance Analytics",
                Font = new Font("Garamond", 20, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006C83"),
                AutoSize = true,
                Location = new Point(20, y: 82), // Moved 50px down

            };
            rightPanel.Controls.Add(title);



            // === Data Grid View for Booking Rates ===
            grid = new DataGridView()
            {
                Location = new Point(20, 250),
                Size = new Size(660, 200),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ColorTranslator.FromHtml("#006C83"),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    BackColor = Color.White
                },
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };



            rightPanel.Controls.Add(grid);

            // === Export Button ===
            Button exportBtn = new Button()
            {
                Text = "Revenue Report",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#006C83"), // Teal color
                Size = new Size(140, 35),
                Location = new Point(20, 470) // Moved 50px down
            };
            // === Export Button ===
            Button exportBtn2 = new Button()
            {
                Text = "Operator Report",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#006C83"), // Teal color
                Size = new Size(140, 35),
                Location = new Point(150, 470) // Moved 50px down
            };

            // === Export Button ===
            Button exportBtn3 = new Button()
            {
                Text = "Abandonment Report",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#006C83"), // Teal color
                Size = new Size(140, 35),
                Location = new Point(280, 470) // Moved 50px down
            };




            rightPanel.Controls.Add(exportBtn);

            rightPanel.Controls.Add(exportBtn2);

            rightPanel.Controls.Add(exportBtn3);

            exportBtn.Click += (s, e) =>
            {
                grid.Columns.Clear();
                grid.Rows.Clear();
                grid.Columns.Add("Metric", "Metric");
                grid.Columns.Add("Value", "Value");

                //string connectionString = "Data Source=DESKTOP-JPIT9K6\\SQLEXPRESS01;Initial Catalog=TravelEase;Integrated Security=True;";
                string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Total Bookings
                        SqlCommand totalCmd = new SqlCommand("SELECT COUNT(*) FROM Bookings", conn);
                        int totalBookings = (int)totalCmd.ExecuteScalar();

                        // Revenue by Category
                        SqlCommand revenueCmd = new SqlCommand(@"
                SELECT T.Category, SUM(B.TotalAmount) AS Revenue
                FROM Bookings B
                JOIN Trips T ON B.TripID = T.TripID
                WHERE B.Status != 'Cancelled'
                GROUP BY T.Category", conn);

                        SqlDataReader reader = revenueCmd.ExecuteReader();
                        string revenueSummary = "";
                        while (reader.Read())
                        {
                            string category = reader["Category"].ToString();
                            decimal revenue = (decimal)reader["Revenue"];
                            revenueSummary += $"{category}: ${revenue:N0}, ";
                        }
                        reader.Close();
                        if (revenueSummary.EndsWith(", ")) revenueSummary = revenueSummary[..^2];

                        // Cancellation Rate
                        SqlCommand cancelCmd = new SqlCommand("SELECT COUNT(*) FROM Bookings WHERE Status = 'Cancelled'", conn);
                        int cancelled = (int)cancelCmd.ExecuteScalar();

                        SqlCommand allCmd = new SqlCommand("SELECT COUNT(*) FROM Bookings", conn);
                        int all = (int)allCmd.ExecuteScalar();

                        double cancelRate = all > 0 ? cancelled * 100.0 / all : 0;

                        // Peak Booking Periods
                        SqlCommand peakCmd = new SqlCommand(@"
                SELECT DATENAME(MONTH, BookingDate) AS MonthName, COUNT(*) AS Count
                FROM Bookings
                GROUP BY DATENAME(MONTH, BookingDate)
                ORDER BY Count DESC", conn);
                        reader = peakCmd.ExecuteReader();

                        List<string> topMonths = new List<string>();
                        int topCount = -1;
                        while (reader.Read())
                        {
                            int count = (int)reader["Count"];
                            string month = reader["MonthName"].ToString();
                            if (topCount == -1 || count == topCount || topMonths.Count < 3)
                            {
                                topMonths.Add(month);
                                topCount = count;
                            }
                        }
                        reader.Close();

                        // Average Booking Value
                        SqlCommand avgCmd = new SqlCommand("SELECT AVG(TotalAmount) FROM Bookings WHERE Status != 'Cancelled'", conn);
                        object avgResult = avgCmd.ExecuteScalar();
                        decimal avg = avgResult != DBNull.Value ? (decimal)avgResult : 0;

                        // Add Rows
                        grid.Rows.Add("Total Bookings", totalBookings.ToString("N0"));
                        grid.Rows.Add("Revenue by Trip Category", revenueSummary);
                        grid.Rows.Add("Cancellation Rate", $"{cancelRate:F1}%");
                        grid.Rows.Add("Peak Booking Periods", string.Join(", ", topMonths));
                        grid.Rows.Add("Average Booking Value", $"${avg:N2}");
                    }

                    MessageBox.Show("Data loaded into grid.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            rightPanel.Controls.Add(exportBtn);


            exportBtn2.Click += (s, e) =>
            {
                grid.Columns.Clear();
                grid.Rows.Clear();
                grid.Columns.Add("Metric", "Metric");
                grid.Columns.Add("Value", "Value");

                //string connectionString = "Data Source=DESKTOP-JPIT9K6\\SQLEXPRESS01;Initial Catalog=TravelEase;Integrated Security=True;";
                string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // a. Average Operator Rating
                        SqlCommand avgRatingCmd = new SqlCommand(@"
                SELECT t.OperatorID, AVG(r.Rating) AS AvgRating
                FROM Trips t
                JOIN Reviews r ON t.TripID = r.TripID
                GROUP BY t.OperatorID", conn);

                        SqlDataReader reader = avgRatingCmd.ExecuteReader();
                        string avgRatingResult = "";
                        while (reader.Read())
                        {
                            string opId = reader["OperatorID"].ToString();
                            string avg = Convert.ToDouble(reader["AvgRating"]).ToString("F2");
                            avgRatingResult += $"Operator {opId}: {avg}, ";
                        }
                        reader.Close();
                        if (avgRatingResult.EndsWith(", ")) avgRatingResult = avgRatingResult[..^2];
                        grid.Rows.Add("Average Operator Rating", avgRatingResult);

                        // b. Revenue per Operator
                        SqlCommand revenueCmd = new SqlCommand(@"
                SELECT toper.OperatorID, u.FullName AS OperatorName, SUM(b.TotalAmount) AS Revenue
                FROM Bookings b
                JOIN Trips t ON b.TripID = t.TripID
                JOIN TourOperators toper ON t.OperatorID = toper.OperatorID
                JOIN Users u ON toper.OperatorID = u.UserID
                GROUP BY toper.OperatorID, u.FullName", conn);

                        reader = revenueCmd.ExecuteReader();
                        string revenueResult = "";
                        while (reader.Read())
                        {
                            string name = reader["OperatorName"].ToString();
                            decimal revenue = (decimal)reader["Revenue"];
                            revenueResult += $"{name}: ${revenue:N0}, ";
                        }
                        reader.Close();
                        if (revenueResult.EndsWith(", ")) revenueResult = revenueResult[..^2];
                        grid.Rows.Add("Revenue per Operator", revenueResult);

                        // c. Response Time (Placeholder)
                        grid.Rows.Add("Average Response Time", "Data Not Available");

                        // d. Trips Managed per Operator
                        SqlCommand tripsCmd = new SqlCommand(@"
                SELECT OperatorID, COUNT(*) AS TripCount
                FROM Trips
                GROUP BY OperatorID", conn);

                        reader = tripsCmd.ExecuteReader();
                        string tripsResult = "";
                        while (reader.Read())
                        {
                            string opId = reader["OperatorID"].ToString();
                            int count = (int)reader["TripCount"];
                            tripsResult += $"Operator {opId}: {count}, ";
                        }
                        reader.Close();
                        if (tripsResult.EndsWith(", ")) tripsResult = tripsResult[..^2];
                        grid.Rows.Add("Trips Managed per Operator", tripsResult);

                        // e. Booking Conversion Rate
                        SqlCommand conversionCmd = new SqlCommand(@"
                SELECT t.OperatorID,
                       (CAST(COUNT(b.BookingID) AS FLOAT) / NULLIF(COUNT(DISTINCT t.TripID), 0)) * 100 AS ConversionRate
                FROM Trips t
                LEFT JOIN Bookings b ON t.TripID = b.TripID
                GROUP BY t.OperatorID", conn);

                        reader = conversionCmd.ExecuteReader();
                        string convResult = "";
                        while (reader.Read())
                        {
                            string opId = reader["OperatorID"].ToString();
                            double rate = reader["ConversionRate"] != DBNull.Value ? (double)reader["ConversionRate"] : 0;
                            convResult += $"Operator {opId}: {rate:F1}%, ";
                        }
                        reader.Close();
                        if (convResult.EndsWith(", ")) convResult = convResult[..^2];
                        grid.Rows.Add("Booking Conversion Rate", convResult);
                    }

                    MessageBox.Show("Tour Operator Report loaded.", "Export 2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };


            exportBtn3.Click += (s, e) =>
            {
                grid.Columns.Clear();
                grid.Rows.Clear();
                grid.Columns.Add("Metric", "Metric");
                grid.Columns.Add("Value", "Value");

                //string connectionString = "Data Source=DESKTOP-JPIT9K6\\SQLEXPRESS01;Initial Catalog=TravelEase;Integrated Security=True;";
                string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // a. Abandonment Rate
                        SqlCommand abandonRateCmd = new SqlCommand(@"
                SELECT 
                    (CAST(COUNT(*) AS FLOAT) / NULLIF((SELECT COUNT(*) FROM AbandonedBookings) + (SELECT COUNT(*) FROM Bookings), 0)) * 100 
                    AS AbandonRate 
                FROM AbandonedBookings", conn);
                        object abandonRateResult = abandonRateCmd.ExecuteScalar();
                        double abandonRate = abandonRateResult != DBNull.Value ? Convert.ToDouble(abandonRateResult) : 0;

                        // b. Common Reasons
                        SqlCommand reasonCmd = new SqlCommand(@"
                SELECT Reason, COUNT(*) AS Count 
                FROM AbandonedBookings 
                GROUP BY Reason", conn);
                        SqlDataReader reader = reasonCmd.ExecuteReader();
                        string reasonSummary = "";
                        while (reader.Read())
                        {
                            string reason = reader["Reason"].ToString();
                            int count = (int)reader["Count"];
                            reasonSummary += $"{reason}: {count}, ";
                        }
                        reader.Close();
                        if (reasonSummary.EndsWith(", ")) reasonSummary = reasonSummary[..^2];

                        // c. Recovery Rate
                        SqlCommand recoveryCmd = new SqlCommand(@"
                SELECT 
                    (CAST(COUNT(*) AS FLOAT) / NULLIF((SELECT COUNT(*) FROM AbandonedBookings), 0)) * 100 AS RecoveryRate
                FROM Bookings
                WHERE EXISTS (
                    SELECT 1 
                    FROM AbandonedBookings ab
                    WHERE ab.TravelerID = Bookings.TravelerID AND ab.TripID = Bookings.TripID
                )", conn);
                        object recoveryResult = recoveryCmd.ExecuteScalar();
                        double recoveryRate = recoveryResult != DBNull.Value ? Convert.ToDouble(recoveryResult) : 0;

                        // d. Potential Revenue Loss
                        SqlCommand lossCmd = new SqlCommand(@"
                SELECT SUM(t.PricePerPerson) AS LostRevenue
                FROM AbandonedBookings ab
                JOIN Trips t ON ab.TripID = t.TripID", conn);
                        object lossResult = lossCmd.ExecuteScalar();
                        decimal lostRevenue = lossResult != DBNull.Value ? (decimal)lossResult : 0;

                        // Add Rows
                        grid.Rows.Add("Abandonment Rate", $"{abandonRate:F1}%");
                        grid.Rows.Add("Common Abandon Reasons", reasonSummary);
                        grid.Rows.Add("Recovery Rate", $"{recoveryRate:F1}%");
                        grid.Rows.Add("Potential Revenue Loss", $"${lostRevenue:N2}");
                    }

                    MessageBox.Show("Abandoned Booking Analysis Report loaded into grid.", "Export 3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            rightPanel.Controls.Add(exportBtn3);


            // === Generate Report Button ===
            Button generateBtn = new Button()
            {
                Text = "Generate Report",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#6f42c1"), // Purple color
                Size = new Size(140, 35),
                Location = new Point(410, 470) // Moved 50px down
            };

            generateBtn.Click += (s, e) =>
            {
                if (grid.Rows.Count == 0)
                {
                    MessageBox.Show("Please load data first using Export.", "Generate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (.pdf)|.pdf",
                    Title = "Save Report",
                    FileName = "Booking_Report.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Using iTextSharp to create PDF
                        using (FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create))
                        {
                            var doc = new iTextSharp.text.Document();
                            iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
                            doc.Open();

                            // Fonts
                            var titleFont = iTextSharp.text.FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.BOLD);
                            var bodyFont = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL);

                            // Add Title
                            doc.Add(new iTextSharp.text.Paragraph("TravelEase Booking Report", titleFont));
                            doc.Add(new iTextSharp.text.Chunk("\n"));

                            // Add grid data
                            foreach (DataGridViewRow row in grid.Rows)
                            {
                                if (row.IsNewRow) continue;

                                string metric = row.Cells[0].Value?.ToString() ?? "";
                                string value = row.Cells[1].Value?.ToString() ?? "";

                                doc.Add(new iTextSharp.text.Paragraph($"{metric}: {value}", bodyFont));
                            }

                            doc.Add(new iTextSharp.text.Chunk("\n"));
                            doc.Add(new iTextSharp.text.Paragraph("Report generated on: " + DateTime.Now.ToString(), bodyFont));

                            doc.Close();
                        }

                        MessageBox.Show("PDF report saved successfully.", "Generate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to save PDF report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            rightPanel.Controls.Add(generateBtn);

            // === Generate Report Button ===
            Button visualizeBtn = new Button()
            {
                Text = "Visualize Report",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#6f42c1"), // Purple color
                Size = new Size(140, 35),
                Location = new Point(540, 470) // Moved 50px down
            };



            visualizeBtn.Click += (s, e) =>
            {
                try
                {
                    if (grid.Rows.Count == 0)
                    {
                        MessageBox.Show("Grid is empty. Please run a report first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Create chart
                    Chart chart = new Chart();
                    chart.Width = 800;
                    chart.Height = 600;

                    ChartArea chartArea = new ChartArea();
                    chart.ChartAreas.Add(chartArea);

                    Series series = new Series("Metrics");
                    series.ChartType = SeriesChartType.Column;
                    series.IsValueShownAsLabel = true;

                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                        {
                            string metric = row.Cells[0].Value.ToString();
                            string rawValue = row.Cells[1].Value.ToString();

                            // Extract numeric value from string
                            string numericPart = new string(rawValue.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());

                            if (double.TryParse(numericPart, out double value))
                            {
                                series.Points.AddXY(metric, value);
                            }
                        }
                    }

                    chart.Series.Add(series);

                    // Save as PNG in TravelEase_Analytics_Charts folder on Desktop
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string chartsFolder = Path.Combine(desktopPath, "TravelEase_Analytics_Charts");
                    if (!Directory.Exists(chartsFolder))
                    {
                        Directory.CreateDirectory(chartsFolder);
                    }
                    string filePath = Path.Combine(chartsFolder, "Chart.png");
                    chart.SaveImage(filePath, ChartImageFormat.Png);

                    MessageBox.Show($"Chart saved as PNG:\n{filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            rightPanel.Controls.Add(visualizeBtn);






            this.Controls.Add(rightPanel);
        }
    }
}
