using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlCore
{
    /// <summary>
    /// Helper methods for API related tasks.
    /// </summary>
    public partial class APIs
    {
        /// <summary>
        /// Delegates returning items from an endpoint, given an offset and limit.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="offset">The position to start at when getting items.</param>
        public delegate Task<IEnumerable<TResult>> GetEndpointItemsHandler<TResult>(int offset);

        /// <summary>
        /// Loads a list of items of a given type from the given endpoints
        /// </summary>
        /// <param name="total">The total number of items to get.</param>
        /// <param name="endpoint">A <see cref="GetEndpointItemsHandler{TResult}"/> used to load paginated results</param>
        public static async Task<IReadOnlyList<TResult>> GetAllItemsAsync<TResult>(int total, GetEndpointItemsHandler<TResult> endpoint)
        {
            // Get the items from the first page
            var list = new List<TResult>();

            // Get the remaining items
            while (list.Count < total)
            {
                var page = await endpoint(list.Count);
                // ReSharper disable once ConstantConditionalAccessQualifier
                var results = page as TResult[] ?? page?.ToArray();

                if (results == null || !results.Any())
                    return list;

                list.AddRange(results);
            }

            return list;
        }

        /// <summary>
        /// Loads a list of items of a given type from the given endpoints
        /// </summary>
        /// <param name="total">The total number of items to get.</param>
        /// <param name="startingOffset">The offset to start at.</param>
        /// <param name="endpoint">A <see cref="Func{T,Result}"/> used to load paginated results</param>
        public static async Task<IReadOnlyList<TResult>> GetAllItemsAsync<TResult>(int total, int startingOffset, GetEndpointItemsHandler<TResult> endpoint)
        {
            // Get the items from the first page
            var list = new List<TResult>();

            // Get the remaining items
            while (list.Count < total)
            {
                var page = await endpoint(list.Count + startingOffset);
                // ReSharper disable once ConstantConditionalAccessQualifier
                var results = page as TResult[] ?? page?.ToArray();

                if (results == null || !results.Any())
                    return list;

                list.AddRange(results);
            }

            return list;
        }

        /// <summary>
        /// Loads a list of items of a given type from the given endpoints
        /// </summary>
        /// <param name="total">The total number of items to get.</param>
        /// <param name="firstPageItems">An existing list of items from the first page.</param>
        /// <param name="endpoint">A <see cref="Func{T,Result}"/> used to load paginated results</param>
        public static async Task<IReadOnlyList<TResult>> GetAllItemsAsync<TResult>(int total, List<TResult> firstPageItems, GetEndpointItemsHandler<TResult> endpoint)
        {
            // The items from the first page are already supplied.
            var list = firstPageItems;

            // Get the remaining items
            while (true)
            {
                var page = await endpoint(list.Count);
                // ReSharper disable once ConstantConditionalAccessQualifier
                var results = page as TResult[] ?? page?.ToArray();

                if (results == null || !results.Any())
                    return list;

                list.AddRange(results);

                if (list.Count >= total)
                    break;
            }

            return list;
        }
    }
}
