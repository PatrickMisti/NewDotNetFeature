# Create and Migrate Database ef
```bash
$ dotnet tool install --global dotnet-ef
$ dotnet add package Microsoft.EntityFrameworkCore.Design
$ dotnet ef migrations add InitialCreate
$ dotnet ef database update
```