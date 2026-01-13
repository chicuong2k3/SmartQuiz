using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record CreateFlashcardSetCommand(
    [property: DataMember, MemoryPackOrder(0)]
    Session Session,
    [property: DataMember, MemoryPackOrder(1)]
    string Title,
    [property: DataMember, MemoryPackOrder(2)]
    string? Description,
    [property: DataMember, MemoryPackOrder(3)]
    bool IsPublic,
    [property: DataMember, MemoryPackOrder(4)]
    List<FlashcardItemCommand> Flashcards
) : ISessionCommand<FlashcardSetDto>;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class FlashcardItemCommand
{
    [DataMember, MemoryPackOrder(0)] public string Term { get; set; } = string.Empty;

    [DataMember, MemoryPackOrder(1)] public string Definition { get; set; } = string.Empty;
}