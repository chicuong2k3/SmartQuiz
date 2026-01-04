#!/bin/bash

# Simple build test script
cd /home/chicuong/Desktop/code/SmartQuiz

echo "====== QUIZ RESULTS BUILD TEST ======"
echo ""
echo "Testing build step by step..."
echo ""

# Test 1: Clean
echo "[1/6] Cleaning..."
dotnet clean > /dev/null 2>&1
echo "✓ Clean completed"

# Test 2: Clear cache
echo "[2/6] Clearing NuGet cache..."
dotnet nuget locals all --clear > /dev/null 2>&1
echo "✓ Cache cleared"

# Test 3: Restore
echo "[3/6] Restoring packages..."
dotnet restore --force > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✓ Packages restored"
else
    echo "✗ Package restore FAILED"
    echo "Run: dotnet restore --verbosity detailed"
    exit 1
fi

# Test 4: Build server
echo "[4/6] Building server project..."
dotnet build SmartQuiz/SmartQuiz.csproj --no-restore > server-build.log 2>&1
if [ $? -eq 0 ]; then
    echo "✓ Server build SUCCESS"
else
    echo "✗ Server build FAILED"
    echo ""
    echo "Errors found:"
    grep -i "error" server-build.log | head -10
    echo ""
    echo "See server-build.log for details"
    exit 1
fi

# Test 5: Build client
echo "[5/6] Building client project..."
dotnet build SmartQuiz.Client/SmartQuiz.Client.csproj --no-restore > client-build.log 2>&1
if [ $? -eq 0 ]; then
    echo "✓ Client build SUCCESS"
else
    echo "✗ Client build FAILED"
    echo ""
    echo "Errors found:"
    grep -i "error" client-build.log | head -10
    echo ""
    echo "See client-build.log for details"
    exit 1
fi

# Test 6: Build solution
echo "[6/6] Building entire solution..."
dotnet build SmartQuiz.sln --no-restore > solution-build.log 2>&1
if [ $? -eq 0 ]; then
    echo "✓ Solution build SUCCESS"
else
    echo "✗ Solution build FAILED"
    echo ""
    echo "Errors found:"
    grep -i "error" solution-build.log | head -10
    echo ""
    echo "See solution-build.log for details"
    exit 1
fi

echo ""
echo "======================================"
echo "✓ ALL BUILDS SUCCESSFUL!"
echo "======================================"
echo ""
echo "Next: Apply migrations and run"
echo "  cd SmartQuiz && dotnet ef database update && cd .."
echo "  dotnet run --project SmartQuiz/SmartQuiz.csproj"
echo ""

