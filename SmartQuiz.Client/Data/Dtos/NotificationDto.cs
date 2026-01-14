using System.Runtime.Serialization;
using MemoryPack;

namespace SmartQuiz.Client.Data.Dtos;

[DataContract, MemoryPackable(GenerateType.VersionTolerant)]
public partial class NotificationDto
{
    [DataMember, MemoryPackOrder(0)] public Guid Id { get; init; }
    [DataMember, MemoryPackOrder(1)] public string Title { get; init; } = string.Empty;
    [DataMember, MemoryPackOrder(2)] public string? Message { get; init; }
    [DataMember, MemoryPackOrder(3)] public NotificationType Type { get; init; }
    [DataMember, MemoryPackOrder(4)] public bool IsRead { get; init; }
    [DataMember, MemoryPackOrder(5)] public DateTime CreatedAt { get; init; }
}

public enum NotificationType
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
}