using Kendoku;
using Kendoku.Converters;
using Kendoku.Implementations;
using Kendoku.Interfaces;
using Kendoku.Models;

if (PrintUsage(args) || PrintHelp(args)) return;

var gameArgs = ParseGameArgs(args);
var matrixSettings = SettingsParser.ParseMatrixSettingsFromArgs(gameArgs);
var cellFactory = new CellFactory(matrixSettings);
var constraints = SettingsParser.ParseConstraintsFromArgs(gameArgs, cellFactory);
var helpers = SettingsParser.ParseHelpersFromArgs(gameArgs, cellFactory);
var cells = CreateMatrix(cellFactory, matrixSettings);

var hashProvider = new HashProvider();
var listener = CreateEventListener(args);
var resolver = new SimpleResolverImpl(listener, hashProvider);

Console.WriteLine($"Playing game with {cells.Length} cells, {helpers.Count()} helpers and {constraints.Count()} constraints...");

var result = resolver.Resolve(cells, constraints, helpers);

if (result.Success && !EnsureMatrixResolved(cells, matrixSettings))
{
    throw new InvalidOperationException("Resolver doesn't tell the truth: game not resolved!");
}

Console.WriteLine();
Console.WriteLine($"Game is {(result.Success ? "resolved!" : "not resolved!")}");
Console.WriteLine($"{result.ResolvedCount} out of {result.TotalCount} cells resolved in {result.ExecutionTime} ({result.IterationCount} iterations)");

Console.WriteLine();
Console.WriteLine("Last result:");
//Console.WriteLine(cells.ToHumanString());
PrintResult(cells, matrixSettings);

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
        Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <filename>");
        return true;
    }

    return false;
}

bool PrintHelp(string[] args)
{
    if (args.Contains("-h") || args.Contains("--help"))
    {
        Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <filename>");
        Console.WriteLine("Resolve Kendoku game");
        // TODO: stampare help formato file
        return true;
    }

    return false;
}

string[] ParseGameArgs(string[] args)
{
    if (args.Length >= 1 && !args[0].StartsWith("-"))
    {
        var converter = new FileToArgsConverter();
        return converter.ConvertToArgs(fileName: args[0]);
    }

    return args;
}

IEventListener CreateEventListener(string[] args)
{
    var verbose = args.Contains("--verbose");
    var actorFormatter = new DefaultActorFormatter();
    var listener = new ConsoleEventListener(actorFormatter, verbose);
    return listener;
}

void PrintResult(IEnumerable<CellStatus> cells, MatrixSettings matrixSettings)
{
    var converter = new CellsToStringConverter();
    Console.WriteLine(converter.ConvertToString(cells, matrixSettings));

    var notResolvedCells = cells.ExcludeResolved();
    if (notResolvedCells.Any())
    {
        Console.WriteLine("Not resolved cells:");
        foreach (var notResolvedCell in cells.ExcludeResolved())
        {
            Console.WriteLine(notResolvedCell.ToHumanString());
        }
    }
}
