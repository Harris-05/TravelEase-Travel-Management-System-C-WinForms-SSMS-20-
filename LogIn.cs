using database_proj;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // InitializeComponent();
            LoginForm();
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public void LoginForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1000, 650);
            BuildUI();
        }

        private void BuildUI()
        {
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
                Text = "TravelEase Login",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
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

            // === Left Panel with Image and Quote ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 600),
                Location = new Point(0, 50),
                BackColor = Color.LightGray
            };
            PictureBox img = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            leftPanel.Controls.Add(img);
            this.Controls.Add(leftPanel);

            // === Right Panel for Login ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(500, 600),
                Location = new Point(500, 50),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            Label appTitle = new Label()
            {
                Text = "TravelEase",
                Font = new Font("Garamond", 20, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                Location = new Point(160, 50),
                AutoSize = true
            };

            TextBox emailBox = new TextBox()
            {
                Text = "Email",
                ForeColor = Color.Gray,
                Location = new Point(150, 120),
                Width = 200
            };
            emailBox.GotFocus += (s, e) =>
            {
                if (emailBox.Text == "Email")
                {
                    emailBox.Text = "";
                    emailBox.ForeColor = Color.Black;
                }
            };
            emailBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(emailBox.Text))
                {
                    emailBox.Text = "Email";
                    emailBox.ForeColor = Color.Gray;
                }
            };

            TextBox passwordBox = new TextBox()
            {
                Text = "Password",
                ForeColor = Color.Gray,
                Location = new Point(150, 170),
                Width = 200

            };
            passwordBox.GotFocus += (s, e) =>
            {
                if (passwordBox.Text == "Password")
                {
                    passwordBox.Text = "";
                    passwordBox.ForeColor = Color.Black;
                    passwordBox.UseSystemPasswordChar = true;
                }
            };
            passwordBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(passwordBox.Text))
                {
                    passwordBox.UseSystemPasswordChar = true;
                    passwordBox.Text = "Password";
                    passwordBox.ForeColor = Color.Gray;
                }
            };
            bool isPasswordVisible = false; // flag to track visibility

            LinkLabel ShowPassword = new LinkLabel()
            {
                Text = "Show Password",
                Location = new Point(150, 320),
                AutoSize = true,
                LinkColor = Color.Gray
            };

            ShowPassword.LinkClicked += (s, e) =>
            {
                if (passwordBox.Text == "Password") return; // don't toggle when placeholder is active

                isPasswordVisible = !isPasswordVisible;

                passwordBox.UseSystemPasswordChar = !isPasswordVisible;

                ShowPassword.Text = isPasswordVisible ? "Hide Password" : "Show Password";
            };
            LinkLabel forgot = new LinkLabel()
            {
                Text = "Forgot your password?",
                Location = new Point(150, 205),
                AutoSize = true,
                LinkColor = Color.Gray
            };

            Button loginBtn = new Button()
            {
                Text = "🔑 Log In",
                Location = new Point(150, 240),
                Width = 200,
                Height = 35,
                BackColor = ColorTranslator.FromHtml("#006c83"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            loginBtn.FlatAppearance.BorderSize = 0;




            LinkLabel signup = new LinkLabel()
            {
                Text = "Don't have an account? Sign up",
                Location = new Point(150, 290),
                AutoSize = true,
                LinkColor = ColorTranslator.FromHtml("#006c83")
            };

            loginBtn.Click += (s, e) =>
            {
                string email = emailBox.Text.Trim();
                string password = passwordBox.Text;

                if (email == "" || password == "" || email == "Email" || password == "Password")
                {
                    MessageBox.Show("Please enter both email and password.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // string connectionString = "Data Source=DESKTOP-JPIT9K6\\SQLEXPRESS01;Initial Catalog=TravelEase;Integrated Security=True;";
                //string connectionString = "Data Source=HARRIS-LAPTOP\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
                string connectionString = "Data Source=LAIQ-VICTUS\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";


                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND PasswordHash = @Password";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Password", password);

                            int userCount = (int)cmd.ExecuteScalar();

                            if (userCount > 0)
                            {
                                //  User exists
                                AuthManager.IsLoggedIn = true;

                                //  Fetch UserId now
                                string idQuery = "SELECT UserID FROM Users WHERE Email = @Email AND PasswordHash = @Password";
                                using (SqlCommand idCmd = new SqlCommand(idQuery, conn))
                                {
                                    idCmd.Parameters.AddWithValue("@Email", email);
                                    idCmd.Parameters.AddWithValue("@Password", password);
                                    object idResult = idCmd.ExecuteScalar();
                                    if (idResult != null)
                                    {
                                        UserSession.UserId = Convert.ToInt32(idResult); // 👈 Save user ID to static class
                                    }
                                }

                                //  Fetch Role now
                                string roleQuery = "SELECT Role FROM Users WHERE Email = @Email AND PasswordHash = @Password";
                                using (SqlCommand roleCmd = new SqlCommand(roleQuery, conn))
                                {
                                    roleCmd.Parameters.AddWithValue("@Email", email);
                                    roleCmd.Parameters.AddWithValue("@Password", password);

                                    object roleResult = roleCmd.ExecuteScalar();
                                    if (roleResult != null)
                                    {
                                        string role = roleResult.ToString().ToLower();
                                        UserSession.CurrentEmail = email;
                                        UserSession.CurrentRole = role;

                                        // ✅ Set static bools based on role
                                        UserSession.IsTraveler = (role == "traveler");
                                        UserSession.IsTourOperator = (role == "tour operator");
                                        UserSession.IsAdmin = (role == "admin");
                                    }
                                }

                                MessageBox.Show("Login successful!", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Insert into AuditTrail
                                string auditQuery = "INSERT INTO AuditTrail (UserID, Action) VALUES (@UserID, @Action)";
                                using (SqlCommand auditCmd = new SqlCommand(auditQuery, conn))
                                {
                                    auditCmd.Parameters.AddWithValue("@UserID", UserSession.UserId);
                                    auditCmd.Parameters.AddWithValue("@Action", "User logged in");
                                    auditCmd.ExecuteNonQuery();
                                }

                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Login failed! Invalid email or password.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            signup.LinkClicked += (s, e) =>
            {
                Form2 signUpForm = new Form2();
                this.Hide();
                signUpForm.Show();
            };


            rightPanel.Controls.Add(appTitle);
            rightPanel.Controls.Add(emailBox);
            rightPanel.Controls.Add(passwordBox);
            rightPanel.Controls.Add(forgot);
            rightPanel.Controls.Add(loginBtn);
            rightPanel.Controls.Add(signup);
            rightPanel.Controls.Add(ShowPassword);
            this.Controls.Add(rightPanel);
        }

        /* private void Login_ButtonClicked(object sender, EventArgs e)
         {
             Form8 form8 = new Form8();
             this.Hide();
             form8.Show();
         }
         private void Signin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
         {
             Form2 signUpForm = new Form2();
             signUpForm.ShowDialog();
         }*/

    }
}