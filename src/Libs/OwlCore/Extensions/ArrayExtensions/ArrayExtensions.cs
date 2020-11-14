using System;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Array"/>s.
    /// </summary>
    public static partial class ArrayExtensions
    {
        /// <summary>
        /// Traverses the given <see cref="Array"/> using an Action that returns the array and the indices.
        /// </summary>
        /// <param name="array">The array to traverse.</param>
        /// <param name="action"></param>
        public static void Traverse(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }
    }

    internal class ArrayTraverse
    {
        public int[] Position;
        private readonly int[] maxLengths;

        public ArrayTraverse(Array array)
        {
            maxLengths = new int[array.Rank];

            for (var i = 0; i < array.Rank; ++i)
            {
                maxLengths[i] = array.GetLength(i) - 1;
            }

            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (var i = 0; i < Position.Length; ++i)
            {
                if (Position[i] >= maxLengths[i]) continue;

                Position[i]++;

                for (var j = 0; j < i; j++)
                {
                    Position[j] = 0;
                }
                return true;
            }
            return false;
        }
    }
}