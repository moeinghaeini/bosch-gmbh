#!/bin/bash

# Bosch Thesis Development Environment Startup Script

echo "🚀 Starting Bosch Thesis Development Environment..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
check_docker() {
    if ! docker info &> /dev/null; then
        print_error "Docker is not running. Please start Docker first."
        exit 1
    fi
    print_success "Docker is running"
}

# Start services with Docker Compose
start_services() {
    print_status "Starting services with Docker Compose..."
    
    # Remove version from docker-compose.yml to avoid warning
    if grep -q "version:" docker-compose.yml; then
        print_status "Updating docker-compose.yml to remove version..."
        sed -i '' '/^version:/d' docker-compose.yml
    fi
    
    # Start services
    docker-compose up -d
    
    if [ $? -eq 0 ]; then
        print_success "Services started successfully!"
    else
        print_error "Failed to start services"
        exit 1
    fi
}

# Wait for services to be ready
wait_for_services() {
    print_status "Waiting for services to be ready..."
    
    # Wait for SQL Server
    print_status "Waiting for SQL Server..."
    timeout=60
    while [ $timeout -gt 0 ]; do
        if docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1" &> /dev/null; then
            print_success "SQL Server is ready"
            break
        fi
        sleep 2
        timeout=$((timeout - 2))
    done
    
    if [ $timeout -le 0 ]; then
        print_warning "SQL Server may not be ready yet"
    fi
    
    # Wait for backend
    print_status "Waiting for backend API..."
    timeout=60
    while [ $timeout -gt 0 ]; do
        if curl -s http://localhost:5000/api/health &> /dev/null; then
            print_success "Backend API is ready"
            break
        fi
        sleep 2
        timeout=$((timeout - 2))
    done
    
    if [ $timeout -le 0 ]; then
        print_warning "Backend API may not be ready yet"
    fi
}

# Show service status
show_status() {
    print_status "Service Status:"
    docker-compose ps
    
    echo ""
    print_success "🎉 Bosch Thesis Development Environment is ready!"
    echo ""
    print_status "Access the application:"
    echo "  🌐 Frontend: http://localhost:3000"
    echo "  🔧 Backend API: http://localhost:5000"
    echo "  📚 Swagger Docs: http://localhost:5000/swagger"
    echo "  🗄️  SQL Server: localhost:1433"
    echo ""
    print_status "Useful commands:"
    echo "  📊 View logs: docker-compose logs -f"
    echo "  🛑 Stop services: docker-compose down"
    echo "  🔄 Restart services: docker-compose restart"
    echo "  🧹 Clean up: docker-compose down -v"
}

# Main function
main() {
    check_docker
    start_services
    wait_for_services
    show_status
}

# Run main function
main
