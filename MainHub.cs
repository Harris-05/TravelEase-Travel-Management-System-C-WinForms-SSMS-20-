using database_proj;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TourPlannerApp;

using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApp3
{
    public partial class MainPageForm : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public MainPageForm()
        {
            BuildMainPageUI();
        }

        private void BuildMainPageUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1000, 700);

            string[] buttonLabels = { "Traveler Hub", "Tour Operator Hub", "🔑 Log In", "Admin Hub", "Service Hub" };
            int buttonCount = buttonLabels.Length;
            int buttonWidth = this.ClientSize.Width / buttonCount;

            Panel buttonPanel = new Panel()
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            for (int i = 0; i < buttonCount; i++)
            {
                Button btn = new Button()
                {
                    Text = buttonLabels[i],
                    Size = new Size(buttonWidth, 50),
                    Location = new Point(i * buttonWidth, 0),
                    BackColor = ColorTranslator.FromHtml("#006c83"),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += ButtonClickHandler;
                buttonPanel.Controls.Add(btn);
            }

            this.Controls.Add(buttonPanel);



            //Divivder

            Panel divider = new Panel()
            {
                Height = 1,
                Dock = DockStyle.Top,
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };
            this.Controls.Add(divider);


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
                Text = "TravelEase",
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
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = true
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();
            titleBar.Controls.Add(closeButton);


            // custom title bar done


            PictureBox backgroundImage = new PictureBox()
            {

                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\Main Page.png"),
                //Image = Image.FromFile("C:\\Users\\DELL\\Documents\\db_images\\main_page.jpg"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };

            this.Controls.Add(backgroundImage);
            this.Controls.SetChildIndex(backgroundImage, 0);
        }

        private void ButtonClickHandler(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || string.IsNullOrWhiteSpace(btn.Text)) return;

            if (btn.Text == "🔑 Log In")
            {
                if (AuthManager.IsLoggedIn)
                {
                    MessageBox.Show("You are already logged in.", "Already Logged In", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                new Form1().ShowDialog(); // Open login
                return;
            }

            if (!AuthManager.IsLoggedIn)
            {
                if (AuthManager.IsLoggedIn)
                {
                    return;
                }
                MessageBox.Show("Please log in first.", "Authentication Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                new Form1().ShowDialog(); // Force login
                if (!AuthManager.IsLoggedIn) return; // User didn't log in
            }

            string role = UserSession.CurrentRole?.ToLower() ?? "unknown";

            switch (btn.Text)
            {
                case "Traveler Hub":
                    if (UserSession.IsTraveler && !UserSession.IsTourOperator && !UserSession.IsAdmin && UserSession.CurrentRole?.ToLower() != "provider")
                        new Form8().ShowDialog();
                    else
                        MessageBox.Show($"Access Denied: You are logged in as {role}. Only travelers can access Traveler Hub.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case "Tour Operator Hub":
                    if (!UserSession.IsTraveler && UserSession.IsTourOperator && !UserSession.IsAdmin && UserSession.CurrentRole?.ToLower() != "provider")
                        new TourOperatorMainForm().ShowDialog();
                    else
                        MessageBox.Show($"Access Denied: You are logged in as {role}. Only tour operators can access Tour Operator Hub.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case "Admin Hub":
                    if (!UserSession.IsTraveler && !UserSession.IsTourOperator && UserSession.IsAdmin && UserSession.CurrentRole?.ToLower() != "provider")
                        new AdminHubForm().ShowDialog();
                    else
                        MessageBox.Show($"Access Denied: You are logged in as {role}. Only admins can access Admin Hub.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case "Service Hub":
                    // Only Service Providers can access Service Hub
                    if (string.Equals(UserSession.CurrentRole?.Trim(), "service provider", StringComparison.OrdinalIgnoreCase))
                        new ServiceHubForm().ShowDialog();
                    else
                        MessageBox.Show($"Access Denied: You are logged in as {role}. Only service providers can access Service Hub.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }
    }
}