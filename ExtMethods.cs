namespace Featherline;

public static class ExtMethods
{
    public static IEnumerable<(T value, int count)> GroupConsecutive<T>(this IEnumerable<T> values)
    {
        var result = Enumerable.Empty<(T, int)>();
        T current = values.First();
        int i = 1;
        int groupStart = 0;
        foreach (T elt in values.Skip(1)) {
            if (!elt.Equals(current)) {
                result = result.Append((current, i - groupStart));
                current = elt;
                groupStart = i;
            }
            i++;
        }
        return result.Append((current, i - groupStart));
    }

    public static int Square(this int n) => n * n;
    public static float Square(this float n) => n * n;
}
