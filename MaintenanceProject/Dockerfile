# Use the official .NET 10 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY MaintenanceProject/*.csproj MaintenanceProject/
COPY MaintenanceProject/Program.cs MaintenanceProject/
RUN dotnet restore MaintenanceProject/MaintenanceProject.csproj

# copy everything else and build
COPY MaintenanceProject/. MaintenanceProject/
WORKDIR /src/MaintenanceProject
RUN dotnet publish -c Release -o /app --no-restore

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "MaintenanceProject.dll"]
