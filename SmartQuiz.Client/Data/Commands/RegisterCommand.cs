using System.Runtime.Serialization;
using ActualLab.Fusion.Authentication;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record RegisterCommand(
    [property: DataMember, MemoryPackOrder(0)]
    string Email,
    [property: DataMember, MemoryPackOrder(1)]
    string Password,
    [property: DataMember, MemoryPackOrder(2)]
    string FullName
) : ICommand<User>;