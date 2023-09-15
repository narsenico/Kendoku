using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface IResolver
{
    public bool Resolve(CellStatus[] cells,
                        Constraint[] constraints,
                        HelpCell[] helpCells);
}