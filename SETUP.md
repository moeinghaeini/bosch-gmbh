# Bosch Industrial Automation Platform Setup

This project implements the infrastructure for three core industrial automation solutions:

1. **Intelligent Automated Test Execution System**
2. **Smart Web Automation Solution**
3. **Enterprise Management Interface for Automation Supervision**

## Technology Stack

- **Backend**: .NET 8 Web API
- **Frontend**: React 18 with TypeScript
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **UI Framework**: Material-UI (MUI)
- **Containerization**: Docker & Docker Compose

## Project Structure

```
bosch-gmbh/
├── backend/                 # .NET Web API
│   ├── IndustrialAutomation.API/     # Main API project
│   ├── IndustrialAutomation.Core/    # Domain entities and interfaces
│   ├── IndustrialAutomation.Infrastructure/ # Data access layer
│   └── Dockerfile
├── frontend/                # React application
│   ├── src/
│   │   ├── components/      # React components
│   │   ├── services/        # API services
│   │   └── App.tsx
│   └── Dockerfile
├── docker-compose.yml       # Multi-container setup
└── README.MD               # Project documentation
```

## Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)
- Node.js 18+ (for local development)
- SQL Server (for local development)

## Quick Start with Docker

1. **Clone and navigate to the project directory**
   ```bash
   cd "bosch-gmbh"
   ```

2. **Start all services**
   ```bash
   docker-compose up -d
   ```

3. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - SQL Server: localhost:1433

4. **Initialize the database**
   ```bash
   # Run Entity Framework migrations
   docker-compose exec backend dotnet ef database update
   ```

## Local Development Setup

### Backend Setup

1. **Navigate to backend directory**
   ```bash
   cd backend
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string** in `BoschThesis.API/appsettings.json`

4. **Run Entity Framework migrations**
   ```bash
   dotnet ef migrations add InitialCreate --project BoschThesis.Infrastructure --startup-project BoschThesis.API
   dotnet ef database update --project BoschThesis.Infrastructure --startup-project BoschThesis.API
   ```

5. **Run the API**
   ```bash
   dotnet run --project BoschThesis.API
   ```

### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start development server**
   ```bash
   npm start
   ```

## API Endpoints

### Automation Jobs
- `GET /api/automationjobs` - Get all automation jobs
- `GET /api/automationjobs/{id}` - Get job by ID
- `POST /api/automationjobs` - Create new job
- `PUT /api/automationjobs/{id}` - Update job
- `DELETE /api/automationjobs/{id}` - Delete job
- `GET /api/automationjobs/status/{status}` - Get jobs by status
- `GET /api/automationjobs/type/{type}` - Get jobs by type

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

## Database Schema

### AutomationJob
- `Id` (int, PK)
- `Name` (string)
- `Description` (string)
- `Status` (string) - Pending, Running, Completed, Failed
- `JobType` (string) - WebAutomation, TestExecution, DataProcessing
- `ScheduledAt` (datetime?)
- `StartedAt` (datetime?)
- `CompletedAt` (datetime?)
- `ErrorMessage` (string?)
- `Configuration` (string) - JSON configuration
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime?)
- `IsDeleted` (bool)

### User
- `Id` (int, PK)
- `Username` (string, unique)
- `Email` (string, unique)
- `PasswordHash` (string)
- `Role` (string) - Admin, User, Viewer
- `IsActive` (bool)
- `LastLoginAt` (datetime?)
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime?)
- `IsDeleted` (bool)

## Features Implemented

### Core Infrastructure
- ✅ Clean Architecture with separation of concerns
- ✅ Entity Framework Core with SQL Server
- ✅ Repository pattern
- ✅ AutoMapper for DTOs
- ✅ Serilog for logging
- ✅ CORS configuration
- ✅ Swagger/OpenAPI documentation

### Frontend Features
- ✅ Modern React with TypeScript
- ✅ Material-UI components
- ✅ React Query for data fetching
- ✅ Responsive design
- ✅ Data grid for tabular data
- ✅ Form dialogs for CRUD operations

### Management Features
- ✅ Automation job management
- ✅ User management
- ✅ Dashboard with statistics
- ✅ Status tracking
- ✅ Job type categorization

## Next Steps for Thesis Implementation

### For Intelligent Test Execution (Topic 1)
- Implement intelligent model integration
- Add test result analysis
- Create automated test execution engine
- Implement result validation

### For Smart Web Automation Solution (Topic 2)
- Integrate intelligent web element detection
- Add browser automation capabilities
- Implement dynamic web interaction
- Create automation script generation

### For Back-Office Interface (Topic 3)
- Add advanced user permissions
- Implement job scheduling
- Create monitoring dashboards
- Add system administration features

## Development Notes

- The project uses a clean architecture pattern
- All database operations are handled through repositories
- The frontend uses React Query for efficient data management
- Docker setup allows for easy deployment and development
- Logging is configured with Serilog for comprehensive monitoring

## Troubleshooting

### Common Issues

1. **Database connection issues**
   - Ensure SQL Server is running
   - Check connection string in appsettings.json
   - Verify database exists

2. **CORS issues**
   - Check CORS configuration in Program.cs
   - Ensure frontend URL is allowed

3. **Docker issues**
   - Ensure Docker is running
   - Check container logs: `docker-compose logs [service-name]`

### Useful Commands

```bash
# View logs
docker-compose logs -f

# Restart services
docker-compose restart

# Rebuild containers
docker-compose up --build

# Stop all services
docker-compose down

# Remove volumes (careful - this deletes data)
docker-compose down -v
```
