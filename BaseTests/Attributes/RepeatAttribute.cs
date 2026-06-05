using System.Reflection;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace BaseTests.Attributes;

public sealed class RepeatAttribute : DataAttribute
{
    private readonly int count;

    public RepeatAttribute(int count)
    {
        if (count < 1)
        {
            throw new System.ArgumentOutOfRangeException(
                paramName: nameof(count),
                message: "Repeat count must be greater than 0."
            );
        }
        this.count = count;
    }
    
    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        var data = Enumerable.Range(start: 1, count: this.count)
            .Select(iterationNumber => (ITheoryDataRow)new TheoryDataRow(iterationNumber))
            .ToList();
        
        return ValueTask.FromResult<IReadOnlyCollection<ITheoryDataRow>>(data);
    }

    public override bool SupportsDiscoveryEnumeration()
    {
        throw new NotImplementedException();
    }
}