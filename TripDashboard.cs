using database_proj;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form4 : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        System.Windows.Forms.Timer abandonTimer = new System.Windows.Forms.Timer();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private int currentUserId = UserSession.UserId;
        private DataGridView tripView;

        public Form4()
        {
            BuildUI();
            LoadBookings();
            abandonTimer.Interval = 60000; // 1 minute = 60,000 ms
            abandonTimer.Tick += (s, e) => AbandonUnpaidBookings();
            abandonTimer.Start();
        }

        private void BuildUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1000, 500);
            this.BackColor = Color.White;

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
                Text = "Trip Dashboard",
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

            Panel contentPanel = new Panel()
            {
                Size = new Size(1000, 450),
                Location = new Point(0, 50),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            Label heading = new Label()
            {
                Text = "My Upcoming Trips",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#006c83")
            };

            tripView = new DataGridView()
            {
                Location = new Point(30, 60),
                Size = new Size(920, 350),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
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
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = ColorTranslator.FromHtml("#23d3f9"),
                    SelectionForeColor = Color.Black
                }
            };

            tripView.Columns.Clear();
            tripView.Columns.Add("TripID", "TripID");
            tripView.Columns.Add("Title", "Title");
            tripView.Columns.Add("Date", "Date");
            tripView.Columns.Add("Destination", "Destination");
            tripView.Columns.Add("Category", "Category");
            tripView.Columns.Add("Status", "Status");
            tripView.Columns.Add("Booking Ref.", "Booking Ref.");
            tripView.Columns.Add("Policy", "Policy");

            DataGridViewLinkColumn refundColumn = new DataGridViewLinkColumn
            {
                Name = "Refund",
                HeaderText = "Refund",
                Text = "Refund",
                UseColumnTextForLinkValue = true,
                LinkColor = Color.Blue,
                VisitedLinkColor = Color.Purple,
                ActiveLinkColor = Color.Red
            };
            tripView.Columns.Add(refundColumn);

            DataGridViewLinkColumn cancelColumn = new DataGridViewLinkColumn
            {
                Name = "Cancel",
                HeaderText = "Cancel Trip",
                Text = "Cancel",
                UseColumnTextForLinkValue = true,
                LinkColor = Color.DarkRed,
                VisitedLinkColor = Color.Maroon,
                ActiveLinkColor = Color.Red
            };
            tripView.Columns.Add(cancelColumn);

            tripView.CellContentClick += TripView_CellContentClick;

            contentPanel.Controls.Add(heading);
            contentPanel.Controls.Add(tripView);
            this.Controls.Add(contentPanel);
        }

        private void LoadBookings()
        {
            //string connectionString = @"Server=HARRIS-LAPTOP\SQLEXPRESS;Database=TravelEase;Trusted_Connection=True;";
            string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

            string query = @"
                SELECT 
                    T.TripID,
                    T.Title AS Trip,
                    CONVERT(varchar, T.StartDate, 23) AS Date,
                    T.DestinationName AS Destination,
                    T.Category,
                    B.Status,
                    B.BookingID AS [Booking Ref.],
                    'N/A' AS Policy
                FROM Bookings B
                INNER JOIN Trips T ON B.TripID = T.TripID
                WHERE B.TravelerID = @UserID
                ORDER BY T.StartDate ASC;
            ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", currentUserId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    tripView.Rows.Clear();

                    while (reader.Read())
                    {
                        int rowIndex = tripView.Rows.Add(
                            reader["TripID"].ToString(),
                            reader["Trip"].ToString(),
                            reader["Date"].ToString(),
                            reader["Destination"].ToString(),
                            reader["Category"].ToString(),
                            reader["Status"].ToString(),
                            reader["Booking Ref."].ToString(),
                            reader["Policy"].ToString(),
                            "Refund", // Refund link
                            ""        // Cancel link placeholder
                        );

                        DateTime tripDate = DateTime.Parse(reader["Date"].ToString());

                        if (tripDate > DateTime.Now.Date)
                        {
                            tripView.Rows[rowIndex].Cells["Cancel"].Value = "Cancel";
                        }
                    }
                }
            }
        }
        private void AbandonUnpaidBookings()
        {
            //string connectionString = "Data Source=HARRIS-LAPTOP\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
            string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Step 1: Select all unpaid bookings
                string selectUnpaid = @"
            SELECT BookingID, TravelerID, TripID
            FROM Bookings
            WHERE Status = 'Unpaid';";

                using (SqlCommand selectCmd = new SqlCommand(selectUnpaid, conn))
                using (SqlDataReader reader = selectCmd.ExecuteReader())
                {
                    List<(int BookingID, int TravelerID, int TripID)> unpaidBookings = new List<(int, int, int)>();

                    while (reader.Read())
                    {
                        int bookingId = reader.GetInt32(0);
                        int travelerId = reader.GetInt32(1);
                        int tripId = reader.GetInt32(2);
                        unpaidBookings.Add((bookingId, travelerId, tripId));
                    }

                    reader.Close();

                    // Step 2: Insert each into AbandonedBookings
                    foreach (var booking in unpaidBookings)
                    {
                        string insertAbandoned = @"
                    INSERT INTO AbandonedBookings (TravelerID, TripID, Reason, CreatedAt)
                    VALUES (@TravelerID, @TripID, @Reason, GETDATE());";

                        using (SqlCommand insertCmd = new SqlCommand(insertAbandoned, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@TravelerID", booking.TravelerID);
                            insertCmd.Parameters.AddWithValue("@TripID", booking.TripID);
                            insertCmd.Parameters.AddWithValue("@Reason", "User did not complete payment");

                            insertCmd.ExecuteNonQuery();
                        }

                        // Step 3 (Optional): Delete or mark original booking
                        string updateBooking = "UPDATE Bookings SET Status = @Status WHERE BookingID = @BookingID;";
                        using (SqlCommand updateCmd = new SqlCommand(updateBooking, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@Status", "Abandoned");
                            updateCmd.Parameters.AddWithValue("@BookingID", booking.BookingID);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            //MessageBox.Show("All unpaid bookings will be moved to AbandonedBookings As Payment Not Done for 60 Sec", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TripView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string bookingId = tripView.Rows[e.RowIndex].Cells["Booking Ref."].Value.ToString();
            string tripId = tripView.Rows[e.RowIndex].Cells["TripID"].Value.ToString();

            if (tripView.Columns[e.ColumnIndex].Name == "Refund")
            {
                //string connectionString = @"Server=HARRIS-LAPTOP\SQLEXPRESS;Database=TravelEase;Trusted_Connection=True;";
                string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get CancellationID
                    string getCancellationIdQuery = "SELECT CancellationID FROM Cancellations WHERE BookingID = @BookingID";
                    SqlCommand cmd = new SqlCommand(getCancellationIdQuery, conn);
                    cmd.Parameters.AddWithValue("@BookingID", bookingId);

                    object cancellationIdObj = cmd.ExecuteScalar();
                    if (cancellationIdObj == null)
                    {
                        MessageBox.Show("No cancellation found for this booking. Refund not applicable.", "Refund Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int cancellationId = Convert.ToInt32(cancellationIdObj);

                    // Insert refund request
                    string insertRefundQuery = @"
                INSERT INTO RefundRequests (CancellationID, RequestedAmount, Status)
                VALUES (@CancellationID, @Amount, 'Applied');
            ";

                    SqlCommand insertCmd = new SqlCommand(insertRefundQuery, conn);
                    insertCmd.Parameters.AddWithValue("@CancellationID", cancellationId);
                    insertCmd.Parameters.AddWithValue("@Amount", 1000.00m); // You can compute actual amount based on policy

                    insertCmd.ExecuteNonQuery();
                    string updatePaymentStatusQuery = @"
    UPDATE Payments 
    SET Status = 'Refunded' 
    WHERE BookingID = @BookingID;
";

                    using (SqlCommand updatePaymentCmd = new SqlCommand(updatePaymentStatusQuery, conn))
                    {
                        updatePaymentCmd.Parameters.AddWithValue("@BookingID", bookingId);
                        updatePaymentCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Refund request submitted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (tripView.Columns[e.ColumnIndex].Name == "Cancel")
            {
                DateTime tripDate = DateTime.Parse(tripView.Rows[e.RowIndex].Cells["Date"].Value.ToString());

                if (tripDate <= DateTime.Now.Date)
                {
                    MessageBox.Show("This trip has already started or passed. Cannot cancel.", "Invalid Cancellation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to cancel this trip?", "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //string connectionString = @"Server=HARRIS-LAPTOP\SQLEXPRESS;Database=TravelEase;Trusted_Connection=True;";
                    string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Insert into Cancellations
                        string insertCancelQuery = @"
                    INSERT INTO Cancellations (BookingID, Reason, CancelledAt, RefundAmount)
                    VALUES (@BookingID, @Reason, GETDATE(), @RefundAmount);
                ";

                        SqlCommand cancelCmd = new SqlCommand(insertCancelQuery, conn);
                        cancelCmd.Parameters.AddWithValue("@BookingID", bookingId);
                        cancelCmd.Parameters.AddWithValue("@Reason", "User cancelled via dashboard");
                        cancelCmd.Parameters.AddWithValue("@RefundAmount", 0.00m); // You can calculate this dynamically if needed
                        cancelCmd.ExecuteNonQuery();

                        // Update booking status
                        string updateBookingQuery = "UPDATE Bookings SET Status = 'Cancelled' WHERE BookingID = @BookingID";
                        SqlCommand updateCmd = new SqlCommand(updateBookingQuery, conn);
                        updateCmd.Parameters.AddWithValue("@BookingID", bookingId);
                        updateCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Trip cancelled successfully.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBookings();
                }
            }
        }

    }
}