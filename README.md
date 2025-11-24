# EventSchedulingPlatformEnhanced

This repository contains two microservices for the Event Scheduling Platform:

1. **AccountService-Api** - Handles user authentication(Login), registration, role management,userinfo and JWT token generation.  
2. **EventService-Api** - Manages event creation, updates,get, delete, and search.

---

## Repository Structure
'''
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
│
├─ README.md  <-- This root file
|
'''
---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [DockerDesktop](https://www.docker.com/)


---

## Running Services Locally

### 1. AccountService-Api


Swagger UI: [https://localhost:7174/swagger/index.html](https://localhost:7174/swagger/index.html)

---

### 2. EventService-Api



Swagger UI: [https://localhost:7218/swagger/index.html](https://localhost:7218/swagger/index.html)

---

## API Communication

- Each microservice is independent and communicates via REST APIs.
- 'EventService-Api' call 'AccountService-Api` endpoints to validate users before creating events.

---

## Environment Configuration

- Environment-specific settings are located in 'appsettings.json' of each service.

---

## Notes

- Each microservice is independently deployable.  
- Swagger/OpenAPI docs are available for both services for easy API exploration.  

---

## Optional Service-Level READMEs

'README.md' inside each microservice folder with:

- Service overview  
- Running instructions  
- Database in-memory setup  
- Docker commands specific to that service  

---

## References
