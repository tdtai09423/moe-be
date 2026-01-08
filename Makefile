# Config
API := src/MOE_System.API/MOE_System.API.csproj
INFRA := src/MOE_System.Infrastructure/MOE_System.Infrastructure.csproj
SLN := MOE_System.sln
TEST := tests/MOE_System.Application.Tests/MOE_System.Application.Tests.csproj

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
# EF Core – Migration & Database
# ===============================

ef-add: ## Add a new EF Core migration
	@if [ -z "$(name)" ]; then \
		echo "Please provide a migration name using 'make ef-add name=MigrationName'"; \
		exit 1; \
	fi
	@dotnet ef migrations add $(name) --project $(INFRA) --startup-project $(API)

ef-remove: ## Remove the last EF Core migration
	@dotnet ef migrations remove --project $(INFRA) --startup-project $(API)

ef-update: ## Update the database to the latest migration
	@dotnet ef database update --project $(INFRA) --startup-project $(API)

ef-drop: ## Drop the database
	@dotnet ef database drop --project $(INFRA) --startup-project $(API) --force

ef-reset: ## Drop DB, remove migration, add new one and update (name=MigrationName)
	@if [ -z "$(name)" ]; then \
		echo "Please provide a migration name using 'make ef-reset name=MigrationName'"; \
		exit 1; \
	fi
	@make ef-drop
	@make ef-remove
	@make ef-add name=$(name)
	@make ef-update

help: ## Show help
	@echo "Available targets:"
	@awk 'BEGIN {FS = ":.*##"} /^[a-zA-Z0-9_-]+:.*##/ { printf "  %-15s %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

