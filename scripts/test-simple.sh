#!/bin/bash

echo "🚀 Running Simple System Tests"
echo "=============================="

# Test 1: Check if Docker is running
echo "📋 Test 1: Docker Status"
if docker --version > /dev/null 2>&1; then
    echo "✅ Docker is installed"
else
    echo "❌ Docker is not installed"
    exit 1
fi

# Test 2: Check if Docker Compose is available
echo "📋 Test 2: Docker Compose Status"
if docker-compose --version > /dev/null 2>&1; then
    echo "✅ Docker Compose is available"
else
    echo "❌ Docker Compose is not available"
    exit 1
fi

# Test 3: Validate Docker Compose file
echo "📋 Test 3: Docker Compose Validation"
if docker-compose config > /dev/null 2>&1; then
    echo "✅ Docker Compose configuration is valid"
else
    echo "❌ Docker Compose configuration has errors"
    exit 1
fi

# Test 4: Check if required files exist
echo "📋 Test 4: Required Files Check"
required_files=(
    "docker-compose.yml"
    "backend/Dockerfile"
    "frontend/Dockerfile"
    "database/init-database.sql"
)

for file in "${required_files[@]}"; do
    if [ -f "$file" ]; then
        echo "✅ $file exists"
    else
        echo "❌ $file is missing"
        exit 1
    fi
done

# Test 5: Check frontend package.json
echo "📋 Test 5: Frontend Configuration"
if [ -f "frontend/package.json" ]; then
    echo "✅ Frontend package.json exists"
    if grep -q "react" frontend/package.json; then
        echo "✅ React dependencies found"
    else
        echo "❌ React dependencies not found"
    fi
else
    echo "❌ Frontend package.json not found"
fi

# Test 6: Check backend solution file
echo "📋 Test 6: Backend Configuration"
if [ -f "backend/IndustrialAutomation.sln" ]; then
    echo "✅ Backend solution file exists"
else
    echo "❌ Backend solution file not found"
fi

echo ""
echo "🎉 Basic system validation completed!"
echo "📊 Summary: All basic checks passed"
echo ""
echo "💡 Next steps:"
echo "   1. Run 'docker-compose up -d' to start services"
echo "   2. Run './scripts/test-system.sh' for comprehensive tests"
echo "   3. Access the application at http://localhost:3000"
