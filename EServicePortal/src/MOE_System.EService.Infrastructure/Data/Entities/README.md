# Scaffolded Entities

This folder contains entity classes generated from the database using EF Core scaffolding.

**⚠️ Important:** 
- These files are auto-generated and will be regenerated when you run scaffold commands
- Do not manually edit these files as changes will be overwritten
- If you need to extend entities, use partial classes in a separate folder

## How to generate entities:

```bash
# From project root
make ef-scaffold conn="Server=.\SQLEXPRESS;Database=MOE_EService_DB;Trusted_Connection=True;Encrypt=False;"
```

## Scaffold specific tables only:

```bash
make ef-scaffold-tables conn="YourConnectionString" tables="Table1,Table2"
```
