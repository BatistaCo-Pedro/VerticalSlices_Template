namespace Common.Domain.Events;

/// <summary>
/// The type of event
/// </summary>
public enum EventType
{
    Domain = 1,
    Command = 2,
    Query = 3,
    External = 4
}
