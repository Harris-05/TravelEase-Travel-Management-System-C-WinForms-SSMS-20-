using database_proj;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinFormsApp1;

using System.Data;
using System.Data.SqlClient;

//Ashher

namespace WinFormsApp1
{
    public class TripManagementForm : Form
    {
        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public TripManagementForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;

            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Trip Creation and Management";
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
                Text = "Trip Creation and Management",
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
                // Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"), // change path if needed
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

            Label title = new Label()
            {
                Text = "Trip Management",
                Font = new Font("Garamond", 18, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006C83"),
                Location = new Point(20, 82), // Moved 50px down
                AutoSize = true,
                Height = 60 // Increased height for better visibility
            };

            rightPanel.Controls.Add(title);

            Font labelFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Font textFont = new Font("Segoe UI", 11);
            int top = 150; // Increased the starting Y-coordinate by 50 pixels

            // === Fields with icons ===
            (string label, string icon)[] fields = {
            ("Trip Title:", "📝"),
            ("Description:", "📄"),
            ("Destination:", "📍"),
            ("Category:", "🏷"),
            ("Tour Operator:", "👤"),
            ("Start Date:", "📅"),
            ("End Date:", "📅"),
            ("Price per Person:", "💲"),
            ("Capacity:", "👥"),
            };


            TextBox[] textboxes = new TextBox[fields.Length];
            DateTimePicker startDatePicker = null;
            DateTimePicker endDatePicker = null;

            for (int i = 0; i < fields.Length; i++)
            {
                Label lbl = new Label()
                {
                    Text = $"{fields[i].icon} {fields[i].label}",
                    Font = labelFont,
                    ForeColor = ColorTranslator.FromHtml("#006C83"),
                    Location = new Point(40, top),
                    Size = new Size(180, 30)
                };

                if (fields[i].label == "Start Date:")
                {
                    startDatePicker = new DateTimePicker()
                    {
                        Font = textFont,
                        Location = new Point(220, top),
                        Size = new Size(220, 30),
                        Format = DateTimePickerFormat.Short
                    };
                    rightPanel.Controls.Add(lbl);
                    rightPanel.Controls.Add(startDatePicker);
                }
                else if (fields[i].label == "End Date:")
                {
                    endDatePicker = new DateTimePicker()
                    {
                        Font = textFont,
                        Location = new Point(220, top),
                        Size = new Size(220, 30),
                        Format = DateTimePickerFormat.Short
                    };
                    rightPanel.Controls.Add(lbl);
                    rightPanel.Controls.Add(endDatePicker);
                }
                else
                {
                    TextBox tb = new TextBox()
                    {
                        Font = textFont,
                        Location = new Point(220, top),
                        Size = new Size(220, 30)
                    };
                    textboxes[i] = tb;
                    rightPanel.Controls.Add(lbl);
                    rightPanel.Controls.Add(tb);
                }
                top += 50;
            }

            // === Create Button Only ===
            Button createButton = new Button()
            {
                Text = "Create",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = ColorTranslator.FromHtml("#006C83"),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(80, top + 20),
                FlatStyle = FlatStyle.Flat
            };

            // === Update Button ===
            Button updateButton = new Button()
            {
                Text = "Update",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = ColorTranslator.FromHtml("#006C83"),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(220, top + 20),
                FlatStyle = FlatStyle.Flat
            };

            // === Delete Button ===
            Button deleteButton = new Button()
            {
                Text = "Delete",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = ColorTranslator.FromHtml("#006C83"),
                ForeColor = Color.White,
                Size = new Size(100, 35),
                Location = new Point(320, top + 20),
                FlatStyle = FlatStyle.Flat
            };

            createButton.Click += (s, e) =>
            {
                try
                {
                    string title = textboxes[0].Text;
                    string description = textboxes[1].Text;
                    string DestinationName = textboxes[2].Text;
                    string category = textboxes[3].Text;
                    string operatorName = textboxes[4].Text;
                    string startDate = startDatePicker.Value.ToString("yyyy-MM-dd");
                    string endDate = endDatePicker.Value.ToString("yyyy-MM-dd");
                    string price = textboxes[7].Text;
                    string capacity = textboxes[8].Text;
                    int userId = UserSession.UserId; // 👈 same way as in login/signup

                    using (var connection = new SqlConnection("Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;"))
                    {
                        connection.Open();
                        string query = @"INSERT INTO trips 
                        (Title, Description, DestinationName, Category, OperatorId, StartDate, EndDate, PricePerPerson, Capacity)
                        VALUES 
                        (@title, @desc, @dest, @cat, @opId, @start, @end, @price, @cap)";

                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@title", title);
                            command.Parameters.AddWithValue("@desc", description);
                            command.Parameters.AddWithValue("@dest", DestinationName);
                            command.Parameters.AddWithValue("@cat", category);  // userId as CategoryId
                            command.Parameters.AddWithValue("@opId", userId);   // userId as OperatorId
                            command.Parameters.AddWithValue("@start", startDate);
                            command.Parameters.AddWithValue("@end", endDate);
                            command.Parameters.AddWithValue("@price", price);
                            command.Parameters.AddWithValue("@cap", capacity);

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                                MessageBox.Show("Trip created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show("Trip creation failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            rightPanel.Controls.Add(createButton);



            updateButton.Click += (s, e) =>
            {
                try
                {
                    string title = textboxes[0].Text;
                    string description = textboxes[1].Text;
                    string destinationName = textboxes[2].Text;
                    string category = textboxes[3].Text;
                    string operatorName = textboxes[4].Text; // still input, but not used for security
                    string startDate = startDatePicker.Value.ToString("yyyy-MM-dd");
                    string endDate = endDatePicker.Value.ToString("yyyy-MM-dd");
                    string price = textboxes[7].Text;
                    string capacity = textboxes[8].Text;

                    int userId = UserSession.UserId;

                    //using (var connection = new SqlConnection("Data Source=DESKTOP-JPIT9K6\\SQLEXPRESS01;Initial Catalog=TravelEase;Integrated Security=True;"))
                    using (var connection = new SqlConnection("Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;"))
                    {
                        connection.Open();

                        // Step 1: Check if a trip with that title exists and fetch its OperatorId
                        string checkQuery = "SELECT OperatorId FROM trips WHERE Title = @title";
                        using (var checkCmd = new SqlCommand(checkQuery, connection))
                        {
                            checkCmd.Parameters.AddWithValue("@title", title);
                            var result = checkCmd.ExecuteScalar();

                            if (result == null)
                            {
                                MessageBox.Show("No trip with this title found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            int operatorIdFromTrip = Convert.ToInt32(result);

                            // Step 2: Allow update only if OperatorId matches logged-in user
                            if (operatorIdFromTrip != userId)
                            {
                                MessageBox.Show("You are not authorized to update this trip.", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        // Step 3: Perform the update
                        string updateQuery = @"
                            UPDATE trips 
                            SET Description = @desc,
                                DestinationName = @dest,
                                Category = @cat,
                                StartDate = @start,
                                EndDate = @end,
                                PricePerPerson = @price,
                                Capacity = @cap
                            WHERE Title = @title";

                        using (var updateCmd = new SqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@title", title);
                            updateCmd.Parameters.AddWithValue("@desc", description);
                            updateCmd.Parameters.AddWithValue("@dest", destinationName);
                            updateCmd.Parameters.AddWithValue("@cat", category);
                            updateCmd.Parameters.AddWithValue("@start", startDate);
                            updateCmd.Parameters.AddWithValue("@end", endDate);
                            updateCmd.Parameters.AddWithValue("@price", price);
                            updateCmd.Parameters.AddWithValue("@cap", capacity);

                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                                MessageBox.Show("Trip updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show("Trip update failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };


            rightPanel.Controls.Add(updateButton);

            deleteButton.Click += (s, e) =>
            {



                try
                {
                    string title = textboxes[0].Text;
                    int userId = UserSession.UserId;

                    //using (var connection = new SqlConnection("Data Source=DESKTOP-JPIT9K6\\SQLEXPRESS01;Initial Catalog=TravelEase;Integrated Security=True;"))
                    using (var connection = new SqlConnection("Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;"))
                    {
                        connection.Open();

                        string checkQuery = "SELECT OperatorId FROM trips WHERE Title = @title";
                        using (var checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@title", title);
                            var result = checkCommand.ExecuteScalar();

                            if (result == null)
                            {
                                MessageBox.Show("No trip of this name found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            int operatorIdFromDb = Convert.ToInt32(result);
                            if (operatorIdFromDb != userId)
                            {
                                MessageBox.Show("You are not authorized to delete this trip.", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        string deleteQuery = "DELETE FROM trips WHERE Title = @title AND OperatorId = @opId";
                        using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@title", title);
                            deleteCommand.Parameters.AddWithValue("@opId", userId);
                            int deleted = deleteCommand.ExecuteNonQuery();

                            if (deleted > 0)
                                MessageBox.Show("Trip deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show("Trip deletion failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            rightPanel.Controls.Add(deleteButton);


            this.Controls.Add(rightPanel);
        }
    }
}