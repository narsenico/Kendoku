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

        foreach (var resolver in _resolvers)
        {
            var result = resolver.Resolve(cells, settings, constraints, helpers);
            if (result.Success)
            {
                return result;
            }
            last = result;
        }

        return last!;
    }
}
