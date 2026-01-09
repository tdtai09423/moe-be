# Config
API := src/MOE_System.EService.API/MOE_System.EService.API.csproj
INFRA := src/MOE_System.EService.Infrastructure/MOE_System.EService.Infrastructure.csproj
SLN := MOE_System.EService.sln
TEST := MOE_System.EService.UnitTest/MOE_System.EService.UnitTest.csproj

FRAMEWORK ?= net10.0

.DEFAULT_GOAL := help

restore: ## Restore NuGet packages
	@dotnet restore $(SLN)

build: ## Build the solution
	@dotnet build $(SLN) -f $(FRAMEWORK) --no-restore

run: ## Run the Web API project
	@dotnet run --project $(API) -f $(FRAMEWORK)

test: ## Run unit test
	@dotnet test $(TEST)

clean: ## Clean the solution
	@dotnet clean ${SLN}

# ===============================
# EF Core – Database First (Scaffolding)
# ===============================

ef-scaffold: ## Scaffold DbContext and entities from existing database
	@if [ -z "$(conn)" ]; then \
		echo "Usage: make ef-scaffold conn=\"YourConnectionString\""; \
		echo "Example: make ef-scaffold conn=\"Server=.\\SQLEXPRESS;Database=MOE_EService_DB;Trusted_Connection=True;\""; \
		exit 1; \
	fi
	@dotnet ef dbcontext scaffold "$(conn)" Microsoft.EntityFrameworkCore.SqlServer \
		--output-dir Data/Entities \
		--context-dir Data \
		--context ApplicationDbContext \
		--project $(INFRA) \
		--startup-project $(API) \
		--force \
		--data-annotations \
		--no-onconfiguring

ef-scaffold-tables: ## Scaffold specific tables (conn=... tables="Table1,Table2")
	@if [ -z "$(conn)" ] || [ -z "$(tables)" ]; then \
		echo "Usage: make ef-scaffold-tables conn=\"YourConnectionString\" tables=\"Table1,Table2\""; \
		exit 1; \
	fi
	@dotnet ef dbcontext scaffold "$(conn)" Microsoft.EntityFrameworkCore.SqlServer \
		--output-dir Data/Entities \
		--context-dir Data \
		--context ApplicationDbContext \
		--project $(INFRA) \
		--startup-project $(API) \
		--table $(tables) \
		--force \
		--data-annotations \
		--no-onconfiguring

ef-list-tables: ## List all tables in the database (requires connection string in appsettings)
	@echo "Connecting to database to list tables..."
	@dotnet ef dbcontext info --project $(INFRA) --startup-project $(API)

help: ## Show help
	@echo "Available targets:"
	@awk 'BEGIN {FS = ":.*##"} /^[a-zA-Z0-9_-]+:.*##/ { printf "  %-15s %s\n", $$1, $$2 }' $(MAKEFILE_LIST)
