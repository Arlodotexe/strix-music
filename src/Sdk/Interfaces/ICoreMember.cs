namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Properties required by all core-based interfaces that live in an <see cref="ICore"/>.
    /// </summary>
    public interface ICoreMember
    {
        /// <summary>
        /// The source core which created the parent.
        /// </summary>
        public ICore SourceCore { get; }
    }
}