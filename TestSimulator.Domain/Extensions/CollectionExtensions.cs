namespace TestSimulator.Domain.Extensions;

public static class CollectionExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        var random = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
}