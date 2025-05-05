using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace clientsys
{
    public partial class Main : Form
    {
        private string placeholderText = "Search";
        private newclientUser newclient_user;
        private editUser edit_user;
        private string connectionString;

        public Main()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            newclient_user = new newclientUser(this);
            edit_user = new editUser(this);
            
            LoadConnectionString();

            searchBox.Text = placeholderText;
            searchBox.ForeColor = Color.Gray;

            FilterSearch();

            // add user controls
            this.Controls.Add(newclient_user);
            this.Controls.Add(edit_user);

            // set init visibility
            newclient_user.Visible = false;
            edit_user.Visible = false;




        }

         private void LoadConnectionString()
        {
            string filePath = "config.json"; 
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                AppConfig config = JsonConvert.DeserializeObject<AppConfig>(json);
                connectionString = config.ConnectionString;
            }
            else
            {
                MessageBox.Show("Configuration file not found.");
            }
        }
        //---------------------------------------------------------------------------------------------------
        private void SearchClients(string searchTerm)
        {
            string query;
            string sortBy = filtercomboBox.SelectedItem?.ToString() ?? string.Empty;
            bool isSingleCharacterSearch = searchTerm.Length == 1;

            // query based on search 
            switch (sortBy)
            {
                case "Name":
                    if (isSingleCharacterSearch)
                    {
                        query = @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE FirstName LIKE CONCAT(@SearchTerm, '%')
                OR LastName LIKE CONCAT(@SearchTerm, '%')";
                    }
                    else
                    {
                        query = @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE FirstName LIKE CONCAT('%', @SearchTerm, '%')
                OR LastName LIKE CONCAT('%', @SearchTerm, '%')";
                    }
                    break;

                case "Phone Number":
                    query = isSingleCharacterSearch
                        ? @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE Phone LIKE CONCAT(@SearchTerm, '%')"
                        : @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE Phone LIKE CONCAT('%', @SearchTerm, '%')";
                    break;

                case "Street":
                    query = isSingleCharacterSearch
                        ? @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE StreetAddress LIKE CONCAT(@SearchTerm, '%')"
                        : @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE StreetAddress LIKE CONCAT('%', @SearchTerm, '%')";
                    break;

                case "City":
                    query = isSingleCharacterSearch
                        ? @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE City LIKE CONCAT(@SearchTerm, '%')"
                        : @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE City LIKE CONCAT('%', @SearchTerm, '%')";
                    break;

                case "State":
                    query = isSingleCharacterSearch
                        ? @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE State LIKE CONCAT(@SearchTerm, '%')"
                        : @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE State LIKE CONCAT('%', @SearchTerm, '%')";
                    break;

                case "Zip Code":
                    query = isSingleCharacterSearch
                        ? @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE ZipCode LIKE CONCAT(@SearchTerm, '%')"
                        : @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE ZipCode LIKE CONCAT('%', @SearchTerm, '%')";
                    break;

                default:
                    // default case
                    query = isSingleCharacterSearch
                        ? @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE FirstName LIKE CONCAT(@SearchTerm, '%')
                OR LastName LIKE CONCAT(@SearchTerm, '%')
                OR Phone LIKE CONCAT(@SearchTerm, '%')
                OR StreetAddress LIKE CONCAT(@SearchTerm, '%')
                OR City LIKE CONCAT(@SearchTerm, '%')
                OR State LIKE CONCAT(@SearchTerm, '%')
                OR ZipCode LIKE CONCAT(@SearchTerm, '%')
                OR Apartment LIKE CONCAT(@SearchTerm, '%')
                OR Email LIKE CONCAT(@SearchTerm, '%')"
                        : @"
                SELECT ID, FirstName, LastName, Phone, StreetAddress, City, State, ZipCode, Apartment, Email
                FROM client
                WHERE FirstName LIKE CONCAT('%', @SearchTerm, '%')
                OR LastName LIKE CONCAT('%', @SearchTerm, '%')
                OR Phone LIKE CONCAT('%', @SearchTerm, '%')
                OR StreetAddress LIKE CONCAT('%', @SearchTerm, '%')
                OR City LIKE CONCAT('%', @SearchTerm, '%')
                OR State LIKE CONCAT('%', @SearchTerm, '%')
                OR ZipCode LIKE CONCAT('%', @SearchTerm, '%')
                OR Apartment LIKE CONCAT('%', @SearchTerm, '%')
                OR Email LIKE CONCAT('%', @SearchTerm, '%')";
                    break;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    // Set the parameter with wildcard
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm);

                    using (var reader = command.ExecuteReader())
                    {
                        // Clear previous search results
                        searchListView.Items.Clear();

                        while (reader.Read())
                        {
                            // Get client details from the reader
                            int clientId = Convert.ToInt32(reader["ID"]);
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            string phone = reader["Phone"].ToString();
                            string streetAddress = reader["StreetAddress"].ToString();
                            string city = reader["City"].ToString();
                            string state = reader["State"].ToString();
                            string zipCode = reader["ZipCode"].ToString();
                            string apartment = reader["Apartment"].ToString();
                            string email = reader["Email"].ToString();

                            // Create a ListViewItem with ID as the first column
                            var listViewItem = new ListViewItem(clientId.ToString());

                            // Add parameters to the other columns
                            listViewItem.SubItems.Add(firstName);
                            listViewItem.SubItems.Add(lastName);
                            listViewItem.SubItems.Add(phone);
                            listViewItem.SubItems.Add(streetAddress);
                            listViewItem.SubItems.Add(city);
                            listViewItem.SubItems.Add(state);
                            listViewItem.SubItems.Add(zipCode);
                            listViewItem.SubItems.Add(apartment);
                            listViewItem.SubItems.Add(email);

                            // Add the ListViewItem to the ListView
                            searchListView.Items.Add(listViewItem);
                        }
                    }
                }
            }
        }

        private void FilterSearch()
        {
            string[] filters = { "", "Name", "Phone Number", "Street", "City", "State", "Zip Code" };

            filtercomboBox.Items.Clear();
            foreach (string filter in filters)
            {
                filtercomboBox.Items.Add(filter);
            }

            // initially blank
            filtercomboBox.SelectedIndex = -1;
        }

        //---------------------------------------------------------------------------------------------------

        private void newclientButton_Click(object sender, EventArgs e)
        {
            ShowNewClientUser();
        }

        private void searchListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (searchListView.SelectedItems.Count == 0)
                return; 

            // Get the selected item
            ListViewItem selectedItem = searchListView.SelectedItems[0];

            // Extract client details from the selected item based on columns
            string clientId = selectedItem.Text; 
            string firstName = selectedItem.SubItems[1].Text; 
            string lastName = selectedItem.SubItems[2].Text; 
            string phone = selectedItem.SubItems[3].Text; 
            string streetAddress = selectedItem.SubItems[4].Text; 
            string city = selectedItem.SubItems[5].Text; 
            string state = selectedItem.SubItems[6].Text; 
            string zipCode = selectedItem.SubItems[7].Text; 
            string apartment = selectedItem.SubItems[8].Text; 
            string email = selectedItem.SubItems[9].Text; 

            // Clear and switch to newclientUser control
            ShowEditClientUser();

            // Populate textboxes on the newclient_user control
            edit_user.idBox2.Text = clientId;
            edit_user.fnameBox2.Text = firstName;
            edit_user.lnameBox2.Text = lastName;
            edit_user.phoneBox2.Text = phone;
            edit_user.staddressBox2.Text = streetAddress;
            edit_user.aptBox2.Text = apartment;
            edit_user.cityBox2.Text = city;
            edit_user.stateBox2.Text = state;
            edit_user.zipcodeBox2.Text = zipCode;
            edit_user.emailBox2.Text = email;
        }

        private void main_keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string searchTerm = searchBox.Text.Trim();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    SearchClients(searchTerm);

                }

            }
        }

        public void ShowNewClientUser()
        {

            // Hide other controls
            edit_user.Visible = false;
            mainlayoutPanel.Visible = false;

            // Show and resize newclient_user
            newclient_user.Visible = true;
            newclient_user.Dock = DockStyle.Fill;
        }

        public void ShowEditClientUser()
        {
            newclient_user.Visible = false;
            edit_user.Visible = true;
            mainlayoutPanel.Visible = false;

            edit_user.Dock = DockStyle.Fill;
        }

        public void ShowMain()
        {
            newclient_user.Visible = false;
            edit_user.Visible = false;
            mainlayoutPanel.Visible = true;
        }

        private void searchbox_Enter(object sender, EventArgs e)
        {
            if (searchBox.Text == placeholderText)
            {
                searchBox.Text = "";
                searchBox.ForeColor = Color.Black;
            }
        }

        private void searchbox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                searchBox.Text = placeholderText;
                searchBox.ForeColor = Color.Gray; 
            }
        }

        private void filtercombobox_keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string searchTerm = searchBox.Text.Trim();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    SearchClients(searchTerm);

                }

            }
        }


        
    }

}
