using Kendoku.Models;

namespace Kendoku;

public static class Extensions
{
  public static string ToHumanString(this CellStatus cell)
  {
    return $"[G{cell.Cell.GroupIndex}] {cell.Cell.Row}/{cell.Cell.Col} => {string.Join(',', cell.Possibilities)}";
  }

  public static string ToHumanString(this IEnumerable<CellStatus> cells)
  {
    return string.Join('\n', cells.Select(c => c.ToHumanString()));
  }

  public static string ToHumanString(this Constraint constraint)
  {
    var cells = constraint.Cells.Select(cell => $"[G{cell.GroupIndex}] {cell.Row}/{cell.Col}");

    return $"Constraint {{ cells={string.Join(',', cells)} sum={constraint.Sum} }}";
  }

  public static string ToHumanString(this Constraint[] constraints)
  {
    return string.Join('\n', constraints.Select(c => c.ToHumanString()));
  }
}