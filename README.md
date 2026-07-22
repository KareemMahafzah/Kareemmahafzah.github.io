# MaintenanceProject

This repository contains a small ASP.NET Core MVC application (MaintenanceProject) for tracking maintenance requests.

Features added by GitHub Copilot automation:
- Security headers middleware and configurable CSP
- AutoMapper integration and view models to prevent overposting
- Unit and integration tests (xUnit, EF InMemory, WebApplicationFactory)
- CI: GitHub Actions workflow to build and test across .NET 8/10
- UI improvements: engineering-themed CSS, sortable/paged/searchable index, AJAX delete, Bootstrap cards
- Dockerfile for containerized deployment

Local development
1. Ensure .NET 10 SDK is installed.
2. Set a connection string for the project (user secrets recommended):
   - From repository root:
	 ```powershell
	 .\MaintenanceProject\set-user-secrets.ps1 -ConnectionString "Server=(localdb)\\mssqllocaldb;Database=MaintenanceDB;Trusted_Connection=True;"
	 ```
3. Run the app:
   dotnet run --project MaintenanceProject/MaintenanceProject.csproj

Run tests
  dotnet test MaintenanceProject.Tests/MaintenanceProject.Tests.csproj

Docker
Build the image from repo root:
  docker build -f MaintenanceProject/Dockerfile -t maintenanceproject:latest .
Run:
  docker run -e ASPNETCORE_ENVIRONMENT=Production -p 8080:80 --rm maintenanceproject:latest

Deployment notes
- The application attempts to apply EF Core migrations at startup. Ensure the production DB user has the required permissions or disable automatic migration.
- Configure production connection string via environment variable DefaultConnection or ConnectionStrings:DefaultConnection.

Security
- CSP and security headers are configurable in appsettings.json or environment-specific config files.
