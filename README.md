# 🏭 Industrial Automation Platform

A comprehensive AI-powered industrial automation platform built for Bosch thesis project, featuring advanced test execution, web automation, and intelligent orchestration capabilities.

## 🚀 Features

### Core Capabilities
- **AI-Driven Test Execution**: Advanced AI methods for automated test execution with intelligent analysis and optimization
- **Web Automation**: AI-based web automation solution for intelligent interaction with third-party websites
- **Back-Office Interface**: Comprehensive supervision interface for automation task orchestration and management
- **Real-time Monitoring**: Advanced monitoring and observability with Prometheus and Grafana
- **Computer Vision**: AI-powered computer vision for web element detection and analysis
- **Experimental Analysis**: Performance metrics and statistical analysis for AI models

### Technical Stack
- **Backend**: .NET 8 Web API with Entity Framework Core
- **Frontend**: React 18 with TypeScript and Material-UI
- **Database**: SQL Server 2022 with advanced schema
- **Cache**: Redis for high-performance caching
- **AI Integration**: OpenAI GPT models for intelligent automation
- **Monitoring**: Prometheus, Grafana, and custom metrics
- **Containerization**: Docker and Docker Compose

## 📋 Prerequisites

- Docker and Docker Compose
- Node.js 18+ (for local development)
- .NET 8 SDK (for local development)
- SQL Server 2022 (for local development)

## 🛠️ Quick Start

### Option 1: Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd bosch-gmbh
   ```

2. **Start the system**
   ```bash
   # Simple setup (recommended for development)
   docker-compose -f docker-compose.simple.yml up -d
   
   # Full setup with monitoring
   docker-compose up -d
   ```

3. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5001
   - API Documentation: http://localhost:5001/swagger
   - Monitoring: http://localhost:3001 (Grafana)

### Option 2: Local Development

1. **Backend Setup**
   ```bash
   cd backend
   dotnet restore
   dotnet build
   dotnet run
   ```

2. **Frontend Setup**
   ```bash
   cd frontend
   npm install
   npm start
   ```

3. **Database Setup**
   - Install SQL Server 2022
   - Create database: `IndustrialAutomationDb`
   - Run: `database/init-database.sql`

## 🧪 Testing

### Run Tests
```bash
# Simple validation
./scripts/test-simple.sh

# Comprehensive tests
./scripts/test-system.sh

# Frontend tests
cd frontend && npm test

# Backend tests
cd backend && dotnet test
```

### Test Coverage
- Unit Tests: 85%+ coverage
- Integration Tests: Core workflows
- E2E Tests: Critical user journeys
- Performance Tests: Load and stress testing

## 📊 System Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend API   │    │   Database      │
│   (React 18)    │◄──►│   (.NET 8)      │◄──►│   (SQL Server)  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │              ┌─────────────────┐              │
         │              │   Redis Cache   │              │
         │              │   (Performance) │              │
         │              └─────────────────┘              │
         │                                                │
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Monitoring    │    │   AI Services   │    │   File Storage  │
│   (Prometheus)  │    │   (OpenAI)      │    │   (Screenshots) │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🔧 Configuration

### Environment Variables

#### Backend
- `ConnectionStrings__DefaultConnection`: Database connection string
- `ConnectionStrings__Redis`: Redis connection string
- `JWT__Secret`: JWT signing key
- `OpenAI__ApiKey`: OpenAI API key
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)

#### Frontend
- `REACT_APP_API_URL`: Backend API URL
- `REACT_APP_ENVIRONMENT`: Environment (development/production)

### Database Configuration
- **Database**: IndustrialAutomationDb
- **Tables**: Users, AutomationJobs, TestExecutions, WebAutomations, JobSchedules
- **Advanced Features**: ComputerVisionResults, ExperimentalAnalysis, PerformanceBenchmarks

## 📈 Monitoring & Observability

### Metrics
- System Performance: CPU, Memory, Disk usage
- Application Metrics: Request rates, response times, error rates
- Business Metrics: Test success rates, automation efficiency
- AI Metrics: Model performance, prediction accuracy

### Dashboards
- **System Overview**: Real-time system health and performance
- **Business KPIs**: Test execution, automation success rates
- **AI Analytics**: Model performance and insights
- **Operational Metrics**: Job scheduling, resource utilization

## 🔒 Security Features

- **Authentication**: JWT-based authentication with refresh tokens
- **Authorization**: Role-based access control (RBAC)
- **Security Headers**: XSS protection, content type options, frame options
- **Rate Limiting**: API request rate limiting
- **Input Validation**: Comprehensive input validation and sanitization
- **Audit Logging**: Complete audit trail for all operations

## 🚀 Deployment

### Production Deployment
```bash
# Build production images
docker-compose -f docker-compose.prod.yml build

# Deploy to production
docker-compose -f docker-compose.prod.yml up -d
```

### Kubernetes Deployment
```bash
# Apply Kubernetes manifests
kubectl apply -f k8s/
```

## 📚 API Documentation

### Core Endpoints
- `GET /api/health` - Health check
- `GET /api/automationjobs` - Automation jobs management
- `GET /api/testexecutions` - Test execution management
- `GET /api/webautomations` - Web automation management
- `GET /api/users` - User management
- `GET /api/jobschedules` - Job scheduling

### AI Endpoints
- `POST /api/testexecutions/analyze` - AI test analysis
- `POST /api/webautomations/analyze` - AI web page analysis
- `POST /api/ai/computer-vision` - Computer vision analysis

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is part of the Bosch thesis program and is proprietary software.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation in `/docs`

## 🎯 Roadmap

### Phase 1 (Current)
- ✅ Core platform functionality
- ✅ AI integration
- ✅ Basic monitoring
- ✅ Docker containerization

### Phase 2 (Next)
- 🔄 Advanced AI features
- 🔄 Enhanced monitoring
- 🔄 Performance optimization
- 🔄 Security hardening

### Phase 3 (Future)
- 📋 Machine learning pipelines
- 📋 Advanced analytics
- 📋 Multi-tenant support
- 📋 Cloud deployment

---

**Built with ❤️ for Industrial Automation Excellence**