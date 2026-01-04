using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record CreateQuizResultCommand(
    [property: DataMember, MemoryPackOrder(0)]
    Guid UserId,
    [property: DataMember, MemoryPackOrder(1)]
    Guid FlashcardSetId,
    [property: DataMember, MemoryPackOrder(2)]
    string QuizTitle,
    [property: DataMember, MemoryPackOrder(3)]
    int TotalQuestions,
    [property: DataMember, MemoryPackOrder(4)]
    int CorrectAnswers,
    [property: DataMember, MemoryPackOrder(5)]
    TimeSpan TimeTaken,
    [property: DataMember, MemoryPackOrder(6)]
    int PointsEarned,
    [property: DataMember, MemoryPackOrder(7)]
    int GlobalRankPercentile,
    [property: DataMember, MemoryPackOrder(8)]
    List<TopicPerformanceDto> TopicPerformances,
    [property: DataMember, MemoryPackOrder(9)]
    List<QuestionAnswerDto> Answers
) : ICommand<QuizResultDto>;