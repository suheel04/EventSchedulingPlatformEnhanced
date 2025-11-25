# 📌 AccountService-Api

- The **Account Service** handles authentication, registration, and role-based security in the Event Scheduling Platform.  
- It issues JWT tokens, hashes passwords securely, and masks PII data during logs.
- Authenticate checks(e.g., Admin vs. User) for set-role Endpoint
- It contains 4 end points register,login,set-role and GetUser.
- Unit test done for register EndPoint
- Dockerfile to create the image

---

## 📁 Project Structure
```
AccountService-Api/src/
├─ AccountService.Api/
│ ├─ Controllers/ → Endpoints for Login, Register, SetRole, GetUser
│ ├─ Program.cs → Auth, DI, Logging configs....
│ └─ appsettings.json → Serilog config
|
├─ AccountService.Core/ → **Business Logic**
│ ├─ Dtos/ → Request + Response DTOs
│ ├─ Services/ → Business logic (Auth, hashing)
│ └─ Interfaces/ → Abstraction for Services
|
├─ AccountService.Infrastructure/ → **Database access**
│ ├─ Database/ → EF Core InMemory DB
│ └─ Repository/ → CRUD operations
docker/
├─ Dockerfile


AccountService-Api/tests/
├─ AccountService.Api.Tests/
│ ├─ Controllers/ → Unit test for Controller
│ 
├─ AccountService.Core.Tests/ → **Business Logic**
│ ├─ Service/ → Unit test for Service
|
├─ AccountService.Infrastructure.Tests/ → **DB Operations**
│ ├─ Repository/ → Unit test for Repository

```
---

## 🔑 Security Implementations

| Feature | Status |
|--------|--------|
| Password hashing | PBKDF2 (`PasswordHasher<TUser>`) |
| Logging | **Structured logs using Serilog** |
| PII Protection | Email masking in logs |
| Authorization | JWT Bearer + Role-based |


### 🔁 API Versioning
 - example **POST /api/v1/account/login**

---

 ## 🧪 Unit Tests

- Xunit + Moq

---

## 🐋 Docker Deployment
### 🧩 Prerequisites

### 1️⃣ Install Docker Desktop  
Make sure Docker Desktop is installed and running.

### 2️⃣ Create & Trust Development Certificate (only once)

Create folder **C:\certs** and export a dev certificate:

```powershell
dotnet dev-certs https -ep C:\certs\devcert.pfx -p DevP@ssw0rd1
dotnet dev-certs https --trust
Get-ChildItem -Force C:\certs | Format-Table Name,Length,Mode,Attributes -AutoSize
Get-Item C:\certs\devcert.pfx | Format-List Name,Length,Mode,Attributes 

```
 ### 3️⃣ Create Docker Network (only once)
 ```
 docker network create microservices
 ```

### 🔨 Build Image
```
docker build --no-cache -f docker/Dockerfile -t accountserviceapi:prd .
```
### ▶️ Run Container
🔹 Production Mode
```
docker run -d --name accountserviceapi_prd `
  --network microservices `
  -p 8001:8080 `
  -p 8002:8081 `
  -v "C:\certs:/https:ro" `
  -e ASPNETCORE_ENVIRONMENT=Production `
  -e ASPNETCORE_URLS="http://0.0.0.0:8080;https://0.0.0.0:8081" `
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/devcert.pfx `
  -e ASPNETCORE_Kestrel__Certificates__Default__Password="DevP@ssw0rd1" `
  accountserviceapi:prd

```
🔹 Staging Mode (For Swagger Testing)
```
docker run -d --name accountserviceapi_stg `
  --network microservices `
  -p 8004:8080 `
  -p 8005:8081 `
  -v "C:\certs:/https:ro" `
  -e ASPNETCORE_ENVIRONMENT=Staging `
  -e ASPNETCORE_URLS="http://0.0.0.0:8080;https://0.0.0.0:8081" `
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/devcert.pfx `
  -e ASPNETCORE_Kestrel__Certificates__Default__Password="DevP@ssw0rd1" `
  accountserviceapi:prd

```
### 📘 Swagger Endpoints
| Protocol  | Swagger UI                                                                                             | Swagger JSON                                                                                                     |
| --------- | ------------------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------- |
| **HTTP**  | [http://localhost:8004/account/swagger/index.html](http://localhost:8004/account/swagger/index.html)   | [http://localhost:8004/account/swagger/v1/swagger.json](http://localhost:8004/account/swagger/v1/swagger.json)   |
| **HTTPS** | [https://localhost:8005/account/swagger/index.html](https://localhost:8005/account/swagger/index.html) | [https://localhost:8005/account/swagger/v1/swagger.json](https://localhost:8005/account/swagger/v1/swagger.json) |

### 🛠 Diagnostics
```
docker ps
docker logs accountserviceapi_prd
docker logs accountserviceapi_stg

```
---

## 🌟Project Goal

Because the goal is **feature demonstration**, not full enterprise scale.

This application focuses on demonstrating:

✔ Password hashing
✔ JWT roles
✔ AutoMapper
✔ FluentValidation
✔ Structured logging
✔ InMemory EF DB
✔ Docker deployment

 - Not all patterns are applied everywhere — only where relevant.
 - Where relevant feature usage is implemented, source file links are provided in root README for easy reference in **Security & Architecture Features**