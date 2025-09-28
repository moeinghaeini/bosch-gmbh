#!/bin/bash

# Bosch Industrial Automation Platform - System Startup Script
# This script starts the entire system with proper initialization

set -e

echo "🚀 Starting Bosch Industrial Automation Platform"
echo "================================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print status
print_status() {
    echo -e "${BLUE}📋 $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker first."
    exit 1
fi

# Check if Docker Compose is available
if ! command -v docker-compose > /dev/null 2>&1; then
    print_error "Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

print_status "Starting system initialization..."

# Clean up any existing containers
print_status "Cleaning up existing containers..."
docker-compose down -v > /dev/null 2>&1 || true

# Build and start services
print_status "Building and starting services..."
docker-compose up -d --build

# Wait for services to be ready
print_status "Waiting for services to initialize..."

# Wait for SQL Server
print_status "Waiting for SQL Server to be ready..."
timeout=60
while [ $timeout -gt 0 ]; do
    if docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q 'SELECT 1' > /dev/null 2>&1; then
        print_success "SQL Server is ready"
        break
    fi
    sleep 2
    timeout=$((timeout - 2))
done

if [ $timeout -le 0 ]; then
    print_error "SQL Server failed to start within 60 seconds"
    exit 1
fi

# Initialize database
print_status "Initializing database..."
docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q "CREATE DATABASE IF NOT EXISTS IndustrialAutomationDb" > /dev/null 2>&1 || true

# Run database initialization script
if [ -f "backend/init-db.sql" ]; then
    print_status "Running database initialization script..."
    docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -d IndustrialAutomationDb -i /docker-entrypoint-initdb.d/init-db.sql > /dev/null 2>&1 || true
fi

# Wait for backend to be ready
print_status "Waiting for backend API to be ready..."
timeout=60
while [ $timeout -gt 0 ]; do
    if curl -f -s http://localhost:5001/health > /dev/null 2>&1; then
        print_success "Backend API is ready"
        break
    fi
    sleep 2
    timeout=$((timeout - 2))
done

if [ $timeout -le 0 ]; then
    print_warning "Backend API may not be fully ready, but continuing..."
fi

# Wait for frontend to be ready
print_status "Waiting for frontend to be ready..."
timeout=60
while [ $timeout -gt 0 ]; do
    if curl -f -s http://localhost:3000 > /dev/null 2>&1; then
        print_success "Frontend is ready"
        break
    fi
    sleep 2
    timeout=$((timeout - 2))
done

if [ $timeout -le 0 ]; then
    print_warning "Frontend may not be fully ready, but continuing..."
fi

# Display service status
print_status "Service Status:"
docker-compose ps

# Display access information
echo -e "\n${GREEN}🎉 Bosch Industrial Automation Platform is now running!${NC}"
echo -e "\n${BLUE}📋 Access Information:${NC}"
echo -e "🌐 Frontend Application: ${GREEN}http://localhost:3000${NC}"
echo -e "🔧 Backend API: ${GREEN}http://localhost:5001${NC}"
echo -e "📚 API Documentation: ${GREEN}http://localhost:5001/swagger${NC}"
echo -e "💊 Health Check: ${GREEN}http://localhost:5001/health${NC}"
echo -e "📊 Prometheus: ${GREEN}http://localhost:9090${NC}"
echo -e "📈 Grafana: ${GREEN}http://localhost:3001${NC}"
echo -e "🗄️ Database: ${GREEN}localhost:1433${NC}"

echo -e "\n${BLUE}🔑 Default Credentials:${NC}"
echo -e "SQL Server: sa / YourStrong@Passw0rd"
echo -e "Grafana: admin / admin123"

echo -e "\n${YELLOW}💡 Useful Commands:${NC}"
echo -e "View logs: ${BLUE}docker-compose logs [service-name]${NC}"
echo -e "Stop system: ${BLUE}docker-compose down${NC}"
echo -e "Restart service: ${BLUE}docker-compose restart [service-name]${NC}"
echo -e "Run tests: ${BLUE}./scripts/test-system.sh${NC}"

echo -e "\n${GREEN}✅ System startup completed successfully!${NC}"
