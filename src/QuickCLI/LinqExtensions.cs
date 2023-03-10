
internal static class LinqExtensions
{
    public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> f)
    {
        var enumerator = collection.GetEnumerator();
        int i = 0;
        while (enumerator.MoveNext())
        {
            if (f(enumerator.Current))
                return i;

            i++;
        }
        return -1;
    }
}