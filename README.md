# Eventify Backend

The backend for **Eventify**, an Event Management System, is built using ASP.NET Core, providing real-time communication for notification, robust API services, and database management. This repository contains the backend code for handling business logic, data access, and real-time event updates.

## Technologies Used

- **Framework:** ASP.NET Core
- **Real-Time Communication:** SignalR for notifications and updates
- **Database:** MySQL Server for data storage
- **Authentication:** JWT (JSON Web Token) for secure authentication

## Project Structure

```
Eventify-Backend/
├── eventify-backend/                # Main project directory
│   ├── Controllers/                 # API Controllers handling requests and responses
│   ├── Data/                        # Database context and entity configurations
│   ├── Helpers/                     # Utility classes and helper functions
│   ├── DTOs/                        # Data Transfer Objects (DTOs)
│   ├── Models/                      # Data models representing entities
│   ├── Properties/                  # Application properties
│   ├── Utility-service/             # Email service for password reset
│   ├── Services/                    # Business logic and external service integration
│   ├── Hubs/                        # SignalR Hubs for real-time communication
│   ├── Migrations/                  # Entity Framework Core migrations for database schema updates
│   ├── appsettings.json             # Configuration settings (database, API keys, etc.)
│   ├── Program.cs                   # Main entry point of the application
│   └── README.md                    # Project documentation
└── eventify-backend-solution/       # Solution-level files and configuration
```

## Acknowledgments

Special thanks to all the contributors and mentors who supported this project!
