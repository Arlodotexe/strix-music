﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the name in reverse order.
    /// </summary>
    public class ReverseNameComparer<TTrack> : Comparer<TTrack> where TTrack : ITrack
    {
        /// <inheritdoc/>
        public override int Compare(TTrack x, TTrack y)
        {
            var result = string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);

            return result == 1 ? result *= -1 : result;
        }
    }
}
