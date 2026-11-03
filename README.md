# TodoList Manager - ASP.NET Core 8 Backend

A professional and scalable ASP.NET Core 8 Web API backend project built on **Clean Architecture**.

---

## 🏛️ Architecture Overview (Clean Architecture)

The project is divided into 4 main layers:

```
TodoList Manager/
│
├── TodoListManager.sln                     # Visual Studio / .NET Solution File
│
├── src/
│   ├── TodoListManager.Domain/             # 1. Domain Layer (Core Enterprise Business Rules)
│   │   ├── Common/
│   │   │   └── BaseEntity.cs               # Id, CreatedAt, UpdatedAt
│   │   ├── Entities/
│   │   │   ├── TodoItem.cs                 # Todo Entity (Title, Priority, Status, DueDate, etc.)
│   │   │   └── Category.cs                 # Category Entity
│   │   └── Enums/
│   │       ├── PriorityLevel.cs            # Low, Medium, High, Urgent
│   │       └── TodoStatus.cs               # Pending, InProgress, Completed, Archived
│   │
│   ├── TodoListManager.Application/        # 2. Application Layer (Business Logic & Use Cases)
│   │   ├── Common/
│   │   │   ├── ApiResponse.cs              # Consistent API Response Envelope
│   │   │   └── PagedResult.cs              # Pagination Metadata Wrapper
│   │   ├── DTOs/
│   │   │   ├── Category/                   # CategoryDto, Create/UpdateCategoryDto
│   │   │   └── Todo/                       # TodoItemDto, Create/UpdateTodoItemDto, TodoStatsDto
│   │   ├── Exceptions/
│   │   │   └── AppExceptions.cs            # NotFoundException, BadRequestException, ValidationException
│   │   ├── Interfaces/
│   │   │   ├── Repositories/               # IRepository<T>, ITodoItemRepository, ICategoryRepository
│   │   │   └── Services/                   # ITodoService, ICategoryService
│   │   └── Services/
│   │       ├── TodoService.cs              # Business logic implementation
│   │       └── CategoryService.cs          # Category logic implementation
│   │
│   ├── TodoListManager.Infrastructure/     # 3. Infrastructure Layer (Database & External Services)
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs     # EF Core DbContext with Audit Tracking
│   │   │   └── DbInitializer.cs            # Auto-migration & Initial Seed Data
│   │   ├── Repositories/
│   │   │   ├── Repository.cs               # Generic Repository implementation
│   │   │   ├── TodoItemRepository.cs       # Filtering, Searching, Sorting, Stats
│   │   │   └── CategoryRepository.cs
│   │   └── DependencyInjection.cs          # Clean Service Registration
│   │
│   └── TodoListManager.API/                # 4. Presentation / Web API Layer (Controllers & HTTP)
│       ├── Controllers/
│       │   ├── BaseApiController.cs        # Common ApiController base
│       │   ├── TodosController.cs          # CRUD, Filter, Search, Pagination, Toggle, Stats
│       │   └── CategoriesController.cs     # Category CRUD
│       ├── Middleware/
│       │   └── GlobalExceptionHandlerMiddleware.cs # Centralized Error Handling
│       ├── Program.cs                      # Pipeline, Swagger, CORS, DI configuration
│       └── appsettings.json                # Connection string and log settings
```

---

## 🚀 How to Run

Running the project is very straightforward. By default, it is configured to use a **SQLite** database, so no additional database installation is required. As soon as the project runs, `todolist.db` will be automatically created and seeded with initial demo data.

### Terminal Commands:

```bash
# 1. Restore dependencies and build
dotnet build

# 2. Run the API
dotnet run --project src/TodoListManager.API
```

Once the API is running, open your browser and navigate to:
- **Swagger Documentation**: [http://localhost:5000/swagger](http://localhost:5000/swagger) or [http://localhost:5000](http://localhost:5000)

---

## 📡 API Endpoints Summary

### 📝 Todos Endpoints (`/api/todos`)
| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/todos` | Get all todos with search, filtering, and pagination |
| `GET` | `/api/todos/{id}` | Get details of a specific task |
| `POST` | `/api/todos` | Create a new task |
| `PUT` | `/api/todos/{id}` | Update an existing task |
| `PATCH` | `/api/todos/{id}/toggle` | Toggle task completion status (Complete / Incomplete) |
| `DELETE` | `/api/todos/{id}` | Delete a task |
| `GET` | `/api/todos/stats` | Get task statistics (Completed, Pending, Overdue, Rate) |
| `GET` | `/api/todos/upcoming?days=7` | Get tasks with upcoming deadlines |

### 📂 Categories Endpoints (`/api/categories`)
| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/categories` | Get all categories along with their task counts |
| `GET` | `/api/categories/{id}` | Get a specific category |
| `POST` | `/api/categories` | Create a new category (Name, ColorHex) |
| `PUT` | `/api/categories/{id}` | Update a category |
| `DELETE` | `/api/categories/{id}` | Delete a category |

---

## 💡 Key Features

1. **Clean Architecture Separation**: Domain, Application, Infrastructure, and Presentation are separated into distinct layers.
2. **Repository & Service Pattern**: Decoupled business logic and database access for easy unit testing and maintainability.
3. **Automatic Auditing**: Automatically tracks `CreatedAt` and `UpdatedAt` in EF Core via `BaseEntity`.
4. **Global Exception Handling**: Returns any errors in a clean, uniform JSON response format (`ApiResponse<T>`).
5. **Swagger / OpenAPI**: Interactive browser interface for testing all API endpoints.
6. **CORS Enabled**: Ready to connect directly from any frontend framework (React, Next.js, Vue, etc.).
