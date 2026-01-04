# Quick Start Guide - Quiz Results Summary

## Step 1: Apply Database Migration

Run the following command to update the database with the new quiz result tables:

```bash
cd /home/chicuong/Desktop/code/SmartQuiz/SmartQuiz
dotnet ef database update
```

## Step 2: Run the Application

Start the application:

```bash
cd /home/chicuong/Desktop/code/SmartQuiz
dotnet run --project SmartQuiz/SmartQuiz.csproj
```

Or if using VS Code/JetBrains Rider, just press F5 to run.

## Step 3: Test the Quiz Results Page

### Option A: Using the Demo Page

1. Open your browser and navigate to: `http://localhost:5000` (or your configured port)
2. Click on **"Quiz Demo"** in the header navigation
3. Click the **"Create Sample Quiz Result"** button
4. Click **"View Results"** to see the Quiz Results Summary page

### Option B: Direct URL (if you have a quiz result ID)

Navigate directly to:
```
http://localhost:5000/quiz-results/{your-quiz-result-id}
```

## What to Expect

You should see a page with:

### ✅ Header Section
- **PASSED** badge (green) or **FAILED** badge (red)
- **"Quiz Complete!"** title
- Quiz description with results summary

### ✅ Score Display
- Large circular progress indicator showing **85%**
- "SCORE" label below the percentage

### ✅ Statistics Cards (3 columns)
1. **Time Taken**: 04:12
2. **Points Earned**: +340 XP
3. **Global Rank**: Top 10%

### ✅ Performance by Topic
- World History: 100% (green bar)
- Modern Science: 80% (blue bar)
- Geography: 60% (orange bar)

### ✅ Answer Key Grid
- 20 question boxes (numbered 1-20)
- Green background = correct answer
- Red background = incorrect answer
- Questions 4, 14, and 17 are incorrect (red)
- All others are correct (green)

### ✅ Action Buttons
- **Review Answers** (outlined button)
- **Retake Quiz** (filled blue button)
- **Return to Dashboard** (text link)

## Sample Data Created

The demo creates a quiz result with:
- **Quiz Title**: "General Knowledge Assessment"
- **Total Questions**: 20
- **Correct Answers**: 17
- **Score**: 85%
- **Time**: 4 minutes 12 seconds
- **Points**: 340 XP
- **Rank**: Top 10%

## Troubleshooting

### Database Error
If you see a database error, make sure:
1. PostgreSQL is running
2. Connection string in `appsettings.json` is correct
3. Run `dotnet ef database update` to apply migrations

### Service Not Registered Error
If you see "service not registered" error:
1. Check that `QuizResultService` is registered in `ServicesExtensions.cs`
2. Check that `IQuizResultService` is registered in client `Program.cs`

### Page Not Found
If the quiz-demo or quiz-results page shows 404:
1. Clear browser cache
2. Rebuild the solution: `dotnet build`
3. Restart the application

## Next Steps

### Integrate with Your Quiz System

When a user completes a quiz, create a result like this:

```csharp
@inject ICommander Commander
@inject NavigationManager NavigationManager

private async Task OnQuizComplete()
{
    // Prepare topic performances
    var topicPerformances = CalculateTopicPerformances();
    
    // Prepare answers
    var answers = quiz.Questions.Select((q, index) => new QuestionAnswerDto
    {
        QuestionNumber = index + 1,
        IsCorrect = q.UserAnswer == q.CorrectAnswer,
        Question = q.Text,
        CorrectAnswer = q.CorrectAnswer,
        UserAnswer = q.UserAnswer
    }).ToList();

    // Create command
    var command = new CreateQuizResultCommand(
        UserId: currentUserId,
        FlashcardSetId: flashcardSetId,
        QuizTitle: quizTitle,
        TotalQuestions: quiz.Questions.Count,
        CorrectAnswers: answers.Count(a => a.IsCorrect),
        TimeTaken: quiz.ElapsedTime,
        PointsEarned: CalculatePoints(correctAnswers),
        GlobalRankPercentile: await CalculateRankAsync(scorePercentage),
        TopicPerformances: topicPerformances,
        Answers: answers
    );

    // Execute command
    var result = await Commander.Call(command);

    // Navigate to results
    NavigationManager.NavigateTo($"/quiz-results/{result.Id}");
}
```

## API Endpoints

The service exposes these methods:

### Read Operations (Cached)
- `GetQuizResultByIdAsync(Guid id)` - Get single result
- `GetUserQuizResultsAsync(Guid userId)` - Get all results for a user

### Write Operations
- `CreateQuizResultAsync(CreateQuizResultCommand command)` - Create new result

## Files Created

### Backend
- `SmartQuiz/Data/Models/QuizResult.cs`
- `SmartQuiz/Data/Configurations/QuizResultConfiguration.cs`
- `SmartQuiz/Application/QuizResultService.cs`
- `SmartQuiz/Data/Migrations/20260104000000_AddQuizResultTables.cs`

### Client
- `SmartQuiz.Client/Data/Dtos/QuizResultDto.cs`
- `SmartQuiz.Client/Data/Commands/CreateQuizResultCommand.cs`
- `SmartQuiz.Client/Data/Services/IQuizResultService.cs`
- `SmartQuiz.Client/Pages/QuizResultsPage.razor`
- `SmartQuiz.Client/Pages/QuizResultState.cs`
- `SmartQuiz.Client/Pages/QuizDemoPage.razor`

## Support

If you encounter any issues, refer to:
- `/QUIZ_RESULTS_README.md` - Complete implementation documentation
- Check console logs for errors
- Verify database connection
- Ensure all services are registered

Enjoy your new Quiz Results Summary feature! 🎉

