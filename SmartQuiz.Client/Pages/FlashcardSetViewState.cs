using System.Collections.Immutable;
using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Pages;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record FlashcardSetViewState
{
    [DataMember, MemoryPackOrder(0)] public FlashcardSetDto? FlashcardSet { get; init; }

    [DataMember, MemoryPackOrder(1)]
    public ImmutableList<FlashcardDto> Flashcards { get; init; } = ImmutableList<FlashcardDto>.Empty;
}