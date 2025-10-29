namespace TheCodeKitchen.Application.Business.Extensions;

public static class LinqExtensions
{
    public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> func)
    {
        return new TimeSpan(source.Sum(item => func(item).Ticks));
    }

    public static IEnumerable<TSource> MultiExcept<TSource>(
        this IEnumerable<TSource> source,
        IEnumerable<TSource> toRemove,
        IEqualityComparer<TSource>? comparer = null) where TSource : notnull
    {
        comparer ??= EqualityComparer<TSource>.Default;

        var toRemoveCounts = toRemove
            .GroupBy(x => x, comparer)
            .ToDictionary(g => g.Key, g => g.Count(), comparer);

        foreach (var item in source)
        {
            if (toRemoveCounts.TryGetValue(item, out var count) && count > 0)
            {
                toRemoveCounts[item] = count - 1;
            }
            else
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<TSource> MultiIntersect<TSource>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        IEqualityComparer<TSource>? comparer = null) where TSource : notnull
    {
        comparer ??= EqualityComparer<TSource>.Default;

        var secondCounts = second
            .GroupBy(x => x, comparer)
            .ToDictionary(g => g.Key, g => g.Count(), comparer);

        foreach (var item in first)
        {
            if (secondCounts.TryGetValue(item, out var count) && count > 0)
            {
                secondCounts[item] = count - 1;
                yield return item;
            }
        }
    }
}