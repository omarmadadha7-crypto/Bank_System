# 🏦 Bank System — Banking REST API
 
> A layered **ASP.NET Core Web API** for a banking system, with **JWT authentication**, **role-based access control**, and a **code-first SQL database** built on Entity Framework Core.
 
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-Code_First-512BD4)
![JWT](https://img.shields.io/badge/Auth-JWT-000000?logo=jsonwebtokens&logoColor=white)
 
---
 
## 🎯 Why this project
 
I built this backend to go beyond tutorials and practice how real systems are structured: separated layers, secure authentication, and a database that can be rebuilt from code.
 
| What I built | What it shows |
|---|---|
| **Three-layer architecture** (API → Business Logic → Data Access) | I separate concerns, so each layer has one job and is easy to change or test |
| **JWT authentication + role-based authorization** (Admin / Manager / Teller) | I understand that staff must only reach what their role allows |
| **EF Core code-first with migrations** | The database schema is versioned and reproducible from code |
| **Automatic data seeding on startup** | The app runs on a fresh machine with roles, currencies, a head-office branch, and an admin user, with no manual SQL |
| **Services and DTOs** | Data is exchanged through DTOs instead of exposing raw database entities |
| **Multi-currency model** (USD, JOD) | Currencies are part of the data model from day one |
| **Ready-to-run `.http` file** | Every endpoint can be tested straight from Visual Studio |
 
---
 
## 🏗️ Architecture
 
Three layers, each one depends only on the layer below it:
 
```
Bank_System      (Web API: controllers, middleware, Program.cs)
    |
    v
Business Logic   (services, DTOs, rules)
    |
    v
Data Access      (entities, enums, BankContext, EF Core configuration)
```
 
| Layer | Responsibility |
|---|---|
| **Bank_System** | Receives HTTP requests, handles authentication and authorization, returns responses |
| **Business Logic** | Applies the banking rules and maps between entities and DTOs |
| **Data Access** | Defines the entities and talks to the database through `BankContext` |
 
---
 
## ✨ Features
 
- 🔐 **JWT login** with secure token generation
- 👥 **Role-based access** for three staff roles: `Admin`, `Manager`, `Teller`
- 🏢 **Branch support**, starting with a seeded head-office branch
- 💱 **Currency support**, with `USD` and `JOD` seeded
- 🌱 **Auto-seeding** of roles, currencies, branch, and the admin user
- 🧱 **Custom middleware**: [what it does, e.g. global error handling]
- [Customers: create / update / search]
- [Accounts: open / close / view balance]
- [Transactions: deposit / withdraw / transfer, with validation such as insufficient balance]
### Roles
 
| Role | Can do |
|---|---|
| Admin | [e.g. manage users, branches, and all data] |
| Manager | [e.g. manage customers and accounts of their branch] |
| Teller | [e.g. handle deposits and withdrawals] |
 
---
 
## 🛠️ Tech Stack & Skills
 
| Category | Tools |
|---|---|
| Language | C# |
| Framework | ASP.NET Core Web API ([.NET version, e.g. 10.0]) |
| ORM | Entity Framework Core (code-first, migrations) |
| Database | [SQL Server / LocalDB] |
| Auth | JWT Bearer tokens |
| IDE | Visual Studio |
| Version control | Git & GitHub |
| Frontend skills | HTML, CSS, JavaScript, TypeScript |
 
---
 
## 🚀 Getting Started
 
### Prerequisites
- Visual Studio 2022 with the **ASP.NET and web development** workload
- .NET SDK [version]
- [SQL Server or LocalDB]
### Setup
 
1. **Clone** the repository
```bash
git clone https://github.com/omarmadadha7-crypto/Bank_System.git
```
2. Open `Bank_System.sln` in Visual Studio and restore the NuGet packages.
3. Edit `Bank_System/appsettings.json`:
   | Setting | Description |
   |---|---|
   | `ConnectionStrings` | Your database connection string |
   | `Jwt:Key` | Secret signing key, **at least 32 characters** |
   | `Seed:AdminPassword` | Password for the seeded `admin` user |
4. Create the database. In the **Package Manager Console**, set *Default project* = `Data Access` and *Startup project* = `Bank_System`, then run:
```
Add-Migration InitialCreate
Update-Database
```
5. Run the `Bank_System` project. On first start it seeds the roles (Admin, Manager, Teller), the currencies (USD, JOD), a head-office branch, and the `admin` user.
6. Open `Bank_System/Bank_System.http`, run request **1 (login)** to get a token, then try the other requests.
> 🔒 Never commit real secrets. For anything beyond local testing, keep `Jwt:Key` and passwords in **User Secrets** or environment variables.
 
---
 
## 🧠 What I Learned
 
- Structuring a backend into **layers** and keeping dependencies one-directional
- Implementing **JWT authentication** and **role-based authorization** in ASP.NET Core
- Modeling a database with **EF Core code-first** and managing it with **migrations**
- Seeding initial data so the project runs anywhere
- Keeping configuration and secrets out of the code
- Testing APIs with `.http` files and reading HTTP status codes properly
---
 
## 🔮 Roadmap
 
- [ ] Unit tests for the Business Logic layer
- [ ] Swagger / OpenAPI documentation
- [ ] Refresh tokens
- [ ] Pagination and filtering on list endpoints
- [ ] Docker support
---
 
## 👋 About Me
 
I'm **Omar Madadha**, a junior **Full Stack Developer**. I build backends with **C# and ASP.NET Core**, and I work on the frontend with **HTML, CSS, JavaScript, and TypeScript**. I care about writing code that is organized, secure, and easy for a team to build on, and this project is a practical example of that.
 
I'm looking for a **junior Full Stack Developer** position where I can contribute and keep growing.
 
**Contact me:**
 
- 📧 Email: [omarmadadha7@gmail.com](mailto:omarmadadha7@gmail.com)
- 💼 LinkedIn: [linkedin.com/in/omar-madadha-0731802b7](https://www.linkedin.com/in/omar-madadha-0731802b7/)
- 🐙 GitHub: [github.com/omarmadadha7-crypto](https://github.com/omarmadadha7-crypto)
 
