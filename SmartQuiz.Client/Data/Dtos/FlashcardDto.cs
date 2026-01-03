using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Dtos;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class FlashcardDto
{
    [DataMember, MemoryPackOrder(0)] public Guid Id { get; init; }
    [DataMember, MemoryPackOrder(1)] public Guid StudySetId { get; init; }
    [DataMember, MemoryPackOrder(2)] public string FrontText { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(3)] public string BackText { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(4)] public string? ImageUrl { get; init; }
    [DataMember, MemoryPackOrder(6)] public string? AudioUrl { get; init; }
    [DataMember, MemoryPackOrder(9)] public DateTime CreatedAt { get; init; }
}