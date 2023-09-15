using System.Text;

namespace Kendoku;

public static class Extensions
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