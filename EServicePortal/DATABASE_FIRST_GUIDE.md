# EServicePortal - Database First Setup Guide

This guide explains how to set up and use the Database First approach in EServicePortal.

## Prerequisites

1. An existing SQL Server database (e.g., MOE_EService_DB)
2. .NET 10 SDK installed
3. EF Core tools installed globally:
   ```bash
   dotnet tool install --global dotnet-ef
   # or update if already installed
   dotnet tool update --global dotnet-ef
   ```

## Quick Start

### 1. Prepare Your Database

Ensure you have an existing database with tables. If not, create one:

```sql
-- Example: Create a sample database
CREATE DATABASE MOE_EService_DB;
GO

USE MOE_EService_DB;
GO

-- Create sample tables
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATETIME2 DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2)
);
```

### 2. Update Connection String

Edit `src/MOE_System.EService.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=MOE_EService_DB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=True;Encrypt=False;TrustServerCertificate=True;"
  }
}
```

### 3. Scaffold from Database

**Option A: Scaffold all tables**

```bash
make ef-scaffold conn="Server=.\SQLEXPRESS;Database=MOE_EService_DB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;"
```

**Option B: Scaffold specific tables only**

```bash
make ef-scaffold-tables conn="Server=.\SQLEXPRESS;Database=MOE_EService_DB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;" tables="Users,Orders"
```

### 4. Verify Generated Files

After scaffolding, check:
- `src/MOE_System.EService.Infrastructure/Data/ApplicationDbContext.cs` - DbContext with DbSets
- `src/MOE_System.EService.Infrastructure/Data/Entities/` - Entity classes

### 5. Build and Run

```bash
make build
make run
```

## Scaffold Command Options

### Full Scaffold Command (Manual)

If you prefer not to use Makefile:

```bash
dotnet ef dbcontext scaffold "YourConnectionString" Microsoft.EntityFrameworkCore.SqlServer \
    --output-dir Data/Entities \
    --context-dir Data \
    --context ApplicationDbContext \
    --project src/MOE_System.EService.Infrastructure \
    --startup-project src/MOE_System.EService.API \
    --force \
    --data-annotations \
    --no-onconfiguring
```

### Scaffold Options Explained:

- `--output-dir Data/Entities` - Where entity classes are generated
- `--context-dir Data` - Where DbContext is generated
- `--context ApplicationDbContext` - Name of the DbContext class
- `--force` - Overwrite existing files
- `--data-annotations` - Use data annotation attributes
- `--no-onconfiguring` - Don't include connection string in DbContext
- `--table TableName` - Scaffold specific table(s)

## Working with Scaffolded Code

### ⚠️ Important Rules

1. **Don't edit generated entity classes directly** - They will be overwritten on next scaffold
2. **Use partial classes** if you need to extend entities
3. **Create DTOs** for API responses (don't expose entities directly)
4. **Custom logic goes in Application layer** - Not in entities

### Extending Entities (Partial Classes)

Create a separate folder for custom entity extensions:

```csharp
// src/MOE_System.EService.Infrastructure/Data/EntityExtensions/User.cs
namespace MOE_System.EService.Infrastructure.Data.Entities;

public partial class User
{
    // Add custom properties or methods here
    public string FullName => $"{FirstName} {LastName}";
    
    // Custom validation logic
    public bool IsEmailValid()
    {
        return !string.IsNullOrEmpty(Email) && Email.Contains("@");
    }
}
```

## When Database Schema Changes

When your database schema changes (new tables, columns, etc.):

1. **Re-scaffold the database:**
   ```bash
   make ef-scaffold conn="YourConnectionString"
   ```

2. **Review changes** in generated files

3. **Update DTOs and services** if needed

4. **Rebuild and test:**
   ```bash
   make build
   make test
   ```

## Common Issues

### Issue 1: "No DbContext was found"

**Solution:** Ensure your connection string is correct and the database exists.

### Issue 2: "Build failed after scaffolding"

**Solution:** The scaffolded code might have naming conflicts. Check:
- Entity class names don't conflict with existing classes
- Namespace issues in generated files

### Issue 3: "Cannot connect to database"

**Solution:** 
- Verify SQL Server is running
- Check connection string format
- Ensure database exists
- Check firewall settings

## Connection String Examples

### SQL Server Express (Windows Authentication)
```
Server=.\\SQLEXPRESS;Database=MOE_EService_DB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;
```

### SQL Server (SQL Authentication)
```
Server=.\\SQLEXPRESS;Database=MOE_EService_DB;User Id=sa;Password=YourPassword;Encrypt=False;TrustServerCertificate=True;
```

### Azure SQL Database
```
Server=tcp:yourserver.database.windows.net,1433;Database=MOE_EService_DB;User Id=username;Password=password;Encrypt=True;TrustServerCertificate=False;
```

### LocalDB
```
Server=(localdb)\\mssqllocaldb;Database=MOE_EService_DB;Trusted_Connection=True;
```

## Best Practices

1. ✅ Always use DTOs for API responses
2. ✅ Keep business logic in Application layer services
3. ✅ Use repository pattern for data access
4. ✅ Add validation using FluentValidation in Application layer
5. ✅ Use partial classes for entity extensions
6. ✅ Version control your connection strings securely
7. ✅ Document database schema changes
8. ⚠️ Never expose entity classes directly in API responses
9. ⚠️ Don't modify scaffolded entity files directly

## Need Help?

- Check EF Core documentation: https://docs.microsoft.com/en-us/ef/core/
- Run `make help` to see all available commands
- Review generated code in `Data/Entities` folder after scaffolding
