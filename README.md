# Industrial Automation Platform

## Overview

This is a comprehensive Industrial Automation Platform designed for modern manufacturing environments, built with a microservices architecture using .NET Core for the backend and React with TypeScript for the frontend. The platform provides a robust foundation for managing automation jobs, monitoring system performance, and integrating with Industry 4.0 standards. The system is containerized using Docker and includes comprehensive monitoring, logging, and security features to meet enterprise-grade requirements.

## Core Features

The platform offers a wide range of automation capabilities including job scheduling, real-time monitoring, and machine learning operations (MLOps). Key features include user authentication and authorization, automated job execution, performance metrics tracking, and web automation capabilities. The system supports multi-tenant architecture with role-based access control, ensuring secure and scalable operations across different organizational units. Advanced features include computer vision integration, experimental analysis tools, and comprehensive audit logging for compliance and traceability.

## Technical Architecture

Built on a modern tech stack, the backend utilizes .NET Core with Entity Framework for data persistence, implementing a clean architecture pattern with separate layers for API, Core business logic, and Infrastructure. The frontend is developed using React with TypeScript, providing a responsive and intuitive user interface. The system includes comprehensive middleware for authentication, rate limiting, input validation, and global exception handling. Database operations are supported by PostgreSQL with advanced schema management and migration scripts, while the entire system is orchestrated using Docker Compose for easy deployment and scaling.

## DevOps and Monitoring

The platform includes extensive DevOps capabilities with automated testing, continuous integration support, and comprehensive monitoring using Prometheus. Security is a top priority with JWT-based authentication, password reset functionality, and blacklisted token management. The system features advanced logging and audit trails, performance benchmarking tools, and health check endpoints for system reliability. Backup and restore scripts are included for data protection, while the monitoring stack provides real-time insights into system performance and operational metrics.

## Getting Started

To deploy the Industrial Automation Platform, ensure you have Docker and Docker Compose installed on your system. The project includes comprehensive setup documentation in SETUP.md and testing guidelines in TESTING.md. Use the provided scripts in the scripts directory to start the system, run tests, and perform system verification. The platform is designed to be production-ready with environment-specific configurations for development, staging, and production deployments. For detailed setup instructions and configuration options, refer to the included documentation files.

## How to Run the Project

### Prerequisites
- Docker and Docker Compose installed on your system
- Git (to clone the repository)
- At least 4GB of available RAM
- Ports 3000, 5000, 5432, and 9090 available on your system

### Step-by-Step Instructions

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd bosch-gmbh
   ```

2. **Initialize the Database**
   ```bash
   # Run the database initialization script
   ./scripts/start-system.sh
   ```
   This script will:
   - Start the PostgreSQL database container
   - Initialize the database schema
   - Seed the database with initial data

3. **Start the Backend Services**
   ```bash
   # Navigate to the backend directory
   cd backend
   
   # Build and start the .NET API
   docker-compose up --build
   ```
   The backend API will be available at `http://localhost:5000`

4. **Start the Frontend Application**
   ```bash
   # In a new terminal, navigate to the frontend directory
   cd frontend
   
   # Install dependencies
   npm install
   
   # Start the development server
   npm start
   ```
   The frontend application will be available at `http://localhost:3000`

5. **Verify the Installation**
   ```bash
   # Run the verification script
   ./scripts/verify-integration.sh
   ```
   This will test all endpoints and ensure the system is running correctly.

### Alternative: Quick Start with Docker Compose
```bash
# From the project root directory
docker-compose up --build
```
This will start all services (database, backend, frontend, and monitoring) in one command.

### Accessing the Application
- **Frontend UI**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger
- **Monitoring Dashboard**: http://localhost:9090 (Prometheus)

### Stopping the Application
```bash
# Stop all services
docker-compose down

# Stop and remove all containers, networks, and volumes
docker-compose down -v
```
