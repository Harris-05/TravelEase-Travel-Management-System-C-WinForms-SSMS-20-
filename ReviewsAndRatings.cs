using database_proj;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form6 : Form
    {
        // Enable draggable title bar
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Form6()
        {
            ReviewsForm();
        }

        private void ReviewsForm()
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
                Text = "Review Experience",
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

            // === Content Panel ===
            Panel contentPanel = new Panel()
            {
                Size = new Size(600, 400),
                Location = new Point(0, 50),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            Label heading = new Label()
            {
                Text = "Rate Your Experience",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(30, 50),
                AutoSize = true,
                ForeColor = ColorTranslator.FromHtml("#006c83")
            };
            Label tripLabel = new Label()
            {
                Text = "Select Trip:",
                Location = new Point(30, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black
            };

            ComboBox tripComboBox = new ComboBox()
            {
                Location = new Point(150, 18),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDown,
                Font = new Font("Segoe UI", 10),
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            // Populate tripComboBox with trip names and IDs
            string connectionString = "Data Source=LAIQ-VICTUS\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string tripQuery = "SELECT TripID, Title FROM Trips";
                SqlCommand tripCmd = new SqlCommand(tripQuery, conn);
                using (SqlDataReader reader = tripCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        tripComboBox.Items.Add($"{title} (ID: {id})");
                    }
                }
            }

            ComboBox ratingBox = new ComboBox()
            {
                Location = new Point(30, 95),
                Width = 120,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            ratingBox.Items.AddRange(new string[] { "1", "2", "3", "4", "5" });

            FlowLayoutPanel starPanel = new FlowLayoutPanel()
            {
                Location = new Point(200, 90),
                Size = new Size(150, 30),
                AutoSize = true,
                WrapContents = true
            };

            ratingBox.SelectedIndexChanged += (s, e) =>
            {
                int rating = int.Parse(ratingBox.SelectedItem.ToString());
                starPanel.Controls.Clear();
                for (int i = 0; i < rating; i++)
                {
                    Label starLabel = new Label()
                    {
                        Text = "★",
                        Font = new Font("Segoe UI", 10),
                        ForeColor = ColorTranslator.FromHtml("#006c83"),
                        AutoSize = true
                    };
                    starPanel.Controls.Add(starLabel);
                }
                starPanel.Refresh();
            };

            TextBox commentBox = new TextBox()
            {
                Location = new Point(30, 140),
                Size = new Size(520, 150),
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            Button submitBtn = new Button()
            {
                Text = "Submit Review",
                Location = new Point(30, 300),
                Width = 200,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#006c83"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            submitBtn.FlatAppearance.BorderSize = 0;
            submitBtn.Click += (s, e) =>
            {
                if (tripComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a trip.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Extract TripID from selected item
                string selected = tripComboBox.SelectedItem.ToString();
                int idStart = selected.LastIndexOf("ID: ") + 4;
                int idEnd = selected.LastIndexOf(")");
                string idStr = selected.Substring(idStart, idEnd - idStart);
                if (!int.TryParse(idStr, out int tripID))
                {
                    MessageBox.Show("Could not determine Trip ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (ratingBox.SelectedItem == null || string.IsNullOrWhiteSpace(commentBox.Text))
                {
                    MessageBox.Show("Please provide a rating and a comment.", "Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int travelerID = UserSession.UserId;
                if (travelerID == -1)
                {
                    MessageBox.Show("User not logged in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int rating = int.Parse(ratingBox.SelectedItem.ToString());
                string comment = commentBox.Text;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        // Retrieve ProviderID for the selected TripID
                        int providerID = -1;
                        using (SqlCommand providerCmd = new SqlCommand(
                            @"SELECT TOP 1 sp.ProviderID
                              FROM ClientServices cs
                              JOIN ServicesProvided sp ON cs.ServiceID = sp.ServiceID
                              WHERE cs.TripID = @TripID", conn))
                        {
                            providerCmd.Parameters.AddWithValue("@TripID", tripID);
                            object result = providerCmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                                providerID = Convert.ToInt32(result);
                        }
                        string query = @"INSERT INTO Reviews (TravelerID, TripID, ProviderID, Rating, Comment)
                             VALUES (@TravelerID, @TripID, @ProviderID, @Rating, @Comment)";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@TravelerID", travelerID);
                        cmd.Parameters.AddWithValue("@TripID", tripID);
                        if (providerID == -1)
                            cmd.Parameters.AddWithValue("@ProviderID", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@ProviderID", providerID);
                        cmd.Parameters.AddWithValue("@Rating", rating);
                        cmd.Parameters.AddWithValue("@Comment", comment);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Review submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error submitting review: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            contentPanel.Controls.AddRange(new Control[] {
    tripLabel, tripComboBox,
    heading, ratingBox, starPanel,
    commentBox, submitBtn
});
            this.Controls.Add(contentPanel);
        }
    }
}