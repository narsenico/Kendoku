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

    public Result Resolve(Cell[] cells,
                          MatrixSettings settings,
                          Constraint[] constraints,
                          Helper[] helpers)
    {
        Result? last = null;
        int totalIterations = 0;

        foreach (var resolver in _resolvers)
        {
            var result = resolver.Resolve(cells, settings, constraints, helpers);
            last = result;
            totalIterations += result.IterationCount;
            if (result.Success)
            {
                break;
            }

            helpers = helpers.Concat(ResolvedCellsToHelpers(result.Cells)).ToArray();
        }

        return last! with { IterationCount = totalIterations };
    }

    private static IEnumerable<Helper> ResolvedCellsToHelpers(CellStatus[] cells)
    {
        return cells.Where(c => c.IsResolved)
            .Select(c => new Helper(c.Cell, c.Value));
    }
}
