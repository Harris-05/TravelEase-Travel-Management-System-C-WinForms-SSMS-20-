using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace database_proj
{
    public partial class FormUserManagement : Form
    {
        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

        public FormUserManagement()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildUserManagementUI();
        }

        private void BuildUserManagementUI()
        {
            this.Text = "User and Operator Management";
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

            Button closeButton = new Button()
            {
                Text = "Close",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 53, 69),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(70, 30),
                Location = new Point(this.Width - 80, 10),  // Moved 50px down
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
                Text = "MANAGE USERS WITH EASE\nAPPROVE OR REJECT REQUESTS",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(360, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 350 + 50),  // Moved 50px down
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
                Text = "User and Operator Management",
                Font = new Font("Garamond", 18, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                Location = new Point(20, 32 + 50),  // Moved 50px down
                AutoSize = true
            };

            Label titleLabel = new Label()
            {
                Text = "User and Operator Management",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            titleBar.Controls.Add(titleLabel);

            DataGridView userGrid = new DataGridView()
            {
                Location = new Point(20, 90 + 50),  // Moved 50px down
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

            userGrid.Columns.Add("ID", "ID");
            userGrid.Columns.Add("FullName", "Full Name");
            userGrid.Columns.Add("Role", "Role");
            userGrid.Columns.Add("IsApproved", "Approved");

            userGrid.Columns["ID"].Width = 40;
            userGrid.Columns["FullName"].Width = 120;
            userGrid.Columns["Role"].Width = 70;
            userGrid.Columns["IsApproved"].Width = 80;

            // Approve Button (green)
            DataGridViewButtonColumn approveBtn = new DataGridViewButtonColumn
            {
                Name = "Approve",
                Text = "Approve",
                UseColumnTextForButtonValue = true,
                Width = 70,
                FlatStyle = FlatStyle.Flat
            };
            userGrid.Columns.Add(approveBtn);
            // Set green color for Approve button
            userGrid.CellFormatting += (s, e) =>
            {
                if (userGrid.Columns[e.ColumnIndex].Name == "Approve" && e.RowIndex >= 0)
                {
                    e.CellStyle.BackColor = Color.FromArgb(40, 167, 69);
                    e.CellStyle.ForeColor = Color.White;
                }
                if (userGrid.Columns[e.ColumnIndex].Name == "Reject" && e.RowIndex >= 0)
                {
                    e.CellStyle.BackColor = Color.FromArgb(220, 53, 69);
                    e.CellStyle.ForeColor = Color.White;
                }
            };

            // Reject Button (red)
            DataGridViewButtonColumn rejectBtn = new DataGridViewButtonColumn
            {
                Name = "Reject",
                Text = "Reject",
                UseColumnTextForButtonValue = true,
                Width = 70,
                FlatStyle = FlatStyle.Flat
            };
            userGrid.Columns.Add(rejectBtn);

            // Load users from database
            LoadUsers(userGrid);

            userGrid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && (e.ColumnIndex == userGrid.Columns["Approve"].Index || e.ColumnIndex == userGrid.Columns["Reject"].Index))
                {
                    int userId = Convert.ToInt32(userGrid.Rows[e.RowIndex].Cells["ID"].Value);
                    bool approve = e.ColumnIndex == userGrid.Columns["Approve"].Index;
                    UpdateUserApproval(userId, approve);
                    LoadUsers(userGrid);
                }
            };

            rightPanel.Controls.Add(header);
            rightPanel.Controls.Add(userGrid);
            this.Controls.Add(rightPanel);
        }

        private void LoadUsers(DataGridView userGrid)
        {
            userGrid.Rows.Clear();
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                // Exclude users with the 'admin' role
                string query = @"SELECT UserID, FullName, Role, CASE WHEN IsApproved = 0 THEN 'Unapproved' ELSE 'Approved' END AS ApprovedStatus, IsApproved FROM Users WHERE LOWER(Role) <> 'admin'";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userGrid.Rows.Add(reader["UserID"], reader["FullName"], reader["Role"], reader["ApprovedStatus"]);
                        }
                    }
                }
            }
        }

        private void UpdateUserApproval(int userId, bool approve)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = "UPDATE Users SET IsApproved = @IsApproved WHERE UserID = @UserID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IsApproved", approve ? 1 : 0);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
