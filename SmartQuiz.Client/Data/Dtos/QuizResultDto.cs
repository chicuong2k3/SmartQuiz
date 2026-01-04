using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Dtos;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class QuizResultDto
{
    [DataMember, MemoryPackOrder(0)] public Guid Id { get; init; }
    [DataMember, MemoryPackOrder(1)] public Guid UserId { get; init; }
    [DataMember, MemoryPackOrder(2)] public Guid FlashcardSetId { get; init; }
    [DataMember, MemoryPackOrder(3)] public string QuizTitle { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(4)] public int TotalQuestions { get; init; }
    [DataMember, MemoryPackOrder(5)] public int CorrectAnswers { get; init; }
    [DataMember, MemoryPackOrder(6)] public int ScorePercentage { get; init; }
    [DataMember, MemoryPackOrder(7)] public TimeSpan TimeTaken { get; init; }
    [DataMember, MemoryPackOrder(8)] public int PointsEarned { get; init; }
    [DataMember, MemoryPackOrder(9)] public int GlobalRankPercentile { get; init; }
    [DataMember, MemoryPackOrder(10)] public bool IsPassed { get; init; }
    [DataMember, MemoryPackOrder(11)] public List<TopicPerformanceDto> TopicPerformances { get; init; } = new();
    [DataMember, MemoryPackOrder(12)] public List<QuestionAnswerDto> Answers { get; init; } = new();
    [DataMember, MemoryPackOrder(13)] public DateTime CompletedAt { get; init; }
    [DataMember, MemoryPackOrder(14)] public DateTime CreatedAt { get; init; }
}

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class TopicPerformanceDto
{
    [DataMember, MemoryPackOrder(0)] public string TopicName { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(1)] public int CorrectCount { get; init; }
    [DataMember, MemoryPackOrder(2)] public int TotalCount { get; init; }
    [DataMember, MemoryPackOrder(3)] public int Percentage { get; init; }
}

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class QuestionAnswerDto
{
    [DataMember, MemoryPackOrder(0)] public int QuestionNumber { get; init; }
    [DataMember, MemoryPackOrder(1)] public bool IsCorrect { get; init; }
    [DataMember, MemoryPackOrder(2)] public string Question { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(3)] public string CorrectAnswer { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(4)] public string UserAnswer { get; init; } = string.Empty;
}