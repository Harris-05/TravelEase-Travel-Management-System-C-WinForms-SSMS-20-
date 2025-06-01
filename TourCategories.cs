using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace database_proj
{
    public partial class TourCategoriesManagementForm : Form
    {
        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
        private int? editingCategoryId = null;

        public TourCategoriesManagementForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildTourCategoriesManagementUI();
        }

        private void BuildTourCategoriesManagementUI()
        {
            this.Text = "Tour Categories Management";
            this.BackColor = ColorTranslator.FromHtml("#fddbc7");
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

            // === Left Panel ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 647),
                Location = new Point(0, 0),
                BackColor = ColorTranslator.FromHtml("#006c83")
            };

            PictureBox backgroundImg = new PictureBox()
            {
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
                Size = new Size(500, 600),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            Label quote = new Label()
            {
                Text = "OVERSEE TOUR CATEGORIES\nMANAGE AND UPDATE",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(360, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 350),
                BackColor = Color.Transparent
            };

            leftPanel.Controls.Add(backgroundImg);
            leftPanel.Controls.Add(quote);
            this.Controls.Add(leftPanel);

            // === Right Panel ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(500, 647),
                Location = new Point(500, 0),
                BackColor = ColorTranslator.FromHtml("#fddbc7")
            };

            Label header = new Label()
            {
                Text = "Tour Categories Management",
                Font = new Font("Garamond", 18, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                Location = new Point(20, 82), // Moved 50px down
                AutoSize = true
            };


            Label titleLabel = new Label()
            {
                Text = "Tour Categories",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            titleBar.Controls.Add(titleLabel);

            DataGridView categoriesGrid = new DataGridView()
            {
                Location = new Point(20, 140), // Moved 50px down
                Size = new Size(450, 350),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ColorTranslator.FromHtml("#006c83"),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = ColorTranslator.FromHtml("#23d3f9")
                },
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            categoriesGrid.Columns.Add("ID", "ID");
            categoriesGrid.Columns.Add("CategoryName", "Category Name");
            categoriesGrid.Columns.Add("TripDescriptions", "Trip Descriptions");

            categoriesGrid.Columns["ID"].Width = 40;
            categoriesGrid.Columns["CategoryName"].Width = 140;
            categoriesGrid.Columns["TripDescriptions"].Width = 400;

            LoadCategories(categoriesGrid);

            rightPanel.Controls.Add(header);
            rightPanel.Controls.Add(categoriesGrid);
            this.Controls.Add(rightPanel);
        }

        private void LoadCategories(DataGridView grid)
        {
            grid.Rows.Clear();
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        c.CategoryID, 
                        c.CategoryName, 
                        STRING_AGG(CAST(t.Description AS varchar(max)), ' | ') AS TripDescriptions
                    FROM TourCategories c
                    INNER JOIN Trips t ON c.CategoryName = t.Category
                    GROUP BY c.CategoryID, c.CategoryName
                ";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            grid.Rows.Add(reader["CategoryID"], reader["CategoryName"], reader["TripDescriptions"] == DBNull.Value ? "" : reader["TripDescriptions"]);
                        }
                    }
                }
            }
        }


    }
}
