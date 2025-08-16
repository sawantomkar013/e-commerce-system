# Database Setup

This project uses **Entity Framework Core** with **SQLite** for a lightweight, file-based database. The database schema is managed through migrations.

### ⚙️ How to Set Up the Database

1.  **Navigate to the `OrderService.Domain.DataAccess` project.**
    ```bash
    cd src/OrderService/Domain/OrderService.Domain.DataAccess
    ```

2.  **Apply the migrations to create the database.**
    The following command will create a `app.db` file in the project's root directory and apply all pending migrations.

    ```bash
    dotnet ef database update --project .
    ```

3.  **Confirm Database Creation.**
    A file named `OrderService.db` should now exist, containing your database schema and tables. This database will be used by the `OrderService` when you run it.
	
### ⬆️ How to Manage Migrations

Migrations are a way to version control your database schema, allowing you to evolve it over time as your data model changes. Here are the key commands for managing migrations in EF Core.

#### 1. Add a new migration

This command scaffolds a new migration file based on the changes you've made to your entity models. It creates a C# file with `Up` and `Down` methods to describe the database changes.

**Command:**
```bash
dotnet ef migrations add <MigrationName>
```
**Example:**
```
dotnet ef migrations add AddNewProductsTable
```

#### 2. Update the database

This command applies all new or pending migrations to the database. It brings your database schema up to date with your latest migration files. This is the command you used in the setup section.

**Command:**
```bash
dotnet ef database update
```

#### 3. Remove the last migration

This command reverts the last migration that was added. It's useful if you've made a mistake and need to undo the last set of changes.

**Command:**
```bash
dotnet ef migrations remove
```