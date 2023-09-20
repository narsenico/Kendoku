using Kendoku.Models;

using System.Text;

namespace Kendoku.Converters;

internal class CellsToStringConverter
{
    private const char CORNER = '*';
    private const char FLOOR = '-';
    private const char WALL = '|';

    public string ConvertToString(IEnumerable<CellStatus> cells,
                                  MatrixSettings matrixSettings)
    {
        var matrixRowSize = matrixSettings.MatrixRowSize;
        var groupRowSize = matrixSettings.GroupRowSize;

        var buff = cells.GroupBy(cell => CalcMatrixRow(cell, matrixRowSize))
            .Aggregate(new StringBuilder(), (buff, cells) => AppendGroups(buff, cells, matrixRowSize, groupRowSize));

        AppendDivider(buff, matrixRowSize, groupRowSize);
        return buff.ToString();
    }

    private static int CalcMatrixRow(CellStatus cell, int matrixRowSize)
    {
        return cell.Cell.GroupIndex / matrixRowSize;
    }

    private static StringBuilder AppendGroups(StringBuilder buff,
                                              IEnumerable<CellStatus> cells,
                                              int matrixRowSize,
                                              int groupRowSize)
    {
        AppendDivider(buff, matrixRowSize, groupRowSize);

        foreach (var cellsInRow in cells.GroupBy(c => c.Cell.Row).OrderBy(g => g.Key))
        {
            foreach (var cellInGroup in cellsInRow.GroupBy(c => c.Cell.GroupIndex).OrderBy(g => g.Key))
            {
                buff.Append(WALL);
                foreach (var cell in cellInGroup.OrderBy(c => c.Cell.Col))
                {
                    buff.Append(FormatCell(cell));
                }
            }
            buff.Append(WALL);
            buff.AppendLine();
        }

        return buff;
    }

    private static StringBuilder AppendDivider(StringBuilder buff, int matrixRowSize, int groupRowSize)
    {
        for (int ii = 0; ii < matrixRowSize; ii++)
        {
            buff.Append(CORNER);
            buff.Append(FLOOR, groupRowSize);
        }
        buff.Append(CORNER);
        buff.AppendLine();
        return buff;
    }

    private static string FormatCell(CellStatus cell)
    {
        if (cell.IsResolved) return cell.Value.ToString();
        return " ";
        // return cell.Value.ToString();
    }
}
