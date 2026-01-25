# 📝 MOM (Minutes of Meeting) Management System

A comprehensive web application designed to streamline the process of scheduling meetings, managing staff/departments, and recording minutes of meetings. Built with **.NET 10.0** and **ASP.NET Core MVC**.

## 🚀 Features

### 👥 Staff & Organization Management
- **Department Management**: Create and manage organizational departments.
- **Staff Management**: Maintain staff profiles and details.
- **User Authentication**: Secure login and access control.

### 📅 Meeting Management
- **Dashboard**: Overview of activities and metrics.
- **Meeting Scheduling**: Create and schedule meetings with specific types.
- **Venue Management**: Manage available meeting rooms and venues.
- **Meeting Members**: Assign participants and members to meetings.
- **Minutes Recording**: Capabilities to track meeting outcomes (implied).

## 🛠️ Technology Stack

- **Framework**: .NET 10.0 (ASP.NET Core MVC)
- **Language**: C#
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Frontend**: Razor Views, CSS, JavaScript

## 📦 Getting Started

Follow these steps to set up and run the project locally.

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server/) (LocalDB or Standard)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MOM
   ```

2. **Configure Database Connection**
   Open `appsettings.json` and update the `MOMConnection` string if necessary (defaults to LocalDB or your local instance).
   ```json
   "ConnectionStrings": {
     "MOMConnection": "Server=(localdb)\\mssqllocaldb;Database=MOM_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **Apply Database Migrations**
   Open your terminal in the project root and run:
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```
   or watch for changes:
   ```bash
   dotnet watch run
   ```

5. **Access the App**
   Open your browser and navigate to `http://localhost:<port>` (typically `http://localhost:5000` or similar provided in terminal).

## 📂 Project Structure

- **Controllers**: Handles request logic (Meetings, Staff, Departments, etc.).
- **Models**: Database entities and ViewModels.
- **Views**: UI pages (Razor templates).
- **Data**: Entity Framework DbContext configuration.
- **wwwroot**: Static assets (CSS, JS, Images).

## 🤝 Contributing

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

[MIT License](LICENSE) (or your preferred license)
