using OwlCore.Remoting.Transfer;

namespace OwlCore.Remoting
{
    /// <summary>
    /// Indicates the member signature used when generating outgoing and processing incoming <see cref="IRemoteMemberMessage"/>s.
    /// <para/>
    /// Allows for sending/receiving member changes in different assemblies, namespaces, or class names.
    /// In addition to the options specified here, the <see cref="MemberRemote.Id"/> must still match for messages to be received on each instance.
    /// <para/>
    /// If you're unsure which option to use, stick with <see cref="MemberSignatureScope.AssemblyQualifiedName"/>.
    /// </summary>
    public enum MemberSignatureScope
    {
        /// <summary>
        /// Member signatures will match the assembly, namespace, containing type and member name.
        /// </summary>
        AssemblyQualifiedName,

        /// <summary>
        /// Member signatures will match the namespace, containing type and member name.
        /// </summary>
        FullName,

        /// <summary>
        /// Member signatures will match the containing type and member name.
        /// </summary>
        DeclaringType,

        /// <summary>
        /// Member signatures will match only the member name.
        /// </summary>
        MemberName,
    }
}
