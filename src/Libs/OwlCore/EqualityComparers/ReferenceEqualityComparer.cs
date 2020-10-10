using System.Collections.Generic;

namespace OwlCore.EqualityComparers
{
    /// <summary>
    /// Compares references instead of values for <see cref="Equals"/>.
    /// </summary>
    public class ReferenceEqualityComparer : EqualityComparer<object>
    {
        /// <inheritdoc />
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        /// <inheritdoc />
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
}