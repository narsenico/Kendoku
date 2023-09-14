using System.Text;
using Kendoku;

var cells = Create(groupCount: 6,
                   cellPerGroup: 6,
                   cellPerGroupRow: 3);

cells.Find(group: 0, row: 1, col: 1)?.Resolve(6);

var constraint = new Constraint(new Cell[] {
  cells.Find(0, 1, 0),
  cells.Find(2, 0, 0)
}, 3);

Console.WriteLine($"Cells {cells.Length}");
Console.WriteLine(cells.ToHumanString());
Console.WriteLine(constraint.ToHumanString());

Cell[] Create(int groupCount, int cellPerGroup, int cellPerGroupRow)
{
  var possibilities = Enumerable.Range(1, cellPerGroup).ToArray();

  return Enumerable.Range(0, groupCount)
    .Select(g => new Group(g, cellPerGroup, cellPerGroupRow))
    .SelectMany(group => group.CreateCells(possibilities))
    .ToArray();
}

static class CellExtension
{
  public static string ToHumanString(this Cell[] cells)
  {
    var buff = new StringBuilder();

    foreach (var cell in cells)
    {
      buff.AppendLine($"[G{cell.Group}/{cell.Index}] {cell.Row}/{cell.Col} => {string.Join(',', cell.Possibilities)}");
    }

    return buff.ToString();
  }

  public static string ToHumanString(this Constraint constraint)
  {
    var cells = constraint.Cells.Select(cell => $"[G{cell.Group}/{cell.Index}] {cell.Row}/{cell.Col}");

    return $"Constraint {{ cells={string.Join(',', cells)} sum={constraint.Sum} resolved={constraint.IsResolved} }}";
  }

  public static string ToHumanString(this Constraint[] constraints)
  {
    return string.Join('\n', constraints.Select(c => c.ToHumanString()));
  }

  public static Cell Find(this Cell[] cells,
                           int group,
                           int row,
                           int col)
  {
    return cells.First(c => c.Group == group && c.Row == row && c.Col == col);
  }

  public static Cell FindByIndex(this Cell[] cells,
                                  int group,
                                  int cellIndex)
  {
    return cells.First(c => c.Group == group && c.Index == cellIndex);
  }
}

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
