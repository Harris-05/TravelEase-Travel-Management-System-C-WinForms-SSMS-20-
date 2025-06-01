using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace database_proj
{
    public class AdminHubForm : Form
    {
        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public AdminHubForm()
        {
            // Hide the default title bar
            this.FormBorderStyle = FormBorderStyle.None;
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Admin Hub";
            this.BackColor = Color.White;
            this.Size = new Size(1000, 650);
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
                Text = "Admin Hub",
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

            // === Left Panel with Image ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 607), // Adjusted height for custom title bar
                Location = new Point(0, 40),
                BackColor = Color.LightGray
            };

            PictureBox imageBox = new PictureBox()
            {
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
               // Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            leftPanel.Controls.Add(imageBox);
            this.Controls.Add(leftPanel);

            // === Right Panel with Buttons ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(500, 607),
                Location = new Point(500, 40),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            int buttonCount = 4;
            int buttonWidth = rightPanel.Width;
            int buttonHeight = rightPanel.Height / buttonCount;

            Font buttonFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Color buttonColor = ColorTranslator.FromHtml("#fddbc7");

            Button[] buttons = new Button[buttonCount];
            string[] buttonTexts = {
                "User and Operator",
                "Tour Categories",
                "Platform Analytics",
                "Review Moderation"
            };

            for (int i = 0; i < buttonCount; i++)
            {
                buttons[i] = new Button()
                {
                    Text = buttonTexts[i],
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = new Point(0, i * buttonHeight),
                    BackColor = buttonColor,
                    ForeColor = ColorTranslator.FromHtml("#006c83"),
                    FlatStyle = FlatStyle.Flat,
                    Font = buttonFont
                };
                rightPanel.Controls.Add(buttons[i]);
            }

            this.Controls.Add(rightPanel); 

            // === Button Events ===
            buttons[0].Click += (s, e) =>
            {
                FormUserManagement userForm = new FormUserManagement();
                userForm.ShowDialog();
            };

            buttons[1].Click += (s, e) =>
            {
                TourCategoriesManagementForm tourCategoriesForm = new TourCategoriesManagementForm();
                tourCategoriesForm.ShowDialog();
            };

            buttons[2].Click += (s, e) =>
            {
                PlatformAnalyticsForm analyticsForm = new PlatformAnalyticsForm();
                analyticsForm.ShowDialog();
            };

            buttons[3].Click += (s, e) =>
            {
                ReviewModerationForm reviewForm = new ReviewModerationForm();
                reviewForm.ShowDialog();
            };
        }
    }
}
