# ⚔️ CodeArena

> CodeArena is an ASP.NET Core MVC web application where users solve coding challenges, earn XP, level up, and climb the leaderboard.

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-blue)
![EF Core](https://img.shields.io/badge/EF_Core-Code_First-green)
![SignalR](https://img.shields.io/badge/SignalR-RealTime-orange)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-blue)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Concept & Goals](#-concept--goals)
- [Technologies Used](#️-technologies-used)
- [Architecture](#architecture)
- [Features](#-features)
- [Business Logic & Design Decisions](#-business-logic--design-decisions)
- [Validation](#-validation)
- [Seeding](#-seeding)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Configuration](#configuration)
- [Testing](#-testing)
- [Deployment](#deployment)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

---

## 📖 About the Project

CodeArena is a gamified coding platform where users:

- Solve programming challenges
- Earn XP based on difficulty
- Level up using a custom progression system
- Compete on a global leaderboard

The application demonstrates a **clean layered architecture**, real-time updates with SignalR, and good OOP practices.

---

## 🎯 Concept & Goals

The project was designed with the following goals:

- Demonstrate **ASP.NET Core MVC best practices**
- Apply **clean architecture and separation of concerns**
- Implement **gamification mechanics** (XP, levels)
- Integrate **real-time updates (SignalR)**
- Provide a **responsive and intuitive UI**
- Ensure **testability and maintainability**

---

## 🛠️ Technologies Used

| Technology           |Version | Purpose                              |
|----------------------|--------|--------------------------------------|
| ASP.NET Core MVC     | 8.0    | Web framework                        |
| Entity Framework Core| 8.0    | ORM (Code-First)                     |
| SQL Server           | -      | Database                             |
| ASP.NET Identity     | -      | Authentication & authorization       |
| SignalR              | -      | Real-time leaderboard updates        |
| Bootstrap            | 5.3    | Frontend styling                     |
| jQuery               | -      | Client-side validation               |
| ToastR               | -      | Notifications                        |
| NUnit                | -      | Unit testing                         |
| Moq                  | -      | Mocking dependencies                 |

---

## Architecture

The project follows a **layered architecture**:

```
CodeArena/
│
├── Web
│   ├── Areas
│   ├── Controllers
│   ├── Views
│   ├── Models
│   └── Hubs
│
├── Services
│   ├── Core
│   └── DTOs
│
├── Data
│   ├── DbContext
│   ├── Models
│   ├── Repositories
│   ├── Migrations
│   └── Seeding
│
└── Common
    ├── ApplicationConstants
    ├── OutputMessages
    └── Utilities
```

Test projects are also divided by layer (for unit tests) and there is a separate project for integration tests.

---

## ✨ Features

### 🧑‍💻 Challenges

1. Public
- Browse, search, and filter
- Difficulty levels: Easy / Medium / Hard
- Tag-based filtering
- Pagination support

2. Admin
- CRUD challenges

### 📤 Submissions

1. Public
- Submit solutions in multiple languages
- Status tracking (Pending / Approved / Rejected)
- Cancel pending submissions

2. Admin
- Review submissions (approve/reject) with feedback

### 📊 User Progress

- XP system based on challenge difficulty
- Level system with nonlinear scaling
- Visual progress bar (XP to next level)

### 🏆 Leaderboard

- Global ranking based on XP
- Rank badges (Top 3)
- Highlights current user
- Real-time updates via SignalR

### 🔔 Notifications

- ToastR-based feedback for main actions

---

## 🧠 Business Logic & Design Decisions

### 1. Layered Architecture

Business logic is isolated in the **Services layer**, ensuring:

- Controllers remain thin
- Logic is reusable and testable
- Clear separation between UI and data

---

### 2. Repository Pattern

Repositories abstract EF Core:

- Prevent tight coupling to DbContext
- Simplify testing (mockable)
- Centralize data access logic

---

### 3. DTO Usage

DTOs are used for:

- Preventing entity overexposure
- Controlling data sent to UI
- Improving performance and clarity

---

### 4. Filtering System

Filtering is implemented via query parameters:

**Challenge:**
- Difficulty
- Status (Solved / Unsolved)
- Tags
- Search
- Active (for admin)

**Submission:**
- Status (Pending / Approved / Rejected)
- Language

Controllers and service take query wrapper parameters which encapsulate filters into a single class.

Reasoning:

- Keeps controllers clean
- Enables composable filtering logic
- Easily extendable

---

### 5. Submission Review Flow

Submissions are approved manually by admin.

Reasoning:

- Simulates real-world code review
- Enables admin-controlled validation
- Adds system depth

---

### XP System

- Easy → 50 XP  
- Medium → 100 XP  
- Hard → 150 XP  

- First solve bonus
- Prevents duplicate rewards per challenge

---

### Level System

- Custom nonlinear progression
- Fast early progression
- Slower scaling at higher levels

### SignalR Integration

Used for leaderboard updates:

- Server broadcasts changes
- Clients update UI dynamically

Reasoning:

- Avoids page refreshes
- Improves UX

### Caching (Memory Cache)

Used for resources that are accessed often, such as challenges or submissions:

- Reduces DB queries
- Improves performance

### Pagination Design

- Lightweight wrapper object encapsulates both the returned enumerable and the total count

Reasoning:

- Cleaner code

### Identity & Security

- User registration, login, logout (ASP.NET Identity)
- User / Admin roles seeded on startup
- Admin with credentials seeded on startup
- Password requirements are configurable 

---

## ✅ Validation

### ViewModel Validation

* Required fields
* Input constraints

### Service Validation

* Prevent duplicates (XP, submission)
* Validates ownership
* Enforce business rules

---

## 🌱 Seeding

* Initial challenges seeded
* Roles and admin user are seeded on startup.
* Designed for easy extension

---

## ✅ Prerequisites

Make sure you have the following installed before running the project:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download)
- [Visual Studio 2022/2026](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Git](https://git-scm.com/)

---

## 🚀 Getting Started

Follow these steps to get the project running locally.

### 1. Clone the repository

```bash
git clone https://github.com/mxrt0/CodeArena.git
cd CodeArena
```

### 2. Restore

```bash
dotnet restore
```

### 3. Configure the Database

- In `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=CodeArenaDbDev;Trusted_Connection=True;Encrypt=false"
}
```

- Configure this default connection string to your liking before proceeding.

### 4. Configure admin credentials

- In `appsettings.Development.json`:

```json
"AdminUser": {
    "Email": "adminemail@abc.com",
    "Password": "secret-admin-pass"
  }
```

- You can use those credentials to log in to the Admin account

### 5. Apply database migrations

**CLI:**
```bash
dotnet ef database update --startup-project CodeArena.Web --project CodeArena.Data
```

**Package Manager Console:**
```powershell
Update-Database -StartupProject CodeArena.Web -Project CodeArena.Data
```

### 6. Run the application

```bash
dotnet run --project CodeArena.Web --launch-profile https
```

- The app will be available at `https://localhost:7113` or `http://localhost:5240`.

---

## Configuration

- The following settings are optional and can be adjusted at any time in `appsettings.json`:

**Identity:**
```json
"Identity": {
  "Password": {
    "RequireDigit": false,
    "RequireUppercase": false,
    "RequireLowercase": false,
    "RequireNonAlphanumeric": false,
    "RequiredLength": 6,
    "RequiredUniqueChars": 1
  },
  "Lockout": {
    ///...
  },
  "SignIn": {
    ///...
  },
  "User": {
    ///...
  }
}
```

**Cookie:**
```json
"Cookie": {
    "LoginPath": "/Identity/Account/Login",
    "LogoutPath": "/Identity/Account/Logout",
    "AccessDeniedPath": "/Home/Error/403",
    "ExpireTimeSpan": 60,
    "SlidingExpiration": true
  }
```

---

## 🧪 Testing

### Unit Tests

* Services

* Utilities

Essential services have 100% or near-100% coverage.
Total service coverage exceeds 65%.

### Integration tests

* Controllers

* Filtering/pagination

* Database interactions

* App flow

Cover main points of application.

### Test Execution

Run:

```bash
dotnet test
```

---

## Deployment

The app is deployed via **Azure App Service** and is publicly available [here](https://codearena-f2b7g2b6aedcc6dc.westeurope-01.azurewebsites.net)

No CI/CD pipeline is configured as of yet and the project is currently manually deployed.

---

## 🤝 Contributing

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a new branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m "Add some feature"`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Open a Pull Request

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE.md) file for details.

---

## 📬 Contact

Martin Terziev – [@mxrt0](https://github.com/mxrt0)

Project Link: [https://github.com/mxrt0/CodeArena](https://github.com/mxrt0/CodeArena)

---
