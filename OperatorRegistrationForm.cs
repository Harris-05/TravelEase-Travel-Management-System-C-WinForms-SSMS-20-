using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Logging;
using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinFormsApp1;

using System.Data;
using System.Data.SqlClient;
using database_proj;


//Ashher

namespace WinFormsApp1
{
    public class OperatorRegistrationForm : Form
    {
        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public OperatorRegistrationForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Operator Registration & Login";
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
                Size = new Size(500, 647),
                Location = new Point(0, 0),
                BackColor = Color.LightGray
            };

            PictureBox logo = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\DELL\\Documents\\db_images\\left_logo.jpg"),
                // Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"), // Change path later
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };

            leftPanel.Controls.Add(logo);
            this.Controls.Add(leftPanel);

            // === Right Panel (Form) ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(500, 647),
                Location = new Point(500, 0),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            Label title = new Label()
            {
                Text = "Operator Registration",
                Font = new Font("Garamond", 18, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006C83"),
                Location = new Point(20, 82), // Moved 50px down
                AutoSize = true

            };

            Label titleLabel = new Label()
            {
                Text = "Operator Registration",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            titleBar.Controls.Add(titleLabel);
            rightPanel.Controls.Add(title);

            // === Form Fields ===
            Font labelFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Font textFont = new Font("Segoe UI", 11);
            int top = 100;

            string[] labels = {
                "Company Name:",
                "Contact info:",
            };

            TextBox[] textboxes = new TextBox[labels.Length];

            for (int i = 0; i < labels.Length; i++)
            {
                Label lbl = new Label()
                {
                    Text = labels[i],
                    Font = labelFont,
                    ForeColor = ColorTranslator.FromHtml("#006C83"),
                    Location = new Point(50, top + 50),
                    Size = new Size(150, 30)
                };

                TextBox tb = new TextBox()
                {
                    Font = textFont,
                    Location = new Point(200, top + 50),
                    Size = new Size(220, 80),
                    UseSystemPasswordChar = labels[i] == "Password:"
                };

                textboxes[i] = tb;

                rightPanel.Controls.Add(lbl);
                rightPanel.Controls.Add(tb);

                top += 50;
            }

            // === Register Button ===
            Button registerBtn = new Button()
            {
                Text = "Register",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Teal,
                ForeColor = Color.White,
                Size = new Size(150, 40),
                Location = new Point(200, top + 70),
                FlatStyle = FlatStyle.Flat
            };


            registerBtn.Click += (s, e) =>
            {
                string companyName = textboxes[0].Text.Trim();
                string contactInfo = textboxes[1].Text.Trim();
                int userId = UserSession.UserId;
                UserSession.OpId = userId;

                if (string.IsNullOrWhiteSpace(companyName) || string.IsNullOrWhiteSpace(contactInfo))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Replace with your actual connection string
                // string connectionString = "your_connection_string_here";
                //string connectionString = "Data Source=DESKTOP-JPIT9K6\\SQLEXPRESS01;Initial Catalog=TravelEase;Integrated Security=True;";
                string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";


                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"INSERT INTO TourOperators (OperatorID, CompanyName, ContactInfo)
                             VALUES (@OperatorID, @CompanyName, @ContactInfo)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@OperatorID", userId);
                            cmd.Parameters.AddWithValue("@CompanyName", companyName);
                            cmd.Parameters.AddWithValue("@ContactInfo", contactInfo);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Tour Operator registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close(); // Close the form or navigate as needed
                            }
                            else
                            {
                                MessageBox.Show("Registration failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Handle specific SQL exceptions if needed
                    MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            rightPanel.Controls.Add(registerBtn);

            this.Controls.Add(rightPanel);
        }
    }
}