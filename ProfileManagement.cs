using database_proj;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form7 : Form
    {
        // WinAPI for dragging custom title bar
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Form7()
        {
            ProfileForm();
        }
        /*private void LoadTravelHistory(DataGridView grid)
        {
            int travelerID = UserSession.UserId;
            string connectionString = "Data Source=HARRIS-LAPTOP\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT T.TripName, B.BookingDate, B.Status
            FROM Bookings B
            JOIN Trips T ON B.TripID = T.TripID
            WHERE B.TravelerID = @TravelerID
            ORDER BY B.BookingDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TravelerID", travelerID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                grid.DataSource = table;
                // ✅ Format columns *here*, where `grid` is valid
                if (grid.Columns.Contains("BookingDate"))
                    grid.Columns["BookingDate"].DefaultCellStyle.Format = "yyyy-MM-dd";

                if (grid.Columns.Contains("TripName"))
                    grid.Columns["TripName"].Width = 150;

                if (grid.Columns.Contains("Status"))
                    grid.Columns["Status"].Width = 100;
            }
        }*/
        private void ProfileForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(600, 450);
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
                Text = "My Profile",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 13)
            };
            titleBar.Controls.Add(titleLabel);

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

            // === Main Content Panel ===
            Panel contentPanel = new Panel()
            {
                Size = new Size(600, 400),
                Location = new Point(0, 50),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            Label heading = new Label()
            {
                Text = "My Profile",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#006c83")
            };

            Label preferencesLabel = new Label()
            {
                Text = "Preferences:",
                Location = new Point(30, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            TextBox preferencesBox = new TextBox()
            {
                Location = new Point(150, 68),
                Width = 350,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White
            };


            MessageBox.Show("Preferences saved successfully!");
            Label historyLabel = new Label()
            {
                Text = "Travel History:",
                Location = new Point(30, 120),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            // === DataGridView for Travel History ===
            DataGridView historyGridView = new DataGridView()
            {
                Location = new Point(150, 120),
                Size = new Size(350, 150),
                Font = new Font("Segoe UI", 10),
                BackgroundColor = Color.White,
                GridColor = ColorTranslator.FromHtml("#006c83"),
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ColorTranslator.FromHtml("#006c83"),
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
                ReadOnly = true
            };


            //LoadTravelHistory(historyGridView);

            Button updateBtn = new Button()
            {
                Text = "Update Preferences",
                Location = new Point(150, 290),
                Width = 200,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#006c83"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            updateBtn.FlatAppearance.BorderSize = 0;
            updateBtn.Click += (s, e) =>
            {
                List<string> selectedPreferences = new List<string>();

                string[] typedPrefs = preferencesBox.Text.Split(',');
                foreach (string pref in typedPrefs)
                {
                    string clean = pref.Trim();
                    if (!string.IsNullOrEmpty(clean)) selectedPreferences.Add(clean);
                }

                int travelerID = UserSession.UserId; // Assuming you renamed UserId to CurrentUserID

                //string connectionString = "Data Source=HARRIS-LAPTOP\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    foreach (string pref in selectedPreferences)
                    {
                        string insertPref = "INSERT INTO Preferences (TravelerID, PreferenceType, Value) VALUES (@TravelerID, 'Activity', @Value)";
                        SqlCommand cmd = new SqlCommand(insertPref, conn);
                        cmd.Parameters.AddWithValue("@TravelerID", travelerID);
                        cmd.Parameters.AddWithValue("@Value", pref);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Preferences saved successfully!");
            };
            contentPanel.Controls.AddRange(new Control[] {
                heading, preferencesLabel, preferencesBox, historyLabel, historyGridView, updateBtn
            });

            this.Controls.Add(contentPanel);
        }
    }
}
