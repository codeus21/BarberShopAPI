# Barber Shop API Backend

## Description
ASP.NET Core Web API for The Barber Book appointment management system.

## Technologies
- ASP.NET Core 7.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- CORS enabled

## Setup Instructions

### Prerequisites
- .NET 7.0 SDK
- SQL Server (LocalDB or SQL Server Express)
- Visual Studio 2022 (recommended)

### Installation
1. Clone the repository
2. Navigate to the project directory
3. Set up user secrets:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "YOUR_JWT_SECRET_KEY"
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
   ```
4. Run database migrations:
   ```bash
   dotnet ef database update
   ```
5. Run the API:
   ```bash
   dotnet run
   ```

### API Endpoints
- `GET /api/Services` - Get all services
- `POST /api/Appointments` - Create appointment
- `GET /api/Admin/appointments` - Get all appointments (Admin)
- `POST /api/Admin/cleanup-completed` - Mark past appointments complete
- `POST /api/Auth/login` - Admin login

### Authentication
Uses JWT Bearer tokens for admin endpoints.

## Environment Variables
The following secrets need to be configured:
- `Jwt:Key` - Secret key for JWT token generation
- `ConnectionStrings:DefaultConnection` - Database connection string

## Frontend Integration
This API is designed to work with the React frontend.