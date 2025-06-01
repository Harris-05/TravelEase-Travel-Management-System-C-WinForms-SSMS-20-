using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data.SqlClient;
using database_proj;
using System.Data;


namespace WindowsFormsApp3
{
    public class Form2 : Form
    {


        // Drag window functionality
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Form2()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 500);
            this.BackColor = Color.White;

            BuildSignUpUI();
        }

        private void BuildSignUpUI()
        {
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
                Text = "Sign Up",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 15)
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
            Panel leftPanel = new Panel()
            {
                Size = new Size(350, 450),
                Location = new Point(0, 50),
                BackColor = ColorTranslator.FromHtml("#006c83")
            };

            PictureBox logo = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            leftPanel.Controls.Add(logo);
            this.Controls.Add(leftPanel);

            // === Right Panel with Fields ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(450, 450),
                Location = new Point(350, 50),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };
            this.Controls.Add(rightPanel);

            Font labelFont = new Font("Segoe UI", 9, FontStyle.Regular);
            Font inputFont = new Font("Segoe UI", 9);

            int startY = 20;
            int spacing = 40;

            // Helper to add label and control
            void AddField(Control label, Control input)
            {
                label.Font = labelFont;
                label.ForeColor = Color.Black;
                label.Location = new Point(30, startY);
                input.Font = inputFont;
                input.Location = new Point(150, startY - 2);
                input.Width = 250;
                rightPanel.Controls.Add(label);
                rightPanel.Controls.Add(input);
                startY += spacing;
            }

            AddField(new Label() { Text = "Full Name:" }, new TextBox());
            AddField(new Label() { Text = "Email:" }, new TextBox());
            AddField(new Label() { Text = "Password:" }, new TextBox() { PasswordChar = '*' });

            ComboBox roleBox = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
            roleBox.Items.AddRange(new string[] { "Traveler", "Tour Operator", "Admin", "Service Provider" });
            AddField(new Label() { Text = "Role:" }, roleBox);

            AddField(new Label() { Text = "Date of Birth:" }, new DateTimePicker());

            AddField(new Label() { Text = "Nationality:" }, new TextBox());

            ComboBox genderBox = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
            genderBox.Items.AddRange(new string[] { "Male", "Female", "Other" });
            AddField(new Label() { Text = "Gender:" }, genderBox);

            // Sign Up Button
            Button signUpButton = new Button()
            {
                Text = "Create Account",
                Location = new Point(150, startY + 10),
                Width = 250,
                Height = 35,
                BackColor = ColorTranslator.FromHtml("#006c83"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            signUpButton.FlatAppearance.BorderSize = 0;


            signUpButton.Click += (s, e) =>
            {
                //string connectionString = "Data Source=HARRIS-LAPTOP\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
                string connectionString = "Data Source=LAIQ-VICTUS\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand dbCmd = new SqlCommand("SELECT DB_NAME()", conn);
                    string dbName = (string)dbCmd.ExecuteScalar();
                    MessageBox.Show("Writing to database: " + dbName);

                    // Get input values
                    string fullName = ((TextBox)rightPanel.Controls[1]).Text;
                    string email = ((TextBox)rightPanel.Controls[3]).Text;
                    string password = ((TextBox)rightPanel.Controls[5]).Text;
                    string role = ((ComboBox)rightPanel.Controls[7]).SelectedItem?.ToString();
                    DateTime dob = ((DateTimePicker)rightPanel.Controls[9]).Value;
                    string nationality = ((TextBox)rightPanel.Controls[11]).Text;
                    string gender = ((ComboBox)rightPanel.Controls[13]).SelectedItem?.ToString();

                    // Validate email
                    if (!email.Contains("@") || !email.EndsWith(".com", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Please enter a valid email address (must contain '@' and end with '.com').", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Append SELECT SCOPE_IDENTITY() to get the new UserID after insert
                    string query = "INSERT INTO Users (FullName, Email, PasswordHash, Role, DateOfBirth, Nationality, Gender) " +
                                   "VALUES (@FullName, @Email, @Password, @Role, @DOB, @Nationality, @Gender); " +
                                   "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@DOB", dob);
                        cmd.Parameters.AddWithValue("@Nationality", nationality);
                        cmd.Parameters.AddWithValue("@Gender", gender);

                        // Execute insert and get the new UserID
                        object idResult = cmd.ExecuteScalar();
                        int newUserID = Convert.ToInt32(idResult);

                        // Optional: store in session class
                        UserSession.UserId = newUserID;

                        // If user is Traveler, insert into Travelers table
                        if (role == "Traveler")
                        {
                            string travelerInsert = "INSERT INTO Travelers (TravelerID, Preferences) VALUES (@TravelerID, '')";
                            SqlCommand travelerCmd = new SqlCommand(travelerInsert, conn);
                            travelerCmd.Parameters.AddWithValue("@TravelerID", newUserID);
                            travelerCmd.ExecuteNonQuery();
                        }
                        /* string auditInsert = "INSERT INTO UserAuditLog (UserID, EventType, Role) VALUES (@UserID, 'Signup', @Role)";
                         SqlCommand auditCmd = new SqlCommand(auditInsert, conn);
                         auditCmd.Parameters.AddWithValue("@UserID", newUserID);
                         auditCmd.Parameters.AddWithValue("@Role", role);
                         auditCmd.ExecuteNonQuery();*/
                    }

                    MessageBox.Show("User account created successfully!");
                    this.Close();
                }
            };



            rightPanel.Controls.Add(signUpButton);
        }
    }
}

// this is finality