using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class MultiResolver : IResolver
{
    private readonly IEventListener _listener;
    private readonly IEnumerable<IResolver> _resolvers;

    public MultiResolver(IEventListener listener,
                         IEnumerable<IResolver> resolvers)
    {
        _listener = listener;
        _resolvers = resolvers;

        if (!_resolvers.Any()) throw new InvalidOperationException("Resolvers can not be empty");
    }

    public Result Resolve(CellStatus[] cells,
                          Constraint[] constraints,
                          Helper[] helpers)
    {
        Result? last = null;
        int totalIterations = 0;

        foreach (var resolver in _resolvers)
        {
            var result = resolver.Resolve(cells, constraints, helpers);
            last = result;
            totalIterations += result.IterationCount;
            if (result.Success)
            {
                break;
            }

            cells = result.Cells;
        }

        return last! with { IterationCount = totalIterations };
    }
}
