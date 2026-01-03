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
}