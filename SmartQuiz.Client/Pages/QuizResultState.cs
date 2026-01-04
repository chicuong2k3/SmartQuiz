using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Pages;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public sealed partial record QuizResultState
{
    [DataMember, MemoryPackOrder(0)] public QuizResultDto? QuizResult { get; init; }

    public static async Task<QuizResultState> LoadAsync(
        IQuizResultService quizResultService,
        Guid quizResultId,
        CancellationToken cancellationToken = default)
    {
        var quizResult = await quizResultService.GetQuizResultByIdAsync(quizResultId, cancellationToken);
        return new QuizResultState { QuizResult = quizResult };
    }
}