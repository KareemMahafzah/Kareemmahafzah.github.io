## Summary

This PR includes security hardening, AutoMapper integration, view model refactors, tests, and CI improvements.

## Changes
- Added SecurityHeadersMiddleware and configured CSP via appsettings.
- Replaced Bind-based model binding with dedicated Create/Edit/Index view models.
- Integrated AutoMapper with MappingProfile.
- Added unit and integration tests (xUnit + EF InMemory + WebApplicationFactory).
- CI: GitHub Actions workflow runs build+tests across .NET 8.x and 10.x.

## Notes
- You may need to set user secrets for the connection string for local runs.
- Review CSP settings for any external assets (CDNs) used in production.

## How to test locally
- dotnet build MaintenanceProject/MaintenanceProject.csproj
- dotnet run --project MaintenanceProject/MaintenanceProject.csproj
- dotnet test MaintenanceProject.Tests/MaintenanceProject.Tests.csproj
