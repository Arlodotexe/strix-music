using System;
using System.Collections.Generic;
using System.Text;

namespace MemberProxySourceGenerator
{
    [Flags]
    public enum MemberTypes
    {
        /// <summary>
        /// No member proxies will be generated.
        /// </summary>
        None = 0,

        /// <summary>
        /// Property proxies will be generated.
        /// </summary>
        Properties = 1,

        /// <summary>
        /// Method proxies will be generated.
        /// </summary>
        Methods = 2,

        /// <summary>
        /// Event proxies will be generated, along with the methods AttachEvents and DetachEvents.
        /// </summary>
        Events = 4,

        /// <summary>
        /// All possible proxies will be generated.
        /// </summary>
        All = Properties | Methods | Events,
    }
}
