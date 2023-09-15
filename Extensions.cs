using System.Text;
using Kendoku.Models;
using Kendoku.Resolvers;

namespace Kendoku;

public static class Extensions
{
  public static string ToHumanString(this IEnumerable<CellStatus> cells)
  {
    var buff = new StringBuilder();

    foreach (var cell in cells)
    {
      buff.AppendLine($"[G{cell.Cell.GroupIndex}/{cell.Cell.Index}] {cell.Cell.Row}/{cell.Cell.Col} => {string.Join(',', cell.Possibilities)}");
    }

    return buff.ToString();
  }

  public static string ToHumanString(this Constraint constraint)
  {
    var cells = constraint.Cells.Select(cell => $"[G{cell.GroupIndex}/{cell.Index}] {cell.Row}/{cell.Col}");

    return $"Constraint {{ cells={string.Join(',', cells)} sum={constraint.Sum} }}";
  }

  public static string ToHumanString(this Constraint[] constraints)
  {
    return string.Join('\n', constraints.Select(c => c.ToHumanString()));
  }
}