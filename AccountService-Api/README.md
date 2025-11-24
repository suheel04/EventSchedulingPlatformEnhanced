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

✓ Docker Desktop installed
✓ Certificate available for HTTPS 

### 🔨 Build Image
```
docker build --no-cache -f docker/Dockerfile -t accountserviceapi:prd .
```
### ▶️ Run Container
```
docker run -d --name accountserviceapi_prd `
  -p 9004:8080 `
  -p 9005:8081 `
  -v "C:\certs:/https:ro" `
  -e ASPNETCORE_ENVIRONMENT=Production `
  -e ASPNETCORE_URLS="http://0.0.0.0:8080;https://0.0.0.0:8081" `
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/devcert.pfx `
  -e ASPNETCORE_Kestrel__Certificates__Default__Password="DevP@ssw0rd1" `
  accountserviceapi:prd
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