using System.Text.RegularExpressions;

using Kendoku.Implementations;
using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku;

// TODO: migliorare messaggi errore perché siano più dettagliati (specificando a cosa serve il tal parametro?)
public static partial class SettingsParser
{
    public static MatrixSettings ParseMatrixSettingsFromArgs(string[] args)
    {
        return new()
        {
            GroupCount = args.FirstArgOrDefault("-c")?.ToInt() ?? throw new ArgumentException("Group count (-c) argument not valid"),
            GroupSize = args.FirstArgOrDefault("-s")?.ToInt() ?? throw new ArgumentException("Group size (-s) argument not valid"),
            GroupRowSize = args.FirstArgOrDefault("-r")?.ToInt() ?? throw new ArgumentException("Group row size (-r) argument not valid"),
            GroupPerMatrixRow = args.FirstArgOrDefault("-m")?.ToInt() ?? throw new ArgumentException("Group per matrix row (-m) argument not valid"),
        };
    }

    public static Constraint[] ParseConstraintsFromArgs(string[] args, ICellFactory cellFactory)
    {
        return args.FindArgs("-t")
            .Select(t => ConvertToConstraint(t, cellFactory))
            .ToArray();
    }

    private static Constraint ConvertToConstraint(string text, ICellFactory cellFactory)
    {
        // g:r:c[,...g:r:c],v
        var match = ConstraintRegex().Match(text);
        var value = match.Groups["Value"].Value.ToInt() ?? throw new ArgumentException($"{nameof(Constraint)} argument not valid");
        var cellCount = match.Groups[2].Captures.Count;
        var cells = new Cell[cellCount];

        for (var ii = 0; ii < cellCount; ii++)
        {
            cells[ii] = cellFactory.CreateCell(
                groupIndex: match.Groups["GroupIndex"].Captures[ii].Value.ToInt()!.Value,
                row: match.Groups["Row"].Captures[ii].Value.ToInt()!.Value,
                col: match.Groups["Col"].Captures[ii].Value.ToInt()!.Value);
        }

        return new(cells, value);
    }

    public static Helper[] ParseHelpersFromArgs(string[] args, ICellFactory cellFactory)
    {
        return args.FindArgs("-l")
            .Select(t => ConvertToHelper(t, cellFactory))
            .ToArray();
    }

    private static Helper ConvertToHelper(string text, ICellFactory cellFactory)
    {
        // g:r:c,v
        var match = HelperRegex().Match(text);
        var value = match.Groups["Value"].Value.ToInt() ?? throw new ArgumentException($"{nameof(Helper)} argument not valid");
        var cell = cellFactory.CreateCell(
            groupIndex: match.Groups["GroupIndex"].Value.ToInt()!.Value,
            row: match.Groups["Row"].Value.ToInt()!.Value,
            col: match.Groups["Col"].Value.ToInt()!.Value);
        return new Helper(cell, value);
    }

    [GeneratedRegex("^(((?<GroupIndex>\\d+):(?<Row>\\d+):(?<Col>\\d+),)+)(?<Value>\\d+)$")]
    private static partial Regex ConstraintRegex();

    [GeneratedRegex("^(?<GroupIndex>\\d+):(?<Row>\\d+):(?<Col>\\d+),(?<Value>\\d+)$")]
    private static partial Regex HelperRegex();
}