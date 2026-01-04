using System.Collections.Immutable;
using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Pages;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record UserProfileState
{
    [DataMember, MemoryPackOrder(0)]
    public ImmutableList<FlashcardSetDto> UserSets { get; init; } = ImmutableList<FlashcardSetDto>.Empty;
}