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
- **Modern UI** with Blazor Server
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
cp .env.example .env  # Linux/macOS
copy .env.example .env  # Windows

# Run migrations
cd AuthServer.Host
dotnet ef database update

# Start the server
dotnet run
```

The application will be available at `http://localhost:5000`

## Configuration

### Environment Variables

Create a `.env` file in the root directory:

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
├── AuthServer.Host/
│   ├── Components/          # Blazor components
│   ├── Controllers/        # API controllers
│   ├── Data/               # DbContext and configuration
│   ├── DTOs/               # Data Transfer Objects
│   ├── Entities/           # Domain entities
│   ├── Migrations/         # EF Core migrations
│   ├── Workers/            # Background workers
│   ├── Program.cs          # Entry point
│   └── *.csproj            # Project file
├── .env                    # Environment variables
├── .gitignore              # Git ignore patterns
└── ServidorIdentidad.slnx  # Solution file
```

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/connect/token` | POST | Get access token |
| `/connect/authorize` | GET | OAuth authorization |

## Tech Stack

- [.NET 10](https://dotnet.microsoft.com/)
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core)
- [OpenIddict](https://documentation.openiddict.io/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [SQL Server](https://www.microsoft.com/sql-server)
- [Blazor](https://blazor.net/)

## Contributing

We welcome contributions! Please read our contributing guidelines before submitting PRs.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Community

- 💬 Join our discussions
- 🐛 Report bugs via Issues
- ⭐ Star the project if you find it useful
