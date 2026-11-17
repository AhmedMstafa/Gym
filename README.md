# Gym Management System

A gym management system built with **ASP.NET Core MVC on .NET 10**. The application manages members, trainers, plans, memberships, sessions, bookings, attendance, authentication, and authorization.

## Features

- Login and logout using ASP.NET Core Identity.
- Identity users and roles: Super Admin and Admin.
- Member management, including health records and addresses.
- Hybrid member photo uploads: Cloudinary integration with seamless local storage fallback (`wwwroot/images/members/`).
- Trainer management with duplicate email and phone validation.
- Subscription plan management.
- Membership creation and cancellation.
- Session listing, scheduling, and booking management.
- Attendance tracking support.
- Soft delete for supported entities.
- Validation through Data Annotations and `ModelState`.
- Repository and Unit of Work patterns.
- Separation between presentation, business, and data-access layers.
- Automatic migrations and database seeding at application startup.
- Local configuration support via `.env` files.
- Containerization ready with Docker.

## Technologies

- .NET 10
- ASP.NET Core MVC and Razor Views
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Mapster / MapsterMapper
- Cloudinary
- Docker
- Bootstrap and Bootstrap Icons

## Project Structure

```text
Gym/
├── Gym.DataAccess/
│   ├── Data/
│   │   ├── Contexts/
│   │   ├── Configration/
│   │   └── Seeder/
│   ├── Entities/
│   ├── Enums/
│   ├── Interceptors/
│   ├── Migrations/
│   ├── Queries/
│   └── Repositories/
│
├── Gym.Business/
│   ├── Common/
│   ├── DependencyInjection/
│   ├── Mapping/
│   ├── Services/
│   ├── Settings/
│   └── ViewModels/
│
└── Gym.Presentation/
	├── Controllers/
	├── Views/
	├── wwwroot/
	├── Dockerfile
	├── Program.cs
	└── appsettings.json
```

## Architecture

### Gym.DataAccess

Responsible for database access and persistence:

- `GymDbContext`.
- Entity Framework configurations.
- EF Core migrations.
- Repositories.
- `UnitOfWork`.
- Session query services.
- Database seeding.
- Audit interceptor for `CreatedAt`, `UpdatedAt`, `DeletedAt`, and `IsDeleted`.

### Gym.Business

Contains the application and business logic:

- Feature services and service interfaces.
- View models.
- Mapster mapping configuration.
- `Result` and `Result<T>` error-handling pattern.
- `IDateTimeProvider` for testable time handling.
- Photo upload and deletion services (Cloudinary + Local fallback).

Main services:

- `MemberService`
- `TrainerService`
- `MembershipService`
- `BookingService`
- `SessionService`
- `HomeService`
- `PhotoService`

### Gym.Presentation

Contains the web layer:

- MVC controllers.
- Razor views.
- Authentication configuration.
- Static files, CSS, and JavaScript.
- Application settings and `.env` loader.
- Dockerfile.

Controllers are kept thin. They receive requests, validate `ModelState`, call the appropriate service, and return a view or redirect. Business and database logic belongs to the `Gym.Business` and `Gym.DataAccess` layers.

## Requirements

Install the following before running the project:

- .NET 10 SDK or a compatible later version.
- SQL Server or SQL Server Express.
- Visual Studio 2026 or Visual Studio Code.
- Optional: Cloudinary account (local storage fallback is used if disabled or unreachable).
- Optional: Docker Desktop (for containerized deployment).

## Environment Configuration (.env or appsettings.json)

The application supports loading configuration from either `appsettings.json`, environment variables, or a local `.env` file located in the `Gym.Presentation/` directory.

### Example `.env` File (`Gym.Presentation/.env`):

```env
ConnectionStrings__DefaultConnection="Server=.;Database=GymTestProject;Trusted_Connection=True;TrustServerCertificate=True;"

# Cloudinary (Set Enabled=false to store photos locally in wwwroot/images/members)
CloudinarySettings__Enabled=false
CloudinarySettings__CloudName=your-cloud-name
CloudinarySettings__ApiKey=your-api-key
CloudinarySettings__ApiSecret=your-api-secret

# Default Admin Seed Accounts
IdentitySeed__SuperAdminEmail=superadmin@example.com
IdentitySeed__SuperAdminPassword=ChangeThisPassword123!
IdentitySeed__AdminEmail=admin@example.com
IdentitySeed__AdminPassword=AdminPassword123!
```

### Database Configuration

Update `DefaultConnection` in `Gym.Presentation/appsettings.json` or `.env`:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.;Database=GymTestProject;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

The application applies EF Core migrations automatically during startup.

To update the database manually:

```powershell
dotnet ef database update --project Gym.DataAccess --startup-project Gym.Presentation
```

## Cloudinary & Photo Uploads

Cloudinary settings are configured under `CloudinarySettings`:

```json
{
  "CloudinarySettings": {
	"Enabled": true,
	"CloudName": "your-cloud-name",
	"ApiKey": "your-api-key",
	"ApiSecret": "your-api-secret"
  }
}
```

### Local Storage Fallback:
- When `"Enabled": false` or when Cloudinary is unreachable (due to network/firewall/timeout), the application automatically falls back to saving photos locally in `wwwroot/images/members/`.
- No photo upload request will fail due to cloud downtime.

> Never commit `ApiSecret`, database credentials, or real passwords to Git. Use `.env` or User Secrets for sensitive local data.

## Default Identity Accounts

Identity accounts and roles are seeded automatically from the `IdentitySeed` configuration:

```json
{
  "IdentitySeed": {
	"SuperAdminEmail": "superadmin@example.com",
	"SuperAdminPassword": "ChangeThisPassword123!",
	"AdminEmail": "admin@example.com",
	"AdminPassword": "AdminPassword123!"
  }
}
```

Use the following default credentials to log in as an administrator:

```text
Email: admin@example.com
Password: AdminPassword123!
```

Change these passwords before deploying to any production or publicly accessible environment.

## Running the Project

### Using .NET CLI

From the solution directory:

```powershell
dotnet restore
dotnet build
dotnet run --project Gym.Presentation
```

Alternatively, open `Gym.Presentation.slnx` in Visual Studio and run the `Gym.Presentation` project.

### Using Docker

Build and run the containerized application:

```powershell
# Build image from solution root
docker build -t gym-app -f Gym.Presentation/Dockerfile .

# Run container on port 8080
docker run -d -p 8080:8080 --name gym-container gym-app
```

## Main Routes

### Authentication

```text
GET  /Account/Login
POST /Account/Login
POST /Account/Logout
```

### Home & Dashboard

```text
GET  /
GET  /Home/Index
```

### Members

```text
GET  /Members
GET  /Members/Create
POST /Members/Create
GET  /Members/Details/{id}
GET  /Members/Edit/{id}
POST /Members/Edit/{id}
GET  /Members/Delete/{id}
POST /Members/Delete/{id}
```

### Trainers

```text
GET  /Trainer
GET  /Trainer/Create
POST /Trainer/Create
GET  /Trainer/Details/{id}
GET  /Trainer/Edit/{id}
POST /Trainer/Edit/{id}
GET  /Trainer/Delete/{id}
POST /Trainer/Delete/{id}
```

### Plans

```text
GET  /Plans
```

### Memberships

```text
GET  /Membership
GET  /Membership/Create
POST /Membership/Create
POST /Membership/Cancel/{id}
```

### Sessions

```text
GET  /Sessions
GET  /Sessions/Create
POST /Sessions/Create
GET  /Sessions/Details/{id}
GET  /Sessions/Edit/{id}
POST /Sessions/Edit/{id}
GET  /Sessions/Delete/{id}
POST /Sessions/Delete/{id}
```

### Bookings

```text
GET  /Booking
GET  /Booking/Create/{id}
POST /Booking/Create
GET  /Booking/GetMembersForUpcomingSession/{id}
GET  /Booking/GetMembersForOngoingSession/{id}
```

## Validation and Duplicate Users

The application prevents duplicate users by validating email addresses and phone numbers across both members and trainers because they inherit from the shared user hierarchy.

Duplicate values are handled cleanly through `Result.Failure` and shown to the user via UI error messages instead of throwing database exceptions.

## Soft Delete

Supported entities implement soft delete:

```csharp
IsDeleted = true;
DeletedAt = DateTime.UtcNow;
```

EF Core global query filters automatically exclude soft-deleted records from standard queries.

## Error Handling Pattern

Business services use the `Result` and `Result<T>` pattern instead of throwing exceptions for expected business failures:

```csharp
return Result.Failure(
	"Email",
	"This email is already registered.");
```

Controllers translate service results into validation messages, view responses, or redirects.

## Verification

Build the solution with:

```powershell
dotnet build
```

## Security Notes

- Change all default passwords before deployment.
- Never commit Cloudinary secrets or production connection strings to GitHub.
- Enable HTTPS in production environments.
- Review Identity lockout and authorization policies before going live.

## Project Status

The project is actively maintained and provides a complete suite of gym management features: authentication & authorization, member/trainer management, subscription plans, memberships, session scheduling, bookings, and dashboard analytics.
