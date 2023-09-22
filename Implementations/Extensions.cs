using Kendoku.Models;

namespace Kendoku.Implementations;

public static class Extensions
{
    public static bool Is(this CellStatus cell, int groupIndex, int row, int col)
    {
        return cell.Cell.GroupIndex == groupIndex
            && cell.Cell.Row == row
            && cell.Cell.Col == col;
    }

    public static IEnumerable<Constraint> FilterBy(this IEnumerable<Constraint> constraints, CellStatus cell)
    {
        return constraints.Where(c => c.Cells.Contains(cell.Cell));
    }

    public static IEnumerable<CellStatus> OnConstarint(this IEnumerable<CellStatus> cells, Constraint constraint)
    {
        return cells.Where(c => constraint.Cells.Contains(c.Cell));
    }

    public static CellStatus Find(this IEnumerable<CellStatus> cells,
                                  Helper helper)
    {
        return cells.First(c => c.Cell == helper.Cell);
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

    public static CellStatus MantainUniqueValueIn(this IEnumerable<int> values, CellStatus cell)
    {
        var uniques = cell.Possibilities.Except(values);
        if (uniques.Count() == 1)
        {
            cell.Resolve(uniques.First());
        }
        return cell;
    }

    public static CellStatus Clone(this CellStatus cell) => 
        new(cell.Cell, cell.Possibilities.ToArray());

    public static IEnumerable<CellStatus> Clone(this IEnumerable<CellStatus> cells) =>
        cells.Select(c => c.Clone());
}
