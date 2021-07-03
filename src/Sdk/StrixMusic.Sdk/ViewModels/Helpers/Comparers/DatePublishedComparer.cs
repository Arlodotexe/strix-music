using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the DatePublished />.
    /// </summary>
    /// <typeparam name="TAlbum">The <inheritdoc cref="IAlbumBase"/> to sort.</typeparam>
    public class DatePublishedComparer<TAlbum> : Comparer<TAlbum> where TAlbum : IAlbumBase
    {
        /// <inheritdoc/>
        public override int Compare(TAlbum x, TAlbum y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>. It also compares the values of the dataType provided and returns greater,less or equal relation.
            return Nullable.Compare(x.DatePublished, y.DatePublished);
        }
    }
}
