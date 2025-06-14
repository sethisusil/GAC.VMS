FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY GAC.WMS.Api/NuGet.config ./NuGet.config

COPY GAC.WMS.Api/GAC.WMS.Api.csproj GAC.WMS.Api/
COPY GAC.WMS.Application/GAC.WMS.Application.csproj GAC.WMS.Application/
COPY GAC.WMS.Core/GAC.WMS.Core.csproj GAC.WMS.Core/
COPY GAC.WMS.Domain/GAC.WMS.Domain.csproj GAC.WMS.Domain/
COPY GAC.WMS.Infrastructure/GAC.WMS.Infrastructure.csproj GAC.WMS.Infrastructure/

RUN dotnet restore GAC.WMS.Api/GAC.WMS.Api.csproj

COPY . .
WORKDIR /src/GAC.WMS.Api

#Remove --no-restore
RUN dotnet publish GAC.WMS.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GAC.WMS.Api.dll"]
