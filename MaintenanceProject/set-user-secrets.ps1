# Run this script from the repository root to initialize and set user secrets for the MaintenanceProject
# Requires the dotnet CLI to be installed and available on PATH.
# Usage: .\MaintenanceProject\set-user-secrets.ps1 -ConnectionString "Server=(localdb)\\mssqllocaldb;Database=MaintenanceDB;Trusted_Connection=True;"
param(
	[Parameter(Mandatory=$true)]
	[string]$ConnectionString
)

Push-Location MaintenanceProject
Write-Host "Initializing user secrets for the project..."
# Initializes user secrets if not already initialized
dotnet user-secrets init | Out-Null
Write-Host "Setting ConnectionStrings:DefaultConnection..."
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$ConnectionString" | Out-Null
Write-Host "Optionally set admin user email and password?"
if ($env:ADMIN_EMAIL -and $env:ADMIN_PASSWORD) {
	Write-Host "Setting AdminUser:Email and AdminUser:Password from environment variables..."
	dotnet user-secrets set "AdminUser:Email" "$env:ADMIN_EMAIL" | Out-Null
	dotnet user-secrets set "AdminUser:Password" "$env:ADMIN_PASSWORD" | Out-Null
}
Write-Host "Done."
Pop-Location
