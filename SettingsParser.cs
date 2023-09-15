using System.Text.RegularExpressions;
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
        };
    }

    public static Constraint[] ParseConstraintsFromArgs(string[] args)
    {
        return args.FindArgs("-t").Select(ConvertToConstraint).ToArray();
    }

    public static HelpCell[] ParseHelpCellsFromArgs(string[] args)
    {
        return args.FindArgs("-l").Select(ConvertToHelpCell).ToArray();
    }

    private static Constraint ConvertToConstraint(string text)
    {
        // g:r:c[,...g:r:c],v
        var match = ConstraintRegex().Match(text);
        var value = match.Groups["Value"].Value.ToInt() ?? throw new ArgumentException($"{nameof(Constraint)} argument not valid");
        var cellCount = match.Groups[2].Captures.Count;
        var cells = new Cell[cellCount];

        for (var ii=0; ii<cellCount; ii++)
        {
            cells[ii] = new(
                GroupIndex: match.Groups["GroupIndex"].Captures[ii].Value.ToInt()!.Value, 
                Row: match.Groups["Row"].Captures[ii].Value.ToInt()!.Value, 
                Col: match.Groups["Col"].Captures[ii].Value.ToInt()!.Value);
        }

        return new(cells, value);
    }

    private static HelpCell ConvertToHelpCell(string text)
    {
        // g:r:c,v
        var match = HelpCellRegex().Match(text);
        var value = match.Groups["Value"].Value.ToInt() ?? throw new ArgumentException($"{nameof(HelpCell)} argument not valid");
        var cell = new Cell(
            GroupIndex: match.Groups["GroupIndex"].Value.ToInt()!.Value,
            Row: match.Groups["Row"].Value.ToInt()!.Value,
            Col: match.Groups["Col"].Value.ToInt()!.Value);

        return new(cell, value);
    }

    [GeneratedRegex("^(((?<GroupIndex>\\d+):(?<Row>\\d+):(?<Col>\\d+),)+)(?<Value>\\d+)$")]
    private static partial Regex ConstraintRegex();
    
    [GeneratedRegex("^(?<GroupIndex>\\d+):(?<Row>\\d+):(?<Col>\\d+),(?<Value>\\d+)$")]
    private static partial Regex HelpCellRegex();
}