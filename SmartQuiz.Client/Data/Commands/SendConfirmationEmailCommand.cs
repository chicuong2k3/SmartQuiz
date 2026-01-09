using System.Reactive;
using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record SendConfirmationEmailCommand(
    [property: DataMember, MemoryPackOrder(0)]
    string Email,
    [property: DataMember, MemoryPackOrder(1)]
    string FullName
) : ICommand<Unit>;