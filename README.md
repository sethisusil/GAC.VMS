## 🎯 Project Objective

Design and implement a .NET-based integration system that connects **GAC’s Warehouse Management System (WMS)** with **external ERP systems**.
The system must support both **real-time API ingestion** and **file-based ingestion for legacy systems**.

## 🔍 Functional Requirements

### 📦 Real-Time ERP Integration
- Expose RESTful APIs to receive:
  - Customer Master Data
  - Product Master Data
  - Purchase Orders (Inbound)
  - Sales Orders (Outbound)
- Validate incoming data and persist it in a relational database.

### 📁 Legacy File-Based Integration
- Schedule polling of XML files from SFTP/shared folders.
- Parse and transform these files to WMS-compatible JSON payloads.
- Push transformed data to WMS REST APIs.

## ⚙️ Technical Requirements
- Platform: `.NET 9`
- Database: SQL Server
- Design Patterns: Clean Architecture, Domain-Driven Design, Repository Pattern
- Retry logic for failure resiliency
- CRON-configurable background jobs
- Unit and integration tests
- Docker & Docker Compose support
- Markdown documentation with setup and architecture

---

## ✅ Implemented Features Checklist
### 📁 Architecture & Structure
- [x] Clean Architecture with proper layering
- [x] Domain-Driven Design (DDD) with rich domain models
- [x] Repositories implemented using the Repository Pattern(Using generic repository)
- [x] Application Services with Interfaces for each entity
- [x] Modular structure with separate projects for Domain, Application, Infrastructure, API, and Tests
- [x] CRON-configurable background jobs for schedule based xml file polling for legacy system and perist in databse.
- [x] Plolly configuration for retry logic for failure resiliency.
- [x] Docker file to support container bases service.

### 🧱 Domain Layer
- [x] Entity base class with `Id`, `CreatedDate`, `CreatedBy`, `UpdatedDate`, `UpdatedBy`, etc property
- [x] Value objects (e.g., `Address`, `Dimensions`,`OrderItem`)
- [x] Domain logic in entities (e.g., `Customer`, `Product`, `SalesOrder`,`PurchaseOrder`)
- [x] Domain-specific exceptions (`DomainException`)

### 🏗️ Infrastructure Layer
- [x] EF Core `GacDbContext` with `IEntityTypeConfiguration<T>` per entity
- [x] EF Core setup with `DbSet<T>` for each aggregate root
- [x] Generic repository base class with full CRUD
- [x] Custom repositories for each entity (Product, Customer,SalesOrder, PurchaseOrder etc.)

### 🔄 Application Layer
- [x] Interfaces for services (e.g., `ICustomerService`,`IProductService`, `ISalesOrderService`, `IPurchaseOrderService`)
- [x] Domain injection via DI
- [x] Service implementations calling domain logic and repositories
- [x] Input validation logic at the application level using fluent validation

### 📡 API Layer
- [x] RESTful controllers for Customer, Product, Purchase Order, Sales Order
- [x] Model validation and exception handling
- [x] Service injection via DI
- [x] OpenAPI/Swagger documentation (if applicable)

### 🧪 TDD(Test Driven Development) (see Section [#test-cases])
- [x] `GAC.WMS.Api.Tests`,`GAC.WMS.Application.Tests`,`GAC.WMS.Infrastructure.Tests`,`GAC.WMS.Worker.Tests` projects
- [x] Nuget package `Microsoft.EntityFrameworkCore.InMemory` to configure EF Core In-Memory
- [x] Test cases using xUnit framework
- [x] Sample API test: create + retrieve customer

### 🐳 Docker Setup (see Section [#docker])
- [ ] Dockerfile for `GAC.WMS.Api`
- [ ] Dockerfile for `GAC.WMS.Worker`
- [ ] docker-compose with API, Worker, SQL Server and file access

### ⏱️ Background Worker (see Section [#background-worker])
- [ ] Polling legacy XML files using CRON
- [ ] Parse + transform XML → send to WMS

### 📃 Documentation (this README)
- [x] Architecture overview
- [x] Project structure with explanation
- [x] Run instructions for API + tests
- [x] Feature checklist (this section)

---

## 🧭 Architecture Overview <a name="architecture-overview"></a>

This solution is structured using Clean Architecture and Domain-Driven Design principles.

```text
GGAC.WMS.Api.sln/
├── GAC.WMS.Api/              --> ASP.NET Core Web API layer
|── GAC.WMS.Application       --> Application services and interfaces
|── GAC.WMS.Core              --> Dtos contract to share with client
├── GAC.Domain/               --> Domain entities and interfaces
├── GAC.WMS.Infrastructure/   --> EF Core DbContext, Repositories, Configuration
├── GAC.WMS.Worker/           --> Background Worker for legacy XML ingestion
├── docker-compose.yml        --> Orchestrates API, Worker, DB
├── Tests/                    --> Test cases uisng xunit framework
    ├── GAC.WMS.Api.Tests/               --> Test cases for api layer, for controller actions
    ├── GAC.WMS.Application.Tests/       --> Test cases for aplication layer layer(Services)
    ├── GAC.WMS.Infrastructure.Tests/    --> Test cases for infrastructure layer layer(Repository)
    ├── GAC.WMS.Worker.Tests/             --> Test cases for background Worker for legacy XML ingestion(Repository)
```


<p align="center">
 <img width="606" src="https://github.com/user-attachments/assets/ce408d84-4fe0-4168-87a4-4e87306335cd"  alt="Architecture diagram" width="800" />
</p>

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)
- [SQL Server](https://www.microsoft.com/en-in/sql-server/sql-server-downloads)
- [Visual Studio](https://visualstudio.microsoft.com/vs/)

---
### Run API Locally(Visual studio or Dotnet CLI)

#### Configuration Changes for Database Connection
- Move to GAC.WMS.Api and change the following section in `appsettings.json` file with correct sql server name in place of `.` and add user id and password if required 
```configuration
 "ConnectionStrings": {
   "DefaultConnection": "Server=.;Database=GACWMS;Integrated Security=true;TrustServerCertificate=true;"
 },
```
---
#### Configuration Changes for SFTP file path for legacy system[GAC.WMS.Worker]
- Create a folder in local directory with the path `C:\\GACWMS\\XmlFiles`
- Create a folder in local directory with the path `C:\\GACWMS\\XmlFiles`
- Copy these files `Customers.xml`, `Products.xml`,`PurchaseOrders.xml`,`SalesOrders.xml` from project root directory `./LegacySupportFiles` and place thme into `C:\\GACWMS\\XmlFiles`.
```configuration
"XmlFileConfig": {
  "BasePath": "C:\\GACWMS\\XmlFiles",
  "CustomerFileName": "Customers.xml",
  "ProductFileName": "Products.xml",
  "PurchaseOrderFileName": "PurchaseOrders.xml",
  "SalesOrderFileName": "SalesOrders.xml",
  "ProcessedPath": "C:\\GACWMS\\ProcessedFiles"
}
```
---
#### Configuration Changes for API communication for legacy system
- Move to GAC.WMS.Worker and open `appsettings.json`
- Replacse the base api base url fo `BaseUrl` in following section(Leavse as it is if your api runing in `https://localhost:7196/`)
```configuration
 "GACWMSApi": {
   "BaseUrl": "https://localhost:7196/",
   "UploadCustomerUrl": "api/Customer/upload",
   "UploadProductUrl": "api/Product/upload",
   "UploadPurchaseOrderUrl": "api/PurchaseOrder/upload",
   "UploadSalesOrderUrl": "api/SalesOrder/upload"
 }
```
---
#### Configuration Changes for cron scheduler
- Move to GAC.WMS.Worker and open `appsettings.json`
- Change following node value based on requirements(currenly it is withe very 5 minutes)
```configuration
"XmlPolling": {
  "Schedule": "*/5 * * * *" // every 5 mins
}
```
---

#### Run `GAC.WMS.Api` project through Visual Studio 2022
- Build project and run it using `F5` or `Visual Studio play button`
- Once it is up, you should be able see the following swagger page
    - You can test directly from swagger for all end points (GET, POST, PUT,DELETE)
    - Make sure not to pass id filed with value while creation
<p align="center">
  <img width="949" src="https://github.com/user-attachments/assets/d76730f3-00dc-4dbc-ba89-cc08c035b8f2"  alt="Swagger UI" width="800" />
</p>



#### Run `GAC.WMS.Worker` project through Visual Studio 2022
- Right click on `GAC.WMS.Worker` and select `Debug` --> `Start New Instance`

---
#### Run projects through .Net CLI
- Open command prompt or powershell(Recomend to open in Admin mode)
```bash
cd GAC.WMS.Api
dotnet restore
dotn build
dotnet run
```
- Open a another new command prompt or powershell(Recomend to open in Admin mode)
```bash
cd GAC.WMS.Worker
dotnet run
```
---
### Run Tests
- Select `Test --> Run All Tests` from visual studio menu(You should be able to see bellow scree)
- This project contains total 226 test cases
[OR]
```bash
cd GAC.WMS.Api
dotnet test
```
<p align="center">
  <img width="248" alt="TestCases" src="https://github.com/user-attachments/assets/afe3c0ff-696e-4770-a003-9579eb2ab235" width="800" />
</p>


---
### Run with Docker Compose
- Make sure you docker desktop is up and runing
```bash
cd GAC.VMS
docker-compose up --build
```
- Make sure everything is good witout any error in console
- You check `http://localhost:5000/index.html` you should see the same swagger page.

---
### How to verify all good
- For API project make sure all GET,POST,PUT,DELETE end points are working witout any error
- For worker project
    - It will run in every 5 minutes
    - If all good al the files from `C:\GACWMS\XmlFiles` should be moved to `C:\GACWMS\C:\GACWMS\ProcessedFiles{ddmmyyyyhhmmss}` folder
    - Should able to see the data through API end points and sql server data verufucation

---

## 🐳 Docker Setup <a name="docker"></a>

- Defined for `GAC.WMS.Api/Dockerfile` for API 
- Defined for `GAC.WMS.Worker/Dockerfile` for Legacy worker 
- created `docker-compose.yml` to run API, Worker, and SQL Server
- `docker-compose.yml`e added under root directory

## ⏱️ Background Worker <a name="background-worker"></a>

- Implemented using `IHostedService`
- CRON expression-based scheduling
- Polls directory or SFTP for XML files
- Transforms and sends data to WMS via REST

---

### `GACWMS` Databse
- Databse is created using `SQL Server`
- Datase is used using EF core code first approach
- Migration should be automatically applied to target DB on application load
<p align="center">
  <img width="618" alt="DatabaseSchema" src="https://github.com/user-attachments/assets/3b6a7494-4e6a-4955-929b-60df657826f1"  width="800" />

</p>

