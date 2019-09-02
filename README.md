# MVCWebApiStarter
#### A simple ASP.NET Core Web API & MVC starter with MVC Based Authentication.

------

### Prerequisites
1. [dotnet sdk v2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)
2. Your favorite editor / IDE (I prefer [Visual Studio Code](https://code.visualstudio.com) & [Visual Studio 2019](https://visualstudio.microsoft.com))

------

### Get Started
1. [Download](https://github.com/TheBinaryGuy/MVCWebApiStarter/releases/latest) the template (MVCWebApiStarter.zip)
2. [Install it](https://docs.microsoft.com/en-us/visualstudio/ide/how-to-locate-and-organize-project-and-item-templates)
3. Set these environment variables using [user-secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets):
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConn" "Data Source=App.db"
dotnet user-secrets set "Jwt:Issuer" "https://example.com"
dotnet user-secrets set "Jwt:Audience" "https://example.com"
dotnet user-secrets set "Jwt:SigningKey" "SUPERSECRETLONGSIGNINGKEY"
```
4. Now, issue these commands in the project directory:
```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```
5. Build amazing apps.
6. Profit.
