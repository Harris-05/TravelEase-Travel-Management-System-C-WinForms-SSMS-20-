using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace database_proj
{
    public class ServiceIntegrationForm : Form
    {
        private DataGridView dgvIntegrations;
        private TextBox txtSearch;
        private Button btnFilterPending;
        private Button btnAcceptSelected;
        private Button btnRejectSelected;

        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public ServiceIntegrationForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Service Integration";
            this.BackColor = ColorTranslator.FromHtml("#fddbc7");
            this.Size = new Size(1000, 647);
            this.StartPosition = FormStartPosition.CenterScreen;

            // === Custom Title Bar ===
            Panel titleBar = new Panel
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

            // Title on TitleBar
            Label titleLabel = new Label
            {
                Text = "Service Integration",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            titleBar.Controls.Add(titleLabel);

            // Close Button
            Button closeButton = new Button
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

            // Panel Title
            Label panelTitle = new Label
            {
                Text = "Manage Service Assignments",
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                Font = new Font("Garamond", 24, FontStyle.Bold),
                Location = new Point(18, 100),
                AutoSize = true
            };
            this.Controls.Add(panelTitle);

            // === Styled DataGridView ===
            dgvIntegrations = new DataGridView
            {
                Location = new Point(20, 245),
                Size = new Size(625, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
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
                AutoGenerateColumns = true
            };
            this.Controls.Add(dgvIntegrations);

            // Remove old event handlers and wire up new ones
            dgvIntegrations.CellClick += DgvIntegrations_CellClick;

            // === Logo ===
            PictureBox logoBox = new PictureBox
            {
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\blue logo.png"),
                Size = new Size(240, 240),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = ColorTranslator.FromHtml("#fddbc7"),
                Location = new Point(this.ClientSize.Width - 250, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Controls.Add(logoBox);

            // Load provider's services on form load
            LoadProviderServices();
        }

        // Placeholder for your SQL connection string
        private string connectionString = @"Server=LAIQ-VICTUS\SQLEXPRESS;Database=TravelEase;Trusted_Connection=True;";

        private void LoadProviderServices()
        {
            int providerId = UserSession.UserId;
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        cs.ClientServiceID,
                        cs.TripID,
                        cs.ServiceID,
                        sp.Title AS ServiceTitle,
                        sp.Description AS ServiceDescription,
                        cs.Status
                    FROM ClientServices cs
                    JOIN ServicesProvided sp ON cs.ServiceID = sp.ServiceID
                    WHERE sp.ProviderID = @ProviderID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProviderID", providerId);
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    var dt = new System.Data.DataTable();
                    adapter.Fill(dt);
                    dgvIntegrations.AutoGenerateColumns = true;
                    dgvIntegrations.DataSource = dt;

                    // Add Approve/Reject button columns if not already present
                    if (!dgvIntegrations.Columns.Contains("Approve"))
                    {
                        var approveBtn = new DataGridViewButtonColumn
                        {
                            Name = "Approve",
                            Text = "Approve",
                            UseColumnTextForButtonValue = true,
                            Width = 80
                        };
                        dgvIntegrations.Columns.Add(approveBtn);
                    }
                    if (!dgvIntegrations.Columns.Contains("Reject"))
                    {
                        var rejectBtn = new DataGridViewButtonColumn
                        {
                            Name = "Reject",
                            Text = "Reject",
                            UseColumnTextForButtonValue = true,
                            Width = 80
                        };
                        dgvIntegrations.Columns.Add(rejectBtn);
                    }
                }
            }
        }

        private void DgvIntegrations_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvIntegrations.Columns[e.ColumnIndex].Name == "Approve" || dgvIntegrations.Columns[e.ColumnIndex].Name == "Reject")
            {
                var clientServiceId = dgvIntegrations.Rows[e.RowIndex].Cells["ClientServiceID"].Value;
                string newStatus = dgvIntegrations.Columns[e.ColumnIndex].Name == "Approve" ? "approved" : "rejected";
                UpdateClientServiceStatus(clientServiceId, newStatus);
                // Refresh the grid
                LoadProviderServices();
            }
        }

        private void UpdateClientServiceStatus(object clientServiceId, string status)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = "UPDATE ClientServices SET Status = @Status WHERE ClientServiceID = @ClientServiceID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ClientServiceID", clientServiceId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}