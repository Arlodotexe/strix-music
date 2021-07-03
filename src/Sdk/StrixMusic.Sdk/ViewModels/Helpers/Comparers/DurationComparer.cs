using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the duration/>.
    /// </summary>
    public class DurationComparer<TPlayableBase> : Comparer<TPlayableBase> where TPlayableBase : IPlayableBase
    {
        /// <inheritdoc/>
        public override int Compare(TPlayableBase x, TPlayableBase y) => TimeSpan.Compare(x.Duration, y.Duration);
    }
}
