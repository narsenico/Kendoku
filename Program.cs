using kendoku.Converters;

using Kendoku;
using Kendoku.Implementations;
using Kendoku.Interfaces;
using Kendoku.Models;

using System.Diagnostics;

if (PrintUsage(args) || PrintHelp(args)) return;

var _args = ParseArgs(args);
var matrixSettings = SettingsParser.ParseMatrixSettingsFromArgs(_args);
var cellFactory = new CellFactory(matrixSettings);
var constraints = SettingsParser.ParseConstraintsFromArgs(_args, cellFactory);
var helpers = SettingsParser.ParseHelpersFromArgs(_args, cellFactory);
var cells = CreateMatrix(cellFactory, matrixSettings);

Console.WriteLine($"Cells {cells.Length}");
Console.WriteLine(helpers.ToHumanString());
Console.WriteLine(constraints.ToHumanString());

var listener = new ConsoleEventListener();
var hashProvider = new HashProvider();
var resolver = new SimpleResolverImpl(listener, hashProvider);

Console.WriteLine();
Console.WriteLine("Start...");

var (resolved, time) = ResolveWithTime(resolver, cells, constraints, helpers);

if (resolved && !EnsureMatrixResolved(cells, matrixSettings))
{
    throw new InvalidOperationException("Resolver doesn't tell the truth: game not resolved!");
}

Console.WriteLine();
Console.WriteLine($"Game is {(resolved ? "resolved!" : "not resolved!")}");
Console.WriteLine($"Time: {time}");

Console.WriteLine();
Console.WriteLine("Last result:");
//Console.WriteLine(cells.ToHumanString());
PrintResult(cells);

/********************************/

CellStatus[] CreateMatrix(CellFactory factory, MatrixSettings settings)
{
    var possibilities = settings.GetPossibilities().ToArray();

    return Enumerable.Range(0, settings.GroupCount)
      .SelectMany(groupIndex => factory.CreateGroupCells(groupIndex))
      .Select(cell => new CellStatus(cell, possibilities))
      .ToArray();
}

bool EnsureMatrixResolved(IEnumerable<CellStatus> cells, MatrixSettings settings)
{
    var possibilities = settings.GetPossibilities().Order().ToArray();

    var notResolvedGroups = cells.GroupBy(c => c.Cell.GroupIndex)
        .Where(group => !EnsureUniqueValues(group, possibilities))
        .Select(group => group.Key);

    var notResolvedRows = cells.GroupBy(c => c.Cell.MatrixRow)
        .Where(group => !EnsureUniqueValues(group, possibilities))
        .Select(group => group.Key);

    var notResolvedCols = cells.GroupBy(c => c.Cell.MatrixCol)
        .Where(group => !EnsureUniqueValues(group, possibilities))
        .Select(group => group.Key);

    foreach (var g in notResolvedGroups)
    {
        Console.WriteLine($"Group {g} not resolved!");
    }

    foreach (var r in notResolvedRows)
    {
        Console.WriteLine($"Row {r} not resolved!");
    }

    foreach (var c in notResolvedCols)
    {
        Console.WriteLine($"Col {c} not resolved!");
    }

    return !notResolvedGroups.Any()
        && !notResolvedRows.Any()
        && !notResolvedCols.Any();
}

bool EnsureUniqueValues(IEnumerable<CellStatus> cells, int[] possibilities)
{
    return cells.Values().Order().SequenceEqual(possibilities);
}

bool PrintUsage(string[] args)
{
    if (args.Length == 0)
    {
        throw new NotImplementedException();
    }

    return false;
}

bool PrintHelp(string[] args)
{
    if (args.Contains("-h") || args.Contains("--help")) 
    { 
        throw new NotImplementedException();
    }

    return false;
}

string[] ParseArgs(string[] args)
{
    if (args.Length == 1)
    {
        var converter = new FileToArgsConverter();
        return converter.ConvertToArgs(fileName: args[0]);
    }

    return args;
}

void PrintResult(IEnumerable<CellStatus> cells)
{
    var converter = new CellsToStringConverter();
    Console.WriteLine(converter.ConvertToString(cells));
}

(bool, TimeSpan) ResolveWithTime(IResolver resolver,
                                 CellStatus[] cells,
                                 Constraint[] constraints,
                                 Helper[] helpers)
{
    var stopWatch = Stopwatch.StartNew();
    var resolved = resolver.Resolve(cells, constraints, helpers);
    stopWatch.Stop();
    return (resolved, stopWatch.Elapsed);
}