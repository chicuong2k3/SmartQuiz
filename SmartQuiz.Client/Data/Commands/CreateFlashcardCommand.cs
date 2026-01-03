using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record CreateFlashcardCommand(
    [property: DataMember, MemoryPackOrder(0)]
    Guid StudySetId,
    [property: DataMember, MemoryPackOrder(1)]
    string FrontText,
    [property: DataMember, MemoryPackOrder(2)]
    string BackText,
    [property: DataMember, MemoryPackOrder(3)]
    string? ImageUrl,
    [property: DataMember, MemoryPackOrder(4)]
    string? AudioUrl)
    : ICommand<FlashcardDto>;