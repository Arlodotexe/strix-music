using System;
using System.Collections.Generic;
using System.Text;

namespace OwlCore.Remoting.Attributes
{
    /// <summary>
    /// Attribute used in conjunction with <see cref="RemoteViewModel"/>. 
    /// Mark a method with this to have calls synchronized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RemoteMethodAttribute : Attribute
    {
    }
}
