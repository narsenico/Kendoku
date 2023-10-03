using Kendoku.Implementations;
using Kendoku.Interfaces;
using Kendoku.Models;

using System.Text;

namespace Kendoku.Converters;

internal class SimpleExporter : IExporter
{
    private const char CORNER = '*';
    private const char FLOOR = '-';
    private const char WALL = '|';

    private readonly bool _verbose;

    public SimpleExporter(bool verbose)
    {
        _verbose = verbose;
    }

    public void Export(MatrixSettings matrixSettings, Result result)
    {
        var buff = new StringBuilder();
        buff.Append($"Game is {(result.Success ? "resolved!" : "not resolved! (use --verbose options for more details)")}");
        buff.Append($"{result.ResolvedCount} out of {result.TotalCount} cells resolved in {result.ExecutionTime} ({result.IterationCount} iterations)");

        buff.AppendLine();
        buff.AppendLine("Last result:");
        buff.AppendLine();

        var matrixRowSize = matrixSettings.MatrixRowSize;
        var groupRowSize = matrixSettings.GroupRowSize;

        buff = result.Cells.GroupBy(cell => CalcMatrixRow(cell, matrixRowSize))
            .Aggregate(buff, (buff, cells) => AppendGroups(buff, cells, matrixRowSize, groupRowSize));

        AppendDivider(buff, matrixRowSize, groupRowSize);

        if (_verbose)
        {
            buff.AppendLine();

            var notResolvedCells = result.Cells.ExcludeResolved();
            if (notResolvedCells.Any())
            {
                buff.AppendLine("Not resolved cells:");
                foreach (var notResolvedCell in result.Cells.ExcludeResolved())
                {
                    buff.AppendLine(notResolvedCell.ToHumanString());
                }
            }
        }

        Console.WriteLine(buff.ToString());
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
