using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Commands;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record MarkNotificationReadCommand(
    [property: DataMember, MemoryPackOrder(0)]
    Session Session,
    [property: DataMember, MemoryPackOrder(1)]
    Guid NotificationId
) : ICommand;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial record MarkAllNotificationsReadCommand(
    [property: DataMember, MemoryPackOrder(0)]
    Session Session
) : ICommand;