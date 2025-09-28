# Docker Setup Guide

This guide explains how to use the Docker configuration for the Industrial Automation Platform.

## 🐳 Docker Files Overview

### Backend Docker Files
- `backend/Dockerfile` - Development and production backend image
- `backend/Dockerfile.prod` - Optimized production backend image
- `backend/.dockerignore` - Files to exclude from backend build context

### Frontend Docker Files
- `frontend/Dockerfile` - Development and production frontend image
- `frontend/Dockerfile.prod` - Optimized production frontend image
- `frontend/.dockerignore` - Files to exclude from frontend build context

### Docker Compose Files
- `docker-compose.yml` - Main development and production setup
- `docker-compose.dev.yml` - Development-specific configuration
- `docker-compose.prod.yml` - Production-specific configuration

## 🚀 Quick Start

### Development Environment
```bash
# Start all services
docker-compose up -d

# Start only database and cache
docker-compose up -d sqlserver redis

# Start with development configuration
docker-compose -f docker-compose.dev.yml up -d
```

### Production Environment
```bash
# Copy environment variables
cp env.example .env
# Edit .env with your production values

# Start production services
docker-compose -f docker-compose.prod.yml up -d
```

## 📋 Services

### Core Services
| Service | Port | Description |
|---------|------|-------------|
| **sqlserver** | 1433 | SQL Server database |
| **redis** | 6379 | Redis cache |
| **backend** | 5001 | .NET 8 API |
| **frontend** | 3000 | React application |
| **nginx** | 80, 443 | Reverse proxy |

### Monitoring Services
| Service | Port | Description |
|---------|------|-------------|
| **prometheus** | 9090 | Metrics collection |
| **grafana** | 3001 | Monitoring dashboards |

## 🔧 Configuration

### Environment Variables
Create a `.env` file based on `env.example`:

```bash
# Database
DB_SA_PASSWORD=YourStrong@Passw0rd
DB_NAME=IndustrialAutomationDb

# API
JWT_SECRET_KEY=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
OPENAI_API_KEY=your-openai-api-key-here

# Monitoring
GRAFANA_PASSWORD=admin
```

### Database Initialization
The database is automatically initialized with:
- Schema creation
- Sample data insertion
- Index optimization

### Health Checks
All services include health checks:
- **Backend**: `GET /health`
- **Frontend**: `GET /`
- **Database**: SQL connection test
- **Redis**: `PING` command

## 🛠️ Development

### Hot Reload
For development with hot reload:

```bash
# Start development environment
docker-compose -f docker-compose.dev.yml up -d

# View logs
docker-compose logs -f backend
docker-compose logs -f frontend
```

### Building Images
```bash
# Build backend
docker-compose build backend

# Build frontend
docker-compose build frontend

# Build all services
docker-compose build
```

### Database Management
```bash
# Access database
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd'

# Run migrations
docker-compose exec backend dotnet ef database update

# Backup database
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q "BACKUP DATABASE IndustrialAutomationDb TO DISK = '/var/opt/mssql/backup/backup.bak'"
```

## 🚀 Production Deployment

### Prerequisites
1. Docker and Docker Compose installed
2. Environment variables configured
3. SSL certificates (for HTTPS)

### Deployment Steps
```bash
# 1. Configure environment
cp env.example .env
# Edit .env with production values

# 2. Build production images
docker-compose -f docker-compose.prod.yml build

# 3. Start production services
docker-compose -f docker-compose.prod.yml up -d

# 4. Verify deployment
curl http://localhost/health
```

### Production Optimizations
- **Multi-stage builds** for smaller images
- **Non-root users** for security
- **Health checks** for reliability
- **Resource limits** for performance
- **Persistent volumes** for data

## 📊 Monitoring

### Prometheus Metrics
- Application metrics: `http://localhost:9090`
- Business KPIs
- System performance
- Error rates

### Grafana Dashboards
- System overview: `http://localhost:3001`
- Application metrics
- Database performance
- Custom dashboards

### Logs
```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f sqlserver
```

## 🔧 Troubleshooting

### Common Issues

#### Services Won't Start
```bash
# Check service status
docker-compose ps

# View logs
docker-compose logs [service-name]

# Restart services
docker-compose restart
```

#### Database Connection Issues
```bash
# Check database status
docker-compose logs sqlserver

# Test connection
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q "SELECT 1"
```

#### Frontend Not Loading
```bash
# Check frontend logs
docker-compose logs frontend

# Rebuild frontend
docker-compose build frontend
docker-compose up -d frontend
```

#### Backend API Errors
```bash
# Check backend logs
docker-compose logs backend

# Rebuild backend
docker-compose build backend
docker-compose up -d backend
```

### Performance Issues
```bash
# Check resource usage
docker stats

# Scale services
docker-compose up -d --scale backend=3

# Monitor logs
docker-compose logs -f | grep ERROR
```

## 🧹 Cleanup

### Stop Services
```bash
# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Stop and remove images
docker-compose down --rmi all
```

### Clean Docker
```bash
# Remove unused containers
docker container prune

# Remove unused images
docker image prune

# Remove unused volumes
docker volume prune

# Clean everything
docker system prune -a
```

## 📚 Additional Resources

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Node.js Docker Images](https://hub.docker.com/_/node)
- [SQL Server Docker Images](https://hub.docker.com/_/microsoft-mssql-server)
- [Redis Docker Images](https://hub.docker.com/_/redis)
