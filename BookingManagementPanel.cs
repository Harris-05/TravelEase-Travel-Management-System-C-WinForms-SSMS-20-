using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data.SqlClient;
using database_proj;
using System.Data;

//Ashher 

namespace WinFormsApp1
{
    public class BookingManagementPanel : Form
    {
        private SqlConnection connection;
        private DataGridView grid;

        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public BookingManagementPanel()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
            connection = new SqlConnection(connectionString); // Initialize the connection field
            BuildUI();
            LoadBookings();
        }


        private void BuildUI()
        {
            this.Text = "Booking Management";
            this.Size = new Size(1200, 647);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

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
                Text = "Booking Management",
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

            // === Left Panel ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 647),
                Location = new Point(0, 0),
                BackColor = Color.LightGray
            };

            PictureBox logo = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\DELL\\Documents\\db_images\\left_logo.jpg"),
                //  Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"), // Replace path if needed
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            leftPanel.Controls.Add(logo);
            this.Controls.Add(leftPanel);

            // === Right Panel ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(700, 647),
                Location = new Point(500, 0),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            // === Title ===
            Label title = new Label()
            {
                Text = "Booking Management",
                Font = new Font("Garamond", 20, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006C83"),
                Location = new Point(20, 82), // Moved 50px down
                AutoSize = true

            };
            rightPanel.Controls.Add(title);



            // === Icon ===
            PictureBox icon = new PictureBox()
            {
                Image = SystemIcons.Warning.ToBitmap(),
                Size = new Size(30, 30),
                Location = new Point(20, 130),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            rightPanel.Controls.Add(icon);

            Label iconLabel = new Label()
            {
                Text = "Track and Manage Reservations",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006C83"),
                Location = new Point(50, 135),
                AutoSize = true
            };
            rightPanel.Controls.Add(iconLabel);





            // === Grid View ===

            grid = new DataGridView()
            {
                Location = new Point(20, 170),
                Size = new Size(660, 400),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
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
                AllowUserToAddRows = false,
                ReadOnly = true
            };




            rightPanel.Controls.Add(grid);
            this.Controls.Add(rightPanel);
            // Call LoadBookings to populate the grid
            LoadBookings();

        }

        private void LoadBookings()
        {
            try
            {
                connection.Open();

                string query = @"
            SELECT 
                b.BookingID,
                b.TravelerID,
                b.TripID,
                b.NumberOfPeople,
                b.TotalAmount,
                b.Status
            FROM Bookings b
            INNER JOIN Trips t ON b.TripID = t.TripID
            WHERE t.OperatorID = @OperatorID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@OperatorID", UserSession.UserId); // get operator ID

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        grid.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bookings: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }







    }
}