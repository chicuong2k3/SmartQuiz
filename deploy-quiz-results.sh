#!/bin/bash

# Quiz Results Summary - Deployment Script
# This script will build, migrate, and run your application

set -e  # Exit on error

echo "=========================================="
echo "Quiz Results Summary - Deployment"
echo "=========================================="
echo ""

# Navigate to project directory
cd "$(dirname "$0")"

echo "Step 1: Building the solution..."
dotnet build SmartQuiz.sln --configuration Release

echo ""
echo "Step 2: Applying database migrations..."
cd SmartQuiz
dotnet ef database update

echo ""
echo "Step 3: Starting the application..."
echo ""
echo "=========================================="
echo "✅ Setup Complete!"
echo "=========================================="
echo ""
echo "The application is starting..."
echo "Once it's running, navigate to:"
echo "  - http://localhost:5000/quiz-demo (to create test data)"
echo "  - http://localhost:5000/quiz-results/{id} (to view results)"
echo ""
echo "Press Ctrl+C to stop the server"
echo ""
echo "=========================================="
echo ""

# Run the application
dotnet run --project SmartQuiz.csproj

echo ""
echo "Application stopped."

