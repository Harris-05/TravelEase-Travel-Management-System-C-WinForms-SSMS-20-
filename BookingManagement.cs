using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace database_proj
{
    public class BookingManagementForm : Form
    {
        private DataGridView dgvBookings;
        private TextBox txtSearch;
        private Button btnFilterPending;
        private Button btnConfirmSelected;
        private Button btnCancelSelected;
        private Button btnTrackPayments;
        private string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public BookingManagementForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Booking Management";
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

            Label titleLabel = new Label()
            {
                Text = "Booking Management Panel",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            titleBar.Controls.Add(titleLabel);

            // Close Button (unchanged)
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

            // Title Label (moved down by net 50px)
            Label lblTitle = new Label()
            {
                Text = "Booking Management Panel",
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                Font = new Font("Garamond", 24, FontStyle.Bold),
                Location = new Point(20, 50 + titleBar.Height),
                AutoSize = true

            };
            this.Controls.Add(lblTitle);

            // Search Label (moved down by net 50px)
            Label lblSearch = new Label()
            {
                Text = "Search by Booking ID or Traveler:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 210),
                AutoSize = true
            };
            this.Controls.Add(lblSearch);

            // Search TextBox (moved down by net 50px)
            txtSearch = new TextBox()
            {
                Location = new Point(300, 208),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtSearch);

            // Filter Button (moved down by net 50px)
            btnFilterPending = new Button()
            {
                Text = "Show Pending",
                Location = new Point(520, 207),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(255, 193, 7),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(btnFilterPending);

            // === DataGridView === (moved down by net 50px)
            dgvBookings = new DataGridView()
            {
                Location = new Point(20, 250),
                Size = new Size(940, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 9),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ColorTranslator.FromHtml("#006c83"),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = ColorTranslator.FromHtml("#23d3f9")
                },
                EnableHeadersVisualStyles = false
            };
            dgvBookings.Columns.Add("BookingID", "Booking ID");
            dgvBookings.Columns.Add("TravelerName", "Traveler");
            dgvBookings.Columns.Add("TripTitle", "Trip");
            dgvBookings.Columns.Add("BookingDate", "Date");
            dgvBookings.Columns.Add("NumPeople", "People");
            dgvBookings.Columns.Add("TotalAmount", "Amount");
            dgvBookings.Columns.Add("Status", "Status");
            this.Controls.Add(dgvBookings);

            // Actions (moved down by net 50px)
            btnConfirmSelected = new Button()
            {
                Text = "Confirm",
                Location = new Point(20, 570),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(btnConfirmSelected);

            btnCancelSelected = new Button()
            {
                Text = "Cancel",
                Location = new Point(160, 570),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(btnCancelSelected);

            btnTrackPayments = new Button()
            {
                Text = "Track Payments",
                Location = new Point(300, 570),
                Size = new Size(130, 35),
                BackColor = ColorTranslator.FromHtml("#6f42c1"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(btnTrackPayments);

            // Event Handlers
            btnFilterPending.Click += BtnFilterPending_Click;
            btnConfirmSelected.Click += BtnConfirmSelected_Click;
            btnCancelSelected.Click += BtnCancelSelected_Click;
            btnTrackPayments.Click += BtnTrackPayments_Click;

            // === Logo === (remains unchanged)
            PictureBox logoBox = new PictureBox()
            {
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\blue logo.png"),
                Size = new Size(240, 240),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = ColorTranslator.FromHtml("#fddbc7"),
                Location = new Point(this.ClientSize.Width - 250, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Controls.Add(logoBox);
        }

        // --- New Methods ---
        private void BtnFilterPending_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        b.BookingID,
                        u.FullName AS TravelerName,
                        t.Title AS TripTitle,
                        b.BookingDate,
                        b.NumberOfPeople AS NumPeople,
                        b.TotalAmount,
                        b.Status
                    FROM Bookings b
                    JOIN Travelers tr ON b.TravelerID = tr.TravelerID
                    JOIN Users u ON tr.TravelerID = u.UserID
                    JOIN Trips t ON b.TripID = t.TripID
                    WHERE (@search = '' OR CAST(b.BookingID AS VARCHAR) = @search OR u.FullName LIKE @search)
";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@search", search == "" ? "" : "%" + search + "%");
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    var dt = new System.Data.DataTable();
                    adapter.Fill(dt);
                    dgvBookings.Rows.Clear();
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        dgvBookings.Rows.Add(row["BookingID"], row["TravelerName"], row["TripTitle"], row["BookingDate"], row["NumPeople"], row["TotalAmount"], row["Status"]);
                    }
                }
            }
        }

        private int? GetSelectedBookingID()
        {
            if (dgvBookings.SelectedRows.Count > 0)
            {
                var val = dgvBookings.SelectedRows[0].Cells["BookingID"].Value;
                if (val != null && int.TryParse(val.ToString(), out int id))
                    return id;
            }
            return null;
        }

        private void BtnConfirmSelected_Click(object sender, EventArgs e)
        {
            int? bookingId = GetSelectedBookingID();
            if (bookingId == null)
            {
                MessageBox.Show("Please select a booking to confirm.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = "UPDATE Bookings SET Status = 'confirmed' WHERE BookingID = @BookingID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            BtnFilterPending_Click(null, null);
        }

        private void BtnCancelSelected_Click(object sender, EventArgs e)
        {
            int? bookingId = GetSelectedBookingID();
            if (bookingId == null)
            {
                MessageBox.Show("Please select a booking to cancel.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Set status to cancelled
                string updateQuery = "UPDATE Bookings SET Status = 'cancelled' WHERE BookingID = @BookingID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                // Add to Cancellations table
                string insertQuery = "INSERT INTO Cancellations (BookingID, Reason, CancelledAt, RefundAmount) VALUES (@BookingID, @Reason, @CancelledAt, @RefundAmount)";
                using (var cmd = new System.Data.SqlClient.SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    cmd.Parameters.AddWithValue("@Reason", "Cancelled by user");
                    cmd.Parameters.AddWithValue("@CancelledAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@RefundAmount", 0); // Set as needed
                    cmd.ExecuteNonQuery();
                }
            }
            BtnFilterPending_Click(null, null);
        }

        private void BtnTrackPayments_Click(object sender, EventArgs e)
        {
            int? bookingId = GetSelectedBookingID();
            if (bookingId == null)
            {
                MessageBox.Show("Please select a booking to track payment.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = "SELECT Status FROM Payments WHERE BookingID = @BookingID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        MessageBox.Show($"Payment Status: {result}", "Payment Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No payment found for this booking.", "Payment Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
