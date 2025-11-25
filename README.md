# ✨ Event Scheduling Platform – Enhanced

A modular microservices-based platform for managing user accounts and scheduling events with secure authentication, API versioning, Docker deployment, and Swagger documentation.

🧩 Microservices Overview

| Service                | Description                                      | Tech            | Swagger                       |
| ---------------------- | ------------------------------------------------ | --------------- | ----------------------------- |
| **AccountService-Api** | Authentication (Login), Registration, Roles, JWT | .NET 8, EF Core | `/account/swagger/index.html` |
| **EventService-Api**   | Event CRUD, Filtering & Search                   | .NET 8, EF Core | `/event/swagger/index.html`   |


---

## 📁 Repository Structure
```
EventSchedulingPlatformEnhanced/
├─ AccountService-Api/
│  ├─ docker/
│  ├─ src/   
│  ├─ tests/
|  ├─ AccountService.Api.sln
│  └─ README.md
|	
├─ EventService-Api/
│  ├─ docker/
│  ├─ src/
│  ├─ tests/
|  ├─ EventService.Api.sln
│  └─ README.md
|
├─ PostmanCollection/
|
├─ README.md  <-- This root file
|
```
---
🚀 Features

✔️ API Versioning
✔️ JWT Authentication + Role-based Access
✔️ Swagger UI with Custom Route Prefix
✔️ Docker Multi-Environment Support (Staging | Prod)
✔️ HTTPS support using local dev cert in Docker
✔️ Refit-based Inter-service communication
✔️ Serilog structured logging
✔️ EF Core with InMemory database
✔️ Unit testing with Moq & xUnit
✔️ Secure password hashing
✔️ Fluent validation
✔ Sensitive logging hygiene Masking PI data  
✔ Proper error handling & validation 

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [DockerDesktop](https://www.docker.com/)

- ### 🧪 Local Execution
Run using Visual Studio, CLI or Docker.

---

## 🚀 Swagger API Documentation (LocalDebug,DockerStagingMode)

Below are the Swagger endpoints available during local debugging:

### 📌 Account Service - Swagger Endpoints 

| Protocol | Swagger UI | Swagger JSON |
|---------|------------|--------------|
| **HTTP** | [http://localhost:8004/account/swagger/index.html](http://localhost:8004/account/swagger/index.html) | [http://localhost:8004/account/swagger/v1/swagger.json](http://localhost:8004/account/swagger/v1/swagger.json) |
| **HTTPS** | [https://localhost:8005/account/swagger/index.html](https://localhost:8005/account/swagger/index.html) | [https://localhost:8005/account/swagger/v1/swagger.json](https://localhost:8005/account/swagger/v1/swagger.json) |

> Note: Swagger route is customized using `c.RoutePrefix = "account/swagger"` to align with service namespace and API versioning structure.

### 📌 Event Service - Swagger Endpoints

| Protocol | Swagger UI | Swagger JSON |
|---------|------------|--------------|
| **HTTP** | [http://localhost:9004/event/swagger/index.html](http://localhost:9004/event/swagger/index.html) | [http://localhost:9004/event/swagger/v1/swagger.json](http://localhost:9004/event/swagger/v1/swagger.json) |
| **HTTPS** | [https://localhost:9005/event/swagger/index.html](https://localhost:9005/event/swagger/index.html) | [https://localhost:9005/event/swagger/v1/swagger.json](https://localhost:9005/event/swagger/v1/swagger.json) |

> Note: Swagger UI route is customized using `c.RoutePrefix = "event/swagger"` to match service namespace and API versioning structure.

---

## API Communication

- Each microservice is independent and communicates via REST APIs.
- 'EventService-Api' call 'AccountService-Api` endpoints to validate users before creating events.

- ## 📦 Docker Container Endpoints
In Docker its deployed in two environment Staging(Swagger access) and Production

Inside Docker Network (Service-to-Service):
> Make sure containers are on the same Docker network (e.g., `microservices`).

- http://accountserviceapi_stg:8080 
- https://accountserviceapi_stg:8081 
- http://eventserviceapi_stg:8080 
- https://eventserviceapi_stg:8081 
---
- http://accountserviceapi_prd:8080 
- https://accountserviceapi_prd:8081 
- http://eventserviceapi_prd:8080 
- https://eventserviceapi_prd:8081


---

## Environment Configuration

- Environment-specific settings are located in 'appsettings.json' of each service.

---

## Notes

- Each microservice is independently deployable.  
- Swagger/OpenAPI docs are available for both services for easy API exploration.  

---

## Service-Level READMEs

'README.md' inside each microservice folder with:

- Service overview  
- Running instructions  
- EndPoint functionalty  
- Docker commands specific to that service  

---
## 🧪 API Testing (Postman Collection)

A Postman Collection is included in the repository to easily test all API endpoints for both microservices.

### 📁 Path
```
EventSchedulingPlatformEnhanced\PostmanCollection\EventSchedulingPlatformEnhanced-Staging.postman_collection.json
```
### 📝 Included Requests:
| Service | Module | Endpoints Included |
|--------|--------|------------------|
| Account Service API | account | Register, Login,Set Role, Get Users |
| Event Service API | event | Create Event, Get Event,Update Event,Delete Event, Search Events |

---
## 🔐 Security & Architecture Features

### 1️⃣ Password Hashing – PBKDF2
- Uses strong hashing algorithm with salt for security  
📌 Implementation:
```
AccountService-Api\src\AccountService.Core\Helpers\PasswordHasher.cs
AccountService-Api\src\AccountService.Core\Services\RegisterService.cs //Save PasswordHash
AccountService-Api\src\AccountService.Core\Services\LoginService.cs  //Verify Password
```

### 2️⃣ JWT Authentication + Role-Based Authorization
- Secures APIs with JWT tokens
- Different role access (Admin/User)
📌 Configuration + Usage:
```
AccountService-Api\src\AccountService.Api\Program.cs
AccountService.Api/Controllers/AccountController.cs //[Authorize(Roles = "Admin")] in SetRole Endpoint; [Authorize] in GetUserById
EventService.Api/Controllers/EventController.cs   //[Authorize(Roles = ...)] All endpoints
```

### 3️⃣ Structured Logging – Serilog
- Logs stored as key-value for powerful filtering/searching  
📌 Setup:
```
AccountService-Api\src\AccountService.Api\appsettings.json
AccountService-Api\src\AccountService.Api\Program.cs
```

📌 Example usage in code:
```
AccountService.Infrastructure\Repository\UserRepository.cs
AccountService-Api\src\AccountService.Api\Controllers\AccountController.cs //Register
```

### 4️⃣ Email Masking in Logs
- Protects sensitive user information in logs  
📌 Implementation:
```
AccountService-Api\src\AccountService.Core\Helpers\MaskingHelper.cs
AccountService-Api\src\AccountService.Core\Services\RegisterService.cs
```
📌 Sample Logs:
```
AccountService-Api\src\AccountService.Api\logs
```

### 5️⃣ AutoMapper for DTO ↔ Entity Mapping
📌 Mapping Profiles:
```
AccountService-Api\src\AccountService.Core\Mappings\RegisterProfile.cs
```

📌 Usage:
```
AccountService-Api\src\AccountService.Core\Services\RegisterService.cs
```

### 6️⃣ Refit Client for Microservice Communication
- Used by Event Service to call Account Service API  
📌 Interface + Configuration:
```
EventService-Api\src\EventService.Core\Interfaces\IAccountApi.cs
EventService-Api\src\EventService.Api\Program.cs     // builder.Services.AddRefitClient(...)
EventService-Api\src\EventService.Core\Service\EventManagementService.cs // CreateAsync() 
```

### 7️⃣ Fluent Validation
- Validates input data like event details & user registration  
📌 Validators:
```
AccountService-Api\src\AccountService.Core\Validation\RegisterDtoValidator.cs
AccountService-Api\src\AccountService.Core\Services\RegisterService.cs //Register(..)
```

### 8️⃣ Docker Deployment
- Both microservices containerized & run via Docker Desktop 
- Detailed instruction how to setup and run from docker has been mentioned in respective Servise README

📌 Dockerfile locations:
```
AccountService-Api\docker\Dockerfile
EventService-Api\docker\Dockerfile
```

📌 Sample Run Command:
```
docker run -d --name eventserviceapi_dev `
  -p 8001:8080 `
  -p 8002:8081 `
  -v "C:\certs:/https:ro" `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e ASPNETCORE_URLS="http://0.0.0.0:8080;https://0.0.0.0:8081" `
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/devcert.pfx `
  -e ASPNETCORE_Kestrel__Certificates__Default__Password="DevP@ssw0rd1" `
  eventserviceapi:dev
```

### 9️⃣ API Versioning
- Implemented API Versioning to support evolution of services without breaking existing clients
📌 Configuration + Usage:
```
AccountService-Api\src\AccountService.Api\Program.cs //builder.Services.AddApiVersioning()
AccountService-Api\src\AccountService.Api\Controllers\AccountController.cs //[Route("api/v{version:apiVersion}/account")]
```

### 🔟 Unit Testing with Moq & xUnit
- Unit Tests implemented using
  - xUnit → Test framework
  - Moq → Mocking dependencies such as repositories and services
  - InMemory EF Core → For DB querying logic without a real database
```

EventService-Api\tests\EventService.Api.Tests\Controllers\EventControllerTests.cs
EventService-Api\tests\EventService.Core.Tests\Service\EventManagementServiceTests.cs
EventService-Api\tests\EventService.Infrastructure.tests\Repository\EventRepositoryTests.cs
```

---

## 🏆 Compliance & Best Practices

✔ Implements Domain Driven segregation (Core, Api, Infrastructure)  
✔ In-Memory DB for development & testing  
✔ Sensitive logging hygiene  
✔ Proper error handling & validation  

## ❕ Note on Architecture Scope
- This project is a sample implementation designed to demonstrate a wide range of best-practice features such as:
  - Secure password hashing
  - Fluent validation
  - Refit client communication
  - Serilog structured logging
  - JWT-based authorization & role handling
  - EF Core with InMemory database
  - Unit testing with Moq & xUnit
  - Docker deployment
  - API Versioning
	
 - Because the goal is __feature demonstration__, not full enterprise scale
   - Business logic distribution is minimal
   - Not all implemented features (e.g., Fluent Validation, AutoMapper, Structured Logging, Authorization, Refit clients, etc.) are applied everywhere across every endpoint
   - The goal is to demonstrate how these techniques can be used in a real solution.
   - Where relevant feature usage is implemented, source file links are provided above for easy reference in **Security & Architecture Features**


