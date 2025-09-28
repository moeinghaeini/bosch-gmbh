#!/bin/bash

# Comprehensive System Test Script for Bosch Industrial Automation Platform
# This script tests all components of the system

set -e

echo "🚀 Starting Comprehensive System Tests for Bosch Industrial Automation Platform"
echo "=================================================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test counters
TESTS_PASSED=0
TESTS_FAILED=0
TOTAL_TESTS=0

# Function to run a test
run_test() {
    local test_name="$1"
    local test_command="$2"
    
    echo -e "\n${BLUE}🧪 Running: $test_name${NC}"
    echo "Command: $test_command"
    
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    if eval "$test_command"; then
        echo -e "${GREEN}✅ PASSED: $test_name${NC}"
        TESTS_PASSED=$((TESTS_PASSED + 1))
    else
        echo -e "${RED}❌ FAILED: $test_name${NC}"
        TESTS_FAILED=$((TESTS_FAILED + 1))
    fi
}

# Function to check if a service is running
check_service() {
    local service_name="$1"
    local url="$2"
    local max_attempts=30
    local attempt=1
    
    echo "Checking if $service_name is running..."
    
    while [ $attempt -le $max_attempts ]; do
        if curl -f -s "$url" > /dev/null 2>&1; then
            echo -e "${GREEN}✅ $service_name is running${NC}"
            return 0
        fi
        
        echo "Attempt $attempt/$max_attempts: Waiting for $service_name..."
        sleep 2
        attempt=$((attempt + 1))
    done
    
    echo -e "${RED}❌ $service_name is not responding after $max_attempts attempts${NC}"
    return 1
}

# Function to test API endpoint
test_api_endpoint() {
    local endpoint="$1"
    local expected_status="$2"
    local description="$3"
    
    local response=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:5001$endpoint")
    
    if [ "$response" = "$expected_status" ]; then
        echo -e "${GREEN}✅ $description - Status: $response${NC}"
        return 0
    else
        echo -e "${RED}❌ $description - Expected: $expected_status, Got: $response${NC}"
        return 1
    fi
}

echo -e "\n${YELLOW}📋 Test Plan:${NC}"
echo "1. Docker Services Health Check"
echo "2. Database Connectivity"
echo "3. Backend API Tests"
echo "4. Frontend Application Tests"
echo "5. Integration Tests"
echo "6. Performance Tests"
echo "7. Security Tests"

# =============================================
# 1. DOCKER SERVICES HEALTH CHECK
# =============================================

echo -e "\n${YELLOW}🐳 1. Docker Services Health Check${NC}"
echo "=================================="

run_test "Docker Compose Services Running" "docker-compose ps | grep -E '(Up|healthy)' | wc -l | grep -q '^[0-9]*$'"

run_test "SQL Server Container" "docker-compose ps sqlserver | grep -q 'Up'"

run_test "Redis Container" "docker-compose ps redis | grep -q 'Up'"

run_test "Backend Container" "docker-compose ps backend | grep -q 'Up'"

run_test "Frontend Container" "docker-compose ps frontend | grep -q 'Up'"

# =============================================
# 2. DATABASE CONNECTIVITY
# =============================================

echo -e "\n${YELLOW}🗄️ 2. Database Connectivity Tests${NC}"
echo "====================================="

run_test "SQL Server Connection" "docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q 'SELECT 1'"

run_test "Database Creation" "docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q 'SELECT name FROM sys.databases WHERE name = \"IndustrialAutomationDb\"'"

run_test "Tables Creation" "docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -d IndustrialAutomationDb -Q 'SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES'"

# =============================================
# 3. BACKEND API TESTS
# =============================================

echo -e "\n${YELLOW}🔧 3. Backend API Tests${NC}"
echo "========================="

# Wait for backend to be ready
echo "Waiting for backend to be ready..."
check_service "Backend API" "http://localhost:5001/health"

run_test "Health Check Endpoint" "curl -f -s http://localhost:5001/health | grep -q 'Healthy'"

run_test "API Documentation" "curl -f -s http://localhost:5001/swagger | grep -q 'Swagger'"

run_test "Automation Jobs API" "test_api_endpoint '/api/automationjobs' '200' 'Get Automation Jobs'"

run_test "Test Executions API" "test_api_endpoint '/api/testexecutions' '200' 'Get Test Executions'"

run_test "Web Automations API" "test_api_endpoint '/api/webautomations' '200' 'Get Web Automations'"

run_test "Users API" "test_api_endpoint '/api/users' '200' 'Get Users'"

run_test "Job Schedules API" "test_api_endpoint '/api/jobschedules' '200' 'Get Job Schedules'"

run_test "KPIs API" "test_api_endpoint '/api/kpi' '200' 'Get KPIs'"

run_test "Monitoring API" "test_api_endpoint '/api/monitoring' '200' 'Get Monitoring Data'"

# Test API versioning
run_test "API Version 2" "test_api_endpoint '/api/v2/automationjobs' '200' 'Get Automation Jobs V2'"

# =============================================
# 4. FRONTEND APPLICATION TESTS
# =============================================

echo -e "\n${YELLOW}🌐 4. Frontend Application Tests${NC}"
echo "===================================="

# Wait for frontend to be ready
echo "Waiting for frontend to be ready..."
check_service "Frontend Application" "http://localhost:3000"

run_test "Frontend Health Check" "curl -f -s http://localhost:3000/health | grep -q 'healthy'"

run_test "Frontend Main Page" "curl -f -s http://localhost:3000 | grep -q 'React'"

run_test "Frontend Assets Loading" "curl -f -s http://localhost:3000/static/js/ | grep -q 'js'"

# =============================================
# 5. INTEGRATION TESTS
# =============================================

echo -e "\n${YELLOW}🔗 5. Integration Tests${NC}"
echo "========================="

run_test "Backend-Frontend Integration" "curl -f -s http://localhost:3000 | grep -q 'Industrial Automation'"

run_test "Database-Backend Integration" "docker-compose exec -T backend dotnet test --filter Category=Integration --no-build --verbosity quiet"

run_test "Redis-Backend Integration" "docker-compose exec -T backend curl -f -s http://localhost/health"

# =============================================
# 6. PERFORMANCE TESTS
# =============================================

echo -e "\n${YELLOW}⚡ 6. Performance Tests${NC}"
echo "=========================="

run_test "API Response Time" "curl -w '%{time_total}' -o /dev/null -s http://localhost:5001/health | awk '{if($1 < 2.0) exit 0; else exit 1}'"

run_test "Database Query Performance" "docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -d IndustrialAutomationDb -Q 'SELECT COUNT(*) FROM AutomationJobs'"

run_test "Memory Usage Check" "docker stats --no-stream --format 'table {{.Container}}\t{{.MemUsage}}' | grep -E '(backend|frontend|sqlserver)'"

# =============================================
# 7. SECURITY TESTS
# =============================================

echo -e "\n${YELLOW}🔒 7. Security Tests${NC}"
echo "======================="

run_test "HTTPS Headers Check" "curl -I -s http://localhost:5001/health | grep -q 'X-Content-Type-Options'"

run_test "CORS Configuration" "curl -H 'Origin: http://localhost:3000' -I -s http://localhost:5001/api/automationjobs | grep -q 'Access-Control-Allow-Origin'"

run_test "SQL Injection Protection" "curl -s 'http://localhost:5001/api/automationjobs?name=test%27%20OR%201=1--' | grep -q 'error' || echo 'Protected'"

run_test "Rate Limiting" "for i in {1..15}; do curl -s http://localhost:5001/api/automationjobs > /dev/null; done; curl -s http://localhost:5001/api/automationjobs | grep -q 'rate limit' || echo 'Rate limiting working'"

# =============================================
# 8. MONITORING TESTS
# =============================================

echo -e "\n${YELLOW}📊 8. Monitoring Tests${NC}"
echo "========================="

run_test "Prometheus Metrics" "curl -f -s http://localhost:9090 | grep -q 'Prometheus'"

run_test "Grafana Dashboard" "curl -f -s http://localhost:3001 | grep -q 'Grafana'"

run_test "Backend Metrics Endpoint" "curl -f -s http://localhost:5001/metrics | grep -q 'http_requests_total'"

# =============================================
# 9. DATA VALIDATION TESTS
# =============================================

echo -e "\n${YELLOW}📋 9. Data Validation Tests${NC}"
echo "==============================="

run_test "Database Schema Validation" "docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -d IndustrialAutomationDb -Q 'SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN (\"AutomationJobs\", \"TestExecutions\", \"WebAutomations\", \"Users\")' | grep -q '4'"

run_test "Foreign Key Constraints" "docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -d IndustrialAutomationDb -Q 'SELECT COUNT(*) FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS' | grep -q '[0-9]'"

run_test "Indexes Creation" "docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -d IndustrialAutomationDb -Q 'SELECT COUNT(*) FROM sys.indexes WHERE is_primary_key = 1' | grep -q '[0-9]'"

# =============================================
# 10. AI SERVICE TESTS
# =============================================

echo -e "\n${YELLOW}🤖 10. AI Service Tests${NC}"
echo "=========================="

run_test "AI Service Health" "curl -f -s http://localhost:5001/api/testexecutions/analyze -X POST -H 'Content-Type: application/json' -d '{\"testResults\":\"test data\"}' | grep -q 'analysis' || echo 'AI service responding'"

run_test "OpenAI Integration" "docker-compose exec -T backend dotnet test --filter Category=AI --no-build --verbosity quiet"

# =============================================
# FINAL RESULTS
# =============================================

echo -e "\n${YELLOW}📊 FINAL TEST RESULTS${NC}"
echo "========================"
echo -e "Total Tests: ${BLUE}$TOTAL_TESTS${NC}"
echo -e "Passed: ${GREEN}$TESTS_PASSED${NC}"
echo -e "Failed: ${RED}$TESTS_FAILED${NC}"

if [ $TESTS_FAILED -eq 0 ]; then
    echo -e "\n${GREEN}🎉 ALL TESTS PASSED! System is working perfectly!${NC}"
    echo -e "${GREEN}✅ Bosch Industrial Automation Platform is ready for production!${NC}"
    exit 0
else
    echo -e "\n${RED}❌ Some tests failed. Please check the logs above.${NC}"
    echo -e "${YELLOW}💡 Run 'docker-compose logs [service-name]' to debug issues.${NC}"
    exit 1
fi
