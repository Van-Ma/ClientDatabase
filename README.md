# Client Management System

A Windows Forms application built in **C# (.NET Framework)** using **MySQL** for database storage. This system allows users to create, search, edit, and manage client records with full CRUD functionality.

> **Note**: This project includes only backend logic (no form design files).

---

## 📦 Features

- ✅ Add new clients  
- 🔍 Search clients (by name, phone, address, etc.)  
- ✏️ Edit existing client information  
- 🗑️ Delete clients  
- 📄 Client database stored in MySQL  
- 📁 External configuration via `config.json`  
- 🛡️ Basic validation and duplicate checking  

---

## 🛠️ Technologies Used

- C# (.NET Framework)  
- Windows Forms  
- MySQL  
- Newtonsoft.Json (for config file parsing)  

---

## 📂 Project Structure (Overview)

```
ClientSys/
│
├── MainForm.cs            # Main entry form with search UI
├── newclientUser.cs       # Control to add new clients
├── editclientUser.cs      # Control to update/edit existing clients
├── searchUser.cs          # Control to search and view clients
├── Client.cs              # Client class model with properties
├── Database.cs            # Handles all DB operations (CRUD)
├── config.json            # Stores database connection info
└── README.md              # Project documentation
```

---

## ⚙️ Configuration

1. Create a `config.json` file in the root directory with the following format:

```json
{
  "Database": {
    "Server": "localhost",
    "Database": "your_db_name",
    "User": "your_username",
    "Password": "your_password"
  }
}
```

2. Make sure your MySQL server is running and the database/table is set up.

---

## 💾 Database Structure

You’ll need a `clients` table with the following fields:

```sql
CREATE TABLE clients (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    Phone VARCHAR(20),
    StreetAddress VARCHAR(255),
    Apartment VARCHAR(50),
    City VARCHAR(100),
    State VARCHAR(100),
    ZipCode VARCHAR(20),
    Email VARCHAR(255)
);
```

