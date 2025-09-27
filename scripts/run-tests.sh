#!/bin/bash

# Industrial Automation Platform - Test Runner Script
# This script runs all tests for the project

set -e

echo "🧪 Starting Industrial Automation Platform Test Suite"
echo "=================================================="

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
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker and try again."
    exit 1
fi

# Check if required tools are installed
check_dependencies() {
    print_status "Checking dependencies..."
    
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK is not installed"
        exit 1
    fi
    
    if ! command -v node &> /dev/null; then
        print_error "Node.js is not installed"
        exit 1
    fi
    
    if ! command -v npm &> /dev/null; then
        print_error "npm is not installed"
        exit 1
    fi
    
    print_success "All dependencies are available"
}

# Start test infrastructure
start_test_infrastructure() {
    print_status "Starting test infrastructure..."
    
    # Start SQL Server for integration tests
    docker run -d \
        --name test-sqlserver \
        -e ACCEPT_EULA=Y \
        -e SA_PASSWORD=YourStrong@Passw0rd \
        -e MSSQL_PID=Express \
        -p 1434:1433 \
        mcr.microsoft.com/mssql/server:2022-latest
    
    # Start Redis for integration tests
    docker run -d \
        --name test-redis \
        -p 6380:6379 \
        redis:7-alpine
    
    # Wait for services to be ready
    print_status "Waiting for services to be ready..."
    sleep 30
    
    print_success "Test infrastructure is ready"
}

# Stop test infrastructure
stop_test_infrastructure() {
    print_status "Stopping test infrastructure..."
    
    docker stop test-sqlserver test-redis 2>/dev/null || true
    docker rm test-sqlserver test-redis 2>/dev/null || true
    
    print_success "Test infrastructure stopped"
}

# Run backend tests
run_backend_tests() {
    print_status "Running backend tests..."
    
    cd backend
    
    # Restore packages
    print_status "Restoring NuGet packages..."
    dotnet restore
    
    # Build solution
    print_status "Building solution..."
    dotnet build --configuration Release --no-restore
    
    # Run unit tests
    print_status "Running unit tests..."
    dotnet test IndustrialAutomation.Tests/IndustrialAutomation.Tests.csproj \
        --configuration Release \
        --no-build \
        --verbosity normal \
        --collect:"XPlat Code Coverage" \
        --results-directory ./coverage \
        --logger "console;verbosity=detailed"
    
    # Run integration tests
    print_status "Running integration tests..."
    dotnet test IndustrialAutomation.Tests/IndustrialAutomation.Tests.csproj \
        --configuration Release \
        --no-build \
        --verbosity normal \
        --filter Category=Integration \
        --logger "console;verbosity=detailed"
    
    cd ..
    print_success "Backend tests completed"
}

# Run frontend tests
run_frontend_tests() {
    print_status "Running frontend tests..."
    
    cd frontend
    
    # Install dependencies
    print_status "Installing npm packages..."
    npm ci
    
    # Run linting
    print_status "Running ESLint..."
    npm run lint || print_warning "Linting issues found"
    
    # Run unit tests
    print_status "Running unit tests..."
    npm test -- --coverage --watchAll=false --verbose
    
    # Run build test
    print_status "Testing build process..."
    npm run build
    
    cd ..
    print_success "Frontend tests completed"
}

# Run integration tests with Docker Compose
run_integration_tests() {
    print_status "Running integration tests with Docker Compose..."
    
    # Start services
    docker-compose -f docker-compose.yml up -d
    
    # Wait for services to be ready
    print_status "Waiting for services to be ready..."
    sleep 60
    
    # Run integration tests
    print_status "Running integration tests..."
    docker-compose exec -T backend dotnet test IndustrialAutomation.Tests/IndustrialAutomation.Tests.csproj \
        --configuration Release \
        --filter Category=Integration \
        --logger "console;verbosity=detailed"
    
    # Stop services
    docker-compose down
    
    print_success "Integration tests completed"
}

# Generate test report
generate_test_report() {
    print_status "Generating test report..."
    
    # Create reports directory
    mkdir -p reports
    
    # Generate backend coverage report
    if [ -d "backend/coverage" ]; then
        print_status "Generating backend coverage report..."
        # Add report generation logic here
    fi
    
    # Generate frontend coverage report
    if [ -d "frontend/coverage" ]; then
        print_status "Generating frontend coverage report..."
        # Add report generation logic here
    fi
    
    print_success "Test report generated"
}

# Main execution
main() {
    echo "Starting test execution at $(date)"
    echo "=================================================="
    
    # Parse command line arguments
    RUN_BACKEND=true
    RUN_FRONTEND=true
    RUN_INTEGRATION=true
    START_INFRASTRUCTURE=true
    
    while [[ $# -gt 0 ]]; do
        case $1 in
            --backend-only)
                RUN_FRONTEND=false
                RUN_INTEGRATION=false
                shift
                ;;
            --frontend-only)
                RUN_BACKEND=false
                RUN_INTEGRATION=false
                shift
                ;;
            --integration-only)
                RUN_BACKEND=false
                RUN_FRONTEND=false
                shift
                ;;
            --no-infrastructure)
                START_INFRASTRUCTURE=false
                shift
                ;;
            --help)
                echo "Usage: $0 [options]"
                echo "Options:"
                echo "  --backend-only      Run only backend tests"
                echo "  --frontend-only     Run only frontend tests"
                echo "  --integration-only  Run only integration tests"
                echo "  --no-infrastructure Skip starting test infrastructure"
                echo "  --help              Show this help message"
                exit 0
                ;;
            *)
                print_error "Unknown option: $1"
                exit 1
                ;;
        esac
    done
    
    # Check dependencies
    check_dependencies
    
    # Start test infrastructure if needed
    if [ "$START_INFRASTRUCTURE" = true ]; then
        start_test_infrastructure
    fi
    
    # Run tests
    if [ "$RUN_BACKEND" = true ]; then
        run_backend_tests
    fi
    
    if [ "$RUN_FRONTEND" = true ]; then
        run_frontend_tests
    fi
    
    if [ "$RUN_INTEGRATION" = true ]; then
        run_integration_tests
    fi
    
    # Generate test report
    generate_test_report
    
    # Stop test infrastructure
    if [ "$START_INFRASTRUCTURE" = true ]; then
        stop_test_infrastructure
    fi
    
    print_success "All tests completed successfully!"
    echo "=================================================="
    echo "Test execution finished at $(date)"
}

# Trap to ensure cleanup on exit
trap 'stop_test_infrastructure' EXIT

# Run main function
main "$@"
