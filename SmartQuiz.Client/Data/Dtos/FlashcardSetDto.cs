using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Dtos;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class FlashcardSetDto
{
    [DataMember, MemoryPackOrder(0)] public Guid Id { get; init; }
    [DataMember, MemoryPackOrder(1)] public string Title { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(2)] public string? Description { get; init; }
    [DataMember, MemoryPackOrder(3)] public DateTime CreatedAt { get; init; }
    [DataMember, MemoryPackOrder(4)] public int FlashcardCount { get; init; }
    [DataMember, MemoryPackOrder(5)] public bool IsPublic { get; init; } = true;
    [DataMember, MemoryPackOrder(6)] public List<FlashcardDto> Flashcards { get; init; } = [];
    [DataMember, MemoryPackOrder(7)] public string UserId { get; init; } = string.Empty;
}