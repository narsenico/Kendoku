namespace Kendoku.Models;

public static class CellStatusExtensions
{
    public static CellStatus Find(this IEnumerable<CellStatus> cells,
                                  Cell cell)
    {
        return cells.First(c => c.Cell == cell);
    }

    public static CellStatus Find(this IEnumerable<CellStatus> cells,
                                  HelpCell cell)
    {
        return cells.First(c => c.Cell.GroupIndex == cell.GroupIndex
                                && c.Cell.Row == cell.Row
                                && c.Cell.Col == cell.Col);
    }

    public static CellStatus Find(this IEnumerable<CellStatus> cells,
                                int group,
                                int row,
                                int col)
    {
        return cells.First(c => c.Cell.GroupIndex == group
                                && c.Cell.Row == row
                                && c.Cell.Col == col);
    }

    // public static CellStatus FindByIndex(this IEnumerable<CellStatus> cells,
    //                                      int group,
    //                                      int cellIndex)
    // {
    //     return cells.First(c => c.Cell.GroupIndex == group
    //                             && c.Cell.Index == cellIndex);
    // }

    public static bool IsResolved(this IEnumerable<CellStatus> cells) => cells.All(c => c.IsResolved);

    public static IEnumerable<CellStatus> OnSameRowOf(this IEnumerable<CellStatus> cells, CellStatus cell)
    {
        throw new NotImplementedException();
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
}