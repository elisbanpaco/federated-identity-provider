# Federated Identity Provider

A robust authentication server built with **OpenIddict** and **.NET 10**, designed to manage identity across multiple applications in a centralized manner.

## Overview

This is an open-source Identity Provider (IdP) ready for community contributions. It provides secure authentication for multiple client applications through industry-standard OAuth 2.0 / OpenID Connect protocols.

## Features

- **OAuth 2.0 / OpenID Connect** authentication via OpenIddict
- **Supported flows**:
  - Password Flow (classic username/password login)
  - Refresh Token Flow (session renewal without re-authentication)
- **User management** with ASP.NET Core Identity
- **Database** SQL Server with Entity Framework Core
- **UI** ASP.NET Core Razor Pages
- **Extensible architecture** for multi-application integration

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server) (Local or Express)

### Quick Start

1. Clone the repository
2. Configure environment variables
3. Run database migrations
4. Start the server

```bash
# Clone and navigate
git clone https://github.com/your-org/federated-identity-provider.git
cd federated-identity-provider

# Configure connection string (see Configuration section)
# Edit appsettings.json or create .env file

# Run migrations
dotnet ef database update

# Start the server
dotnet run
```

The application will be available at `http://localhost:5000`

## Configuration

### appsettings.json

Configure the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=IdentityDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Environment Variables (Optional)

You can also use environment variables:

```env
ConnectionStrings__DefaultConnection=Server=YOUR_SERVER;Database=IdentityDB;Trusted_Connection=True;TrustServerCertificate=True
```

### Database Setup

The project uses Entity Framework Core migrations. After configuring the connection string:

```bash
dotnet ef database update
```

## Project Structure

```
federated-identity-provider/
├── Controllers/           # API controllers
│   ├── AuthorizationController.cs
│   └── ProtectedRoutesController.cs
├── Data/                  # DbContext
│   └── ApplicationDbContext.cs
├── Models/                # Domain entities
│   └── AppUser.cs
├── Pages/                 # Razor Pages
│   ├── Shared/
│   ├── Error.cshtml
│   ├── Index.cshtml
│   └── Privacy.cshtml
├── wwwroot/               # Static files (css, js, lib)
├── Properties/
│   └── launchSettings.json
├── .env                   # Environment variables (not committed)
├── appsettings.json       # Application configuration
├── appsettings.Development.json
├── federated-identity-provider.csproj
├── federated-identity-provider.sln
├── LICENSE
└── README.md
```

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/connect/token` | POST | Get access token |
| `/connect/authorize` | GET | OAuth authorization |
| `/api/protected` | GET | Protected resource example |

## Tech Stack

- [.NET 10](https://dotnet.microsoft.com/)
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core)
- [OpenIddict](https://documentation.openiddict.io/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [SQL Server](https://www.microsoft.com/sql-server)
- [ASP.NET Core Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)

## Contributing

We welcome contributions! Please read our contributing guidelines before submitting PRs.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
