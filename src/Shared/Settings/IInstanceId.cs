namespace StrixMusic.Services;

/// <summary>
/// Uniquely identifies an instance.
/// </summary>
public interface IInstanceId
{
    /// <summary>
    /// Uniquely identifies this instance.
    /// </summary>
    public string InstanceId { get; }
}
