using System.Reactive;
using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class DeleteFlashcardSetCommand : ICommand<Unit>
{
    [DataMember, MemoryPackOrder(0)] public Guid Id { get; set; }
}