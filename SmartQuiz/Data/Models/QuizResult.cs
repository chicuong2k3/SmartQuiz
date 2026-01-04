namespace SmartQuiz.Data.Models;

public class QuizResult : Entity
{
    private QuizResult()
    {
    }

    public QuizResult(
        Guid userId,
        Guid flashcardSetId,
        string quizTitle,
        int totalQuestions,
        int correctAnswers,
        TimeSpan timeTaken,
        int pointsEarned,
        int globalRankPercentile)
    {
        UserId = userId;
        FlashcardSetId = flashcardSetId;
        QuizTitle = quizTitle;
        TotalQuestions = totalQuestions;
        CorrectAnswers = correctAnswers;
        ScorePercentage = totalQuestions > 0 ? (int)Math.Round((double)correctAnswers / totalQuestions * 100) : 0;
        TimeTaken = timeTaken;
        PointsEarned = pointsEarned;
        GlobalRankPercentile = globalRankPercentile;
        IsPassed = ScorePercentage >= 60;
        CompletedAt = DateTime.UtcNow;
    }

    public Guid UserId { get; set; }
    public Guid FlashcardSetId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int ScorePercentage { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public int PointsEarned { get; set; }
    public int GlobalRankPercentile { get; set; }
    public bool IsPassed { get; set; }
    public DateTime CompletedAt { get; set; }

    // Navigation properties
    public FlashcardSet? FlashcardSet { get; set; }
    public ICollection<TopicPerformance> TopicPerformances { get; set; } = new List<TopicPerformance>();
    public ICollection<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
}

public class TopicPerformance : Entity
{
    private TopicPerformance()
    {
    }

    public TopicPerformance(Guid quizResultId, string topicName, int correctCount, int totalCount)
    {
        QuizResultId = quizResultId;
        TopicName = topicName;
        CorrectCount = correctCount;
        TotalCount = totalCount;
        Percentage = totalCount > 0 ? (int)Math.Round((double)correctCount / totalCount * 100) : 0;
    }

    public Guid QuizResultId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int CorrectCount { get; set; }
    public int TotalCount { get; set; }
    public int Percentage { get; set; }

    // Navigation property
    public QuizResult? QuizResult { get; set; }
}

public class QuestionAnswer : Entity
{
    private QuestionAnswer()
    {
    }

    public QuestionAnswer(
        Guid quizResultId,
        int questionNumber,
        bool isCorrect,
        string question,
        string correctAnswer,
        string userAnswer)
    {
        QuizResultId = quizResultId;
        QuestionNumber = questionNumber;
        IsCorrect = isCorrect;
        Question = question;
        CorrectAnswer = correctAnswer;
        UserAnswer = userAnswer;
    }

    public Guid QuizResultId { get; set; }
    public int QuestionNumber { get; set; }
    public bool IsCorrect { get; set; }
    public string Question { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string UserAnswer { get; set; } = string.Empty;

    // Navigation property
    public QuizResult? QuizResult { get; set; }
}