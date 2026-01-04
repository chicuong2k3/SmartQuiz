#!/bin/bash

echo "========================================"
echo "Quiz Results - Build & Run Script"
echo "========================================"
echo ""

cd "$(dirname "$0")"

echo "Step 1: Cleaning previous builds..."
dotnet clean --verbosity quiet

echo ""
echo "Step 2: Restoring packages..."
dotnet restore --verbosity quiet

echo ""
echo "Step 3: Building solution..."
dotnet build --configuration Debug --no-restore --verbosity minimal

BUILD_STATUS=$?

if [ $BUILD_STATUS -eq 0 ]; then
    echo ""
    echo "========================================"
    echo "✅ BUILD SUCCESSFUL!"
    echo "========================================"
    echo ""
    echo "Step 4: Applying database migrations..."
    cd SmartQuiz
    dotnet ef database update --verbose
    cd ..
    
    echo ""
    echo "========================================"
    echo "🚀 Starting Application..."
    echo "========================================"
    echo ""
    echo "Navigate to: http://localhost:5000/quiz-demo"
    echo "Press Ctrl+C to stop"
    echo ""
    
    dotnet run --project SmartQuiz/SmartQuiz.csproj
else
    echo ""
    echo "========================================"
    echo "❌ BUILD FAILED!"
    echo "========================================"
    echo ""
    echo "Please check the error messages above."
    echo "Common issues:"
    echo "  - Missing NuGet packages (run: dotnet restore)"
    echo "  - Compilation errors (check error messages)"
    echo "  - IDE cache issues (clean and rebuild)"
    echo ""
    exit 1
fi

