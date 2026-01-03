using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record UpdateFlashcardCommand(
    [property: DataMember, MemoryPackOrder(0)]
    Guid Id,
    [property: DataMember, MemoryPackOrder(1)]
    string FrontText,
    [property: DataMember, MemoryPackOrder(2)]
    string BackText,
    [property: DataMember, MemoryPackOrder(3)]
    string? FrontImageUrl,
    [property: DataMember, MemoryPackOrder(4)]
    string? BackImageUrl,
    [property: DataMember, MemoryPackOrder(5)]
    string? AudioUrl,
    [property: DataMember, MemoryPackOrder(6)]
    string? CodeSnippet,
    [property: DataMember, MemoryPackOrder(7)]
    int? DifficultyLevel)
    : ICommand<FlashcardDto>;