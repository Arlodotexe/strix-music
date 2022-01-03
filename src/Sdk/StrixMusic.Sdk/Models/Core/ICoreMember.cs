namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Properties required by all core-based interfaces that live in an <see cref="ICore"/>.
    /// </summary>
    public interface ICoreMember
    {
        /// <summary>
        /// The <see cref="ICore"/> instance which created this object.
        /// </summary>
        /// <remarks>
        /// This is required to be present on all classes implemented by a core, from the SDK.
        /// </remarks>
        public ICore SourceCore { get; }
    }
}