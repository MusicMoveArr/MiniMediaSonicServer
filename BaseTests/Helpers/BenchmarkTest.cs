using System.Diagnostics;
using AwesomeAssertions;

namespace BaseTests.Helpers;

public class BenchmarkTest
{
    public static async Task BenchmarkTestAsync(int maxLatency, int iterations, Func<Task> action)
    {
        await action();
        
        List<long> timings = new List<long>();
        for (int i = 0; i < iterations; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            await action();
            sw.Stop();
            timings.Add(sw.ElapsedMilliseconds);
        }
        
        var p95Index = (int)Math.Ceiling(iterations * 0.95) - 1;
        
        timings.Sort();
        var p95 = timings[p95Index];
        p95.Should().BeLessThanOrEqualTo(maxLatency);
    }
}