using System.Collections.Generic;
using System.Linq;

public static class SequenceExtensions
{
    public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
        return Enumerable.SequenceEqual(first, second);
    }
}