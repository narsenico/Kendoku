using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface IResolver
{
    public Result Resolve(CellStatus[] cells,
                          Constraint[] constraints,
                          Helper[] helpers);
}
