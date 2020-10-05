# Point of sale
Is an API in C# .Net Core 3.1 based in Clean Architecture using pattern CQRS.


## Technologies

- ASP
- FluentValidation
- MediatR
- AutoMapper
- EntityFrameworkCore
- Identity
- JWT
- Mailkit 


## Features

- CQRS with MediatR Library
- MediatR Pipeline 
- Entity Framework Core - Code First
- Response Wrappers
- Healthchecks
- Pagination
- Microsoft Identity with JWT Authentication
- Refresh Tokens
- Role based Authorization
- Custom Exception Handling Middlewares
- Fluent Validation
- MapperProfile
- Auth Tables



## Prerequisites

- NET Core 3.1 SDK
- Visual Studio or Visual Code
- Entity Framework Tools

## Install

Download the project and restore.


```bash
dotnet restore
```

Configure your settings in CleanCQRS\API\appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CleanCQRS;User Id=sa;Password=Contrasena01;"
  },
  "AppSettings": {
    "Token": "Super Secret Key"
  },
  "MailSettings": {
    "SmtpHost": "in-v3.mailjet.com",
    "SmtpPort": 587,
    "SmtpUser": "9d13c4b81d82621ce577f326cfa7e6fe",
    "SmtpPass": "dac9843de60d7a1086e082aba84725c9",
    "DisplayName": "Angel Martinez"
  },
  "JWTSettings": {
    "Key": "C1CF4B7DC4C4175B6618DE4F55CA428",
    "Issuer": "CoreIdentity",
    "Audience": "CoreIdentityUser",
    "DurationInMinutes": 4
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

Open the Package Manager Console in Visual Studio and run next command.
```
Update-Database -Project Infrastructure -Context ApplicationDbContext
```

Set start project 'API' and run with Kestrel, no with IIS.


### Install postman
- Visit [www.getpostman.com](www.getpostman.com) and download the version of Postman required for your platform 
- Install Postman 

### Import request in postman
- Click in button 'Import'
- Push in Upload Files
- Select  CleanCQRS\API Point of sale.postman_collection


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
