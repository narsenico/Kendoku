using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface IResolver
{
    public Result Resolve(Cell[] cells,
                          MatrixSettings settings,
                          Constraint[] constraints,
                          Helper[] helpers);
}
