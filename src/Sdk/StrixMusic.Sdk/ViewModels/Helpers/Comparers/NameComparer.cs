using System.Collections.Generic;
using System.Globalization;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// A class that can compare names.
    /// </summary>
    public class NameComparer<TPlayableBase> : Comparer<TPlayableBase> where TPlayableBase : IPlayableBase
    {
        /// <inheritdoc/>
        public override int Compare(TPlayableBase x, TPlayableBase y) => string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);
    }
}