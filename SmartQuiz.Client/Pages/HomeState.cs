using System.Collections.Immutable;
using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Pages;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record HomeState
{
    [DataMember, MemoryPackOrder(0)]
    public ImmutableList<FlashcardSetDto> RecentSets { get; init; } = ImmutableList<FlashcardSetDto>.Empty;

    [DataMember, MemoryPackOrder(1)]
    public ImmutableList<FlashcardSetDto> RecommendedSets { get; init; } = ImmutableList<FlashcardSetDto>.Empty;
}