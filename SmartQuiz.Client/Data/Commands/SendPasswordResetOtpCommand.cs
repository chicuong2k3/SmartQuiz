using System.Reactive;
using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record SendPasswordResetOtpCommand(
    [property: DataMember, MemoryPackOrder(0)]
    string Email
) : ICommand<Unit>;