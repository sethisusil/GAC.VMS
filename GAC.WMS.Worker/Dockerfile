# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy NuGet config and project files
COPY GAC.WMS.Worker/NuGet.config ./
COPY GAC.WMS.Worker/GAC.WMS.Worker.csproj GAC.WMS.Worker/
COPY GAC.WMS.Core/GAC.WMS.Core.csproj GAC.WMS.Core/

# Restore dependencies
RUN dotnet restore GAC.WMS.Worker/GAC.WMS.Worker.csproj

# Copy the rest of the source code
COPY . .

WORKDIR /src/GAC.WMS.Worker

# Publish the application
RUN dotnet publish GAC.WMS.Worker.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (optional, for diagnostics or if you have endpoints)
EXPOSE 8080

# Set environment variable for ASP.NET Core (optional for Worker, but harmless)
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GAC.WMS.Worker.dll"]
