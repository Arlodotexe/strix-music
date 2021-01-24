using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    public static partial class AsyncExtensions
    {
        /// <summary>
        /// Zips together the enumeration of multiple <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="sources">The sources to zip together.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
        public static async IAsyncEnumerable<TSource[]> Zip<TSource>(this IEnumerable<IAsyncEnumerable<TSource>> sources, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerators = sources.Select(x => x.GetAsyncEnumerator(cancellationToken)).ToArray();
            try
            {
                while (true)
                {
                    var array = new TSource[enumerators.Length];
                    for (var i = 0; i < enumerators.Length; i++)
                    {
                        if (!await enumerators[i].MoveNextAsync()) yield break;
                        array[i] = enumerators[i].Current;
                    }
                    yield return array;
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                {
                    await enumerator.DisposeAsync();
                }
            }
        }
    }
}