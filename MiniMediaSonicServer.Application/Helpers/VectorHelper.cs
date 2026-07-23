namespace MiniMediaSonicServer.Application.Helpers;

public class VectorEntry<T>
{
    public required T Item;
    public required float[] Vector;
    public required float SquaredNorm;
}

public class VectorIndex<T>
{
    private readonly List<VectorEntry<T>> _entries;

    public VectorIndex(List<(T Item, float[] Vector)> data)
    {
        _entries = data.Select(d => new VectorEntry<T>
        {
            Item = d.Item,
            Vector = d.Vector,
            SquaredNorm = DotProduct(d.Vector, d.Vector)
        }).ToList();
    }

    public static float DotProduct(float[] a, float[] b)
    {
        float sum = 0f;
        for (int i = 0; i < a.Length; i++)
            sum += a[i] * b[i];
        return sum;
    }

    public List<(T Item, float SquaredDistance)> KNearest(float[] query, int count)
    {
        float queryNormSq = DotProduct(query, query);

        var pq = new PriorityQueue<(T Item, float DistSq), float>();

        foreach (var entry in _entries)
        {
            float dot = DotProduct(query, entry.Vector);
            float squaredDistance = queryNormSq + entry.SquaredNorm - 2f * dot;

            if (pq.Count < count)
            {
                pq.Enqueue((entry.Item, squaredDistance), -squaredDistance);
            }
            else
            {
                pq.TryPeek(out _, out float worstNegPriority);
                if (squaredDistance < -worstNegPriority)
                {
                    pq.Dequeue();
                    pq.Enqueue((entry.Item, squaredDistance), -squaredDistance);
                }
            }
        }

        var results = new List<(T Item, float DistSq)>(pq.Count);
        while (pq.Count > 0)
            results.Add(pq.Dequeue());

        results.Sort((a, b) => a.DistSq.CompareTo(b.DistSq));
        return results;
    }

    public List<(T Item, float SquaredDistance)> KNearestAnchor(
        float[] startVector,
        int totalCount,
        int anchorCount,
        int anchorInterval)
    {
        var picked = new List<(T Item, float SquaredDistance)>();
        var used = new HashSet<T>();
        float[] anchorVector = startVector;

        for (int i = 0; i < totalCount; i++)
        {
            var candidates = KNearest(anchorVector, anchorCount + used.Count);

            (T Item, float SquaredDistance)? next = candidates.FirstOrDefault(c => !used.Contains(c.Item));

            if (next == null)
            {
                break;
            }

            picked.Add(next.Value);
            used.Add(next.Value.Item);

            //anchor every x times to a new point
            //maybe improves playlists/getSimilar tracks (at least makes it more interesting)
            bool isAnchorStep = anchorInterval > 0 && (i + 1) % anchorInterval == 0;
            if (isAnchorStep)
            {
                anchorVector = GetVectorFor(next.Value.Item);
            }
        }

        return picked;
    }
    
    private float[] GetVectorFor(T item)
    {
        return _entries.First(e => EqualityComparer<T>.Default.Equals(e.Item, item)).Vector;
    }
}