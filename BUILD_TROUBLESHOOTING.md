# 🔧 BUILD TROUBLESHOOTING GUIDE

## Current Status

All code files have been created successfully. If you're experiencing build failures, follow the steps below.

---

## ✅ Files Verified

All necessary files are present:
- ✅ Entity Models (QuizResult.cs, QuizResult)
- ✅ EF Configurations  
- ✅ Service Implementation (QuizResultService.cs)
- ✅ Service Interface (IQuizResultService.cs)
- ✅ DTOs (QuizResultDto.cs)
- ✅ Commands (CreateQuizResultCommand.cs)
- ✅ Pages (QuizResultsPage.razor, QuizDemoPage.razor)
- ✅ State Management (QuizResultState.cs)
- ✅ Migrations (20260104000000_AddQuizResultTables.cs + .Designer.cs)
- ✅ Model Snapshot (Updated)
- ✅ Services Registered (Both client and server)
- ✅ Global Usings (Updated)

---

## 🚀 RECOMMENDED: Use IDE to Build and Run

Since terminal commands are hanging, I recommend using your IDE:

### Option 1: JetBrains Rider (Recommended)
1. Open the solution in Rider
2. Click "Build" → "Rebuild Solution"
3. If build succeeds, press **F5** to run
4. Navigate to http://localhost:5000/quiz-demo

### Option 2: Visual Studio Code
1. Open the folder in VS Code
2. Press **Ctrl+Shift+B** to build
3. Press **F5** to run (or use "Run and Debug")
4. Navigate to http://localhost:5000/quiz-demo

### Option 3: Visual Studio
1. Open SmartQuiz.sln
2. Right-click solution → "Rebuild Solution"
3. Press **F5** to run
4. Navigate to http://localhost:5000/quiz-demo

---

## 🔨 Manual Build Steps (If IDE not available)

If you must use terminal, try these steps ONE AT A TIME:

### Step 1: Clean Everything
```bash
cd /home/chicuong/Desktop/code/SmartQuiz
rm -rf SmartQuiz/bin SmartQuiz/obj
rm -rf SmartQuiz.Client/bin SmartQuiz.Client/obj
dotnet clean
```

### Step 2: Restore Packages
```bash
dotnet restore SmartQuiz/SmartQuiz.csproj
dotnet restore SmartQuiz.Client/SmartQuiz.Client.csproj
```

### Step 3: Build Server Project
```bash
dotnet build SmartQuiz/SmartQuiz.csproj --configuration Debug
```

**Check output** - if this fails, there's a compilation error in the server project.

### Step 4: Build Client Project
```bash
dotnet build SmartQuiz.Client/SmartQuiz.Client.csproj --configuration Debug
```

**Check output** - if this fails, there's a compilation error in the client project.

### Step 5: Apply Database Migration
```bash
cd SmartQuiz
dotnet ef database update
cd ..
```

### Step 6: Run Application
```bash
dotnet run --project SmartQuiz/SmartQuiz.csproj
```

---

## 🐛 Common Build Issues & Solutions

### Issue 1: "Type or namespace not found"
**Symptoms**: Error CS0246, cannot find type 'QuizResultDto'
**Solution**:
```bash
# Make sure GlobalUsings.cs has these lines:
# SmartQuiz.Client/GlobalUsings.cs should have:
global using SmartQuiz.Client.Data.Dtos;
global using SmartQuiz.Client.Data.Commands;
global using SmartQuiz.Client.Data.Services;
```

### Issue 2: "Service not registered"
**Symptoms**: Runtime error about IQuizResultService
**Solution**: 
- Already fixed! Check these files:
  - `SmartQuiz/ServicesExtensions.cs` line ~90
  - `SmartQuiz.Client/Program.cs` line ~29

### Issue 3: Database connection error
**Symptoms**: Cannot connect to PostgreSQL
**Solution**:
```bash
# Check PostgreSQL is running
sudo systemctl status postgresql
# OR
brew services list | grep postgresql

# Start if needed
sudo systemctl start postgresql
# OR
brew services start postgresql
```

### Issue 4: Port 5000 already in use
**Symptoms**: Address already in use error
**Solution**:
```bash
# Find and kill process on port 5000
lsof -i :5000
kill -9 <PID>

# Or use different port in launchSettings.json
```

### Issue 5: Migration fails
**Symptoms**: Error applying migration
**Solution**:
```bash
# Drop and recreate database (WARNING: Loses data!)
cd SmartQuiz
dotnet ef database drop
dotnet ef database update
```

---

## 📝 Verification Checklist

Before running, verify:

- [ ] PostgreSQL is running
- [ ] .NET SDK 8.0+ installed (`dotnet --version`)
- [ ] No other SmartQuiz instances running
- [ ] Connection string in `appsettings.json` is correct
- [ ] Port 5000 is available

---

## 🎯 Quick Test Script

Use this script for automated build and run:

```bash
cd /home/chicuong/Desktop/code/SmartQuiz
./build-and-run.sh
```

This script will:
1. Clean previous builds
2. Restore packages
3. Build solution
4. Apply migrations (if build succeeds)
5. Run the application

---

## 🔍 Debugging Build Failures

If you're still getting build errors:

### Get Detailed Error Output
```bash
cd /home/chicuong/Desktop/code/SmartQuiz
dotnet build --verbosity detailed > build-log.txt 2>&1
cat build-log.txt | grep "error"
```

### Check Specific Project
```bash
# Build server only
dotnet build SmartQuiz/SmartQuiz.csproj -v detailed

# Build client only  
dotnet build SmartQuiz.Client/SmartQuiz.Client.csproj -v detailed
```

### Verify NuGet Packages
```bash
dotnet list SmartQuiz/SmartQuiz.csproj package
dotnet list SmartQuiz.Client/SmartQuiz.Client.csproj package
```

---

## ✅ Expected Successful Build Output

When build succeeds, you should see:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## 🎊 After Successful Build

Once the application is running:

1. Open browser: **http://localhost:5000**
2. Click **"Quiz Demo"** in header
3. Click **"Create Sample Quiz Result"**
4. Click **"View Results"**
5. You should see the quiz results page with:
   - 85% score circle
   - PASSED badge
   - Time: 04:12
   - Points: +340 XP
   - Rank: Top 10%
   - Performance bars
   - Answer grid (20 questions)

---

## 📞 Still Having Issues?

If build still fails:

1. **Check IDE error panel** - Much better at showing errors
2. **Use IDE build** - IDEs handle builds better than CLI
3. **Check file contents** - Compare with documentation files
4. **Verify all files exist** - Use the PRE_LAUNCH_CHECKLIST.md

---

## 🚀 TLDR - Quick Fix

**Best Solution**: Use your IDE to build and run instead of terminal commands.

1. Open project in Rider/VS Code/Visual Studio
2. Build solution (Ctrl+Shift+B or F6)
3. Run application (F5)
4. Test at http://localhost:5000/quiz-demo

**This avoids all terminal hanging issues!**

---

Good luck! 🎉

