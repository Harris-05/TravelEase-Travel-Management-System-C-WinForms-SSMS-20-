using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace database_proj
{
    public class ServiceListingForm : Form
    {
        private DataGridView dgvServices;
        private Button btnAddService;
        private string connectionString = "Data Source=Laiq-Victus\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";

        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        // Declare new controls for adding a service
        private TextBox txtProviderName;
        private TextBox txtProviderType;
        private TextBox txtTitle;
        private TextBox txtDescription;
        private TextBox txtPrice;
        private TextBox txtAvailability;

        public ServiceListingForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            BuildUI();
            LoadServices();
        }

        private void BuildUI()
        {
            this.Text = "Service Listing";
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

            // Title on Title Bar
            Label titleLabel = new Label
            {
                Text = "Service Listing",
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
                Text = "List and Manage Services",
                ForeColor = ColorTranslator.FromHtml("#006c83"),
                Font = new Font("Garamond", 24, FontStyle.Bold),
                Location = new Point(18, 120),  // Adjusted Y-axis position
                AutoSize = true
            };
            this.Controls.Add(panelTitle);

            // === Add Service Controls ===
            int addTop = 180;
            int labelWidth = 90;
            int boxWidth = 120;
            int spacing = 10;
            int left = 20;

            // Provider Name
            Label lblProviderName = new Label { Text = "Provider Name:", Location = new Point(left, addTop + 5), Width = labelWidth };
            this.Controls.Add(lblProviderName);
            txtProviderName = new TextBox { Location = new Point(left + labelWidth, addTop), Width = boxWidth };
            this.Controls.Add(txtProviderName);
            left += labelWidth + boxWidth + spacing;

            // Provider Type
            Label lblProviderType = new Label { Text = "Type:", Location = new Point(left, addTop + 5), Width = labelWidth - 40 };
            this.Controls.Add(lblProviderType);
            txtProviderType = new TextBox { Location = new Point(left + labelWidth - 40, addTop), Width = boxWidth };
            this.Controls.Add(txtProviderType);
            left += labelWidth - 40 + boxWidth + spacing;

            // Title
            Label lblTitle = new Label { Text = "Title:", Location = new Point(left, addTop + 5), Width = labelWidth - 40 };
            this.Controls.Add(lblTitle);
            txtTitle = new TextBox { Location = new Point(left + labelWidth - 40, addTop), Width = boxWidth };
            this.Controls.Add(txtTitle);
            left += labelWidth - 40 + boxWidth + spacing;

            // Description
            Label lblDescription = new Label { Text = "Description:", Location = new Point(left, addTop + 5), Width = labelWidth };
            this.Controls.Add(lblDescription);
            txtDescription = new TextBox { Location = new Point(left + labelWidth, addTop), Width = boxWidth };
            this.Controls.Add(txtDescription);
            left += labelWidth + boxWidth + spacing;

            // Price
            Label lblPrice = new Label { Text = "Price:", Location = new Point(left, addTop + 5), Width = labelWidth - 40 };
            this.Controls.Add(lblPrice);
            txtPrice = new TextBox { Location = new Point(left + labelWidth - 40, addTop), Width = boxWidth };
            this.Controls.Add(txtPrice);
            left += labelWidth - 40 + boxWidth + spacing;

            // Availability
            Label lblAvailability = new Label { Text = "Availability:", Location = new Point(left, addTop + 5), Width = labelWidth };
            this.Controls.Add(lblAvailability);
            txtAvailability = new TextBox { Location = new Point(left + labelWidth, addTop), Width = boxWidth };
            this.Controls.Add(txtAvailability);
            left += labelWidth + boxWidth + spacing;

            // Add New Service Button (smaller, far right)
            btnAddService = new Button
            {
                Text = "Add",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#fddbc7"),
                BackColor = ColorTranslator.FromHtml("#006c83"),
                Size = new Size(80, 30),
                Location = new Point(left, addTop - 2),
                FlatStyle = FlatStyle.Flat
            };
            btnAddService.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnAddService);

            // === Styled DataGridView ===
            dgvServices = new DataGridView
            {
                Location = new Point(20, 245),  // Adjusted Y-axis position
                Size = new Size(875, 280),
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
            this.Controls.Add(dgvServices);

            // Adjust the width of the ID column
            /*dgvServices.Columns["ServiceID"].Width = 40; // Reduced width of ID column
            dgvServices.Columns["ProviderName"].Width = 101; // Reduced width of ID column
            dgvServices.Columns["ServiceType"].Width = 101; // Reduced width of ID column*/


            var deleteBtn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                Text = "🗑️",
                Width = 70,
                UseColumnTextForButtonValue = true
            };
            dgvServices.Columns.Add(deleteBtn);

            // Event Handlers
          

            // === Logo ===
            PictureBox logoBox = new PictureBox
            {
                //Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\blue logo.png"),
                Size = new Size(240, 240),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = ColorTranslator.FromHtml("#fddbc7"),
                Location = new Point(this.ClientSize.Width - 250, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Controls.Add(logoBox);
        }

        private void LoadServices()
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        s.ServiceID,
                        sp.OrganizationName AS Provider,
                        sp.ProviderType AS Type,
                        s.Title,
                        s.Description,
                        s.Price,
                        s.Availability
                    FROM ServicesProvided s
                    JOIN ServiceProviders sp ON s.ProviderID = sp.ProviderID
                ";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    var dt = new System.Data.DataTable();
                    adapter.Fill(dt);
                    dgvServices.AutoGenerateColumns = true;
                    dgvServices.DataSource = dt;
                }
            }
        }

        private void DeleteService(object serviceId)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = "DELETE FROM ServicesProvided WHERE ServiceID = @ServiceID";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ServiceID", serviceId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AddNewService()
        {
            string providerName = txtProviderName.Text.Trim();
            string providerType = txtProviderType.Text.Trim();
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            string priceText = txtPrice.Text.Trim();
            string availabilityText = txtAvailability.Text.Trim();

            if (string.IsNullOrEmpty(providerName) || string.IsNullOrEmpty(providerType) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(priceText) || string.IsNullOrEmpty(availabilityText))
            {
                MessageBox.Show("Please fill in all required fields (Provider Name, Type, Title, Price, Availability).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Invalid price value.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(availabilityText, out int availability))
            {
                MessageBox.Show("Invalid availability value.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int providerId = -1;
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string lookupQuery = "SELECT ProviderID FROM ServiceProviders WHERE OrganizationName = @OrganizationName AND ProviderType = @ProviderType";
                using (var cmd = new System.Data.SqlClient.SqlCommand(lookupQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@OrganizationName", providerName);
                    cmd.Parameters.AddWithValue("@ProviderType", providerType);
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Provider not found. Please check the Provider Name and Type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    providerId = Convert.ToInt32(result);
                }
            }

            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO ServicesProvided (ProviderID, ServiceType, Title, Description, Price, Availability) VALUES (@ProviderID, @ServiceType, @Title, @Description, @Price, @Availability)";
                using (var cmd = new System.Data.SqlClient.SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ProviderID", providerId);
                    cmd.Parameters.AddWithValue("@ServiceType", providerType);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Availability", availability);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Service added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadServices();
            // Optionally clear fields
            txtTitle.Text = txtDescription.Text = txtPrice.Text = txtAvailability.Text = string.Empty;
        }
    }
}
