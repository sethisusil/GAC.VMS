## 🎯 Project Objective

This solution delivers a modern, scalable, and extensible integration system that bridges **GAC’s Warehouse Management System (WMS)** with various **external ERP systems**.  
It supports:
- **Real-Time Data Exchange** via RESTful APIs
- **Legacy File-Based Ingestion** through scheduled XML polling

## 🔍 Functional Requirements

### ⚡ Real-Time ERP Integration
- Exposes RESTful APIs for ingesting and processing the following:
  - 📘 Customer Master Data  
  - 📦 Product Master Data  
  - 📥 Inbound Purchase Orders  
  - 📤 Outbound Sales Orders  
- Validate incoming data and persist it in a relational database.

**Key Features:**
- Automatic validation
- Data persistence
- Structured exception handling

---

### 🗂️ Legacy File-Based Integration

Ensures compatibility with traditional systems:

- Polls XML files from SFTP/shared folders using CRON schedule 
- Parses and transforms XML to WMS-compatible JSON payloads  
- Pushes to WMS APIs  
- Resilient retries via Polly

---

## ⚙️ Technical Requirements

| Layer                    | Technology |
|--------------------------|------------|
| **Backend Platform**     | .NET 9     |
| **Database**             | SQL Server |
| **Patterns**             | Clean Architecture, DDD, Repository Pattern |
| **Jobs**                 | Hosted Background Services + CRON |
| **Resilience**           | Polly Retry Policies |
| **Testing**              | xUnit + EF Core InMemory |
| **Deployment**           | Docker & Docker Compose |
| **Docs**                 | Markdown + Swagger (OpenAPI) |
---

## ✅ Feature Implementation

### 📁 Architecture & Structure

- ✅ Clean Architecture with domain separation  
- ✅ Domain-Driven Design (DDD) with rich aggregates  
- ✅ Repositories implemented using the Repository Pattern(Using generic repository)
- ✅ Application Services with Interfaces for each entity
- ✅ Modular structure: Domain, Application, Infrastructure, API, Tests  
- ✅ CRON-configurable background jobs for schedule based xml file polling for legacy system and perist in databse.  
- ✅ Plolly configuration for retry logic for failure resiliency.  
- ✅ Docker file to support container bases service.

---

### 🧱 Domain Layer

- ✅ Base `Entity` class with audit fields (`Id`, `CreatedDate`, `CreatedBy`, `UpdatedDate`, `UpdatedBy`)
- ✅ Value Objects: `Address`, `Dimensions`, `OrderItem`  
- ✅ Business logic encapsulated in domain models (e.g., `Customer`, `Product`, `SalesOrder`,`PurchaseOrder`)

---

### 🏗️ Infrastructure Layer

- ✅ EF Core `GacDbContext` with `IEntityTypeConfiguration<T>` per entity  
- ✅ `DbSet<T>` for each aggregate  
- ✅ Base `GenericRepository<T>` with full CRUD
- ✅ Entity-specific repositories for core entities (Product, Customer,SalesOrder, PurchaseOrder etc.)

---

### 🌐 API Layer

- ✅ RESTful endpoints for Customer, Product, Purchase Order, Sales Order
- ✅ Model validation + exception handling  
- ✅ Swagger/OpenAPI integration  
- ✅ DI-based controller setup  

---

### 🧪 Testing (TDD)

- ✅ Projects:  
  - `GAC.WMS.Api.Tests`  
  - `GAC.WMS.Application.Tests`  
  - `GAC.WMS.Infrastructure.Tests`  
  - `GAC.WMS.Worker.Tests`  
- ✅ xUnit + InMemory DB  
- ✅ Sample tests: Create and retrieve entities  

---

### 🐳 Docker Setup

- ✅ Dockerfile for `GAC.WMS.Api` and `GAC.WMS.Worker`  
- ✅ `docker-compose.yml` with services:  
  - API  
  - Legacy Worker  
  - SQL Server  
  - File mount for XML ingestion  

---

### ⏱️ Background Worker

- ✅ Scheduled polling via CRON  
- ✅ XML parsing and JSON transformation  
- ✅ POST data to WMS API  
- ✅ Retry on failure using Polly

---

## 📚 Documentation Coverage

- ✅ Architecture Overview  
- ✅ Project Structure  
- ✅ Setup & Run Instructions  
- ✅ Test Instructions  
- ✅ Feature Checklist  

---

## 🧭 Architecture Overview <a name="architecture-overview"></a>

This solution is structured using Clean Architecture and Domain-Driven Design principles.

```text
GAC.WMS.Api.sln
├── 📦 GAC.WMS.Api/ → Entry point: ASP.NET Core Web API
├── 🧠 GAC.WMS.Application/ → Application logic & service interfaces
├── 📃 GAC.WMS.Core/ → Shared DTOs & contracts for communication
├── 🧱 GAC.Domain/ → Core domain models, aggregates & interfaces
├── 🗄️ GAC.WMS.Infrastructure/ → EF Core, Repositories, DbContext configurations
├── 🛠️ GAC.WMS.Worker/ → Background worker for scheduled legacy file ingestion
├── 🐳 docker-compose.yml → Container orchestration (API + Worker + DB)
└── 🧪 Tests/ → xUnit-based test projects
  ├── ✅ GAC.WMS.Api.Tests/ → API Layer tests (Controllers)
  ├── ✅ GAC.WMS.Application.Tests/ → Application Layer tests (Services)
  ├── ✅ GAC.WMS.Infrastructure.Tests/ → Infrastructure Layer tests (Repositories)
  └── ✅ GAC.WMS.Worker.Tests/ → Worker tests (Polling & File Processing)
```

---

### 🗺️ Visual Architecture Diagram

> A high-level overview of the integration flow and core components

<p align="center">
  <img width="606" src="https://github.com/user-attachments/assets/ce408d84-4fe0-4168-87a4-4e87306335cd"  alt="Architecture diagram" width="800" />
</p>

---

✅ **Design Highlights**
- Layered architecture using ports and adapters
- Domain-centric business rules
- Seamless testability via abstraction
- Background worker integration with CRON support
- Dockerized for containerized deployments

---

## 🚀 Getting Started

Ready to launch your local development environment? Follow the steps below to get everything up and running smoothly.

---

### 🔧 Prerequisites

Ensure you have the following tools installed:

| Tool | Description | Download |
|------|-------------|----------|
| 🟣 [.NET 9 SDK] | Core framework for building and running the solution | [Download .NET 9](https://dotnet.microsoft.com/download) |
| 🐳 Docker | Required for containerizing and orchestrating services | [Get Docker](https://www.docker.com/products/docker-desktop) |
| 🛢️ SQL Server | Backend relational database for persistence | [SQL Server](https://www.microsoft.com/en-in/sql-server/sql-server-downloads) |
| 💻 Visual Studio (2022+) | IDE for building, debugging, and testing | [Download Visual Studio](https://visualstudio.microsoft.com/vs/) |

---

### ▶️ Running the API Locally

You can run the API either through **Visual Studio** or using the **.NET CLI** or using the **Docker**.

---


#### 🛠️ Step 1: Configure Your Database Connection

Before running the application, update the connection string in `GAC.WMS.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=GACWMS;Integrated Security=true;TrustServerCertificate=true;"
}
```

🔁 Replace . with your actual SQL Server instance name.
🔑 If you're using SQL authentication, provide User ID and Password as shown below:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=GACWMS;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
}

```

---
#### 🛠️ Step 2: Configure File-Based Ingestion for Legacy System (`GAC.WMS.Worker`)

The legacy integration is powered by scheduled XML file polling via a background worker. To enable this:

#### 🛠️ Setup Local XML Directory

1. 📁 Create a directory on your machine at:

```bash
C:\GACWMS\XmlFiles
```
2. 📥 Copy the legacy XML sample files:

| File Name                | Description              |
|--------------------------|--------------------------|
| **Customers.xml**        | Customer Master Data     |
| **Products.xml**         | Product Master Data      |
| **PurchaseOrders.xml**   | Inbound Purchase Orders  |
| **SalesOrders.xml**      | Outbound Sales Orders    |
	
    🗂️ You can find these files in the project folder: ./LegacySupportFiles
	
3. 📌 Paste all 4 XML files into the newly created directory:

4. ⚙️ Update Configuration in appsettings.json (GAC.WMS.Worker)
```json
"XmlFileConfig": {
  "BasePath": "C:\\GACWMS\\XmlFiles",
  "CustomerFileName": "Customers.xml",
  "ProductFileName": "Products.xml",
  "PurchaseOrderFileName": "PurchaseOrders.xml",
  "SalesOrderFileName": "SalesOrders.xml",
  "ProcessedPath": "C:\\GACWMS\\ProcessedFiles"
}
```
5. ✅ Ensure ProcessedPath folder exists:
  Create it if it doesn't: C:\GACWMS\ProcessedFiles

---
### 🔌 Step 3: Configure API Communication for Legacy Integration
The background worker (`GAC.WMS.Worker`) needs to communicate with the **WMS API** to upload parsed legacy data. Here's how to configure the API endpoints:

#### 🛠️ Update API Configuration in `appsettings.json`
1. Navigate to the `GAC.WMS.Worker` project directory.

2. Open the `appsettings.json` file.

3. Locate the `GACWMSApi` section and verify or update the following values:

```json
"GACWMSApi": {
  "BaseUrl": "https://localhost:7196/",
  "UploadCustomerUrl": "api/Customer/upload",
  "UploadProductUrl": "api/Product/upload",
  "UploadPurchaseOrderUrl": "api/PurchaseOrder/upload",
  "UploadSalesOrderUrl": "api/SalesOrder/upload"
}
```
---
### ⏱️ Step 4: Configure CRON Scheduler for File Polling
The background worker polls XML files based on a configurable **CRON schedule**. This allows flexible timing for legacy file ingestion.

#### 🛠️ Update CRON Expression in `appsettings.json`
1. Navigate to the `GAC.WMS.Worker` project.

2. Open the `appsettings.json` file.

3. Locate and update the `XmlPolling` section as needed:

```json
"XmlPolling": {
  "Schedule": "*/5 * * * *" // every 5 minutes
}
```
---

### 🎉 Final Step: Run & Test the WMS API & legacy worker project
After completing all four setup steps, you're now ready to launch and interact with the system.

#### 🚀 Run the API Project (`GAC.WMS.Api`)

1. Open the solution in **Visual Studio 2022**.
2. Set `GAC.WMS.Api` as the **Startup Project**.
3. Hit `F5` or click the green **Play** ▶️ button.

> ✅ The API should launch and open the **Swagger UI** in your browser.

#### 🔍 Explore Swagger UI

Once the API is running, you’ll see a fully interactive Swagger interface like this:

<p align="center">
  <img width="949" src="https://github.com/user-attachments/assets/d76730f3-00dc-4dbc-ba89-cc08c035b8f2"  alt="Swagger UI" width="800" />

</p>

#### 🧪 What You Can Do

- Use **Swagger** to test all API endpoints (GET, POST, PUT, DELETE).
- Validate the request/response structure for:
  - `/api/Customer`
  - `/api/Product`
  - `/api/PurchaseOrder`
  - `/api/SalesOrder`
- 📌 **Important**: When creating new records via `POST`, **do not** include the `Id` field — it will be auto-generated.

---

### 🏃‍♂️ Run `GAC.WMS.Worker` (Legacy Ingestion Service)
Once all configurations are in place, it's time to run the background worker that processes legacy XML files and pushes data to the WMS API.

#### 🛠️ Launch via Visual Studio 2022

1. In **Solution Explorer**, right-click on the project:  
   📁 `GAC.WMS.Worker`

2. Navigate to:  
   🧭 `Debug` → `Start New Instance`

---

### 💻 Alternate Way: Run Projects Using .NET CLI
If you're not using Visual Studio, you can run both the API and the Worker services directly from the command line using the .NET CLI.

#### 🧪 Prerequisites
- Make sure the [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download) is installed.
- Recommended: Open your **Command Prompt** or **PowerShell** in **Administrator Mode**.

#### ▶️ Step 1: Run the API Project (`GAC.WMS.Api`)

```bash
# Navigate to the API project directory
cd GAC.WMS.Api

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run the API
dotnet run
```
- ✅ The API should start on https://localhost:7196 (or the port defined in launchSettings.json), and Swagger UI should be available in the browser.

#### ▶️ Step 2: Run the Worker Project (GAC.WMS.Worker)
- Open a new terminal window to run the background worker simultaneously.
```bash
# Navigate to the Worker project directory
cd GAC.WMS.Worker

# Run the worker service
dotnet run
```
- 🌀 The background service will start polling XML files as per your configured CRON schedule and send data to the WMS API.

---

### 🐳 Alternate Approach: Run with Docker Compose
Take advantage of Docker to spin up the **entire system (API, Worker, SQL Server)** with a single command — no need to install SDKs or manage local environments.

#### 🔧 Prerequisites

- Ensure **Docker Desktop** is installed and **running**  
  👉 [Download Docker Desktop](https://www.docker.com/products/docker-desktop)

#### 🚀 Run the Entire Stack

```bash
# Navigate to the root directory containing docker-compose.yml
cd GAC.VMS

# Build and run all services in containers
docker-compose up --build
```

#### 🛠️ Docker will:

- Build the API and Worker services
- Create and initialize a SQL Server container
- Start polling XML files and expose the API

#### 🌐 Access the Application
Once all services are up and running without errors:

- 🔗 Open your browser and visit:
http://localhost:5000/index.html

- ✅ You should see the Swagger UI, just like running it locally through Visual Studio or .NET CLI.

#### 🧠 Helpful Tips
- Logs from API & Worker are shown live in the terminal
- Use CTRL+C to stop the containers
- Use docker-compose down to stop and clean up the environment
- Once file processed the file should be moved to proccessed folder
---

### 🧪 Run Automated Tests
Ensure code reliability and correctness by running the comprehensive test suite included in this solution — with **226+ test cases** covering API, Application, Infrastructure, and Worker layers.

#### 🧭 Option 1: Run All Tests via Visual Studio
1. Open the solution in **Visual Studio 2022**
2. Navigate to the top menu:  
   ➡️ `Test` → `Run All Tests`
3. The **Test Explorer** will show the test results in real time.

<p align="center">
  <img width="248" alt="TestCases" src="https://github.com/user-attachments/assets/afe3c0ff-696e-4770-a003-9579eb2ab235" width="800" />
</p>

- ✅ You should see all test cases pass successfully.

#### 🧭 Option 2: Run Tests via .NET CLI

Prefer the command line? You can run tests using the .NET CLI:

```bash
# Navigate to the API project (or any test project root)
cd GAC.WMS.Api

# Execute all tests
dotnet test
```

#### 🧪 This will run all associated test projects, including:
- GAC.WMS.Api.Tests

- GAC.WMS.Application.Tests

- GAC.WMS.Infrastructure.Tests

- GAC.WMS.Worker.Tests

- 💡 Note: Tests use xUnit and EF Core InMemory to simulate real database interactions.

---

### ✅ Final Checklist: How to Verify Everything is Working
After setup and execution, follow this quick validation guide to ensure all components are functioning as expected.

#### 🔍 API Project – Functional Check

- 🔄 Test all **CRUD endpoints** (`GET`, `POST`, `PUT`, `DELETE`) using:
  - Swagger UI (`http://localhost:5000/index.html`)
  - Postman / Curl / HTTP client
- ✅ Make sure responses return the expected data without errors
- 🗃️ Verify the data is persisted correctly in the SQL Server database

#### 🔁 Legacy Worker Project – Ingestion Verification

- ⏱️ The background worker runs based on the CRON schedule  
  *(default: every **5 minutes**)*  
  ➤ Wait a few minutes after startup to allow the job to trigger

- 📂 Source folder:  
  `C:\GACWMS\XmlFiles`

- 📤 If everything works:
  - The processed XML files will be moved to:  
    `C:\GACWMS\ProcessedFiles\{ddMMyyyyHHmmss}`
  - Example:  
    `C:\GACWMS\ProcessedFiles\15062025121245`

- 🔎 Validate processed data:
  - Use **API endpoints** to fetch uploaded data
  - Use **SQL Server Management Studio** (SSMS) or any client to verify records in the database

🧠 **Pro Tips:**
- Enable verbose logging for deeper inspection of file processing
- Check API logs and Worker console output for runtime exceptions or warnings

---

## 🐳 Docker Setup <a name="docker"></a>
Seamlessly containerize and orchestrate your services using **Docker** for local development or deployment.

### 📦 What’s Included:
- ✅ `GAC.WMS.Api/Dockerfile`  
  ➤ Containerizes the ASP.NET Core API project

- ✅ `GAC.WMS.Worker/Dockerfile`  
  ➤ Containerizes the background worker for legacy XML ingestion

- ✅ `docker-compose.yml`  
  ➤ Orchestrates **API**, **Worker**, and **SQL Server** in a unified development environment  
  ➤ Located in the **root directory** for easy access

### 🚀 Benefits:
- Zero local dependencies (no need to install .NET SDK or SQL Server)
- Fast environment spin-up
- Ideal for CI/CD pipeline integrations

---

## ⏱️ Background Worker <a name="background-worker"></a>
A robust background processing system built to support **legacy file-based integration** via scheduled polling and data transformation.

### ⚙️ Key Highlights:

- 🧵 Implemented using **`IHostedService`** from .NET Core background service pattern
- ⏳ **CRON-based scheduler** to trigger polling jobs at configurable intervals  
  *(default: every 5 minutes)*

- 📁 Scans a local folder or SFTP location for incoming XML files
- 🔄 Parses & transforms XML → JSON payloads
- 🚀 Sends the transformed data to WMS via RESTful API endpoints

---

## 🗄️ `GACWMS` Database Overview
The application leverages a **SQL Server** database to store and manage all operational data — from master records to transaction documents.

### 🧱 Key Highlights:
- 🧩 Built using **Entity Framework Core** with the **Code-First** approach  
- 🔄 **Automatic migration** is applied on application startup — no manual intervention required  
- 🔐 Supports rich domain models with relationships between entities like `Customer`, `Product`, `SalesOrder`, and `PurchaseOrder`

### 🧬 Database Schema
Visual representation of the current schema:
<p align="center">
  <img width="618" alt="DatabaseSchema" src="https://github.com/user-attachments/assets/3b6a7494-4e6a-4955-929b-60df657826f1"  width="800" />
</p>

---

