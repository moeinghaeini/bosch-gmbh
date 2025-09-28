# Bosch Industrial Automation Platform

A comprehensive enterprise-grade industrial automation platform designed for modern manufacturing environments. This solution provides advanced automation capabilities for test execution, web automation, and process orchestration with intelligent decision-making capabilities.

## 🏭 Overview

This platform delivers three core automation solutions for industrial environments:

1. **Intelligent Test Execution System** - Advanced automated test execution with intelligent analysis and validation
2. **Smart Web Automation Solution** - Intelligent web automation for vendor interactions using natural language processing
3. **Enterprise Management Interface** - Comprehensive web-based system for managing, scheduling, and monitoring automation workflows

The platform provides a complete enterprise solution for orchestrating automated processes, managing test executions, and handling web-based automations with intelligent decision-making capabilities.

## 🚀 Features

### Intelligent Test Execution System
- **Advanced Test Analysis**: Machine learning models for intelligent test result classification
- **Automated Test Execution**: Enhanced test execution engine with .NET integration
- **Intelligent Validation**: Smart analysis of test outcomes and failure pattern recognition
- **Performance Metrics**: Comprehensive KPI tracking for execution time optimization and coverage improvement
- **Log Analysis**: Advanced log classification for error detection and anomaly identification

### Smart Web Automation Solution
- **Natural Language Processing**: Convert natural language commands to automation actions
- **Intelligent Element Recognition**: Advanced web element detection without hardcoded selectors
- **Multi-Vendor Support**: Scalable automation across different vendor portals
- **Adaptive Automation**: Machine learning for improved interaction strategies
- **Workflow Orchestration**: Complex multi-step automation workflows

### Enterprise Management Interface
- **Role-Based Access Control**: Admin, operator, and viewer permission levels
- **Job Orchestration**: Schedule, pause, resume, and cancel automation jobs
- **Real-time Monitoring**: Live dashboards with job status and performance metrics
- **User Management**: Comprehensive user administration and audit logging
- **System Administration**: Complete back-office functionality for automation supervision

### Technical Features
- **Clean Architecture**: Scalable backend with separation of concerns
- **Real-time Updates**: Live data synchronization across all components
- **Intelligent Integration**: Machine learning models for smart automation
- **Performance Monitoring**: Comprehensive metrics and health checks
- **Database Optimization**: Advanced indexing and query optimization
- **Security**: Enterprise-grade security with audit logging

## 🛠️ Technology Stack

### Backend
- **.NET 8**: Modern C# framework with latest features
- **Entity Framework Core**: Advanced ORM with performance optimizations
- **SQL Server**: Enterprise database with high availability
- **Redis**: Caching and session management
- **Serilog**: Structured logging with multiple sinks

### Frontend
- **React 18**: Modern React with hooks and concurrent features
- **TypeScript**: Type-safe development
- **Material-UI**: Professional component library
- **React Query**: Advanced data fetching and caching
- **React Router**: Client-side routing

### Infrastructure
- **Docker**: Containerized deployment
- **Docker Compose**: Multi-service orchestration
- **Nginx**: Reverse proxy and load balancing
- **Prometheus**: Metrics collection
- **Grafana**: Monitoring dashboards

## 📁 Project Structure

```
bosch-gmbh/
├── backend/
│   ├── IndustrialAutomation.API/          # Web API layer
│   ├── IndustrialAutomation.Core/        # Domain entities and interfaces
│   ├── IndustrialAutomation.Infrastructure/ # Data access and external services
│   ├── database/                          # Database schemas, migrations, and scripts
│   └── Dockerfile                         # Backend container configuration
├── frontend/                             # React application
│   ├── src/
│   │   ├── components/                    # React components
│   │   ├── services/                      # API services
│   │   └── hooks/                         # Custom React hooks
│   └── Dockerfile                         # Frontend container configuration
├── monitoring/                           # Prometheus monitoring configuration
├── nginx/                                # Nginx reverse proxy configuration
├── scripts/                              # Deployment and utility scripts
├── config/                               # Environment configurations
└── docker-compose.yml                    # Multi-container orchestration
```

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- .NET 8 SDK (for local development)
- Node.js 18+ (for frontend development)

### Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd bosch-gmbh
   ```

2. **Start the development environment**
   ```bash
   docker-compose up -d
   ```

3. **Initialize the database**
   ```bash
   # Run database migrations
   docker-compose exec backend dotnet ef database update
   ```

4. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5001
   - API Documentation: http://localhost:5001/swagger
   - Database: localhost:1433

### Production Deployment

1. **Configure environment variables**
   ```bash
   cp .env.example .env
   # Edit .env with production values
   ```

2. **Deploy with production configuration**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d
   ```

## 📖 User Guide

### 🚀 How to Run the Application

#### Option 1: Docker Compose (Recommended)

1. **Start all services**
   ```bash
   docker-compose up -d
   ```

2. **Check service status**
   ```bash
   docker-compose ps
   ```

3. **View logs (if needed)**
   ```bash
   docker-compose logs [service-name]
   # Examples:
   docker-compose logs backend
   docker-compose logs frontend
   docker-compose logs sqlserver
   ```

4. **Stop the application**
   ```bash
   docker-compose down
   ```

#### Option 2: Manual Development Setup

1. **Start database and Redis**
   ```bash
   docker-compose up -d sqlserver redis
   ```

2. **Run backend locally**
   ```bash
   cd backend
   dotnet run --project IndustrialAutomation.API
   ```

3. **Run frontend locally**
   ```bash
   cd frontend
   npm install
   npm start
   ```

### 🌐 Accessing the Application

| Service | URL | Description |
|---------|-----|-------------|
| **Main Application** | http://localhost:3000 | React frontend interface |
| **API Documentation** | http://localhost:5001/swagger | Interactive API documentation |
| **Health Check** | http://localhost:5001/health | Backend health status |
| **Database** | localhost:1433 | SQL Server database |

### 🎯 How to Use the Application

#### 1. **Dashboard Overview**
- **Access**: Navigate to http://localhost:3000
- **Features**: 
  - Real-time KPIs and metrics
  - System health status
  - Recent automation jobs
  - Performance indicators

#### 2. **User Management**
- **Navigate to**: Users section in the sidebar
- **Features**:
  - Create new users
  - Assign roles (Admin, Operator, Viewer)
  - Manage user permissions
  - View user activity

#### 3. **Automation Jobs**
- **Navigate to**: Automation Jobs section
- **Features**:
  - Create new automation jobs
  - Monitor job status (Pending, Running, Completed, Failed)
  - View job details and logs
  - Manage job schedules

#### 4. **Test Executions**
- **Navigate to**: Test Executions section
- **Features**:
  - View test execution history
  - Analyze test results with AI
  - Generate new test cases
  - Optimize test suites

#### 5. **Web Automations**
- **Navigate to**: Web Automations section
- **Features**:
  - Configure web automation tasks
  - AI-powered element recognition
  - Natural language automation commands
  - Multi-step workflow creation

#### 6. **Job Scheduling**
- **Navigate to**: Job Schedules section
- **Features**:
  - Schedule recurring jobs
  - Set up automation workflows
  - Monitor scheduled executions
  - Enable/disable schedules

#### 7. **KPI Dashboard**
- **Navigate to**: KPIs section
- **Features**:
  - Test execution metrics
  - Web automation success rates
  - Job scheduling performance
  - Overall system performance

### 🔧 Common Operations

#### Creating a New Automation Job
1. Go to **Automation Jobs** section
2. Click **"Add New Job"**
3. Fill in job details:
   - Name and description
   - Job type (Test Execution, Web Automation, etc.)
   - Priority level
   - Configuration parameters
4. Click **"Save"**

#### Scheduling a Job
1. Go to **Job Schedules** section
2. Click **"Create Schedule"**
3. Configure schedule:
   - Select job to schedule
   - Set frequency (Daily, Weekly, Monthly)
   - Choose execution time
   - Set dependencies
4. Click **"Save"**

#### Monitoring System Health
1. Check the **Dashboard** for overall status
2. Use **KPIs** section for detailed metrics
3. Monitor **Performance Monitor** for real-time data
4. Check **Health** endpoint: http://localhost:5001/health

#### Using AI Features
1. **Test Analysis**: Go to Test Executions → Select test → Click "Analyze"
2. **Web Automation**: Go to Web Automations → Click "AI Analysis"
3. **Element Recognition**: Use "Identify Element" for smart web interactions
4. **Script Generation**: Use "Generate Script" for automation code

### 🛠️ Troubleshooting

#### Application Won't Start
```bash
# Check if all services are running
docker-compose ps

# Restart services
docker-compose restart

# Check logs for errors
docker-compose logs backend
docker-compose logs frontend
```

#### Database Connection Issues
```bash
# Check database status
docker-compose logs sqlserver

# Restart database
docker-compose restart sqlserver

# Wait for database to initialize (30-60 seconds)
```

#### Frontend Not Loading
```bash
# Check frontend logs
docker-compose logs frontend

# Restart frontend
docker-compose restart frontend

# Clear browser cache
```

#### API Not Responding
```bash
# Check backend health
curl http://localhost:5001/health

# Check backend logs
docker-compose logs backend

# Restart backend
docker-compose restart backend
```

### 📊 Performance Monitoring

#### System Metrics
- **Response Time**: Monitor API response times
- **Throughput**: Track requests per second
- **Error Rate**: Monitor failed requests
- **Resource Usage**: CPU, memory, and disk usage

#### Business Metrics
- **Job Success Rate**: Percentage of successful automation jobs
- **Test Coverage**: Test execution coverage metrics
- **User Activity**: Active users and session duration
- **Automation Efficiency**: Time saved through automation

### 🔐 Security Best Practices

#### User Management
- Use strong passwords
- Implement role-based access control
- Regular user access reviews
- Enable audit logging

#### API Security
- Use HTTPS in production
- Implement rate limiting
- Validate all inputs
- Monitor for suspicious activity

#### Data Protection
- Encrypt sensitive data
- Regular backups
- Secure database connections
- Implement data retention policies

### 📱 Mobile and Responsive Design

The application is fully responsive and works on:
- **Desktop**: Full feature access
- **Tablet**: Optimized interface
- **Mobile**: Core functionality available

### 🔄 Backup and Recovery

#### Database Backup
```bash
# Create backup
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourPassword' -Q "BACKUP DATABASE IndustrialAutomationDb TO DISK = '/var/opt/mssql/backup/backup.bak'"

# Restore backup
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourPassword' -Q "RESTORE DATABASE IndustrialAutomationDb FROM DISK = '/var/opt/mssql/backup/backup.bak'"
```

#### Application Data Backup
```bash
# Backup application data
docker-compose exec backend dotnet run --project IndustrialAutomation.Infrastructure -- backup

# Restore application data
docker-compose exec backend dotnet run --project IndustrialAutomation.Infrastructure -- restore
```

## 📊 Database Schema

The platform uses a comprehensive database schema with the following key entities:

- **Users**: User management with role-based access
- **AutomationJobs**: Automation job definitions and execution
- **TestExecutions**: Test execution records with AI analysis
- **WebAutomations**: Web automation configurations
- **JobSchedules**: Scheduled job definitions
- **MLModels**: Machine learning model management
- **AuditLogs**: Comprehensive audit trail

## 🔧 Configuration

### Environment Variables

```bash
# Database Configuration
DB_SA_PASSWORD=YourStrong@Passw0rd
DB_NAME=IndustrialAutomationDb

# API Configuration
JWT_SECRET_KEY=your-secret-key
API_URL=http://localhost:5001

# Redis Configuration
REDIS_PASSWORD=your-redis-password

# Monitoring
GRAFANA_PASSWORD=your-grafana-password
```

### Database Connection

The platform supports multiple database configurations:
- **Development**: Local SQL Server with sample data
- **Production**: High-availability SQL Server cluster
- **Testing**: In-memory database for unit tests

## 📈 Monitoring and Observability

### Metrics Collection
- **Application Metrics**: Performance and business metrics
- **Infrastructure Metrics**: System resource utilization
- **Custom Metrics**: Business-specific KPIs

### Dashboards
- **System Health**: Overall system status
- **Performance Metrics**: Response times and throughput
- **Business Metrics**: Automation success rates
- **Error Tracking**: Error rates and patterns

## 🔒 Security

### Authentication & Authorization
- **JWT Tokens**: Secure token-based authentication
- **Role-Based Access**: Granular permission system
- **Multi-Factor Authentication**: Enhanced security options

### Data Protection
- **Encryption**: Data encryption at rest and in transit
- **Audit Logging**: Comprehensive activity tracking
- **Input Validation**: Protection against injection attacks

## 🧪 Testing

### Test Types
- **Unit Tests**: Component-level testing
- **Integration Tests**: API and database testing
- **End-to-End Tests**: Full workflow testing
- **Performance Tests**: Load and stress testing

### Test Execution
```bash
# Backend tests
cd backend
dotnet test

# Frontend tests
cd frontend
npm test

# Integration tests
docker-compose -f docker-compose.test.yml up --abort-on-container-exit
```

## 📚 API Documentation

The API provides comprehensive endpoints for all platform functionality:

### Automation Jobs (`/api/automationjobs`)
- `GET /api/automationjobs` - Get all automation jobs
- `GET /api/automationjobs/{id}` - Get job by ID
- `POST /api/automationjobs` - Create new job
- `PUT /api/automationjobs/{id}` - Update job
- `DELETE /api/automationjobs/{id}` - Delete job
- `GET /api/automationjobs/status/{status}` - Get jobs by status
- `GET /api/automationjobs/type/{jobType}` - Get jobs by type

### Test Executions (`/api/testexecutions`)
- `GET /api/testexecutions` - Get all test executions
- `GET /api/testexecutions/{id}` - Get test execution by ID
- `POST /api/testexecutions` - Create new test execution
- `PUT /api/testexecutions/{id}` - Update test execution
- `DELETE /api/testexecutions/{id}` - Delete test execution
- `GET /api/testexecutions/status/{status}` - Get by status
- `GET /api/testexecutions/type/{testType}` - Get by test type
- `GET /api/testexecutions/suite/{testSuite}` - Get by test suite
- `GET /api/testexecutions/failed` - Get failed tests
- `POST /api/testexecutions/{id}/analyze` - Intelligent analysis of test results
- `POST /api/testexecutions/generate` - Generate test cases with intelligent algorithms
- `POST /api/testexecutions/optimize` - Optimize test suite with smart algorithms

### Web Automations (`/api/webautomations`)
- `GET /api/webautomations` - Get all web automations
- `GET /api/webautomations/{id}` - Get web automation by ID
- `POST /api/webautomations` - Create new web automation
- `PUT /api/webautomations/{id}` - Update web automation
- `DELETE /api/webautomations/{id}` - Delete web automation
- `GET /api/webautomations/status/{status}` - Get by status
- `GET /api/webautomations/type/{automationType}` - Get by type
- `GET /api/webautomations/website/{websiteUrl}` - Get by website
- `GET /api/webautomations/failed` - Get failed automations
- `POST /api/webautomations/analyze` - Intelligent web page analysis
- `POST /api/webautomations/identify-element` - Smart element identification
- `POST /api/webautomations/generate-selector` - Intelligent selector generation
- `POST /api/webautomations/validate-action` - Smart action validation
- `POST /api/webautomations/extract-data` - Intelligent data extraction
- `POST /api/webautomations/generate-script` - Smart script generation

### Users (`/api/users`)
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Job Schedules (`/api/jobschedules`)
- `GET /api/jobschedules` - Get all job schedules
- `GET /api/jobschedules/{id}` - Get schedule by ID
- `POST /api/jobschedules` - Create new schedule
- `PUT /api/jobschedules/{id}` - Update schedule
- `DELETE /api/jobschedules/{id}` - Delete schedule

### KPIs (`/api/kpi`)
- `GET /api/kpi` - Get KPI metrics and dashboards

### Health (`/api/health`)
- `GET /api/health` - Health check endpoint

Full API documentation is available at `/swagger` when running the application.

## 🤝 Contributing

### Development Guidelines
1. Follow the established coding standards
2. Write comprehensive tests for new features
3. Update documentation for API changes
4. Use conventional commit messages

### Code Review Process
1. Create feature branches from `main`
2. Submit pull requests with detailed descriptions
3. Ensure all tests pass
4. Request review from team members

## 📄 License

This project is proprietary software developed for industrial automation purposes.

## 🆘 Support

For technical support and questions:
- **Documentation**: Check the `/docs` directory
- **Issues**: Create GitHub issues for bugs and feature requests
- **Contact**: Reach out to the development team

## 🎓 Research Context

This platform implements three core research proposals for industrial automation:

### Proposal 1: Intelligent Automated Test Execution
- **Objective**: Develop intelligent solution to automate test execution, validation, and result analysis
- **Scope**: Study verification approaches, implement enhanced test execution engine
- **Architecture**: React dashboard, .NET backend, SQL Server database, intelligent module for log classification
- **KPIs**: Test execution time reduction, coverage improvement, accuracy, human intervention reduction

### Proposal 2: Smart Web Automation Solution
- **Objective**: Develop intelligent solution for third-party vendor website automation
- **Scope**: Multi-step workflows, intelligent element recognition, natural language task execution
- **Architecture**: React interface, .NET backend, intelligent module with computer vision and NLP
- **KPIs**: Success rate, execution time reduction, generalization across vendors, error recovery

### Proposal 3: Enterprise Management Interface
- **Objective**: Design React-based enterprise system for automation supervision
- **Scope**: Role-based user management, job orchestration, real-time monitoring
- **Architecture**: React admin panel, .NET APIs, SQL Server for metadata and logs
- **KPIs**: Jobs managed, user adoption, time saved, system uptime

## 🔄 Version History

### v1.0.0 - Initial Implementation
- Core infrastructure with clean architecture
- Basic automation job management
- User management system
- Database schema and migrations

### v1.1.0 - Intelligent Integration
- Intelligent test execution analysis
- Web automation with natural language processing
- Advanced scheduling and monitoring
- Enhanced security and audit logging

### v1.2.0 - Current Version
- Complete thesis implementation
- All three research proposals integrated
- Comprehensive API documentation
- Production-ready deployment configuration

---

**Bosch Industrial Automation Platform** - Empowering the future of industrial automation through intelligent, scalable, and secure solutions for thesis research and enterprise applications.