# TodoList Manager - ASP.NET Core 8 Backend

একটি প্রফেশনাল এবং স্কেলেবল **Clean Architecture** ভিত্তিক ASP.NET Core 8 Web API ব্যাকএন্ড প্রজেক্ট।

---

## 🏛️ Architecture Overview (Clean Architecture)

প্রজেক্টটি ৪টি প্রধান লেয়ারে বিভক্ত:

```
TodoList Manager/
│
├── TodoListManager.sln                     # Visual Studio / .NET Solution File
│
├── src/
│   ├── TodoListManager.Domain/             # ১. Domain Layer (Core Enterprise Business Rules)
│   │   ├── Common/
│   │   │   └── BaseEntity.cs               # Id, CreatedAt, UpdatedAt
│   │   ├── Entities/
│   │   │   ├── TodoItem.cs                 # Todo Entity (Title, Priority, Status, DueDate, etc.)
│   │   │   └── Category.cs                 # Category Entity
│   │   └── Enums/
│   │       ├── PriorityLevel.cs            # Low, Medium, High, Urgent
│   │       └── TodoStatus.cs               # Pending, InProgress, Completed, Archived
│   │
│   ├── TodoListManager.Application/        # ২. Application Layer (Business Logic & Use Cases)
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
│   ├── TodoListManager.Infrastructure/     # ৩. Infrastructure Layer (Database & External Services)
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs     # EF Core DbContext with Audit Tracking
│   │   │   └── DbInitializer.cs            # Auto-migration & Initial Seed Data
│   │   ├── Repositories/
│   │   │   ├── Repository.cs               # Generic Repository implementation
│   │   │   ├── TodoItemRepository.cs       # Filtering, Searching, Sorting, Stats
│   │   │   └── CategoryRepository.cs
│   │   └── DependencyInjection.cs          # Clean Service Registration
│   │
│   └── TodoListManager.API/                # ৪. Presentation / Web API Layer (Controllers & HTTP)
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

## 🚀 কিভাবে রান করবেন (How to Run)

প্রজেক্টটি রান করা খুবই সহজ। ডিফল্টভাবে **SQLite** ডাটাবেজ কনফিগার করা আছে, তাই কোনো অতিরিক্ত ডাটাবেজ ইনস্টল করার প্রয়োজন নেই। প্রজেক্ট রান হওয়ামাত্র স্বয়ংক্রিয়ভাবে `todolist.db` তৈরি হয়ে প্রাথমিক ডেমো ডাটা সিড হয়ে যাবে।

### টার্মিনাল কমান্ড:

```bash
# ১. ডিপেনডেন্সি রিস্টোর ও বিল্ড
dotnet build

# ২. API রান করা
dotnet run --project src/TodoListManager.API
```

API চালু হওয়ার পর ব্রাউজারে প্রবেশ করুন:
- **Swagger Documentation**: [http://localhost:5000/swagger](http://localhost:5000/swagger) অথবা [http://localhost:5000](http://localhost:5000)

---

## 📡 API Endpoints Summary

### 📝 Todos Endpoints (`/api/todos`)
| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/todos` | সার্চ, ফিল্টারিং ও পেজিনেশন সহ সকল টোডো পাওয়া |
| `GET` | `/api/todos/{id}` | নির্দিষ্ট টাস্কের বিস্তারিত দেখা |
| `POST` | `/api/todos` | নতুন টাস্ক তৈরি করা |
| `PUT` | `/api/todos/{id}` | টাস্ক আপডেট করা |
| `PATCH` | `/api/todos/{id}/toggle` | টাস্ক কমপ্লিট / আনকমপ্লিট টগল করা |
| `DELETE` | `/api/todos/{id}` | টাস্ক ডিলিট করা |
| `GET` | `/api/todos/stats` | টাস্কের পরিসংখ্যান (Completed, Pending, Overdue, Rate) |
| `GET` | `/api/todos/upcoming?days=7` | আসন্ন ডেডলাইনের টাস্কসমূহ |

### 📂 Categories Endpoints (`/api/categories`)
| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/categories` | সব ক্যাটাগরি ও তাদের টাস্ক সংখ্যা দেখা |
| `GET` | `/api/categories/{id}` | নির্দিষ্ট ক্যাটাগরি দেখা |
| `POST` | `/api/categories` | নতুন ক্যাটাগরি তৈরি করা (Name, ColorHex) |
| `PUT` | `/api/categories/{id}` | ক্যাটাগরি আপডেট করা |
| `DELETE` | `/api/categories/{id}` | ক্যাটাগরি মুছে ফেলা |

---

## 💡 মূল বৈশিষ্ট্যসমূহ (Key Features)

1. **Clean Architecture Separation**: ডোমেইন, অ্যাপ্লিকেশন, ইনফ্রাস্ট্রাকচার ও প্রেজেন্টেশন সম্পূর্ণ আলাদা লেয়ারে বিভক্ত।
2. **Repository & Service Pattern**: বিজনেস লজিক এবং ডাটাবেজ অ্যাক্সেস আলাদা থাকায় সহজে টেস্ট করা যায়।
3. **Automatic Auditing**: `BaseEntity` এর মাধ্যমে `CreatedAt` এবং `UpdatedAt` স্বয়ংক্রিয়ভাবে EF Core এ ট্র্যাকিং হয়।
4. **Global Exception Handling**: যেকোনো এরর সুন্দর ইউনিফর্ম JSON রেসপন্সে রিটার্ন করে (`ApiResponse<T>`)।
5. **Swagger / OpenAPI**: ব্রাউজার থেকেই সব API টেস্ট করার চমৎকার ইন্টারফেস।
6. **CORS Enabled**: যেকোনো ফ্রন্টএন্ড (React, Next.js, Vue, ইত্যাদি) থেকে সরাসরি কানেক্ট করা সম্ভব।
