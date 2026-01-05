using System.Runtime.Serialization;
using ActualLab.Fusion.Authentication;
using MemoryPack;
using MessagePack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant), MessagePackObject(true)]
public partial record SignInCommand(
    [property: DataMember, MemoryPackOrder(0)]
    Session Session,
    [property: DataMember, MemoryPackOrder(1)]
    string Email,
    [property: DataMember, MemoryPackOrder(2)]
    string Password
) : ISessionCommand<User?>;