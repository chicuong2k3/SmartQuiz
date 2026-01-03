using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class UpdateFlashcardSetCommand : ICommand<FlashcardSetDto>
{
    [DataMember, MemoryPackOrder(0)] public Guid Id { get; set; }

    [DataMember, MemoryPackOrder(1)] public string Title { get; set; } = string.Empty;

    [DataMember, MemoryPackOrder(2)] public string? Description { get; set; }
}