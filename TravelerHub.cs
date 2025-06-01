using database_proj;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form8 : Form
    {
        // WinAPI to enable draggable title bar
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private int userID = UserSession.UserId;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Form8(/*int loggedInUserID*/)
        {
            BuildUI();

        }

        private void BuildUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Text = "Traveller Hub";
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
                Text = "Traveler Hub",
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
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 607),
                Location = new Point(0, 43),
                BackColor = Color.LightGray
            };

            PictureBox imageBox = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                  Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"), // Update path if needed
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            leftPanel.Controls.Add(imageBox);
            this.Controls.Add(leftPanel);

            // === Right Panel with Buttons ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(500, 607),
                Location = new Point(500, 43),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            Button[] buttons = new Button[5];
            string[] buttonTexts = {
                "Trip Dashboard",
                "Digital Travel Pass",
                "Submit Reviews",
                "My Profile",
                "Bookings Management"
            };

            Font buttonFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Color buttonColor = ColorTranslator.FromHtml("#fddbc7");

            int buttonCount = buttonTexts.Length;
            int buttonWidth = rightPanel.Width;
            int buttonHeight = rightPanel.Height / buttonCount;

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
                Form formToOpen = new Form4();
                formToOpen.ShowDialog();
            };

            buttons[1].Click += (s, e) =>
            {
                Form formToOpen = new Form5();
                formToOpen.ShowDialog();
            };

            buttons[2].Click += (s, e) =>
            {
                Form formToOpen = new Form6();
                formToOpen.ShowDialog();
            };

            buttons[3].Click += (s, e) =>
            {
                Form formToOpen = new Form7();
                formToOpen.ShowDialog();
            };
            buttons[4].Click += (s, e) =>
            {
                Form formToOpen = new Booking();
                formToOpen.ShowDialog();
            };
        }
    }
}
