#!/bin/bash

# Bosch Thesis Development Environment Stop Script

echo "🛑 Stopping Bosch Thesis Development Environment..."

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

# Stop services
stop_services() {
    print_status "Stopping services..."
    
    docker-compose down
    
    if [ $? -eq 0 ]; then
        print_success "Services stopped successfully!"
    else
        print_error "Failed to stop services"
        exit 1
    fi
}

# Clean up (optional)
cleanup() {
    if [ "$1" = "--clean" ] || [ "$1" = "-c" ]; then
        print_status "Cleaning up volumes and containers..."
        docker-compose down -v --remove-orphans
        print_success "Cleanup completed!"
    fi
}

# Show status
show_status() {
    print_status "Checking remaining containers..."
    docker-compose ps
    
    echo ""
    print_success "🛑 Bosch Thesis Development Environment stopped!"
    echo ""
    print_status "To start again:"
    echo "  🚀 ./scripts/start-dev.sh"
    echo ""
    print_status "To clean up completely:"
    echo "  🧹 ./scripts/stop-dev.sh --clean"
}

# Main function
main() {
    stop_services
    cleanup "$1"
    show_status
}

# Run main function with arguments
main "$@"
