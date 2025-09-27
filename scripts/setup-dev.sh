#!/bin/bash

# Bosch Thesis Development Environment Setup Script

echo "🚀 Setting up Bosch Thesis Development Environment..."

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

# Check if Docker is installed
check_docker() {
    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed. Please install Docker first."
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        print_error "Docker Compose is not installed. Please install Docker Compose first."
        exit 1
    fi
    
    print_success "Docker and Docker Compose are installed"
}

# Check if .NET SDK is installed
check_dotnet() {
    if ! command -v dotnet &> /dev/null; then
        print_warning ".NET SDK is not installed. You can still run with Docker."
    else
        print_success ".NET SDK is installed"
    fi
}

# Check if Node.js is installed
check_node() {
    if ! command -v node &> /dev/null; then
        print_warning "Node.js is not installed. You can still run with Docker."
    else
        print_success "Node.js is installed"
    fi
}

# Setup backend environment
setup_backend() {
    print_status "Setting up backend environment..."
    
    cd backend
    
    # Restore packages if .NET is available
    if command -v dotnet &> /dev/null; then
        print_status "Restoring .NET packages..."
        dotnet restore
        print_success "Backend packages restored"
    else
        print_warning "Skipping .NET package restore (Docker will handle this)"
    fi
    
    cd ..
}

# Setup frontend environment
setup_frontend() {
    print_status "Setting up frontend environment..."
    
    cd frontend
    
    # Install packages if Node.js is available
    if command -v npm &> /dev/null; then
        print_status "Installing frontend packages..."
        npm install
        print_success "Frontend packages installed"
    else
        print_warning "Skipping npm install (Docker will handle this)"
    fi
    
    cd ..
}

# Create environment files
create_env_files() {
    print_status "Creating environment configuration files..."
    
    # Create .env file for Docker Compose
    cat > .env << EOF
# Database Configuration
DB_HOST=localhost
DB_PORT=1433
DB_NAME=BoschThesisDb
DB_USER=sa
DB_PASSWORD=YourStrong@Passw0rd

# API Configuration
API_URL=http://localhost:5000
API_PORT=5000

# Frontend Configuration
REACT_APP_API_URL=http://localhost:5000/api
REACT_APP_PORT=3000

# Environment
NODE_ENV=development
ASPNETCORE_ENVIRONMENT=Development

# Docker Configuration
COMPOSE_PROJECT_NAME=bosch-thesis
EOF

    # Create frontend .env file
    cat > frontend/.env << EOF
REACT_APP_API_URL=http://localhost:5000/api
REACT_APP_PORT=3000
GENERATE_SOURCEMAP=false
EOF

    print_success "Environment files created"
}

# Main setup function
main() {
    print_status "Starting Bosch Thesis development environment setup..."
    
    # Check prerequisites
    check_docker
    check_dotnet
    check_node
    
    # Create environment files
    create_env_files
    
    # Setup backend
    setup_backend
    
    # Setup frontend
    setup_frontend
    
    print_success "Development environment setup complete!"
    
    echo ""
    print_status "Next steps:"
    echo "1. Run 'docker-compose up -d' to start all services"
    echo "2. Access the application at:"
    echo "   - Frontend: http://localhost:3000"
    echo "   - Backend API: http://localhost:5000"
    echo "   - Swagger: http://localhost:5000/swagger"
    echo ""
    print_status "For local development:"
    echo "- Backend: cd backend && dotnet run"
    echo "- Frontend: cd frontend && npm start"
}

# Run main function
main
