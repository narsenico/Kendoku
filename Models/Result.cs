using Kendoku.Implementations;

namespace Kendoku.Models;

public record Result(bool Success,
                     CellStatus[] Cells,
                     int IterationCount,
                     TimeSpan ExecutionTime)
{
    public int ResolvedCount => Cells.OnlyResolved().Count();
    public int TotalCount => Cells.Count();
}