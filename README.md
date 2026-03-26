# WorkSwap

A shift management and swap system built with **ASP.NET Core** and **SQLite**, designed to help teams manage work schedules, swap shifts, and offer shifts to colleagues.

## 🚀 Tech Stack

- **Backend**: ASP.NET Core 10.0 (Web API)
- **Database**: SQLite (local file-based database)
- **Authentication**: ASP.NET Identity + JWT Bearer tokens
- **ORM**: Entity Framework Core
- **Testing**: xUnit + WebApplicationFactory (integration tests)
- **API Documentation**: Scalar (interactive API explorer)

## 📋 Features

- **User Authentication**: Register, login, and JWT-based authorization
- **Department Management**: Create and manage organizational departments
- **Shift Management**: CRUD operations for work shifts with filtering
- **Shift Swapping**: Request and respond to shift swaps between employees
- **Shift Offers**: Offer shifts to the team and claim available shifts
- **Notifications**: Real-time notifications for swap requests and offers
- **Health Check**: `/api/health` endpoint for monitoring

## 🏗️ Architecture

This project follows **clean architecture** principles:

- **Thin Controllers**: Controllers are lightweight and delegate business logic to services
- **Service Layer**: Business logic is encapsulated in dedicated service classes (`IShiftService`, `ISwapService`, `IShiftOfferService`)
- **DTOs**: Data Transfer Objects ensure a clear API contract and prevent over-posting
- **Separation of Concerns**: Each layer has a single responsibility

### Key Design Decisions

1. **SQLite over PostgreSQL**: Chosen for simplicity, zero-cost deployment, and ease of local development
2. **Service Pattern**: Extracted business logic from controllers into testable service classes
3. **Constants over Enums**: Used `const string` for status values to avoid EF value converter complexity
4. **JWT Authentication**: Stateless authentication for scalability

## 🛠️ Local Setup

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- A code editor (VS Code, Visual Studio, or Rider)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/workswap.git
   cd workswap
   ```

2. **Configure JWT settings**:
   - Copy `.env.example` to `.env` (if using environment variables)
   - Or update `appsettings.Development.json` with your JWT secret:
     ```json
     {
       "JWT_SECRET": "your-secret-key-minimum-32-characters-long",
       "JWT_ISSUER": "workswap-api",
       "JWT_AUDIENCE": "workswap-client",
       "JWT_DURATION_MINUTES": "60"
     }
     ```

3. **Restore dependencies**:
   ```bash
   cd workswap
   dotnet restore
   ```

4. **Apply database migrations**:
   ```bash
   dotnet ef database update
   ```

5. **Run the application**:
   ```bash
   dotnet run
   ```

6. **Access the API**:
   - API: `https://localhost:5001`
   - Interactive API Docs (Scalar): `https://localhost:5001/scalar/v1`

## 📡 API Overview

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive a JWT token
- `GET /api/auth/me` - Get current user info (requires auth)

### Departments
- `GET /api/departments` - List all departments
- `POST /api/departments` - Create a department (admin only)

### Shifts
- `GET /api/shifts` - List shifts (supports filtering by department, user, date range)
- `GET /api/shifts/{id}` - Get a specific shift
- `POST /api/shifts` - Create a new shift
- `PUT /api/shifts/{id}` - Update a shift
- `DELETE /api/shifts/{id}` - Delete a shift

### Swaps
- `GET /api/swaps` - Get my swap requests
- `POST /api/swaps` - Create a swap request
- `PUT /api/swaps/{id}/respond` - Accept or reject a swap

### Shift Offers
- `GET /api/shiftoffers` - List active shift offers
- `POST /api/shifts/{shiftId}/offer` - Offer a shift
- `POST /api/shiftoffers/{id}/claim` - Claim an offered shift

### Health
- `GET /api/health` - Health check endpoint

## 🧪 Testing

Run the integration tests:

```bash
dotnet test
```

The test suite includes:
- **Auth flow tests**: Register → Login → Access protected endpoint
- **Shift CRUD tests**: Create and retrieve shifts

## 🚢 Deployment

This application is designed for easy deployment:

1. **Build for production**:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Deploy** the `./publish` folder to your hosting provider (Azure, AWS, Heroku, etc.)

3. **Set environment variables** for production:
   - `JWT_SECRET` (required)
   - `JWT_ISSUER`
   - `JWT_AUDIENCE`
   - `JWT_DURATION_MINUTES`

## 📝 License

This project is licensed under the MIT License.

## 👤 Author

**[Your Name]**  
[GitHub](https://github.com/yourusername) | [LinkedIn](https://linkedin.com/in/yourprofile)
