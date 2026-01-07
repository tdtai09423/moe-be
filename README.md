# MOE System - Clean Architecture Web API

## Project Structure

This project follows Clean Architecture principles with four main layers:

### 1. **Domain Layer** (`MOE_System.Domain`)
- Contains enterprise business logic and entities
- No dependencies on other layers
- **Folders:**
  - `Entities/` - Domain entities
  - `Common/` - Common domain classes (BaseEntity)

### 2. **Application Layer** (`MOE_System.Application`)
- Contains business logic and use cases
- Depends only on Domain layer
- **Folders:**
  - `Interfaces/` - Application interfaces
  - `Services/` - Application services
- **Key Files:**
  - `DependencyInjection.cs` - Registers application services

### 3. **Infrastructure Layer** (`MOE_System.Infrastructure`)
- Contains data access, external services, and infrastructure concerns
- Depends on Application layer
- **Folders:**
  - `Data/` - Database context (ApplicationDbContext) and configurations
  - `Repositories/` - Repository implementations (GenericRepository, UnitOfWork)
- **Key Files:**
  - `DependencyInjection.cs` - Registers infrastructure services and DbContext

### 4. **API Layer** (`MOE_System.API`)
- ASP.NET Core Web API presentation layer
- Contains controllers and API configuration
- Depends on Application and Infrastructure layers
- **Folders:**
  - `Controllers/` - API controllers
- **Key Files:**
  - `Program.cs` - Application startup and configuration

## Dependencies

### NuGet Packages
- **Swashbuckle.AspNetCore** (10.1.0) - Swagger/OpenAPI documentation
- **Microsoft.Extensions.DependencyInjection.Abstractions** (10.0.1) - Dependency injection

## Running the Application

### Build the solution:
```bash
dotnet build
```

### Run the API:
```bash
cd src/MOE_System.API
dotnet run
```

### Access Swagger UI:
Once the application is running, navigate to:
- **Swagger UI:** `http://localhost:5221/swagger`
- **API Base URL:** `http://localhost:5221`

## Project References

The dependency flow follows Clean Architecture:
```
API → Infrastructure → Application → Domain
```

- **Domain** - No dependencies
- **Application** → Domain
- **Infrastructure** → Application
- **API** → Application, Infrastructure

## Configuration

### Swagger Configuration
Swagger is configured in [Program.cs](src/MOE_System.API/Program.cs):
- Enabled in Development environment
- Accessible at `/swagger` endpoint
- Provides interactive API documentation

### Dependency Injection
Each layer has its own `DependencyInjection.cs` file with extension methods:
- `AddApplication()` - Registers application services
- `AddInfrastructure()` - Registers infrastructure services

## Getting Started

1. **Add Domain Entities:**
   - Create entity classes in `src/MOE_System.Domain/Entities/`
   - Inherit from `BaseEntity` for common properties

2. **Create Application Services:**
   - Define interfaces in `src/MOE_System.Application/Interfaces/`
   - Implement services in `src/MOE_System.Application/Services/`
   - Register in `DependencyInjection.cs`

3. **Implement Infrastructure:**
   - Create repository implementations in `src/MOE_System.Infrastructure/Repositories/`
   - Register in `DependencyInjection.cs`

4. **Add API Controllers:**
   - Create controllers in `src/MOE_System.API/Controllers/`
   - Use dependency injection to access services

## Example API Endpoint

A sample `WeatherForecastController` is included to demonstrate:
- Controller structure
- Dependency injection
- Swagger documentation with XML comments

## Next Steps

- Add Entity Framework Core for database access
- Implement CQRS pattern with MediatR
- Add authentication and authorization
- Implement logging and error handling
- Add unit tests and integration tests
