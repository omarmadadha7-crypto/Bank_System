# Bank System

Three layers, each one depends only on the layer below it:

    Bank_System      (Web API: controllers, middleware, Program.cs)
        |
        v
    Business Logic   (services, DTOs, rules)
        |
        v
    Data Access      (entities, enums, BankContext, EF Core configuration)

## First run

1. Open `Bank_System.sln` in Visual Studio and restore the packages.
2. Edit `Bank_System/appsettings.json`: connection string and `Jwt:Key` (at least 32 characters).
3. Package Manager Console (Default project = `Data Access`, Startup project = `Bank_System`):

       Add-Migration InitialCreate
       Update-Database

4. Run `Bank_System`. On startup it seeds: roles (Admin, Manager, Teller), USD and JOD,
   a head-office branch and the user `admin` (password from `Seed:AdminPassword`).
5. Open `Bank_System/Bank_System.http` and run request 1 (login), then the others.

## Already have a solution?

Copy only the folders of C# code. Do NOT overwrite your own:
`*.csproj`, `Bank_System.sln`, `Program.cs`, `appsettings.json`, `Properties/launchSettings.json`.
Use them as a reference and merge what you need.
