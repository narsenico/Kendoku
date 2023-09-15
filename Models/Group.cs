namespace Kendoku.Models;

record Group(int GroupIndex, int Size, int CellPerRow)
{
  public Cell[] CreateCells(int[] possibilities)
  {
    return Enumerable.Range(0, Size)
      .Select(i =>
      {
        var row = i / CellPerRow;
        var col = i - (CellPerRow * row);
        var cellIndex = CellPerRow * row + col;
        return new Cell(row, col, GroupIndex, cellIndex, possibilities);
      })
      .ToArray();
  }
}
