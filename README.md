# Barber Shop API Backend

## Description
ASP.NET Core Web API for The Barber Book multi-tenant appointment management system. Supports multiple barber shops with isolated data and customizable themes.

## Technologies
- ASP.NET Core 7.0
- Entity Framework Core with migrations and seeding
- PostgreSQL (hosted on Railway)
- JWT Authentication
- CORS enabled
- Multi-tenant architecture

## Setup Instructions

### Prerequisites
- .NET 7.0 SDK
- PostgreSQL (or use Railway for cloud hosting)
- Visual Studio 2022 (recommended)

### Installation
1. Clone the repository
2. Navigate to the project directory
3. Set up user secrets:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "YOUR_JWT_SECRET_KEY"
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_POSTGRESQL_CONNECTION_STRING"
   ```
4. Run database migrations:
   ```bash
   dotnet ef database update
   ```
5. The database will be automatically seeded with sample data for "Clean Cuts" and "Elite Cuts" tenants
6. Run the API:
   ```bash
   dotnet run
   ```

### API Endpoints

#### Public Endpoints
- `GET /api/Services?tenant=<tenant>` - Get services for specific tenant
- `GET /api/BarberShops?tenant=<tenant>` - Get barber shop info for tenant
- `POST /api/Appointments` - Create appointment
- `GET /api/Appointments/available-slots/{date}?tenant=<tenant>` - Get available time slots

#### Admin Endpoints (JWT Required)
- `POST /api/Auth/login` - Admin login
- `GET /api/Admin/appointments` - Get all appointments
- `PUT /api/Admin/appointments/{id}/cancel` - Cancel appointment
- `PUT /api/Admin/appointments/{id}/reschedule` - Reschedule appointment
- `POST /api/Admin/cleanup-completed` - Mark past appointments complete

### Authentication
Uses JWT Bearer tokens for admin endpoints.

## 🏢 Multi-Tenant System

### Supported Tenants
- **Clean Cuts** (Default) - Blue theme with classic styling
- **Elite Cuts** - Purple theme with premium styling
- **Vintage Cuts** - Gold theme with retro styling

### Tenant Detection
The API automatically detects tenants through:
- Query parameter: `?tenant=<tenant-name>`
- Subdomain: `<tenant>.yourdomain.com`
- Default fallback to "Clean Cuts"

### Data Isolation
Each tenant has:
- Separate barber shop information
- Isolated service offerings
- Independent appointment scheduling
- Unique admin access

## 🗄️ Database Structure

### Entities
- **BarberShop** - Tenant information (name, contact, theme colors)
- **Admin** - Tenant-specific admin users
- **Service** - Services offered by each tenant
- **Appointment** - Customer bookings with tenant association

### Seeding
The database is automatically seeded with:
- Sample barber shops (Clean Cuts, Elite Cuts)
- Default services for each tenant
- Admin users for testing

## Environment Variables
The following secrets need to be configured:
- `Jwt:Key` - Secret key for JWT token generation
- `ConnectionStrings:DefaultConnection` - PostgreSQL connection string

## Frontend Integration
This API is designed to work with the React frontend and supports:
- Multi-tenant theming
- Dynamic content loading
- Tenant-specific data isolation
- Real-time appointment availability

## 🚀 Recent Improvements

### Multi-Tenant Architecture
- **Tenant Middleware** - Automatic tenant detection and data filtering
- **Data Isolation** - Each tenant has completely separate data
- **Scalable Design** - Easy to add new tenants without code changes

### Database Enhancements
- **PostgreSQL Migration** - Moved from SQL Server to PostgreSQL
- **Railway Hosting** - Cloud database hosting for better scalability
- **Automatic Seeding** - Database pre-populated with sample data
- **Entity Framework Migrations** - Version-controlled database schema

### API Improvements
- **Tenant-Aware Endpoints** - All endpoints support tenant filtering
- **Enhanced Error Handling** - Better error messages and status codes
- **CORS Configuration** - Proper cross-origin resource sharing setup
- **JWT Security** - Secure admin authentication

## 🔧 Development Notes

### Adding New Tenants
1. Add tenant data to `SeedData` method in `BarberShopContext.cs`
2. Create admin user for the tenant
3. Add services specific to the tenant
4. No code changes needed - tenant detection is automatic

### Database Migrations
- Use `dotnet ef migrations add <MigrationName>` to create migrations
- Use `dotnet ef database update` to apply migrations
- Migrations are automatically applied on startup in development

### Testing
- Use the seeded data for testing different tenants
- Admin credentials are seeded for each tenant
- All endpoints support tenant parameter for testing