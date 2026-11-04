# Gym Management System

A gym management system built with **ASP.NET Core MVC on .NET 10**. The application manages members, trainers, plans, memberships, sessions, bookings, attendance, authentication, and authorization.

## Features

- Login and logout using ASP.NET Core Identity.
- Identity users and roles: Super Admin and Admin.
- Member management, including health records and addresses.
- Member photo upload through Cloudinary.
- Trainer management with duplicate email and phone validation.
- Subscription plan management.
- Membership creation and cancellation.
- Session listing and booking management.
- Attendance tracking support.
- Soft delete for supported entities.
- Validation through Data Annotations and `ModelState`.
- Repository and Unit of Work patterns.
- Separation between presentation, business, and data-access layers.
- Automatic migrations and database seeding at application startup.

## Technologies

- .NET 10
- ASP.NET Core MVC and Razor Views
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Mapster / MapsterMapper
- Cloudinary
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
- Photo upload and deletion services.

Main services:

- `MemberService`
- `TrainerService`
- `MembershipService`
- `BookingService`
- `SessionService`
- `HomeService`

### Gym.Presentation

Contains the web layer:

- MVC controllers.
- Razor views.
- Authentication configuration.
- Static files, CSS, and JavaScript.
- Application settings.

Controllers are kept thin. They receive requests, validate `ModelState`, call the appropriate service, and return a view or redirect. Business and database logic belongs to the `Gym.Business` and `Gym.DataAccess` layers.

## Requirements

Install the following before running the project:

- .NET 10 SDK or a compatible later version.
- SQL Server or SQL Server Express.
- Visual Studio 2026 or Visual Studio Code.
- A Cloudinary account if photo uploads are required.

## Database Configuration

Update `DefaultConnection` in `Gym.Presentation/appsettings.json`:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=.;Database=Gym;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

The application applies EF Core migrations automatically during startup.

To update the database manually:

```powershell
dotnet ef database update --project Gym.DataAccess --startup-project Gym.Presentation
```

## Cloudinary Configuration

Cloudinary settings are configured under `CloudinarySettings`:

```json
{
  "CloudinarySettings": {
	"CloudName": "your-cloud-name",
	"ApiKey": "your-api-key",
	"ApiSecret": "your-api-secret"
  }
}
```

Cloudinary can be disabled in development:

```json
{
  "CloudinarySettings": {
	"Enabled": false
  }
}
```

> Never commit `ApiSecret`, database credentials, or other sensitive values to Git. Use User Secrets or environment variables for real environments.

## Default Identity Accounts

Identity accounts and roles are seeded from the `IdentitySeed` configuration:

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

Use the following credentials to log in as an administrator:

```text
Email: admin@example.com
Password: AdminPassword123!
```

Change these passwords before using the application in a real environment.

## Running the Project

From the solution directory:

```powershell
dotnet restore
dotnet build
dotnet run --project Gym.Presentation
```

Alternatively, open `Gym.Presentation.slnx` in Visual Studio and run the `Gym.Presentation` project.

## Main Routes

### Authentication

```text
GET  /Account/Login
POST /Account/Login
POST /Account/Logout
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

### Memberships

```text
GET  /Membership
GET  /Membership/Create
POST /Membership/Create
POST /Membership/Cancel/{id}
```

### Bookings

```text
GET  /Booking
GET  /Booking/Create/{id}
POST /Booking/Create
GET  /Booking/GetMembersForUpcomingSession/{id}
GET  /Booking/GetMembersForOngoingSession/{id}
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

## Validation and Duplicate Users

The application prevents duplicate users by validating email addresses and phone numbers.

The validation is applied across both members and trainers because email and phone numbers are stored in the shared user hierarchy.

Duplicate values are returned as validation results and shown to the user instead of exposing database exceptions.

## Soft Delete

Supported entities use soft delete values:

```csharp
IsDeleted = true;
DeletedAt = DateTime.UtcNow;
```

EF Core query filters prevent soft-deleted records from appearing in normal queries.

## Error Handling Pattern

Business services use `Result` and `Result<T>` instead of throwing exceptions for expected business failures.

Example:

```csharp
return Result.Failure(
	"Email",
	"This email is already registered.");
```

Controllers convert service results into validation messages, views, or redirects.

## Verification

Build the solution with:

```powershell
dotnet build
```

Stop the application before changing interfaces or service signatures, because some changes require a full restart instead of Hot Reload.

## Security Notes

- Change all default passwords before deployment.
- Never commit Cloudinary secrets to GitHub.
- Never commit production connection strings.
- Enable HTTPS in production.
- Review authorization policies before deployment.

## Project Status

The project is under development and currently provides the core gym-management features for members, trainers, plans, memberships, sessions, bookings, and authentication.
