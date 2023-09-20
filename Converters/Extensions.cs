namespace Kendoku.Converters;

static class Extensions
{
    public static IEnumerable<string> PrefixWith(this IEnumerable<string> source,
                                                 string prefix)
    {
        foreach (var item in source)
        {
            yield return prefix;
            yield return item;
        }
    }
}
