using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public class Form3 : Form
    {
        // Drag support
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Form3()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            BuildTripSearchForm();
        }

        private void BuildTripSearchForm()
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
                Text = "TravelEase - Trip Search",
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

            // === Left Panel (Image) ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(300, 550),
                Location = new Point(0, 50),
                BackColor = ColorTranslator.FromHtml("#006c83")
            };
            PictureBox imageBox = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo.jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };
            leftPanel.Controls.Add(imageBox);
            this.Controls.Add(leftPanel);

            // === Right Panel (Form Controls) ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(600, 550),
                Location = new Point(300, 50),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };
            this.Controls.Add(rightPanel);

            Font labelFont = new Font("Segoe UI", 10);
            Font inputFont = new Font("Segoe UI", 10);

            int startY = 20;
            int spacing = 45;

            Label heading = new Label()
            {
                Text = " Book Your Trip",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                Location = new Point(20, startY),
                AutoSize = true
            };
            rightPanel.Controls.Add(heading);
            startY += 50;

            void AddField(string labelText, Control input)
            {
                Label lbl = new Label()
                {
                    Text = labelText,
                    Location = new Point(20, startY),
                    Font = labelFont,
                    AutoSize = true
                };
                input.Location = new Point(150, startY - 3);
                input.Width = 250;
                input.Font = inputFont;
                rightPanel.Controls.Add(lbl);
                rightPanel.Controls.Add(input);
                startY += spacing;
            }

            AddField("Destination:", new TextBox());
            AddField("Date:", new DateTimePicker());

            ComboBox priceBox = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
            priceBox.Items.AddRange(new string[] { "$100 - $500", "$500 - $1000", "$1000+" });
            AddField("Price Range:", priceBox);

            ComboBox activityBox = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
            activityBox.Items.AddRange(new string[] { "Hiking", "Cultural Tours", "Relaxation", "Adventure" });
            AddField("Activity Type:", activityBox);

            NumericUpDown groupBox = new NumericUpDown() { Minimum = 1, Maximum = 20 };
            AddField("Group Size:", groupBox);

            Button searchBtn = new Button()
            {
                Text = "Search",
                Location = new Point(150, startY),
                Width = 250,
                Height = 35,
                BackColor = ColorTranslator.FromHtml("#006c83"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            searchBtn.FlatAppearance.BorderSize = 0;
            rightPanel.Controls.Add(searchBtn);
            startY += 50;

            ListBox tripList = new ListBox()
            {
                Location = new Point(20, startY + 10),
                Size = new Size(550, 200),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            rightPanel.Controls.Add(tripList);
        }
    }
}