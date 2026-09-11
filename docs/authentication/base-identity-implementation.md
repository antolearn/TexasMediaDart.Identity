# TexasMediaDart — Base Identity Implementation

**Document Type:** Technical Implementation Documentation  
**Module:** Identity  
**Application:** TexasMediaDart  
**Status:** Base Implementation Complete  
**Last Updated:** September 2026

---

## 1. Purpose

The TexasMediaDart Identity module provides the authentication foundation for the TexasMediaDart platform.

The Identity module is implemented as an independently deployable service with its own:

- .NET Identity API
- Application layer
- Domain layer
- Infrastructure layer
- SQL database
- CI/CD pipeline
- LOCAL, DEV, and PROD environments

The Flutter frontend integrates with the Identity API to provide:

- User registration
- User login
- Session persistence
- JWT authentication
- Current-user validation
- Protected routes
- Logout
- Refresh-token infrastructure

The Base Identity implementation has been validated end-to-end in the LOCAL environment.

> **Current baseline:** Base Identity is complete. Automatic access-token renewal is the next planned enhancement.

---

## 2. Architecture Overview

The Identity module follows the TexasMediaDart modular architecture.

```text
┌─────────────────────────────┐
│      Flutter Frontend       │
│                             │
│ Registration                │
│ Login                       │
│ Session Management          │
│ Route Protection            │
│ Current User Validation     │
└──────────────┬──────────────┘
               │
               │ HTTP / REST
               │ JWT Bearer Token
               ▼
┌─────────────────────────────┐
│   TexasMediaDart Identity   │
│           API               │
│                             │
│ Register                    │
│ Login                       │
│ Refresh                     │
│ Logout                      │
│ /api/auth/me                │
└──────────────┬──────────────┘
               │
               │ Dapper
               │ Stored Procedures
               ▼
┌─────────────────────────────┐
│ TexasMediaDart Identity DB  │
│                             │
│ Users                       │
│ RefreshTokens               │
│ Stored Procedures           │
└─────────────────────────────┘
```

The Identity module remains independent from the main TexasMediaDart API and database.

---

## 3. Repository Structure

The Identity repository follows a Clean Architecture structure.

```text
TexasMediaDart.Identity/
│
├── .github/
│   └── workflows/
│
├── database/
│   └── TexasMediaDart.Identity.Database/
│       │
│       ├── Tables/
│       │   ├── Users.sql
│       │   └── RefreshTokens.sql
│       │
│       ├── StoredProcedures/
│       │   ├── sp_Users_Create.sql
│       │   ├── sp_Users_GetByEmail.sql
│       │   ├── sp_Users_GetById.sql
│       │   ├── sp_RefreshTokens_Create.sql
│       │   ├── sp_RefreshTokens_GetByTokenHash.sql
│       │   ├── sp_RefreshTokens_Revoke.sql
│       │   ├── sp_RefreshTokens_RevokeFamily.sql
│       │   ├── sp_RefreshTokens_Rotate.sql
│       │   └── sp_RefreshTokens_Cleanup.sql
│       │
│       └── TexasMediaDart.Identity.Database.sqlproj
│
├── docs/
│   ├── architecture/
│   ├── authentication/
│   │   └── base-identity-implementation.md
│   ├── api/
│   ├── deployment/
│   └── testing/
│
├── src/
│   ├── TexasMediaDart.Identity.Api/
│   ├── TexasMediaDart.Identity.Application/
│   ├── TexasMediaDart.Identity.Domain/
│   └── TexasMediaDart.Identity.Infrastructure/
│
├── README.md
└── TexasMediaDart.Identity.slnx
```

Tests are intentionally maintained separately from production application code according to the TexasMediaDart repository strategy.

---

## 4. Technology Stack

### Backend

- .NET 10
- ASP.NET Core Web API
- JWT Bearer Authentication
- Clean Architecture
- Dapper
- SQL Server / Azure SQL
- Stored Procedures

### Frontend

- Flutter
- Dart
- HTTP client
- `shared_preferences`
- Runtime environment configuration

### Database

- SQL Server for LOCAL development
- Azure SQL Database for DEV and PROD
- SQL Database Project
- DACPAC deployment

### CI/CD

- GitHub Actions
- Azure App Service
- Azure SQL
- GitHub OIDC / managed identity
- Environment-specific deployments

---

# 5. Identity Capabilities

## 5.1 User Registration

The Identity API provides:

```http
POST /api/auth/register
```

The base registration request contains:

```json
{
  "email": "user@example.com",
  "password": "Password",
  "confirmPassword": "Password"
}
```

The application validates the registration request and creates the user in the Identity database.

The backend command is conceptually represented by:

```csharp
public sealed record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword);
```

The Flutter registration page calls this endpoint and redirects the user to Login after successful registration.

---

## 5.2 Login

Authentication is performed through:

```http
POST /api/auth/login
```

The user supplies:

```json
{
  "email": "user@example.com",
  "password": "Password"
}
```

After successful authentication, the Identity API returns:

- User ID
- Email
- JWT access token
- Access-token expiration
- Refresh token
- Refresh-token expiration

The backend result is represented by:

```csharp
public sealed record LoginResult(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
```

The Flutter frontend persists the returned session.

---

# 6. JWT Access Token

TexasMediaDart uses JWT access tokens for authenticated API requests.

After login, Flutter receives an access token and sends it using the HTTP `Authorization` header:

```http
Authorization: Bearer <access-token>
```

The Identity API validates the JWT before allowing access to protected endpoints.

For example:

```http
GET /api/auth/me
Authorization: Bearer <access-token>
```

The access token is intentionally shorter-lived than the refresh token.

---

# 7. Refresh Tokens

Refresh tokens allow a user session to continue without requiring the user to repeatedly enter credentials.

The Identity API supports:

```http
POST /api/auth/refresh
```

The refresh operation returns a new access token and a new refresh token.

The result is represented by:

```csharp
public sealed record RefreshResult(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
```

---

## 7.1 Refresh-Token Rotation

TexasMediaDart implements refresh-token rotation.

A refresh token is not intended to be repeatedly reused.

The lifecycle is:

```text
Refresh Token A
      │
      │ refresh
      ▼
Token A revoked
      │
      ▼
Refresh Token B created
      │
      │ refresh
      ▼
Token B revoked
      │
      ▼
Refresh Token C created
```

This reduces the security risk associated with stolen refresh tokens.

---

## 7.2 Atomic Refresh Rotation

Refresh-token rotation is implemented atomically at the database level.

The implementation uses SQL locking semantics such as:

```sql
UPDLOCK, HOLDLOCK
```

This prevents two concurrent requests from successfully rotating the same refresh token.

Concurrency testing verified that when the same refresh token is submitted concurrently:

```text
Request 1 → 200 OK
Request 2 → 401 Unauthorized
```

Only one request is allowed to successfully rotate the token.

---

## 7.3 Refresh-Token Family Revocation

Refresh tokens belong to a token family.

Example:

```text
A → B → C
```

If an already-used token such as `A` is reused, the Identity service treats the request as suspicious token reuse.

The associated token family can then be revoked.

Testing validated the following scenario:

```text
A → B
B → C

Reuse A
   ↓
Reuse detected
   ↓
Token family revoked
   ↓
C is no longer valid
```

This provides additional protection against stolen refresh tokens.

---

# 8. Logout

Logout is provided through:

```http
POST /api/auth/logout
```

The request contains the active refresh token.

Example:

```json
{
  "refreshToken": "<refresh-token>"
}
```

The server revokes the refresh token.

The Flutter application then clears its local session.

The Flutter implementation ensures the local session is cleared even if the server logout operation encounters an error.

Conceptually:

```text
Logout
   │
   ├── Revoke refresh token on server
   │
   └── Clear Flutter session
```

---

# 9. Current User Endpoint

The Identity API exposes:

```http
GET /api/auth/me
```

The endpoint requires authorization.

Example request:

```http
GET /api/auth/me
Authorization: Bearer <access-token>
```

Example response:

```json
{
  "userId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "email": "user@example.com"
}
```

This endpoint is used by Flutter to validate that a persisted session still represents an authenticated user.

---

# 10. Flutter Identity Integration

The Flutter frontend contains Identity-specific components under:

```text
lib/features/identity/
```

The implementation includes models, services, controllers, and pages for authentication.

Representative structure:

```text
lib/
└── features/
    └── identity/
        ├── controllers/
        │   └── login_controller.dart
        │
        ├── models/
        │   ├── auth_tokens.dart
        │   ├── register_response.dart
        │   └── user_profile.dart
        │
        ├── pages/
        │   ├── auth_guard.dart
        │   ├── login_page.dart
        │   ├── register_page.dart
        │   └── startup_page.dart
        │
        └── services/
            ├── auth_service.dart
            ├── identity_api_service.dart
            └── session_manager.dart
```

---

# 11. Frontend Routing

The base routing model is:

```text
/          → StartupPage
/welcome   → IntroductionPage
/login     → LoginPage
/signup    → RegisterPage
/home      → AuthGuard → HomePage
```

The root route is intentionally assigned to `StartupPage`.

This allows the application to determine whether a saved authenticated session exists before deciding which page should be displayed.

---

# 12. Startup Session Handling

When Flutter starts, `StartupPage` performs session validation.

The flow is:

```text
Application starts
       │
       ▼
StartupPage
       │
       ▼
Read saved session
       │
       ├── No session
       │      │
       │      ▼
       │   /welcome
       │
       └── Session exists
              │
              ▼
       Check refresh-token expiry
              │
              ├── Expired
              │      │
              │      ▼
              │ Clear session
              │      │
              │      ▼
              │   /welcome
              │
              └── Not expired
                     │
                     ▼
              GET /api/auth/me
                     │
              ┌──────┴──────┐
              │             │
            200           401
              │             │
              ▼             ▼
           /home       Clear session
                            │
                            ▼
                         /welcome
```

This prevents Flutter from relying solely on the existence of locally stored data.

The Identity API validates the access token.

---

# 13. Route Protection

The `/home` route is protected by `AuthGuard`.

The route is conceptually configured as:

```dart
case home:
  return MaterialPageRoute(
    builder: (_) => const AuthGuard(
      child: HomePage(),
    ),
    settings: settings,
  );
```

This prevents unauthenticated users from directly navigating to:

```text
/home
```

---

# 14. Avoiding Duplicate JWT Validation

Initially, both `StartupPage` and `AuthGuard` called server-side session validation.

That caused:

```text
StartupPage
     │
     ▼
GET /api/auth/me
     │
     ▼
/home
     │
     ▼
AuthGuard
     │
     ▼
GET /api/auth/me
```

The same JWT was therefore validated twice during application startup.

The implementation was refined so responsibilities are separated.

### StartupPage

Performs server-side authentication validation:

```text
isLoggedIn()
     │
     ▼
GET /api/auth/me
```

### AuthGuard

Performs only a lightweight local-session check:

```text
hasSession()
```

The optimized startup lifecycle is therefore:

```text
Application Start
       │
       ▼
StartupPage
       │
       ▼
isLoggedIn()
       │
       ▼
GET /api/auth/me
       │
       ▼
200 OK
       │
       ▼
/home
       │
       ▼
AuthGuard
       │
       ▼
hasSession()
       │
       ▼
HomePage
```

This results in only one logical `/api/auth/me` GET request during startup.

---

# 15. Session Manager

Flutter uses `SessionManager` to persist authentication information.

The stored session contains information such as:

```text
Access Token
Access Token Expiration
Refresh Token
Refresh Token Expiration
User ID
Email
```

The session is saved after successful login.

Conceptually:

```text
POST /api/auth/login
       │
       ▼
Receive AuthTokens
       │
       ▼
SessionManager.saveSession()
       │
       ▼
Persistent browser/app storage
```

For the initial implementation, `shared_preferences` is used for session persistence.

A production-grade secure-storage strategy should be evaluated separately for each supported Flutter platform.

---

# 16. AuthService Responsibilities

`AuthService` acts as the authentication orchestration layer between Flutter UI components, the session manager, and the Identity API.

Responsibilities include:

```text
AuthService
   │
   ├── login()
   │
   ├── logout()
   │
   ├── getCurrentUser()
   │
   ├── isLoggedIn()
   │
   └── hasSession()
```

### `login()`

Authenticates through the Identity API and saves the resulting session.

### `getCurrentUser()`

Retrieves the current user using `/api/auth/me`.

### `isLoggedIn()`

Performs full authentication validation.

Conceptually:

```text
Read session
     │
     ├── Missing → false
     │
     ▼
Check refresh-token expiration
     │
     ├── Expired → clear session → false
     │
     ▼
GET /api/auth/me
     │
     ├── 200 → true
     │
     └── 401 → clear session → false
```

### `hasSession()`

Performs only a local session check.

Conceptually:

```text
Read session
     │
     ├── Missing → false
     │
     ▼
Check refresh-token expiration
     │
     ├── Expired → clear session → false
     │
     └── Valid → true
```

This distinction prevents duplicate server-side validation.

---

# 17. CORS

Flutter Web runs from a browser origin different from the Identity API.

For example:

```text
Flutter:
http://localhost:<dynamic-port>

Identity API:
http://localhost:5248
```

Therefore CORS configuration is required.

For LOCAL Development, the Identity API permits localhost origins with dynamic ports.

Example concept:

```csharp
.SetIsOriginAllowed(origin =>
{
    if (!Uri.TryCreate(
            origin,
            UriKind.Absolute,
            out var uri))
    {
        return false;
    }

    return uri.Host.Equals(
               "localhost",
               StringComparison.OrdinalIgnoreCase)
           ||
           uri.Host.Equals(
               "127.0.0.1",
               StringComparison.OrdinalIgnoreCase);
})
.AllowAnyHeader()
.AllowAnyMethod();
```

DEV and PROD remain restricted to explicitly configured frontend origins.

Browser testing confirmed successful CORS behavior:

```text
OPTIONS /api/auth/me → 204 No Content
GET     /api/auth/me → 200 OK
```

---

# 18. Local Development Architecture

The validated LOCAL environment is:

```text
Flutter Web
localhost:<dynamic-port>
        │
        ├──────────────────────────────────┐
        │                                  │
        ▼                                  ▼
Main API                            Identity API
localhost:5295                      localhost:5248
        │                                  │
        ▼                                  ▼
TexasMediaDartLocal                TexasMediaDartIdentityLocal
        │                                  │
        └──────────── SQL Server ──────────┘
                     localhost:14330
```

No Azure dependency is required for normal LOCAL development.

---

# 19. Environment Configuration

Flutter uses runtime configuration to determine API endpoints.

Example LOCAL configuration:

```json
{
  "environment": "local",
  "apiBaseUrl": "http://localhost:5295",
  "identityApiBaseUrl": "http://localhost:5248"
}
```

The same Flutter application can therefore be configured independently for:

```text
LOCAL
DEV
PROD
```

without hardcoding environment-specific URLs into application logic.

---

# 20. Environment Separation

TexasMediaDart maintains strict environment separation.

```text
LOCAL
  │
  │ Development and initial testing
  ▼
DEV
  │
  │ Integration / QA validation
  ▼
PROD
```

Each environment has independent:

- APIs
- Databases
- Configuration
- Secrets
- Deployment controls

LOCAL development should not depend on Azure DEV or PROD resources.

---

# 21. DEV Environment

The Identity DEV environment consists of an independently deployed Identity API and Identity database.

The DEV environment is used after LOCAL validation for:

- Integration testing
- QA testing
- Deployment validation
- Environment validation

Changes should reach DEV through the GitHub CI/CD process rather than manual production-style modifications.

---

# 22. PROD Environment

The PROD Identity environment is independently configured from DEV.

Production promotion follows the TexasMediaDart deployment principle:

> **Build once, test the artifact, and promote the exact tested artifact.**

PROD should not rebuild source code independently after DEV validation.

Conceptually:

```text
Source
   │
   ▼
Build
   │
   ▼
Versioned Artifact
   │
   ▼
Deploy DEV
   │
   ▼
Test / Approve
   │
   ▼
Promote same artifact
   │
   ▼
PROD
```

---

# 23. Database Deployment

The Identity database is maintained as a SQL Database Project.

The project produces a DACPAC.

Conceptually:

```text
SQL Project
    │
    ▼
Build
    │
    ▼
DACPAC
    │
    ├── LOCAL deployment
    │
    ├── DEV deployment
    │
    └── PROD promotion
```

Database objects include:

### Tables

```text
Users
RefreshTokens
```

### Stored Procedures

```text
sp_Users_Create
sp_Users_GetByEmail
sp_Users_GetById

sp_RefreshTokens_Create
sp_RefreshTokens_GetByTokenHash
sp_RefreshTokens_Revoke
sp_RefreshTokens_RevokeFamily
sp_RefreshTokens_Rotate
sp_RefreshTokens_Cleanup
```

---

# 24. Refresh-Token Cleanup

The Identity service contains refresh-token cleanup functionality.

The API includes a background service:

```text
RefreshTokenCleanupService
```

The service works with:

```text
sp_RefreshTokens_Cleanup
```

to remove or clean up refresh-token records according to the configured retention rules.

This prevents indefinite growth of historical token records.

---

# 25. Base Security Controls

The current Identity baseline includes the following security controls:

- JWT Bearer authentication
- Password-based login
- Short-lived access tokens
- Longer-lived refresh tokens
- Server-side refresh-token validation
- Refresh-token hashing
- Refresh-token rotation
- Atomic refresh rotation
- Refresh-token family tracking
- Token reuse detection
- Family revocation
- Authenticated `/api/auth/me`
- Logout token revocation
- Expired-session cleanup
- Environment-specific CORS
- Independent LOCAL, DEV, and PROD secrets

---

# 26. Validation Completed

The following scenarios have been tested successfully.

## Backend

- User registration
- User login
- JWT generation
- JWT authorization
- `/api/auth/me`
- Refresh-token creation
- Refresh-token rotation
- Concurrent refresh protection
- Refresh-token reuse detection
- Refresh-token family revocation
- Logout
- Database connectivity

## Flutter

- Registration page
- Login page
- Session persistence
- Startup session restoration
- Protected `/home` route
- Direct `/home` access without session
- `/api/auth/me` integration
- Browser refresh with existing session
- CORS preflight
- Authenticated GET request
- Removal of duplicate `/api/auth/me` validation

## Integrated LOCAL Environment

The following end-to-end flow has been validated:

```text
Flutter
   │
   ▼
Register
   │
   ▼
Identity API
   │
   ▼
Identity Database
   │
   ▼
Login
   │
   ▼
JWT + Refresh Token
   │
   ▼
Flutter Session
   │
   ▼
Protected Home
   │
   ▼
Browser Refresh
   │
   ▼
Startup Session Check
   │
   ▼
GET /api/auth/me
   │
   ▼
200 OK
   │
   ▼
Home
```

---

# 27. Base Identity Status

## Status: COMPLETE

The TexasMediaDart Base Identity implementation is considered complete.

The current implementation provides the minimum authentication foundation required for additional TexasMediaDart modules.

### Completed

| Capability | Status |
|---|---|
| User Registration | Complete |
| User Login | Complete |
| JWT Access Token | Complete |
| Refresh Token | Complete |
| Refresh-Token Rotation | Complete |
| Token-Family Revocation | Complete |
| Logout API | Complete |
| `/api/auth/me` | Complete |
| Flutter Registration | Complete |
| Flutter Login | Complete |
| Flutter Session Persistence | Complete |
| Startup Session Validation | Complete |
| Route Protection | Complete |
| Duplicate JWT Validation Cleanup | Complete |
| LOCAL End-to-End Validation | Complete |

---

# 28. Pending Enhancements

The following capabilities are enhancements to the Base Identity implementation.

They are not considered blockers for the Base Identity baseline.

## 28.1 Automatic Access-Token Refresh

This is the highest-priority next enhancement.

Current behavior:

```text
Access token expires
       │
       ▼
GET /api/auth/me
       │
       ▼
401 Unauthorized
       │
       ▼
Session cleared
       │
       ▼
User logs in again
```

Target behavior:

```text
Authenticated request
       │
       ▼
401 Unauthorized
       │
       ▼
Refresh token still valid?
       │
       ├── No
       │    │
       │    ▼
       │  Logout
       │
       └── Yes
            │
            ▼
POST /api/auth/refresh
            │
            ▼
Rotate refresh token
            │
            ▼
Receive new tokens
            │
            ▼
Save new session
            │
            ▼
Retry original request once
            │
            ▼
Continue application
```

---

## 28.2 Centralized Authenticated API Client

Future authenticated API requests should ideally use a centralized HTTP client or equivalent request layer.

The client should be responsible for:

- Adding the access token
- Detecting HTTP 401
- Refreshing tokens when appropriate
- Retrying the original request once
- Preventing refresh loops
- Coordinating simultaneous refresh requests

---

## 28.3 Password Management

Future Identity functionality may include:

- Forgot password
- Reset password
- Change password

---

## 28.4 Email Verification

Email verification may be introduced depending on TexasMediaDart product requirements.

---

## 28.5 Account Protection

Additional production security controls may include:

- Login rate limiting
- Account lockout
- Suspicious-login detection
- Security auditing
- Authentication event logging

---

## 28.6 Secure Client Storage

The initial Flutter implementation uses `shared_preferences`.

Before production rollout, token-storage requirements should be evaluated independently for:

- Flutter Web
- Windows
- macOS
- iOS
- Android

The appropriate secure-storage mechanism may differ by platform.

---

# 29. Design Principles

The Identity implementation follows several TexasMediaDart architectural principles.

### Independent Module Ownership

Identity owns its:

- API
- Application logic
- Domain
- Infrastructure
- Database
- Deployment

### Database-per-Module

Identity data is stored in a dedicated Identity database.

Other TexasMediaDart modules should not directly own or modify Identity tables.

### Environment Isolation

LOCAL, DEV, and PROD resources remain separate.

### Local-First Validation

Development and initial testing are performed locally before DEV deployment.

### Artifact Promotion

Production receives the exact artifact that was tested and approved in DEV.

### Clean Architecture

Business/application concerns remain separated from infrastructure and API concerns.

### Database Access

Dapper and stored procedures are used for database access according to the TexasMediaDart backend architecture.

---

# 30. Future Module Integration

The Identity module establishes the authentication foundation for other TexasMediaDart modules.

Future modules can rely on the authenticated identity established by the JWT.

Examples include:

```text
Identity
   │
   ├── Organizations & User Profiles
   ├── Properties & Channels
   ├── Advertisers
   ├── Agencies
   ├── Programs & Titles
   ├── Logs & Schedules
   ├── Orders
   └── Additional TexasMediaDart Modules
```

Identity should remain focused on authentication and identity concerns.

Business-specific information should remain within the appropriate module boundary.

---

# 31. Summary

The TexasMediaDart Identity module now provides a working authentication foundation across the backend, database, and Flutter frontend.

The validated baseline includes:

```text
Registration
     +
Login
     +
JWT Authentication
     +
Refresh Tokens
     +
Refresh-Token Rotation
     +
Reuse Detection
     +
Logout
     +
Current User Validation
     +
Session Persistence
     +
Startup Session Restoration
     +
Route Protection
     +
LOCAL End-to-End Integration
```

Therefore:

> **TexasMediaDart Base Identity Implementation is COMPLETE.**

The next authentication enhancement is:

> **Automatic access-token refresh and transparent retry of authenticated requests.**

---

## Document Maintenance

This Markdown file is the source of truth for the Base Identity implementation.

Update this document whenever a material change is made to:

- Authentication architecture
- Token lifecycle
- Identity API contracts
- Session management
- Route protection
- Security controls
- Environment architecture

More detailed topics should be moved into dedicated documents under the appropriate `docs/` subdirectory and linked from this document.

Suggested future documentation:

```text
docs/
├── README.md
├── architecture/
│   └── identity-architecture.md
├── authentication/
│   ├── base-identity-implementation.md
│   ├── token-lifecycle.md
│   └── refresh-token-rotation.md
├── api/
│   └── identity-api-contracts.md
├── deployment/
│   ├── local-development.md
│   ├── dev-deployment.md
│   └── prod-promotion.md
└── testing/
    └── identity-validation-checklist.md
```