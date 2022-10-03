using System.Collections.Generic;
using System.Linq;

namespace MathCore.GPX.Infrastructure;

internal static class EnumerableEx
{
    public static IEnumerable<T> SelectAll<T>(this IEnumerable<IEnumerable<T>> sources) => sources.SelectMany(s => s);
}
