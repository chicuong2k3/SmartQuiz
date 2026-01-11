using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record ResetPasswordCommand(
    [property: DataMember, MemoryPackOrder(0)]
    string Email,
    [property: DataMember, MemoryPackOrder(1)]
    string Otp,
    [property: DataMember, MemoryPackOrder(2)]
    string NewPassword
) : ICommand<bool>;