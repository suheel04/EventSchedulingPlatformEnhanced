# 📌 Event Service-Api

- The **Event Service** handles Event CRUD operation and Serarch events by date range and location with pagimation, and role-based security in the Event Scheduling Platform.  
- JWT Authentication- Only Authenticated users can create event, Authenticate checks(e.g., Admin vs. User)
- It contains 5 Endpoints Event CRUD-4 and searchevents with pagination
- Unit test done for Search EndPoint
- Dockerfile to create the image

---

## 📁 Project Structure
```
EventService-Api/src/
├─ EventService.Api/
│ ├─ Controllers/ → Endpoints for Event CRUD-4 and searchevents with pagination
│ ├─ Program.cs → Auth, DI, Logging configs....
│ └─ appsettings.json → Serilog config
|
├─ EventService.Core/ → **Business Logic**
│ ├─ Dtos/ → Request + Response DTOs
│ ├─ Services/ → Business logic (Auth, hashing)
│ └─ Interfaces/ → Abstraction for Services
|
├─ EventService.Infrastructure/ → **Database access**
│ ├─ Database/ → EF Core InMemory DB
│ └─ Repository/ → CRUD operations
docker/
├─ Dockerfile


EventService-Api/tests/
├─ EventService.Api.Tests/
│ ├─ Controllers/ → Unit test for Controller
│ 
├─ EventService.Core.Tests/ → **Business Logic**
│ ├─ Service/ → Unit test for Service
|
├─ EventService.Infrastructure.Tests/ → **DB Operations**
│ ├─ Repository/ → Unit test for Repository

```
---

## 🔑 Security Implementations

| Feature | Status |
|--------|--------|
| Logging | **Structured logs using Serilog** |
| Authorization | JWT Bearer + Role-based |

### 🔄 Cross-Service Flow: Event Creation

📌 EventService ensures events are tied to real users:

1️⃣ API receives CreateEventRequest with UserId
2️⃣ Calls AccountService using Refit
→ GET /api/v1/account/{id}
3️⃣ If user exists → create event
4️⃣ If not → return error "User not registered"

🧠 This ensures data integrity and prevents orphan events.

### 📌 Relationship Structure

  - One Category can have multiple Events
  - Each Event must reference a CategoryId

### 🔁 API Versioning
 - example **POST api/v1/events**

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
docker build --no-cache -f docker/Dockerfile -t eventserviceapi:prd .
```
### ▶️ Run Container
```
docker run -d --name eventserviceapi_prd `
  -p 8001:8080 `
  -p 8002:8081 `
  -v "C:\certs:/https:ro" `
  -e ASPNETCORE_ENVIRONMENT=Production `
  -e ASPNETCORE_URLS="http://0.0.0.0:8080;https://0.0.0.0:8081" `
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/devcert.pfx `
  -e ASPNETCORE_Kestrel__Certificates__Default__Password="DevP@ssw0rd1" `
  eventserviceapi:prd
```
---

## 🌟Project Goal

Because the goal is **feature demonstration**, not full enterprise scale.

This application focuses on demonstrating:

✔ JWT roles
✔ FluentValidation
✔ Structured logging
✔ InMemory EF DB
✔ Docker deployment

 - Not all patterns are applied everywhere — only where relevant.
 - Where relevant feature usage is implemented, source file links are provided in root README for easy reference in **Security & Architecture Features**