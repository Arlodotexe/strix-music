using StrixMusic.Sdk.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IPlayableBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IPlayable : IPlayableBase, ISdkMember
    {
    } 
}
