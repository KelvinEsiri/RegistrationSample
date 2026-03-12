# RegistrationSample

A full-stack user registration and profile management application built with **ASP.NET Core 8**, following **Clean Architecture** principles. It consists of a REST API backend and a Blazor WebAssembly frontend.

---

## Changelog

### 2026-03-12 — User Profile Management Features

- **New personal fields:** `middleName`, `maritalStatus` added to registration, profile view, and profile edit.
- **New professional section:** `occupation`, `employer`, `yearsOfExperience`, `linkedInUrl` added to all DTOs, the `User` entity, and both web views.
  - **View Models added:** `RegisterViewModel`, `LoginViewModel`, `EditProfileViewModel`, `UserProfileViewModel`, `AuthResponseViewModel` — used as strongly-typed Blazor form models.
- **EF Core migrations:** `AddProfessionalDetails` and `AddMaritalStatus` applied to the SQLite schema.
- **Profile picture URL** is now updatable via `PUT /api/profile`.
- **Security fix:** Upgraded `MimeKit` to 4.15.1 to resolve a CRLF injection vulnerability; SMTP port parsing made safe against invalid config values.
- **CORS** origins updated to align with the Web project's development URLs.

---

## Features

- User registration with personal, professional, and academic information
- JWT-based authentication (login / token issuance)
- Protected profile view and update endpoints
- ASP.NET Core Identity for password hashing and user management
- SQLite database (zero-configuration, file-based)
- EF Core code-first migrations
- Swagger UI for API exploration
- Blazor WebAssembly frontend (client-side SPA) that consumes the API via `HttpClient`
- JWT stored in `localStorage` via `TokenService`; injected into requests by `ApiAuthorizationMessageHandler`
- Email service integration (configurable SMTP via MailKit)

---

## Tech Stack

### Framework — .NET 8 / ASP.NET Core 8

All projects target `net8.0`. The API uses `Microsoft.NET.Sdk.Web`; the Web project uses `Microsoft.NET.Sdk.BlazorWebAssembly`.

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

---

### Authentication & Identity — ASP.NET Core Identity + JWT Bearer

`Microsoft.AspNetCore.Identity.EntityFrameworkCore` handles user creation, password hashing, and role management. `Microsoft.AspNetCore.Authentication.JwtBearer` issues and validates JWT tokens on every protected request.

**Login → receive token:**
```json
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "YourPassword1"
}

→ { "token": "eyJhbGci...", "expiration": "2026-03-12T00:00:00Z" }
```

**Use token on protected routes:**
```
GET /api/profile
Authorization: Bearer eyJhbGci...
```

---

### Database — SQLite + Entity Framework Core

`Microsoft.EntityFrameworkCore.Sqlite` provides a zero-configuration, file-based database. No database server to install — EF Core creates the `.db` file on first run and applies migrations automatically.

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=registration.db"
}
```

Code-first migrations track schema changes:
```
Migrations/
├── 20260311121507_AddProfessionalDetails.cs
└── 20260311121643_AddMaritalStatus.cs
```

---

### Email — MailKit + MimeKit

`MailKit` and `MimeKit` provide SMTP email sending (e.g. registration confirmation, notifications). Configured via `appsettings.json`:

```json
"Email": {
  "Host": "smtp.example.com",
  "Port": "587",
  "Username": "user@example.com",
  "Password": "password",
  "SenderName": "RegistrationSample",
  "SenderEmail": "noreply@example.com"
}
```

---

### API Documentation — Swagger / OpenAPI

`Swashbuckle.AspNetCore` generates interactive API docs, available in Development mode at:

```
http://localhost:5257/swagger
```

You can test all endpoints (including authenticated ones by pasting your JWT) directly from the browser.

---

### Frontend — Blazor WebAssembly (Client-Side SPA)

`RegistrationSample.Web` is a Blazor WebAssembly application — the .NET runtime and app are downloaded to the browser and run entirely client-side. It communicates with the API via `HttpClient`:

```
Browser (Blazor WASM)  →  HttpClient + ApiAuthorizationMessageHandler  →  REST API
```

Authentication state is managed by `TokenService`, which persists the JWT in `localStorage`. `ApiAuthorizationMessageHandler` automatically attaches the `Authorization: Bearer` header to every outgoing API request.

---

## Architecture

The solution follows Clean Architecture with four layers:

```
RegistrationSample/
├── RegistrationSample.Domain          # Entities, domain interfaces
├── RegistrationSample.Application     # DTOs, application service interfaces
├── RegistrationSample.Infrastructure  # EF Core, ASP.NET Identity, service implementations
├── RegistrationSample.API             # ASP.NET Core Web API (backend)
└── RegistrationSample.Web             # Blazor WebAssembly SPA (frontend)
```

### Dependency flow

```
API  →  Application  →  Domain
              ↑
        Infrastructure

Blazor WASM (browser)  →  REST API  (via HttpClient)
```

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

No database server is required — the app uses SQLite and creates the database file automatically on first run.

---

## Getting Started

### 1. Clone the repository

```bash
git clone <repo-url>
cd RegistrationSample
```

### 2. Configure the API

Open `src/RegistrationSample.API/appsettings.json` and update the following sections as needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=registration.db"
  },
  "Jwt": {
    "Key": "<your-secret-key-min-32-chars>",
    "Issuer": "RegistrationSample.API",
    "Audience": "RegistrationSample.Web"
  },
  "Email": {
    "Host": "smtp.example.com",
    "Port": "587",
    "Username": "user@example.com",
    "Password": "password",
    "SenderName": "RegistrationSample",
    "SenderEmail": "noreply@example.com"
  }
}
```

> **Security note:** Never commit real secrets to source control. Use environment variables or [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) in development.

### 3. Run the API

```bash
cd src/RegistrationSample.API
dotnet run
```

The API starts on `http://localhost:5257` (HTTP) / `https://localhost:7265` (HTTPS).  
Swagger UI is available at `http://localhost:5257/swagger` in Development mode.

### 4. Run the Web frontend

In a separate terminal:

```bash
cd src/RegistrationSample.Web
dotnet run
```

The Blazor WebAssembly app runs on `http://localhost:5289` (HTTP) / `https://localhost:7071` (HTTPS). It calls the API at the URL configured in `src/RegistrationSample.Web/appsettings.json` under `ApiBaseUrl` (defaults to `http://localhost:5257`).

---

## API Endpoints

### Authentication — `POST /api/auth/register`

Registers a new user.

**Request body:**

| Field | Type | Required | Description |
|---|---|---|---|
| `firstName` | string | ✓ | |
| `middleName` | string | | |
| `lastName` | string | ✓ | |
| `email` | string | ✓ | Must be unique |
| `password` | string | ✓ | Min 8 chars, upper + lower + digit |
| `confirmPassword` | string | ✓ | Must match `password` |
| `dateOfBirth` | datetime | ✓ | |
| `gender` | string | ✓ | |
| `maritalStatus` | string | | |
| `phone` | string | | |
| `address` | string | | |
| `city` | string | | |
| `state` | string | | |
| `country` | string | | |
| `postalCode` | string | | |
| `occupation` | string | | Professional title |
| `employer` | string | | Current employer |
| `yearsOfExperience` | int | | |
| `linkedInUrl` | string (URL) | | |
| `institution` | string | ✓ | |
| `degree` | string | ✓ | |
| `fieldOfStudy` | string | ✓ | |
| `graduationYear` | int | | |
| `gradePointAverage` | string | | |
| `certifications` | string | | |
| `studentId` | string | | |
| `academicYear` | string | | |

---

### Authentication — `POST /api/auth/login`

Returns a JWT bearer token.

**Request body:**

```json
{
  "email": "user@example.com",
  "password": "YourPassword1"
}
```

**Response:**

```json
{
  "token": "<jwt>",
  "userId": "<guid>",
  "email": "user@example.com",
  "fullName": "John Doe",
  "expiration": "<utc-datetime>"
}
```

Tokens are valid for **8 hours**.

---

### Profile — `GET /api/profile` *(requires Bearer token)*

Returns the authenticated user's profile.

---

### Profile — `PUT /api/profile` *(requires Bearer token)*

Updates the authenticated user's profile. Accepts the same fields as registration (excluding password fields), plus `profilePictureUrl` (URL string).

---

## Password Policy

| Rule | Requirement |
|---|---|
| Minimum length | 8 characters |
| Uppercase letter | Required |
| Lowercase letter | Required |
| Digit | Required |
| Special character | Not required |
| Unique email | Enforced |

---

## Project Structure

```
src/
├── RegistrationSample.API/
│   ├── Controllers/
│   │   ├── AuthController.cs       # POST /api/auth/register, /login
│   │   └── ProfileController.cs    # GET/PUT /api/profile (authorized)
│   └── Program.cs                  # JWT auth, Swagger, CORS, DB init
│
├── RegistrationSample.Application/
│   ├── DTOs/                       # Request/response data transfer objects
│   └── Interfaces/                 # IAuthService, IUserProfileService
│
├── RegistrationSample.Domain/
│   ├── Entities/User.cs            # IdentityUser extended with profile fields
│   └── Interfaces/IEmailService.cs
│
├── RegistrationSample.Infrastructure/
│   ├── Data/ApplicationDbContext.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── UserProfileService.cs
│   │   └── EmailService.cs
│   └── DependencyInjection.cs      # Extension method to register all services
│
└── RegistrationSample.Web/
    ├── Pages/                      # Blazor routable components
    │   ├── Register.razor
    │   ├── Login.razor
    │   └── Profile/
    │       ├── ProfileIndex.razor  # View profile
    │       └── ProfileEdit.razor   # Edit profile
    ├── Services/
    │   ├── TokenService.cs         # localStorage JWT state (token, fullName, userId)
    │   └── ApiAuthorizationMessageHandler.cs  # Injects Bearer token into HttpClient
    ├── Models/
    │   ├── RegisterViewModel.cs
    │   ├── LoginViewModel.cs
    │   ├── EditProfileViewModel.cs
    │   ├── UserProfileViewModel.cs
    │   ├── AuthResponseViewModel.cs
    │   └── ErrorViewModel.cs
    ├── Layout/                     # MainLayout, NavMenu
    ├── App.razor                   # Client-side router
    └── Program.cs                  # HttpClient, TokenService, auth handler DI
```

---

## Configuration Reference

| Key | Location | Description |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | API `appsettings.json` | SQLite connection string |
| `Jwt:Key` | API `appsettings.json` | Signing key (keep secret) |
| `Jwt:Issuer` | API `appsettings.json` | Token issuer claim |
| `Jwt:Audience` | API `appsettings.json` | Token audience claim |
| `Email:Host` | API `appsettings.json` | SMTP server hostname |
| `ApiBaseUrl` | Web `appsettings.json` | Base URL of the API |

---

## License

This project is provided as a sample/reference implementation. No license is applied by default.
