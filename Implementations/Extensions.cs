using Kendoku.Models;

namespace kendoku.Implementations;

public static class Extensions
{
    public static IEnumerable<Constraint> FilterBy(this IEnumerable<Constraint> constraints, CellStatus cell)
    {
        return constraints.Where(c => c.Cells.Contains(cell.Cell));
    }

    public static CellStatus Find(this IEnumerable<CellStatus> cells,
                                  Helper helper)
    {
        return cells.First(c => c.Cell == helper.cell);
    }

    public static bool IsResolved(this IEnumerable<CellStatus> cells) => cells.All(c => c.IsResolved);

    public static IEnumerable<CellStatus> OnSameMatrixRowOf(this IEnumerable<CellStatus> cells, CellStatus cell)
    {
        return cells.Where(c => c.Cell.MatrixRow == cell.Cell.MatrixRow);
    }

    public static IEnumerable<CellStatus> OnSameMatrixColOf(this IEnumerable<CellStatus> cells, CellStatus cell)
    {
        return cells.Where(c => c.Cell.MatrixCol == cell.Cell.MatrixCol);
    }

    public static IEnumerable<CellStatus> OnGroupOf(this IEnumerable<CellStatus> cells, CellStatus cell)
    {
        var groupIndex = cell.Cell.GroupIndex;
        return cells.Where(c => c.Cell.GroupIndex == groupIndex);
    }

    public static IEnumerable<CellStatus> Exclude(this IEnumerable<CellStatus> cells, CellStatus cell)
    {
        return cells.Where(c => c != cell);
    }

    public static IEnumerable<CellStatus> OnlyResolved(this IEnumerable<CellStatus> cells)
    {
        return cells.Where(c => c.IsResolved);
    }

    public static IEnumerable<CellStatus> ExcludeResolved(this IEnumerable<CellStatus> cells)
    {
        return cells.Where(c => !c.IsResolved);
    }

    public static CellStatus RemovePossibilities(this CellStatus cell, IEnumerable<int> values)
    {
        foreach (var value in values)
        {
            cell.RemovePossibility(value);
        }
        return cell;
    }

    public static IEnumerable<int> Values(this IEnumerable<CellStatus> cells)
    {
        return cells.Select(c => c.Value);
    }

    public static CellStatus PurgePossibilitiesOf(this IEnumerable<CellStatus> cells, CellStatus cell)
    {
        var values = cells.Values();
        cell.RemovePossibilities(values);
        return cell;
    }
}