using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using database_proj;


//Ashher
namespace WinFormsApp1
{
    public class TripsListingForm : Form
    {
        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public TripsListingForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Trips Listing";
            this.BackColor = Color.White;
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

            Label titleLabel = new Label()
            {
                Text = "Trips Listing",
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

            // === Left Panel (Logo) ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 647),
                Location = new Point(0, 0),
                BackColor = Color.LightGray
            };

            PictureBox logo = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\DELL\\Documents\\db_images\\left_logo.jpg"),
                // Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                 Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"), // change path later
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            leftPanel.Controls.Add(logo);
            this.Controls.Add(leftPanel);

            // === Right Panel ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(500, 647),
                Location = new Point(500, 0),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };
            this.Controls.Add(rightPanel);

            // === Title ===
            Label title = new Label()
            {
                Text = "Trips Listing",
                Font = new Font("Garamond", 22, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006C83"),
                Location = new Point(20, 82), // Moved 50px down
                AutoSize = true
            };
            rightPanel.Controls.Add(title);

            // === DataGridView for Trips ===
            DataGridView grid = new DataGridView()
            {
                Location = new Point(20, 140),
                Size = new Size(460, 350),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                BackgroundColor = Color.White,
                GridColor = ColorTranslator.FromHtml("#006C83"),
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ColorTranslator.FromHtml("#006C83"),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    SelectionBackColor = Color.LightGray,
                    SelectionForeColor = Color.Black
                },
                AllowUserToAddRows = false,
                ReadOnly = false, // Allow editing to change column width
                AllowUserToResizeColumns = true, // Enable resizing of columns
                AllowUserToResizeRows = false // Disable row resizing (optional)
            };

            // Add Columns to the Grid
            grid.Columns.Add("Title", "Trip Title");
            grid.Columns.Add("Duration", "Duration");
            grid.Columns.Add("Destination", "Destination");
            grid.Columns.Add("Price", "Price");
            grid.Columns.Add("Capacity", "Capacity");

            // Set custom column widths (optional - if you want to set default widths)
            grid.Columns["Title"].Width = 105;
            grid.Columns["Duration"].Width = 100;
            grid.Columns["Destination"].Width = 90;
            grid.Columns["Price"].Width = 100;
            grid.Columns["Capacity"].Width = 102;

            LoadTripsFromDatabase(grid);

            rightPanel.Controls.Add(grid);




        }




        private void LoadTripsFromDatabase(DataGridView grid)
        {
            //string connectionString = "Data Source=DESKTOP-JPIT9K6\\SQLEXPRESS01;Initial Catalog=TravelEase;Integrated Security=True;";
            string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

            int userId = UserSession.UserId; // Get the currently logged-in user's ID

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT Title, StartDate, EndDate, DestinationName, PricePerPerson, Capacity 
                             FROM Trips 
                             WHERE OperatorId = @userId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string title = reader["Title"].ToString();
                                DateTime startDate = Convert.ToDateTime(reader["StartDate"]);
                                DateTime endDate = Convert.ToDateTime(reader["EndDate"]);
                                string duration = (endDate - startDate).Days + " Days";
                                string destination = reader["DestinationName"].ToString();
                                decimal pricePerPerson = Convert.ToDecimal(reader["PricePerPerson"]);
                                int capacity = Convert.ToInt32(reader["Capacity"]);
                                decimal totalPrice = pricePerPerson * capacity;

                                grid.Rows.Add(title, duration, destination, totalPrice, capacity);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading trips: " + ex.Message);
                }
            }
        }
    }
}