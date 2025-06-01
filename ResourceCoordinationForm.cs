using database_proj;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinFormsApp1;


//Ashher

namespace WinFormsApp1
{
    public class ResourceCoordinationForm : Form
    {

        // WinAPI functions for dragging the window
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;


        public ResourceCoordinationForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;

            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Resource Coordination";
            this.BackColor = Color.White;
            this.Size = new Size(1200, 647); // Extended form width
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
                Text = "Resource Coordination",
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

            // === Left Panel (Logo/Image) ===
            Panel leftPanel = new Panel()
            {
                Size = new Size(500, 647),
                Location = new Point(0, 0),
                BackColor = Color.LightGray
            };

            PictureBox logo = new PictureBox()
            {
                //Image = Image.FromFile("C:\\Users\\DELL\\Documents\\db_images\\left_logo.jpg"),
                // Image = Image.FromFile("C:\\Users\\EliteSeries\\Downloads\\TravelEase Logo (1).jpg"),
                Image = Image.FromFile("C:\\Users\\ahmed\\OneDrive\\Desktop\\TravelEase Media\\TravelEase Logo.jpg"), // Replace with actual path
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };

            leftPanel.Controls.Add(logo);
            this.Controls.Add(leftPanel);

            // === Right Panel (Grid & Title) ===
            Panel rightPanel = new Panel()
            {
                Size = new Size(700, 647),
                Location = new Point(500, 0),
                BackColor = ColorTranslator.FromHtml("#fddbc7") // Updated background color
            };

            // === Provider Selection Dropdown ===
            Label lblProviderSelect = new Label() { Text = "Select Provider:", Location = new Point(30, 140), AutoSize = true };
            ComboBox cbProviderSelect = new ComboBox() { Location = new Point(180, 138), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            // Populate provider dropdown
            string connectionString = "Data Source=LAIQ-VICTUS\\SQLEXPRESS;Initial Catalog=TravelEase;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string providerQuery = "SELECT ProviderID, OrganizationName FROM ServiceProviders";
                using (SqlCommand cmd = new SqlCommand(providerQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        cbProviderSelect.Items.Add($"{name} (ID: {id})");
                    }
                }
            }
            rightPanel.Controls.Add(lblProviderSelect);
            rightPanel.Controls.Add(cbProviderSelect);

            // === Replace the Grid View section with this ===
            Label providerSectionLabel = new Label()
            {
                Text = "Service Provider Information",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(30, 170),
                AutoSize = true
            };
            rightPanel.Controls.Add(providerSectionLabel);

            // --- Provider Input Fields ---
            Label lblOrgName = new Label() { Text = "Organization Name:", Location = new Point(30, 200), AutoSize = true };
            TextBox txtOrgName = new TextBox() { Location = new Point(180, 198), Width = 200 };

            Label lblProviderType = new Label() { Text = "Provider Type:", Location = new Point(30, 230), AutoSize = true };
            ComboBox cbProviderType = new ComboBox() { Location = new Point(180, 228), Width = 200 };
            cbProviderType.Items.AddRange(new string[] { "Hotel", "Transport", "Guide", "Meal" });

            Label lblContact = new Label() { Text = "Contact Info:", Location = new Point(30, 260), AutoSize = true };
            TextBox txtContact = new TextBox() { Location = new Point(180, 258), Width = 200 };

            rightPanel.Controls.AddRange(new Control[] { lblOrgName, txtOrgName, lblProviderType, cbProviderType, lblContact, txtContact });

            // === Service Input Fields ===
            Label serviceSectionLabel = new Label()
            {
                Text = "Service Provided Information",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(30, 300),
                AutoSize = true
            };
            rightPanel.Controls.Add(serviceSectionLabel);

            Label lblServiceType = new Label() { Text = "Service Type:", Location = new Point(30, 330), AutoSize = true };
            ComboBox cbServiceType = new ComboBox() { Location = new Point(180, 328), Width = 200 };
            cbServiceType.Items.AddRange(new string[] { "Hotel", "Transport", "Guide", "Meal" });

            Label lblTitle = new Label() { Text = "Title:", Location = new Point(30, 360), AutoSize = true };
            TextBox txtTitle = new TextBox() { Location = new Point(180, 358), Width = 200 };

            Label lblDescription = new Label() { Text = "Description:", Location = new Point(30, 390), AutoSize = true };
            TextBox txtDescription = new TextBox() { Location = new Point(180, 388), Width = 200, Height = 60, Multiline = true };

            Label lblPrice = new Label() { Text = "Price:", Location = new Point(30, 460), AutoSize = true };
            TextBox txtPrice = new TextBox() { Location = new Point(180, 458), Width = 200 };

            Label lblAvailability = new Label() { Text = "Availability:", Location = new Point(30, 490), AutoSize = true };
            TextBox txtAvailability = new TextBox() { Location = new Point(180, 488), Width = 200 };

            Label lblTripName = new Label() { Text = "Trip Name:", Location = new Point(30, 520), AutoSize = true };
            ComboBox cbTripName = new ComboBox() { Location = new Point(180, 518), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            // Populate trip name dropdown
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string tripQuery = "SELECT Title FROM Trips";
                using (SqlCommand cmd = new SqlCommand(tripQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string title = reader.GetString(0);
                        cbTripName.Items.Add(title);
                    }
                }
            }
            rightPanel.Controls.Add(lblTripName);
            rightPanel.Controls.Add(cbTripName);

            rightPanel.Controls.AddRange(new Control[]
            {
                     lblServiceType, cbServiceType,
                     lblTitle, txtTitle,
                     lblDescription, txtDescription,
                     lblPrice, txtPrice,
                     lblAvailability, txtAvailability
            });

            // === Submit Button ===
            Button btnSubmit = new Button()
            {
                Text = "Submit",
                Location = new Point(180, 560),
                Size = new Size(100, 35),
                BackColor = ColorTranslator.FromHtml("#006c83"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += (s, e) =>
            {
                try
                {
                    // Get selected provider ID from dropdown
                    if (cbProviderSelect.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a provider.");
                        return;
                    }
                    string selected = cbProviderSelect.SelectedItem.ToString();
                    int idStart = selected.LastIndexOf("ID: ") + 4;
                    int idEnd = selected.LastIndexOf(")");
                    string idStr = selected.Substring(idStart, idEnd - idStart);
                    if (!int.TryParse(idStr, out int providerID))
                    {
                        MessageBox.Show("Could not determine Provider ID.");
                        return;
                    }

                    // --- Get Provider Info ---
                    string orgName = txtOrgName.Text.Trim();
                    string providerType = cbProviderType.SelectedItem?.ToString() ?? "";
                    string contact = txtContact.Text.Trim();

                    // --- Get Service Info ---
                    string serviceType = cbServiceType.SelectedItem?.ToString() ?? "";
                    string title = txtTitle.Text.Trim();
                    string description = txtDescription.Text.Trim();
                    decimal price = decimal.Parse(txtPrice.Text.Trim());
                    int availability = int.Parse(txtAvailability.Text.Trim());

                    // Get Trip Name from dropdown
                    string tripName = cbTripName.SelectedItem?.ToString() ?? "";
                    if (string.IsNullOrEmpty(tripName))
                    {
                        MessageBox.Show("Please select the trip name.");
                        return;
                    }

                    int tripId = -1;
                    int serviceId = -1;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // 1. Get TripID from trip name
                        string getTripIdQuery = "SELECT TripID FROM Trips WHERE Title = @TripName";
                        using (SqlCommand cmd = new SqlCommand(getTripIdQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@TripName", tripName);
                            object result = cmd.ExecuteScalar();
                            if (result == null)
                            {
                                MessageBox.Show("Trip not found.");
                                return;
                            }
                            tripId = Convert.ToInt32(result);
                        }

                        // 2. Only insert into ServiceProviders if provider does not already exist
                        string checkProviderQuery = "SELECT COUNT(*) FROM ServiceProviders WHERE ProviderID = @ProviderID";
                        using (SqlCommand cmd = new SqlCommand(checkProviderQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProviderID", providerID);
                            int count = (int)cmd.ExecuteScalar();
                            if (count == 0)
                            {
                                string insertProviderQuery = @"
                                INSERT INTO ServiceProviders (ProviderID, ProviderType, OrganizationName, ContactInfo) 
                                VALUES (@ProviderID, @ProviderType, @OrgName, @Contact)";
                                using (SqlCommand insertCmd = new SqlCommand(insertProviderQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@ProviderID", providerID);
                                    insertCmd.Parameters.AddWithValue("@ProviderType", providerType);
                                    insertCmd.Parameters.AddWithValue("@OrgName", orgName);
                                    insertCmd.Parameters.AddWithValue("@Contact", contact);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // 3. Insert into ServicesProvided and get ServiceID
                        string insertServiceQuery = @"
                        INSERT INTO ServicesProvided (ProviderID, ServiceType, Title, Description, Price, Availability)
                        OUTPUT INSERTED.ServiceID
                        VALUES (@ProviderID, @ServiceType, @Title, @Desc, @Price, @Avail)";
                        using (SqlCommand cmd = new SqlCommand(insertServiceQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProviderID", providerID);
                            cmd.Parameters.AddWithValue("@ServiceType", serviceType);
                            cmd.Parameters.AddWithValue("@Title", title);
                            cmd.Parameters.AddWithValue("@Desc", description);
                            cmd.Parameters.AddWithValue("@Price", price);
                            cmd.Parameters.AddWithValue("@Avail", availability);
                            serviceId = (int)cmd.ExecuteScalar();
                        }

                        // 4. Insert into ClientServices
                        string insertClientServiceQuery = @"
                        INSERT INTO ClientServices (TripID, ServiceID, Status)
                        VALUES (@TripID, @ServiceID, 'Pending')";
                        using (SqlCommand cmd = new SqlCommand(insertClientServiceQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@TripID", tripId);
                            cmd.Parameters.AddWithValue("@ServiceID", serviceId);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Service linked to trip successfully!");
                    }

                }
                catch (FormatException)
                {
                    MessageBox.Show("Price and Availability must be valid numbers.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Submission Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };


            rightPanel.Controls.Add(btnSubmit);

            this.Controls.Add(rightPanel);
        }
    }
}