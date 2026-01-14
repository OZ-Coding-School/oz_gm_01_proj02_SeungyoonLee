using System;
using System.Collections.Generic;

namespace LunarCube
{
    // TODO: make static comparer
    public class NameComparer<T> : IEqualityComparer<T> where T : class, INamed
    {
        public bool Equals(T left, T right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (null == left || null == right) return false;
            return string.Equals(left.Name, right.Name, StringComparison.Ordinal);
        }

        public int GetHashCode(T t)
        {
            return t?.Name?.GetHashCode() ?? 0;
        }
    }
}
