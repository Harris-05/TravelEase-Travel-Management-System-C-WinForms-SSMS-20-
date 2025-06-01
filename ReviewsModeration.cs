using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace database_proj
{
    public class ReviewModerationForm : Form
    {
        private DataGridView dgvReviews;
        private TextBox txtSearch;
        private Button btnFilterInappropriate;

        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;


        public ReviewModerationForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Review Moderation";
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

            //custom title bar finish



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


            // === Title Label ===
            Label lblTitle = new Label()
            {
                Text = "Review Moderation Panel",
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                Font = new Font("Garamond", 24, FontStyle.Bold),
                Location = new Point(18, 50 + titleBar.Height),  // Adjust Y-axis position
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // === Search Label ===
            Label lblSearch = new Label()
            {
                Text = "Search by Username or Review ID:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 160 + titleBar.Height), // Adjust Y-axis position
                AutoSize = true
            };
            this.Controls.Add(lblSearch);

            // === Search TextBox ===
            txtSearch = new TextBox()
            {
                Location = new Point(300, 157 + titleBar.Height), // Adjust Y-axis position
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtSearch);

            Label titleLabel = new Label()
            {
                Text = "Reviews Moderation",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 10)
            };
            titleBar.Controls.Add(titleLabel);

            // === Search Button ===
            btnFilterInappropriate = new Button()
            {
                Text = "Search",
                Location = new Point(520, 155 + titleBar.Height), // Adjust Y-axis position
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(btnFilterInappropriate);

            // Add event handler for Search button
            btnFilterInappropriate.Click += BtnSearch_Click;

            dgvReviews = new DataGridView()
            {
                Location = new Point(20, 225 + titleBar.Height), // Adjust Y-axis position
                Size = new Size(900, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = ColorTranslator.FromHtml("#006c83"),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = ColorTranslator.FromHtml("#23d3f9"),
                    SelectionForeColor = Color.Black
                },
                EnableHeadersVisualStyles = false
            };
            this.Controls.Add(dgvReviews);

            // Add Delete button column as a button (no icon)
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.Name = "Delete";
            deleteColumn.HeaderText = "";
            deleteColumn.Text = "Delete";
            deleteColumn.UseColumnTextForButtonValue = true;
            deleteColumn.Width = 60;
            dgvReviews.Columns.Add(deleteColumn);

            dgvReviews.CellClick += DgvReviews_CellClick;

           

            // === Logo ===
            PictureBox logoBox = new PictureBox()
            {
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\blue logo.png"),
                Size = new Size(240, 240),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = ColorTranslator.FromHtml("#fddbc7"),
                Location = new Point(this.ClientSize.Width - 250, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Controls.Add(logoBox);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            string query = @"SELECT r.ReviewID, r.TravelerID AS UserID, u.FullName AS Username, r.TripID AS TourID, r.Rating, r.Comment, r.CreatedAt AS Timestamp
                             FROM Reviews r
                             JOIN Users u ON r.TravelerID = u.UserID
                             WHERE (@searchText = '' OR u.FullName LIKE @searchPattern OR r.ReviewID = @reviewId)";
            int reviewId;
            bool isReviewId = int.TryParse(searchText, out reviewId);
            string searchPattern = "%" + searchText + "%";
            using (var conn = new System.Data.SqlClient.SqlConnection("Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;"))
            using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@searchText", searchText);
                cmd.Parameters.AddWithValue("@searchPattern", searchPattern);
                cmd.Parameters.AddWithValue("@reviewId", isReviewId ? reviewId : -1);
                var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                var dt = new System.Data.DataTable();
                adapter.Fill(dt);

                // Remove existing delete column if present
                if (dgvReviews.Columns.Contains("Delete"))
                    dgvReviews.Columns.Remove("Delete");

                dgvReviews.DataSource = dt;

                // Add Delete button column after setting DataSource
                DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
                deleteColumn.Name = "Delete";
                deleteColumn.HeaderText = "";
                deleteColumn.Text = "Delete";
                deleteColumn.UseColumnTextForButtonValue = true;
                deleteColumn.Width = 60;
                dgvReviews.Columns.Add(deleteColumn);

                // Set button style to red
                foreach (DataGridViewRow row in dgvReviews.Rows)
                {
                    if (row.Cells["Delete"] is DataGridViewButtonCell btnCell)
                    {
                        btnCell.Style.BackColor = Color.Red;
                        btnCell.Style.ForeColor = Color.White;
                    }
                }
            }
        }

        private void DgvReviews_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvReviews.Columns[e.ColumnIndex].Name == "Delete")
            {
                var reviewId = dgvReviews.Rows[e.RowIndex].Cells["ReviewID"].Value;
                var confirm = MessageBox.Show("Are you sure you want to delete this review?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    using (var conn = new System.Data.SqlClient.SqlConnection("Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;"))
                    using (var cmd = new System.Data.SqlClient.SqlCommand("DELETE FROM Reviews WHERE ReviewID = @ReviewID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ReviewID", reviewId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    dgvReviews.Rows.RemoveAt(e.RowIndex);
                }
            }
        }
    }
}
