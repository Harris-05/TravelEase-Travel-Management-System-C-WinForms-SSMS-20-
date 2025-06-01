using database_proj;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinFormsApp1; // Add this if your other forms like ResourceCoordinationForm are in this namespace

//Ashher

namespace TourPlannerApp

{
    public class TourOperatorMainForm : Form
    {
        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public TourOperatorMainForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;

            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Tour Operator Hub";
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
                Text = "Tour Operator Hub",
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

            // === Left Panel with Image ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 600),
                Location = new Point(0, 50), // Moved down by 50px
                BackColor = Color.LightGray
            };

            PictureBox imageBox = new PictureBox()
            {
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
                //Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };

            leftPanel.Controls.Add(imageBox);
            this.Controls.Add(leftPanel);

            // === Right Panel with Buttons ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(500, 600),
                Location = new Point(500, 50), // Moved down by 50px
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            int buttonCount = 6;
            int buttonWidth = rightPanel.Width;
            int buttonHeight = rightPanel.Height / buttonCount;

            Font buttonFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Color buttonColor = ColorTranslator.FromHtml("#fddbc7");

            string[] buttonTexts = {
                " Operator Registration and Login",
                " Trip Creation and Management",
                " Trips Listing",
                " Resource Coordination",
                " Booking Management",
                " Performance Analytics"
            };

            for (int i = 0; i < buttonCount; i++)
            {
                Button btn = new Button()
                {
                    Text = buttonTexts[i],
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = new Point(0, i * buttonHeight), // Moved down by 50px
                    BackColor = buttonColor,
                    ForeColor = ColorTranslator.FromHtml("#006c83"),
                    FlatStyle = FlatStyle.Flat,
                    Font = buttonFont
                };

                int index = i;
                btn.Click += (s, e) =>
                {
                    Form newForm = null;

                    switch (index)
                    {
                        case 0:
                            newForm = new OperatorRegistrationForm();
                            break;
                        case 1:
                            newForm = new TripManagementForm();
                            break;
                        case 2:
                            newForm = new TripsListingForm();
                            break;
                        case 3:
                            newForm = new ResourceCoordinationForm();
                            break;
                        case 4:
                            newForm = new BookingManagementPanel();
                            break;
                        case 5:
                            newForm = new PerformanceAnalyticsForm();
                            break;
                    }

                    if (newForm != null)
                    {
                        newForm.FormClosed += (sender, args) => this.Show(); // Show main form again on close
                        this.Hide();
                        newForm.Show();
                    }
                };

                rightPanel.Controls.Add(btn);
            }

            this.Controls.Add(rightPanel);
        }
    }
}
