namespace Kendoku.Models;

public static class CellStatusExtensions
{
    public static CellStatus Find(this IEnumerable<CellStatus> cells,
                                  Cell cell)
    {
        return cells.First(c => c.Cell == cell);
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

    public static bool IsResolved(this CellStatus[] cells) => cells.All(c => c.IsResolved);
}