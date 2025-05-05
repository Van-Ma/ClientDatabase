using System;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using OfficeOpenXml;
using System.IO;
using System.Diagnostics;
using System.Linq;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Reflection.Emit;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Drawing;
using Label = System.Windows.Forms.Label;


namespace clientsys
{
    public partial class newclientUser : UserControl
    {
        private string connectionString;
        private Main mainForm;
        private int previousWidth = 0;
        private int previousHeight;
        public newclientUser(Main mainform)
        {
            InitializeComponent();
            this.mainForm = mainform;

            addstatesComboBox();
            previousWidth = this.ClientSize.Width;
            previousHeight = this.Height;

            LoadConnectionString();

            // title case

            fnameBox.Text = ToTitleCase(fnameBox.Text);
            lnameBox.Text = ToTitleCase(lnameBox.Text);
            staddressBox.Text = ToTitleCase(staddressBox.Text);
            cityBox.Text = ToTitleCase(cityBox.Text);
            aptBox.Text = ToTitleCase(aptBox.Text);

            string firstName = fnameBox.Text.Trim();
            string lastName = lnameBox.Text.Trim();
            string phone = phoneBox.Text.Trim();
            string streetAddress = staddressBox.Text.Trim();
            string city = cityBox.Text.Trim();
            string state = stateBox.Text.Trim();
            string zipCode = zipcodeBox.Text.Trim();
            string apartment = aptBox.Text.Trim();
            string email = emailBox.Text.Trim();

          

        }

        private string LoadConnectionString()
        {
            try
            {
                string configFilePath = Path.Combine(Application.StartupPath, "config.json");

                if (File.Exists(configFilePath))
                {
                    string jsonContent = File.ReadAllText(configFilePath);
                    dynamic config = JsonConvert.DeserializeObject(jsonContent);

                    string server = config.Database.Server;
                    string database = config.Database.Database;
                    string user = config.Database.User;
                    string password = config.Database.Password;

                    return $"Server={server};Database={database};User={user};Password={password};";
                }
                else
                {
                    MessageBox.Show("Configuration file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public void UpdateClientDetails(string firstName, string lastName, string phone, string streetAddress, string city, string state, string zipCode, string apartment, string email)
        {
            fnameBox.Text = firstName;
            lnameBox.Text = lastName;
            phoneBox.Text = phone;
            staddressBox.Text = streetAddress;
            cityBox.Text = city;
            stateBox.Text = state;
            zipcodeBox.Text = zipCode;
            aptBox.Text = apartment;
            emailBox.Text = email;
        }
        private void addstatesComboBox()
        {
            string[] states = {
            "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA",
            "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD",
            "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ",
            "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC",
            "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY" };

            foreach (string state in states)
            {
                stateBox.Items.Add(state);
            }

            stateBox.SelectedIndex = 0;

        }
        private void ClearTextBoxes()
        {
            fnameBox.Clear();
            lnameBox.Clear();
            phoneBox.Clear();
            staddressBox.Clear();
            cityBox.Clear();
            stateBox.Text = "";
            zipcodeBox.Clear();
            aptBox.Clear();
            emailBox.Clear();
        }
        private void ClearErrorBoxes()
        {
            fnameErrorBox.Clear();
            lnameErrorBox.Clear();
            phoneErrorBox.Clear();
            staddressErrorBox.Clear();
            cityErrorBox.Clear();
            stateErrorBox.Clear();
            zipcodeErrorBox.Clear();
            aptErrorBox.Clear();
            emailErrorBox.Clear();
        }
        private string ToTitleCase(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
        private bool ValidateTextboxes()
        {
            Regex letters = new Regex("^[a-zA-Z ]+$"); //allows spaces
            Regex phone = new Regex(@"^\d{3}-\d{3}-\d{4}$");
            Regex address = new Regex(@"^\d+\s+[a-zA-Z\s]+$");
            Regex numbers = new Regex("^[0-9]+$");
            Regex email = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            bool isValid = true;

            // First name
            if (!letters.IsMatch(fnameBox.Text.Trim().Replace(" ", "")))
            {
                fnameErrorBox.Text = "Invalid name.";
                isValid = false;
            }
            else
            {
                fnameErrorBox.Clear();
            }

            // Last name
            if (!letters.IsMatch(lnameBox.Text.Trim().Replace(" ", "")))
            {
                lnameErrorBox.Text = "Invalid last name.";
                isValid = false;
            }
            else
            {
                lnameErrorBox.Clear();
            }

            // Phone number
            if (!numbers.IsMatch(phoneBox.Text.Replace("-", "").Trim()) || phoneBox.Text.Length != 10)
            {
                phoneErrorBox.Text = "Invalid phone number. 'xxxxxxxxxx'";
                isValid = false;
            }
            else
            {
                phoneErrorBox.Clear();
            }

            // Street address
            if (!address.IsMatch(staddressBox.Text.Trim()))
            {
                staddressErrorBox.Text = "Invalid street address. '123 Street Name'";
                isValid = false;
            }
            else
            {
                staddressErrorBox.Clear();
            }

            // City
            if (!letters.IsMatch(cityBox.Text.Trim()))
            {
                cityErrorBox.Text = "Invalid city name.";
                isValid = false;
            }
            else
            {
                cityErrorBox.Clear();
            }

            // State
            if (string.IsNullOrEmpty(stateBox.Text.Trim()))
            {
                stateErrorBox.Text = "Please select a state.";
                isValid = false;
            }
            else
            {
                stateErrorBox.Clear();
            }

            // Zip code
            if (!numbers.IsMatch(zipcodeBox.Text.Trim()))
            {
                zipcodeErrorBox.Text = "Invalid zipcode.";
                isValid = false;
            }
            else
            {
                zipcodeErrorBox.Clear();
            }

            // Apartment
            if (string.IsNullOrEmpty(aptBox.Text.Trim()))
            {
                aptBox.Text = " ";
            }

            // Email
            if (!email.IsMatch(emailBox.Text.Trim()))
            {
                emailErrorBox.Text = "Invalid email.";
                isValid = false;
            }
            else
            {
                emailErrorBox.Clear();
            }

            return isValid;
        }
        private void MessageboxAns()
        {
            if (ValidateTextboxes())
            {
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("Are you sure you want to add this client? \n");
                messageBuilder.AppendLine($"{fnameBox.Text} {lnameBox.Text}");
                messageBuilder.AppendLine($"{phoneBox.Text}");
                messageBuilder.AppendLine($"{staddressBox.Text} {aptBox.Text}");
                messageBuilder.AppendLine($"{cityBox.Text}, {stateBox.Text}, {zipcodeBox.Text}");
                messageBuilder.AppendLine($"{emailBox.Text}");

                DialogResult result = MessageBox.Show(messageBuilder.ToString(), "Confirm", MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    // Save clients to the database
                    SaveClientsToDatabase();
                    ClearErrorBoxes();
                    ClearTextBoxes();
                }
                else if (result == DialogResult.Cancel)
                {
                    MessageBox.Show("Operation canceled.", "Canceled", MessageBoxButtons.OK);
                }
            }
            else
            {
                fnameBox.Focus();
                fnameBox.Select(fnameBox.Text.Length, 0);
            }
        }
     
        private void textboxes_keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }

        }
        private void textbox_textchanged(object sender, EventArgs e)
        {
            fnameBox.Text = ToTitleCase(fnameBox.Text);
            fnameBox.Select(fnameBox.Text.Length, 0);

            lnameBox.Text = ToTitleCase(lnameBox.Text);
            lnameBox.Select(lnameBox.Text.Length, 0);

            phoneBox.Select(phoneBox.Text.Length, 0);

            staddressBox.Text = ToTitleCase(staddressBox.Text);
            staddressBox.Select(staddressBox.Text.Length, 0);

            stateBox.Text = stateBox.Text.ToUpper();
            stateBox.Select(stateBox.Text.Length, 0);

            cityBox.Text = ToTitleCase(cityBox.Text);
            cityBox.Select(cityBox.Text.Length, 0);

            zipcodeBox.Select(zipcodeBox.Text.Length, 0);

            aptBox.Text = ToTitleCase(aptBox.Text);
            aptBox.Select(aptBox.Text.Length, 0);


        }
        private void DatabaseExists()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string databaseName = "clients";

                // Check if the database exists
                string checkDbQuery = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{databaseName}'";

                using (var command = new MySqlCommand(checkDbQuery, connection))
                {
                    var result = command.ExecuteScalar();

                    // If the database does not exist, create it
                    if (result == null)
                    {
                        string createDbQuery = $"CREATE DATABASE {databaseName}";
                        using (var createDbCommand = new MySqlCommand(createDbQuery, connection))
                        {
                            createDbCommand.ExecuteNonQuery();
                            MessageBox.Show($"Database '{databaseName}' created successfully.", "Success", MessageBoxButtons.OK);
                        }
                    }
                }
            }
        }
        private void SaveClientsToDatabase()
        {
            // Ensure the database exists
            DatabaseExists();

            // Update connection string to include database name
            string connectionStringWithDb = LoadConnectionString();

            if (connectionStringWithDb != null)
            {
                using (var connection = new MySqlConnection(connectionStringWithDb))
                {
                    connection.Open();

                // Ensure the table exists
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS client (
                    ID INT AUTO_INCREMENT PRIMARY KEY,
                    FirstName VARCHAR(30),
                    LastName VARCHAR(30),
                    Phone VARCHAR(10),
                    StreetAddress VARCHAR(30),
                    City VARCHAR(30),
                    State VARCHAR(30),
                    ZipCode VARCHAR(15),
                    Apartment VARCHAR(15),
                    Email VARCHAR(30)
                )";

                using (var createTableCommand = new MySqlCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                // Check for duplicates
                string checkDuplicateQuery = @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client 
                WHERE Phone = @Phone 
                OR (StreetAddress = @StreetAddress AND City = @City AND State = @State AND ZipCode = @ZipCode AND (Apartment = @Apartment OR @Apartment IS NULL OR Apartment IS NULL))
                OR Email = @Email";

                using (var checkDuplicateCommand = new MySqlCommand(checkDuplicateQuery, connection))
                {
                    checkDuplicateCommand.Parameters.AddWithValue("@Phone", phoneBox.Text.Trim());
                    checkDuplicateCommand.Parameters.AddWithValue("@StreetAddress", staddressBox.Text.Trim());
                    checkDuplicateCommand.Parameters.AddWithValue("@City", cityBox.Text.Trim());
                    checkDuplicateCommand.Parameters.AddWithValue("@State", stateBox.Text.Trim());
                    checkDuplicateCommand.Parameters.AddWithValue("@ZipCode", zipcodeBox.Text.Trim());
                    checkDuplicateCommand.Parameters.AddWithValue("@Apartment", string.IsNullOrEmpty(aptBox.Text.Trim()) ? (object)DBNull.Value : aptBox.Text.Trim());
                    checkDuplicateCommand.Parameters.AddWithValue("@Email", emailBox.Text.Trim());

                    using (var reader = checkDuplicateCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Extract the details of the duplicate client
                            string existingFirstName = reader["FirstName"].ToString();
                            string existingLastName = reader["LastName"].ToString();
                            string existingPhone = reader["Phone"].ToString();
                            string existingStreetAddress = reader["StreetAddress"].ToString();
                            string existingCity = reader["City"].ToString();
                            string existingState = reader["State"].ToString();
                            string existingZipCode = reader["ZipCode"].ToString();
                            string existingApartment = reader["Apartment"].ToString();
                            string existingEmail = reader["Email"].ToString();

                            string message = $"A client with the same details already exists:\n\n";

                            // Name
                            message += $"Name: {(fnameBox.Text.Trim() == existingFirstName ? $"{existingFirstName}*" : existingFirstName)} " +
                                       $"{(lnameBox.Text.Trim() == existingLastName ? $"{existingLastName}*" : existingLastName)}\n";

                            // Phone
                            message += $"Phone: {(phoneBox.Text.Trim() == existingPhone ? $"{existingPhone}*" : phoneBox.Text.Trim())}\n";

                            // Address
                            string addressLine = $"{existingStreetAddress} {(string.IsNullOrEmpty(existingApartment) ? "" : existingApartment + " ")}{existingCity}, {existingState} {existingZipCode}";
                            string inputAddressLine = $"{staddressBox.Text.Trim()} {(string.IsNullOrEmpty(aptBox.Text.Trim()) ? "" : aptBox.Text.Trim() + " ")}{cityBox.Text.Trim()}, {stateBox.Text.Trim()} {zipcodeBox.Text.Trim()}";
                            message += $"Address: {(inputAddressLine == addressLine ? $"{addressLine}*" : addressLine)}\n";

                            // Email
                            message += $"Email: {(emailBox.Text.Trim() == existingEmail ? $"{existingEmail}*" : existingEmail)}\n\n";

                            // Show the message and ask for confirmation
                            DialogResult result = MessageBox.Show(
                                message + "Do you still want to add this client?",
                                "Duplicate Found",
                                MessageBoxButtons.YesNo);

                            if (result == DialogResult.No)
                            {
                                MessageBox.Show("Operation canceled.", "Canceled", MessageBoxButtons.OK);
                                return;
                            }
                        }
                    }
                }



                // Insert new client
                string insertQuery = @"
                INSERT INTO client (FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email)
                VALUES (@FirstName, @LastName, @Phone, @StreetAddress, @City, @State, @ZipCode, @Apartment, @Email)";

                using (var insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@FirstName", fnameBox.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@LastName", lnameBox.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@Phone", phoneBox.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@StreetAddress", staddressBox.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@City", cityBox.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@State", stateBox.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@ZipCode", zipcodeBox.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@Apartment", string.IsNullOrEmpty(aptBox.Text.Trim()) ? (object)DBNull.Value : aptBox.Text.Trim());
                    insertCommand.Parameters.AddWithValue("@Email", emailBox.Text.Trim());

                    insertCommand.ExecuteNonQuery();
                }

                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("Are you sure you want to add this client? \n");
                messageBuilder.AppendLine($"{fnameBox.Text} {lnameBox.Text}");
                messageBuilder.AppendLine($"{phoneBox.Text}");
                messageBuilder.AppendLine($"{staddressBox.Text} {aptBox.Text}");
                messageBuilder.AppendLine($"{cityBox.Text}, {stateBox.Text}, {zipcodeBox.Text}");
                messageBuilder.AppendLine($"{emailBox.Text}");

                MessageBox.Show("Client saved to database successfully.", "Success", MessageBoxButtons.OK);
                ClearTextBoxes();
                fnameBox.Focus();

            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (ValidateTextboxes())
            {
              
                if (ValidateTextboxes())
                {
                  
                    MessageboxAns();
                }
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            mainForm.ShowMain();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }


    }
}




        

