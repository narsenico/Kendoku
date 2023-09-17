using Kendoku.Models;

using System.Text;

namespace kendoku.Converters;

internal class CellsToStringConverter
{
    public string ConvertToString(IEnumerable<CellStatus> cells)
    {
        // TODO: aggiungere elementi della matrice
        // *---*---*
        // |123|456|
        // |456|123|
        // *---*---*

        return cells.GroupBy(cell => cell.Cell.MatrixRow)
            .Aggregate(new StringBuilder(), (buff, cells) => AppendRow(buff, cells))
            .ToString();
    }

    private static StringBuilder AppendRow(StringBuilder buff, IEnumerable<CellStatus> cells)
    {
        return cells.Aggregate(buff, (buff, cell) => buff.Append(FomratCell(cell))).AppendLine();
    }

    private static string FomratCell(CellStatus cell)
    {
        //if (cell.IsResolved) return cell.Value.ToString();
        //return " ";
        return cell.Value.ToString();
    }
}
