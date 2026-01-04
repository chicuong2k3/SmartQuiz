#!/bin/bash

echo "=================================================="
echo "Quiz Results - Build Verification & Fix Script"
echo "=================================================="
echo ""

cd "$(dirname "$0")"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "Step 1: Checking prerequisites..."
echo "-----------------------------------"

# Check .NET SDK
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓${NC} .NET SDK found: $DOTNET_VERSION"
else
    echo -e "${RED}✗${NC} .NET SDK not found!"
    exit 1
fi

# Check PostgreSQL
if command -v psql &> /dev/null; then
    echo -e "${GREEN}✓${NC} PostgreSQL client found"
else
    echo -e "${YELLOW}!${NC} PostgreSQL client not found (may be OK if server is remote)"
fi

echo ""
echo "Step 2: Verifying critical files..."
echo "-----------------------------------"

FILES_TO_CHECK=(
    "SmartQuiz/Data/Models/QuizResult.cs"
    "SmartQuiz/Data/Configurations/QuizResultConfiguration.cs"
    "SmartQuiz/Application/QuizResultService.cs"
    "SmartQuiz/Data/Migrations/20260104000000_AddQuizResultTables.cs"
    "SmartQuiz/Data/Migrations/20260104000000_AddQuizResultTables.Designer.cs"
    "SmartQuiz.Client/Data/Dtos/QuizResultDto.cs"
    "SmartQuiz.Client/Data/Commands/CreateQuizResultCommand.cs"
    "SmartQuiz.Client/Data/Services/IQuizResultService.cs"
    "SmartQuiz.Client/Pages/QuizResultsPage.razor"
    "SmartQuiz.Client/Pages/QuizResultState.cs"
    "SmartQuiz.Client/Pages/QuizDemoPage.razor"
)

ALL_FILES_EXIST=true
for file in "${FILES_TO_CHECK[@]}"; do
    if [ -f "$file" ]; then
        echo -e "${GREEN}✓${NC} $file"
    else
        echo -e "${RED}✗${NC} $file - MISSING!"
        ALL_FILES_EXIST=false
    fi
done

if [ "$ALL_FILES_EXIST" = false ]; then
    echo ""
    echo -e "${RED}ERROR: Some required files are missing!${NC}"
    exit 1
fi

echo ""
echo "Step 3: Cleaning previous builds..."
echo "-----------------------------------"
dotnet clean --verbosity quiet
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓${NC} Clean completed"
else
    echo -e "${YELLOW}!${NC} Clean had warnings (continuing)"
fi

echo ""
echo "Step 4: Restoring NuGet packages..."
echo "-----------------------------------"
dotnet restore --verbosity quiet
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓${NC} Packages restored successfully"
else
    echo -e "${RED}✗${NC} Package restore failed!"
    exit 1
fi

echo ""
echo "Step 5: Building Server Project..."
echo "-----------------------------------"
dotnet build SmartQuiz/SmartQuiz.csproj --configuration Debug --no-restore --verbosity normal
SERVER_BUILD_STATUS=$?

if [ $SERVER_BUILD_STATUS -eq 0 ]; then
    echo -e "${GREEN}✓${NC} Server project built successfully"
else
    echo -e "${RED}✗${NC} Server project build FAILED!"
    echo ""
    echo "Please check the error messages above."
    exit 1
fi

echo ""
echo "Step 6: Building Client Project..."
echo "-----------------------------------"
dotnet build SmartQuiz.Client/SmartQuiz.Client.csproj --configuration Debug --no-restore --verbosity normal
CLIENT_BUILD_STATUS=$?

if [ $CLIENT_BUILD_STATUS -eq 0 ]; then
    echo -e "${GREEN}✓${NC} Client project built successfully"
else
    echo -e "${RED}✗${NC} Client project build FAILED!"
    echo ""
    echo "Please check the error messages above."
    exit 1
fi

echo ""
echo "Step 7: Building Entire Solution..."
echo "-----------------------------------"
dotnet build SmartQuiz.sln --configuration Debug --no-restore --verbosity minimal
SOLUTION_BUILD_STATUS=$?

if [ $SOLUTION_BUILD_STATUS -eq 0 ]; then
    echo -e "${GREEN}✓${NC} Solution built successfully"
else
    echo -e "${RED}✗${NC} Solution build FAILED!"
    exit 1
fi

echo ""
echo "=================================================="
echo -e "${GREEN}✓ BUILD SUCCESSFUL!${NC}"
echo "=================================================="
echo ""
echo "Next steps:"
echo "  1. Apply database migrations:"
echo "     cd SmartQuiz && dotnet ef database update && cd .."
echo ""
echo "  2. Run the application:"
echo "     dotnet run --project SmartQuiz/SmartQuiz.csproj"
echo ""
echo "  3. Open browser:"
echo "     http://localhost:5000/quiz-demo"
echo ""
echo "=================================================="

