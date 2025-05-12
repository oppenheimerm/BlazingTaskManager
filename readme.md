# BlazingTaskManager

##	 About
A fully responsive task managment application uisng Asp.net Core and 
Entity Framework.  This app allows users to manage tasks efficiently with 
an intuitive interface, create, update, and track tasks with due dates 
and priorities, automated status updates, and assign tasks to multiple 
users and track completion etc.

## Prerequisites
- Visual Studio 2022 or later
- NET 9
- Entity Framework Core tools reference - .NET Core CLI
- SQL Server or any other database
- Entity Framework Core
- [TailwindCss](https://tailwindcss.com/)
- Node.js and npm (for TailwindCss)

## Getting Started 🦸

If not alreeady installed, install Entity Framework Core tools reference - .NET Core CLI

```dotnet tool install --global dotnet-ef```

Update the tool using the following command:
```dotnet tool update --global dotnet-ef```

## Projects

### BlazingTaskManager.AuthenticationAPI

#### Dependencies
- ```BCrypt.Net-NextBCrypt.Net-Next```
- ```Microsoft.EntityFrameworkCore.Design"```
- ```Swashbuckle.AspNetCore```

#### Setup
 **Enable secret storage** - The Secret Manager tool operates on 
 project-specific configuration settings stored in your user profile.

 **Use the CLI**
The Secret Manager tool includes an init command. To use user secrets, 
run the following command in the project directory ``BlazingTaskManager.AuthenticationAPI``) :

```dotnet user-secrets init```

or RIght click on the project in the Solution Explorer and select 
Manage User Secrets. This will create a new file called secrets.json
for more information see [Safe storage of app secrets in development in ASP.NET Core]
(https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=windows)

Setup your database connection string in the secrets.json file
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=localhost;Database=BlazingTaskManager;User Id=sa;Password=your_password;"
  }
}
```

#### Database Migration

We're ready to create the database. Run the following command in the
project directory ``BlazingTaskManager.AuthenticationAPI``:

```bash
dotnet ef migrations add InitialCreate
```
Update the database

```bash
dotnet ef database update
```

#### Initialize the Roles

Now the database is created, we need to initialize the roles. 
Uncomment the following in the ***RolesController*** :
``` [Authorize]```
then run the app and navigate to the following endpoint in your browser:

```
/api/roles/init-roles
```

Your role are created and you can now use the app.

### BlazingTaskManager.Client
This is the Blazor WebAssembly client application. It is a single-page
application (SPA) that communicates with the BlazingTaskManager.AuthenticationAPI
using HTTP requests. The client is responsible for rendering the user interface and handling user interactions.


#### Dependencies
- ```Blazored.LocalStorage```

#### Setup
 
 ##### Dependency Injection - Add a scoped HttpClient in `Program.cs`, with your
 app host adddress.(found in ***BlazingTaskMager.Client/Properties/ `launchSettings.json`***)
 i.e.
 ```csharp
 builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7015/") });
 ```

 ##### WebConfig

 ```json
 {
  "ConnectionStrings": {
    "AccountAPIBaseURL": "api/Accounts",
  },
  "ApplicationSettings": {
    "LocalStorageKey": "BT_LS"
  },
  "JwtSettings": {
    "Secret": ">>INSERT YOUR SECRET KEY HERE<<",
    "Issuer": "http://localhost:5000",
    "Audience": "http://localhost:5000"
  }
}
```

### BlazingTaskManager.Shared
#### Dependencies
- ```Microsoft.AspNetCore.Authentication.JwtBearer```
- ```Microsoft.AspNetCore.Identity.EntityFrameworkCore```
- ```Microsoft.EntityFrameworkCore" Version="9.0.4```
- ```Microsoft.EntityFrameworkCore.SqlServer```
- ```Microsoft.Extensions.Identity.Core```
- ```Serilog.AspNetCore" Version=```