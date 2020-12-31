using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OwlCore.Helpers
{
    /// <summary>
    /// Helper methods for API related tasks.
    /// </summary>
    public partial class APIs
    {
        /// <summary>
        /// Loads a list of items of a given type from the given endpoints
        /// </summary>
        /// <param name="total">The total number of items to get.</param>
        /// <param name="endpoint">A <see cref="Func{T,Result}"/> used to load paginated results</param>
        /// <remarks>
        /// Thanks to Sergio Pedri for the original version of this.
        /// https://discordapp.com/channels/372137812037730304/521423927613063190/570575449483378688
        /// </remarks>
        public static async Task<IReadOnlyList<TResult>> GetAllItemsAsync<TResult>(int total, Func<int, Task<IEnumerable<TResult>>> endpoint)
        {
            // Get the items from the first page
            var list = new List<TResult>();
            var page = await endpoint(0);

            list.AddRange(page);

            // Get the remaining items
            while (true)
            {
                page = await endpoint(list.Count);

                if (page is null)
                    return list;

                list.AddRange(page);

                if (list.Count >= total)
                    break;
            }

            return list;
        }

        /// <summary>
        /// Loads a list of items of a given type from the given endpoints
        /// </summary>
        /// <param name="total">The total number of items to get.</param>
        /// <param name="startingOffset">The offset to start at.</param>
        /// <param name="endpoint">A <see cref="Func{T,Result}"/> used to load paginated results</param>
        /// <remarks>
        /// Thanks to Sergio Pedri for the original version of this.
        /// https://discordapp.com/channels/372137812037730304/521423927613063190/570575449483378688
        /// </remarks>
        public static async Task<IReadOnlyList<TResult>> GetAllItemsAsync<TResult>(int total, int startingOffset, Func<int, Task<IEnumerable<TResult>>> endpoint)
        {
            // Get the items from the first page
            var list = new List<TResult>();
            var page = await endpoint(startingOffset);

            list.AddRange(page);

            if (list.Count >= total)
                return list;

            // Get the remaining items
            while (true)
            {
                page = await endpoint(list.Count + startingOffset);

                if (page is null)
                    return list;

                list.AddRange(page);

                if (list.Count >= total)
                    break;
            }

            return list;
        }

        /// <summary>
        /// Loads a list of items of a given type from the given endpoints
        /// </summary>
        /// <param name="total">The total number of items to get.</param>
        /// <param name="firstPageItems">An existing list of items from the first page.</param>
        /// <param name="endpoint">A <see cref="Func{T,Result}"/> used to load paginated results</param>
        /// <remarks>
        /// Thanks to Sergio Pedri for the original version of this.
        /// https://discordapp.com/channels/372137812037730304/521423927613063190/570575449483378688
        /// </remarks>
        public static async Task<IReadOnlyList<TResult>> GetAllItemsAsync<TResult>(int total, List<TResult> firstPageItems, Func<int, Task<IEnumerable<TResult>?>> endpoint)
        {
            // The items from the first page are already supplied.
            var list = firstPageItems;

            // Get the remaining items
            while (true)
            {
                var page = await endpoint(list.Count);

                if (page is null)
                    return list;

                list.AddRange(page);

                if (list.Count >= total)
                    break;
            }

            return list;
        }
    }
}
