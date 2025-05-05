using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clientsys
{
    public partial class editUser : UserControl
    {
        private string connectionString;
        private Main mainForm;
        public editUser(Main mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;


            fnameBox2.Text = ToTitleCase(fnameBox2.Text);
            lnameBox2.Text = ToTitleCase(lnameBox2.Text);
            staddressBox2.Text = ToTitleCase(staddressBox2.Text);
            cityBox2.Text = ToTitleCase(cityBox2.Text);
            aptBox2.Text = ToTitleCase(aptBox2.Text);

            string firstName = fnameBox2.Text.Trim();
            string lastName = lnameBox2.Text.Trim();
            string phone = phoneBox2.Text.Trim();
            string streetAddress = staddressBox2.Text.Trim();
            string city = cityBox2.Text.Trim();
            string state = stateBox2.Text.Trim();
            string zipCode = zipcodeBox2.Text.Trim();
            string apartment = aptBox2.Text.Trim();
            string email = emailBox2.Text.Trim();


            LoadConnectionString();
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

        private void ClearTextBoxes()
        {
            fnameBox2.Clear();
            lnameBox2.Clear();
            phoneBox2.Clear();
            staddressBox2.Clear();
            cityBox2.Clear();
            stateBox2.Text = "";
            zipcodeBox2.Clear();
            aptBox2.Clear();
            emailBox2.Clear();
        }

     
        private void ClearErrorBoxes()
        {
            fnameErrorBox2.Clear();
            lnameErrorBox2.Clear();
            phoneErrorBox2.Clear();
            staddressErrorBox2.Clear();
            cityErrorBox2.Clear();
            stateErrorBox2.Clear();
            zipcodeErrorBox2.Clear();
            aptErrorBox2.Clear();
            emailErrorBox2.Clear();
        }
        private string ToTitleCase(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        private bool ValidateTextboxes()
        {
            Regex letters = new Regex("^[a-zA-Z ]+$");//allow spaces
            Regex phone = new Regex(@"^\d{3}-\d{3}-\d{4}$");
            Regex address = new Regex(@"^\d+\s+[a-zA-Z\s]+$");
            Regex numbers = new Regex("^[0-9]+$");
            Regex email = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            bool isValid = true;

            // First name
            if (!letters.IsMatch(fnameBox2.Text.Trim().Replace(" ", "")))
            {
                fnameErrorBox2.Text = "Invalid name.";
                isValid = false;
            }
            else
            {
                fnameErrorBox2.Clear();
            }

            // Last name
            if (!letters.IsMatch(lnameBox2.Text.Trim().Replace(" ", "")))
            {
                lnameErrorBox2.Text = "Invalid last name.";
                isValid = false;
            }
            else
            {
                lnameErrorBox2.Clear();
            }

            // Phone number
            if (!numbers.IsMatch(phoneBox2.Text.Replace("-", "").Trim()) || phoneBox2.Text.Length != 10)
            {
                phoneErrorBox2.Text = "Invalid phone number. 'xxxxxxxxxx'";
                isValid = false;
            }
            else
            {
                phoneErrorBox2.Clear();
            }

            // Street address
            if (!address.IsMatch(staddressBox2.Text.Trim()))
            {
                staddressErrorBox2.Text = "Invalid street address. '123 Street Name'";
                isValid = false;
            }
            else
            {
                staddressErrorBox2.Clear();
            }

            // City
            if (!letters.IsMatch(cityBox2.Text.Trim()))
            {
                cityErrorBox2.Text = "Invalid city name.";
                isValid = false;
            }
            else
            {
                cityErrorBox2.Clear();
            }

            // State
            if (string.IsNullOrEmpty(stateBox2.Text.Trim()))
            {
                stateErrorBox2.Text = "Please select a state.";
                isValid = false;
            }
            else
            {
                stateErrorBox2.Clear();
            }

            // Zip code
            if (!numbers.IsMatch(zipcodeBox2.Text.Trim()))
            {
                zipcodeErrorBox2.Text = "Invalid zipcode.";
                isValid = false;
            }
            else
            {
                zipcodeErrorBox2.Clear();
            }

            // Apartment
            if (string.IsNullOrEmpty(aptBox2.Text.Trim()))
            {
                aptBox2.Text = " ";
            }

            // Email
            if (!email.IsMatch(emailBox2.Text.Trim()))
            {
                emailErrorBox2.Text = "Invalid email.";
                isValid = false;
            }
            else
            {
                emailErrorBox2.Clear();
            }

            return isValid;
        }

        private void addButton2_Click(object sender, EventArgs e)
        {
            if (ValidateTextboxes())
            {
                // Show confirmation dialog
                DialogResult result = MessageBox.Show("Are you sure you want to save these changes?",
                                                      "Confirm Changes",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    UpdateClientInDatabase();
                    // Clear searchBox and searchListbox
                    mainForm.searchBox.Text = string.Empty;
                    mainForm.searchListView.Items.Clear();

                    // Show the main form
                    mainForm.ShowMain();
                }
                else
                {
             
                    MessageBox.Show("Changes have not been saved.");
                }
            }
        }

        private void clearButton2_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=localhost;Port=3306;Database=clients;Uid=root;Pwd=Devry123;";

            if (int.TryParse(idBox2.Text, out int clientId))
            {
                // Ask the user to confirm deletion
                DialogResult result = MessageBox.Show("Are you sure you want to delete this client?",
                                                      "Confirm Deletion",
                                                      MessageBoxButtons.OKCancel,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string deleteQuery = "DELETE FROM client WHERE id = @clientId";

                        using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                        {
                            // Set parameter for the query
                            command.Parameters.AddWithValue("@clientId", clientId);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Client deleted successfully.");
                                ClearTextBoxes();
                                // Clear searchBox and searchListbox
                                mainForm.searchBox.Text = string.Empty;
                                mainForm.searchListView.Items.Clear();

                                // Show the main form
                                mainForm.ShowMain();
                            }
                            else
                            {
                                MessageBox.Show("Client not found.");
                            }
                        }
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    MessageBox.Show("Deletion canceled.");
                }
            }
            else
            {
                MessageBox.Show("Invalid client ID.");
            }
        }

        private void textboxes2_keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);

            }
        }

        private void textboxes_textchanged(object sender, EventArgs e)
        {
            fnameBox2.Text = ToTitleCase(fnameBox2.Text);
            fnameBox2.Select(fnameBox2.Text.Length, 0);

            lnameBox2.Text = ToTitleCase(lnameBox2.Text);
            lnameBox2.Select(lnameBox2.Text.Length, 0);

            phoneBox2.Select(phoneBox2.Text.Length, 0);

            staddressBox2.Text = ToTitleCase(staddressBox2.Text);
            staddressBox2.Select(staddressBox2.Text.Length, 0);

            stateBox2.Text = stateBox2.Text.ToUpper();
            stateBox2.Select(stateBox2.Text.Length, 0);

            cityBox2.Text = ToTitleCase(cityBox2.Text);
            cityBox2.Select(cityBox2.Text.Length, 0);

            zipcodeBox2.Select(zipcodeBox2.Text.Length, 0);

            aptBox2.Text = ToTitleCase(aptBox2.Text);
            aptBox2.Select(aptBox2.Text.Length, 0);
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

        private void UpdateClientInDatabase()
        {
            string connectionString = "Server=localhost;Port=3306;Database=clients;Uid=root;Pwd=Devry123;";
            if (int.TryParse(idBox2.Text, out int clientId))
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = @"
                 UPDATE client
                SET FirstName = @firstName,
                    LastName = @lastName,
                    Phone = @phone,
                    StreetAddress = @streetAddress,
                    City = @city,
                    State = @state,
                    ZipCode = @zipcode,
                    Apartment = @apartment,
                    Email = @email
                WHERE id = @ID";

                    using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                    {
                        // Set parameters from textboxes
                        command.Parameters.AddWithValue("@firstName", fnameBox2.Text);
                        command.Parameters.AddWithValue("@lastName", lnameBox2.Text);
                        command.Parameters.AddWithValue("@phone", phoneBox2.Text);
                        command.Parameters.AddWithValue("@streetAddress", staddressBox2.Text);
                        command.Parameters.AddWithValue("@city", cityBox2.Text);
                        command.Parameters.AddWithValue("@state", stateBox2.Text);
                        command.Parameters.AddWithValue("@zipcode", zipcodeBox2.Text);
                        command.Parameters.AddWithValue("@apartment", aptBox2.Text);
                        command.Parameters.AddWithValue("@email", emailBox2.Text);
                        command.Parameters.AddWithValue("@ID", clientId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Client updated successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Client not found.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid client ID.");
            }
        }

        private void backbutton2_CLick(object sender, MouseEventArgs e)
        {
            mainForm.ShowMain();
        }

    }
}
