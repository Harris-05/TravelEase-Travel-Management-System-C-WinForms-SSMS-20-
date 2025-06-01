using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using System.Runtime.InteropServices;
using iTextSharp.text;
using iTextSharp.text.pdf;

public partial class PerformanceReportsForm : Form
{
    // Drag functionality
    [DllImport("user32.dll")] public static extern bool ReleaseCapture();
    [DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 0x2;

    private string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
    private DataTable currentReportData;

    public PerformanceReportsForm()
    {

        CustomizeUI();
    }

    private void CustomizeUI()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(1000, 650);
        this.BackColor = Color.White;

        // === Title Bar ===
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
            Font = new System.Drawing.Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(15, 10),
            AutoSize = true
        };
        titleBar.Controls.Add(titleLabel);

        Button closeButton = new Button()
        {
            Text = "X",
            Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(220, 53, 69),
            FlatStyle = FlatStyle.Flat,
            Size = new Size(40, 30),
            Location = new Point(this.Width - 50, 10),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        closeButton.FlatAppearance.BorderSize = 0;
        closeButton.Click += (s, e) => this.Close();
        titleBar.Controls.Add(closeButton);

        // === Main Controls ===
        System.Drawing.Font commonFont = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
        Color primaryColor = ColorTranslator.FromHtml("#006c83");

        Button btnSection4 = new Button()
        {
            Text = "📊 Service Provider Efficiency Report",
            Font = commonFont,
            BackColor = primaryColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(50, 70),
            Size = new Size(200, 40)
        };
        btnSection4.FlatAppearance.BorderSize = 0;
        btnSection4.Click += btnSection4_Click;
        this.Controls.Add(btnSection4);

        Button btnSection5 = new Button()
        {
            Text = "📍 Destination Popularity Report",
            Font = commonFont,
            BackColor = primaryColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(280, 70),
            Size = new Size(200, 40)
        };
        btnSection5.FlatAppearance.BorderSize = 0;
        btnSection5.Click += btnSection5_Click;
        this.Controls.Add(btnSection5);

        Button btnGenerateCharts = new Button()
        {
            Text = "📈 Generate Charts",
            Font = commonFont,
            BackColor = primaryColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(510, 70),
            Size = new Size(200, 40)
        };
        btnGenerateCharts.FlatAppearance.BorderSize = 0;
        btnGenerateCharts.Click += btnGenerateCharts_Click;
        this.Controls.Add(btnGenerateCharts);

        Button btnExportPDF = new Button()
        {
            Text = "📄 Export to PDF",
            Font = commonFont,
            BackColor = primaryColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(740, 70),
            Size = new Size(200, 40)
        };
        btnExportPDF.FlatAppearance.BorderSize = 0;
        btnExportPDF.Click += btnExportPDF_Click;
        this.Controls.Add(btnExportPDF);

        // === DataGridView ===
        dataGridView1 = new DataGridView()
        {
            Location = new Point(50, 130),
            Size = new Size(900, 450),
            ReadOnly = true,
            BackgroundColor = Color.White,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Font = new System.Drawing.Font("Segoe UI", 9),
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
            {
                Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = ColorTranslator.FromHtml("#006c83"),
                ForeColor = Color.White
            },
            EnableHeadersVisualStyles = false
        };

        this.Controls.Add(dataGridView1);
    }

    private DataGridView dataGridView1;
    private void btnSection4_Click(object sender, EventArgs e)
    {
        string[] queries = new string[]
        {
                // a. Hotel Occupancy Rate
                "SELECT ProviderID, ServiceType, SUM(CASE WHEN Availability = 0 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS ApproxOccupancyRate FROM ServicesProvided WHERE ServiceType = 'Hotel' GROUP BY ProviderID, ServiceType",

                // b. Guide Ratings
                "SELECT sp.ProviderID, sp.OrganizationName, AVG(r.Rating) AS AvgRating FROM Reviews r JOIN ServiceProviders sp ON r.ProviderID = sp.ProviderID WHERE sp.ProviderType = 'Guide' GROUP BY sp.ProviderID, sp.OrganizationName",

               

                // d. Service Utilization Rate
                "SELECT sp.ServiceType, SUM(UsedCount) * 100.0 / NULLIF(SUM(UsedCount + sp.Availability), 0) AS UtilizationRate FROM (SELECT ServiceID, COUNT(*) AS UsedCount FROM TripServices GROUP BY ServiceID) AS usage JOIN ServicesProvided sp ON usage.ServiceID = sp.ServiceID GROUP BY sp.ServiceType",

                // e. Services Delivered per Provider
                "SELECT sp.ProviderID, p.OrganizationName, COUNT(ts.TripServiceID) AS ServicesDelivered FROM TripServices ts JOIN ServicesProvided sp ON ts.ServiceID = sp.ServiceID JOIN ServiceProviders p ON sp.ProviderID = p.ProviderID GROUP BY sp.ProviderID, p.OrganizationName"
        };
        RunReportQueries(queries);
    }

    private void btnSection5_Click(object sender, EventArgs e)
    {
        string[] queries = new string[]
        {
                // a. Most Booked Destinations
                "SELECT td.DestinationName, COUNT(*) AS Bookings FROM Bookings b JOIN Trips t ON b.TripID = t.TripID JOIN TripDestinations td ON t.TripID = td.TripID GROUP BY td.DestinationName ORDER BY Bookings DESC",

                // b. Destination-wise Hotel Availability
                "SELECT td.DestinationName, AVG(CAST(sp.Availability AS FLOAT)) AS AvgHotelAvailability FROM Trips t JOIN TripDestinations td ON t.TripID = td.TripID JOIN TripServices ts ON t.TripID = ts.TripID JOIN ServicesProvided sp ON ts.ServiceID = sp.ServiceID WHERE sp.ServiceType = 'Hotel' GROUP BY td.DestinationName",

                // c. Destination-wise Guide Ratings
                "SELECT td.DestinationName, AVG(r.Rating) AS AvgGuideRating FROM Trips t JOIN TripDestinations td ON t.TripID = td.TripID JOIN Reviews r ON t.TripID = r.TripID JOIN ServiceProviders sp ON r.ProviderID = sp.ProviderID WHERE sp.ProviderType = 'Guide' GROUP BY td.DestinationName",

                // d. Destination Transport Performance
                "SELECT td.DestinationName, AVG(CASE WHEN ts.Status = 'CompletedOnTime' THEN 1.0 ELSE 0.0 END) * 100 AS OnTimeRate FROM Trips t JOIN TripDestinations td ON t.TripID = td.TripID JOIN TripServices ts ON t.TripID = ts.TripID JOIN ServicesProvided sp ON ts.ServiceID = sp.ServiceID WHERE sp.ServiceType = 'Transport' GROUP BY td.DestinationName",

                // e. Seasonal Trends
                "SELECT td.DestinationName, DATEPART(MONTH, b.BookingDate) AS Month, COUNT(*) AS Bookings FROM Bookings b JOIN Trips t ON b.TripID = t.TripID JOIN TripDestinations td ON t.TripID = td.TripID GROUP BY td.DestinationName, DATEPART(MONTH, b.BookingDate)"
        };
        RunReportQueries(queries);
    }

    private void RunReportQueries(string[] queries)
    {
        currentReportData = new DataTable();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            foreach (var query in queries)
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable temp = new DataTable();
                    adapter.Fill(temp);
                    currentReportData.Merge(temp);
                }
            }
        }

        dataGridView1.DataSource = currentReportData;
    }

    private void btnGenerateCharts_Click(object sender, EventArgs e)
    {
        string chartFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TravelEase_Analytics_Charts");
        Directory.CreateDirectory(chartFolder);

        int chartIndex = 1;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            foreach (string query in currentReportData.Rows.Count > 0 ? currentReportData.TableName.Split(';') : new string[0])
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable chartData = new DataTable();
                    adapter.Fill(chartData);

                    if (chartData.Columns.Count < 2) continue;

                    Chart chart = new Chart();
                    chart.Size = new Size(600, 400);
                    ChartArea area = new ChartArea();
                    chart.ChartAreas.Add(area);
                    Series series = new Series
                    {
                        ChartType = SeriesChartType.Column
                    };

                    foreach (DataRow row in chartData.Rows)
                    {
                        string xVal = row[0].ToString();
                        double yVal = Convert.ToDouble(row[1]);
                        series.Points.AddXY(xVal, yVal);
                    }

                    chart.Series.Add(series);
                    string chartPath = Path.Combine(chartFolder, $"Chart_{chartIndex++}.png");
                    chart.SaveImage(chartPath, ChartImageFormat.Png);
                }
            }
        }

        MessageBox.Show("Charts saved successfully to Desktop/TravelEase_Analytics_Charts.");
    }

    private void btnExportPDF_Click(object sender, EventArgs e)
    {
        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {
            saveFileDialog.Filter = "PDF files (.pdf)|.pdf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document();
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    PdfPTable pdfTable = new PdfPTable(currentReportData.Columns.Count);
                    foreach (DataColumn column in currentReportData.Columns)
                        pdfTable.AddCell(new Phrase(column.ColumnName));

                    foreach (DataRow row in currentReportData.Rows)
                        foreach (var cell in row.ItemArray)
                            pdfTable.AddCell(new Phrase(cell.ToString()));

                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();
                }
                MessageBox.Show("PDF exported successfully.");
            }
        }
    }

}