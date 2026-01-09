# Project Architecture Comparison

## AdminPortal vs EServicePortal

### AdminPortal - Code First Approach ✍️

**Philosophy:** Define your domain models in code, and let EF Core create/update the database.

#### Workflow:
1. **Create entity classes** in `Domain/Entities/`
2. **Configure relationships** in `ApplicationDbContext`
3. **Create migration** using `make ef-add name=MigrationName`
4. **Update database** using `make ef-update`
5. Database schema is generated from your code

#### Example:
```csharp
// 1. Create entity
public class AccountHolder : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

// 2. Add to DbContext
public DbSet<AccountHolder> AccountHolders { get; set; }

// 3. Configure in OnModelCreating
modelBuilder.Entity<AccountHolder>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Email).IsRequired();
});

// 4. Create migration
// make ef-add name=AddAccountHolder

// 5. Update database
// make ef-update
```

#### Makefile Commands:
```bash
make ef-add name=MigrationName    # Create new migration
make ef-remove                    # Remove last migration
make ef-update                    # Apply migrations to database
make ef-drop                      # Drop database
make ef-reset name=MigrationName  # Reset everything
```

#### Pros:
- ✅ Full control over domain models
- ✅ Version control for schema changes (migrations)
- ✅ Better for new projects
- ✅ Easier to maintain consistency
- ✅ Works well with DDD principles

#### Cons:
- ⚠️ Initial setup required
- ⚠️ Need to maintain migrations

---

### EServicePortal - Database First Approach 🗄️

**Philosophy:** Database already exists, generate your domain models from it.

#### Workflow:
1. **Ensure database exists** with tables
2. **Scaffold from database** using `make ef-scaffold`
3. **Entity classes are generated** in `Data/Entities/`
4. **DbContext is generated** with all DbSets
5. Code is generated from your database schema

#### Example:
```bash
# 1. Database already has these tables:
# - Users (Id, UserName, Email, CreatedAt)
# - Orders (Id, UserId, OrderDate, TotalAmount)

# 2. Scaffold from database
make ef-scaffold conn="Server=.\SQLEXPRESS;Database=MOE_EService_DB;Integrated Security=True;Encrypt=False;"

# 3. Generated files:
# - Data/Entities/User.cs
# - Data/Entities/Order.cs
# - Data/ApplicationDbContext.cs (with DbSets)
```

#### Generated Code Example:
```csharp
// Auto-generated in Data/Entities/User.cs
public partial class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual ICollection<Order> Orders { get; set; }
}

// Auto-generated in Data/ApplicationDbContext.cs
public partial class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
}
```

#### Makefile Commands:
```bash
make ef-scaffold conn="..."                    # Scaffold all tables
make ef-scaffold-tables conn="..." tables="..."  # Scaffold specific tables
make ef-list-tables                            # List database tables
```

#### Pros:
- ✅ Quick setup with existing database
- ✅ DBAs can manage schema directly
- ✅ Works well with legacy databases
- ✅ No migrations to maintain
- ✅ Direct reflection of database structure

#### Cons:
- ⚠️ Need to re-scaffold when schema changes
- ⚠️ Generated code should not be edited
- ⚠️ Less control over entity design
- ⚠️ Must use partial classes for extensions

---

## Side-by-Side Comparison

| Aspect | AdminPortal (Code First) | EServicePortal (Database First) |
|--------|-------------------------|--------------------------------|
| **Entity Location** | `Domain/Entities/` | `Infrastructure/Data/Entities/` |
| **Entity Creation** | Manual | Auto-generated |
| **DbContext** | Manually configured | Auto-generated |
| **Schema Changes** | Create migration → Update DB | Modify DB → Re-scaffold |
| **Version Control** | Migrations tracked | Database is source of truth |
| **Setup Time** | Slower (initial) | Faster (with existing DB) |
| **Flexibility** | High | Medium |
| **Best For** | New projects, DDD | Legacy DBs, DBA-managed |
| **Commands** | `ef-add`, `ef-update` | `ef-scaffold` |

---

## When to Use Each Approach

### Use Code First (AdminPortal) When:
- ✅ Starting a new project from scratch
- ✅ You want full control over domain model design
- ✅ Following Domain-Driven Design principles
- ✅ Need version-controlled schema changes
- ✅ Team prefers code-centric development

### Use Database First (EServicePortal) When:
- ✅ Working with an existing database
- ✅ Database is managed by DBAs
- ✅ Multiple applications share the same database
- ✅ Need to quickly integrate with legacy systems
- ✅ Database schema is stable and well-designed

---

## Working with Both Projects

### AdminPortal Development:
```bash
cd AdminPortal

# 1. Create entity class
# Edit: src/MOE_System.Domain/Entities/NewEntity.cs

# 2. Update DbContext
# Edit: src/MOE_System.Infrastructure/Data/ApplicationDbContext.cs

# 3. Create migration
make ef-add name=AddNewEntity

# 4. Update database
make ef-update

# 5. Build and run
make build
make run
```

### EServicePortal Development:
```bash
cd EServicePortal

# 1. Ensure database exists and has tables

# 2. Scaffold from database (first time or after schema changes)
make ef-scaffold conn="Server=.\SQLEXPRESS;Database=MOE_EService_DB;Integrated Security=True;Encrypt=False;"

# 3. Create DTOs for API
# Edit: src/MOE_System.EService.Application/DTOs/

# 4. Create services
# Edit: src/MOE_System.EService.Application/Services/

# 5. Build and run
make build
make run
```

---

## Extending Generated Entities (EServicePortal)

Since generated entities should not be edited directly, use partial classes:

```csharp
// ❌ Don't edit: Data/Entities/User.cs (generated)
public partial class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}

// ✅ Do create: Data/EntityExtensions/User.cs
namespace MOE_System.EService.Infrastructure.Data.Entities;

public partial class User
{
    // Add custom properties
    public string DisplayName => $"{UserName} ({Email})";
    
    // Add custom methods
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(UserName) && 
               !string.IsNullOrEmpty(Email);
    }
}
```

---

## Summary

Both portals follow **Clean Architecture** but with different approaches to data modeling:

- **AdminPortal**: Control from code → Database follows
- **EServicePortal**: Database exists → Code follows

Choose the approach that best fits your project requirements and team workflow!
