using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace database_proj
{
    public partial class Booking : Form
    {
        // Required for dragging the custom title bar
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        System.Windows.Forms.Timer abandonTimer = new System.Windows.Forms.Timer();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        private DataGridView tripGrid = new DataGridView();
        private TextBox searchBox = new TextBox();
        private Button searchBtn = new Button();

        public Booking()
        {
            //InitializeComponent();
            BuildUI();

        }

        private void BuildUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Text = "Bookings";
            this.Size = new Size(1000, 650);
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
                Text = "Booking Panel",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10)
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

            // === Left Panel with Logo ===
            /*Panel leftPanel = new Panel()
            {
                Size = new Size(500, 607),
                Location = new Point(0, 43),
                BackColor = Color.LightGray
            };*/

            /*PictureBox imageBox = new PictureBox()
            {
                Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"), // Adjust path
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            leftPanel.Controls.Add(imageBox);
            this.Controls.Add(leftPanel);*/

            // === Right Panel ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(1000, 607),
                Location = new Point(0, 43),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };
            this.Controls.Add(rightPanel);

            // === Setup the UI inside right panel ===
            SetupUI(rightPanel);
        }

        private void SetupUI(Panel parentPanel)
        {
            // === Search Label (Styled) ===
            Label searchLabel = new Label()
            {
                Text = "Search Destination:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                BackColor = Color.Transparent
            };

            // === Search TextBox (Styled)
            searchBox.Location = new Point(200, 18);
            searchBox.Size = new Size(200, 27);
            searchBox.Font = new Font("Segoe UI", 10);

            // === Search Button (Styled) ===
            searchBtn.Text = "Search";
            searchBtn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            searchBtn.ForeColor = Color.White;
            searchBtn.BackColor = ColorTranslator.FromHtml("#006c83");
            searchBtn.FlatStyle = FlatStyle.Flat;
            searchBtn.FlatAppearance.BorderSize = 0;
            searchBtn.Size = new Size(90, 30);
            searchBtn.Location = new Point(420, 16);
            searchBtn.Click += (s, e) => LoadTrips();

            // === DataGridView for Trips (Styled) ===
            tripGrid.Location = new Point(20, 70);
            tripGrid.Size = new Size(840, 500);
            tripGrid.ReadOnly = true;
            tripGrid.AllowUserToAddRows = false;
            tripGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tripGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tripGrid.BackgroundColor = Color.White;
            tripGrid.BorderStyle = BorderStyle.None;
            tripGrid.Font = new Font("Segoe UI", 9);

            tripGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ColorTranslator.FromHtml("#006c83"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            tripGrid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10),
                SelectionBackColor = ColorTranslator.FromHtml("#23d3f9")
            };

            tripGrid.EnableHeadersVisualStyles = false;
            tripGrid.CellClick += TripGrid_CellClick;

            // === Add controls to the panel ===
            parentPanel.Controls.AddRange(new Control[] { searchLabel, searchBox, searchBtn, tripGrid });
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

            MessageBox.Show("All unpaid bookings will be moved to AbandonedBookings As Payment Not Done for 60 Sec", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void LoadTrips()
        {
            string destination = searchBox.Text.Trim().ToLower();
            //string connectionString = "Data Source=HARRIS-LAPTOP\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
            string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
SELECT 
    T.TripID, 
    T.Title AS [Trip Name], 
    T.DestinationName AS [Destination],
    CONVERT(varchar, T.StartDate, 23) AS [Start Date], 
    CONVERT(varchar, T.EndDate, 23) AS [End Date], 
    T.PricePerPerson AS [Price (PKR)]
FROM Trips T
WHERE T.StartDate >= CAST(GETDATE() AS DATETIME)
  AND T.DestinationName COLLATE SQL_Latin1_General_CP1_CI_AS LIKE '%' + @Destination + '%'
  AND T.TripID NOT IN (
      SELECT TripID FROM Bookings WHERE TravelerID = @TravelerID
  );";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Destination", destination);
                cmd.Parameters.AddWithValue("@TravelerID", UserSession.UserId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                tripGrid.Columns.Clear();
                tripGrid.DataSource = table;

                DataGridViewButtonColumn bookCol = new DataGridViewButtonColumn
                {
                    Name = "Book",
                    HeaderText = "Action",
                    Text = "Book",
                    UseColumnTextForButtonValue = true,
                    FlatStyle = FlatStyle.Popup
                };
                tripGrid.Columns.Add(bookCol);

                tripGrid.CellClick -= TripGrid_CellClick;
                tripGrid.CellClick += TripGrid_CellClick;
            }
        }


        private void TripGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == tripGrid.Columns["Book"].Index)
            {
                int tripID = Convert.ToInt32(((DataTable)tripGrid.DataSource).Rows[e.RowIndex]["TripID"]);
                decimal price = Convert.ToDecimal(tripGrid.Rows[e.RowIndex].Cells["Price (PKR)"].Value);

                int travelerID = UserSession.UserId; // Globally set session user
                int numberOfPeople = 1;
                decimal totalAmount = price * numberOfPeople;

                //string connectionString = "Data Source=HARRIS-LAPTOP\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
                string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Insert booking with status 'Unpaid'
                    string insertBooking = @"
                INSERT INTO Bookings (TravelerID, TripID, BookingDate, NumberOfPeople, TotalAmount, Status)
                VALUES (@TravelerID, @TripID, GETDATE(), @NumPeople, @Total, 'Unpaid');
                SELECT SCOPE_IDENTITY();";

                    int bookingID;
                    using (SqlCommand cmd = new SqlCommand(insertBooking, conn))
                    {
                        cmd.Parameters.AddWithValue("@TravelerID", travelerID);
                        cmd.Parameters.AddWithValue("@TripID", tripID);
                        cmd.Parameters.AddWithValue("@NumPeople", numberOfPeople);
                        cmd.Parameters.AddWithValue("@Total", totalAmount);

                        bookingID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Ask if user wants to pay now
                    DialogResult result = MessageBox.Show("Do you want to pay now?", "Payment Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Insert payment and update booking status to 'Paid'
                        string insertPayment = @"
                    INSERT INTO Payments (BookingID, Amount, PaymentMethod, Status, PaymentDate)
                    VALUES (@BookingID, @Amount, @Method, 'Paid', GETDATE());";

                        using (SqlCommand cmd = new SqlCommand(insertPayment, conn))
                        {
                            cmd.Parameters.AddWithValue("@BookingID", bookingID);
                            cmd.Parameters.AddWithValue("@Amount", totalAmount);
                            cmd.Parameters.AddWithValue("@Method", "Credit Card"); // You can customize this
                            cmd.ExecuteNonQuery();
                        }

                        string updateBooking = "UPDATE Bookings SET Status = 'Paid' WHERE BookingID = @BookingID;";
                        using (SqlCommand cmd = new SqlCommand(updateBooking, conn))
                        {
                            cmd.Parameters.AddWithValue("@BookingID", bookingID);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Payment successful and trip booked!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Trip booked. Please complete payment in 60 Secs or payement will be Abandoned.", "Unpaid Booking", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        abandonTimer.Interval = 60000; // 1 minute = 60,000 ms
                        abandonTimer.Tick += (s, e) => AbandonUnpaidBookings();
                        abandonTimer.Start();
                    }
                }
            }
        }


    }
}