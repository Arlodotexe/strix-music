using System.Threading.Tasks;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for anything that is playable. Multiple view models implement <see cref="IPlayable"/> and this interface allows us to 
    /// </summary>
    public interface IPlayableViewModel : ISdkViewModel, IPlayable
    {
    }
}
