namespace Kendoku.Models;

public sealed class Constraint
{
    public readonly Cell[] _cells;
    public int Sum { get; init; }

    public Constraint(Cell[] cells, int sum)
    {
        _cells = cells;
        Sum = sum;
    }

    public IEnumerable<Cell> Cells => _cells;

    public bool IsResolved => _cells.All(c => c.IsResolved);
}