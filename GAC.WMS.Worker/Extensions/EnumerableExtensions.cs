using System.Diagnostics.CodeAnalysis;

namespace GAC.WMS.Worker.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class EnumerableExtensions
    {
        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            List<T> batch = new List<T>(batchSize);

            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == batchSize)
                {
                    yield return new List<T>(batch);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                yield return batch;
            }
        }
    }
}
