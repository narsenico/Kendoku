using Kendoku;
using Kendoku.Converters;
using Kendoku.Implementations;
using Kendoku.Interfaces;
using Kendoku.Models;

if (PrintUsage(args) || PrintHelp(args)) return;

var verbose = args.Contains("--verbose");
var gameArgs = ParseGameArgs(args);
var matrixSettings = SettingsParser.ParseMatrixSettingsFromArgs(gameArgs);
var cellFactory = new CellFactory(matrixSettings);
var constraints = SettingsParser.ParseConstraintsFromArgs(gameArgs, cellFactory);
var helpers = SettingsParser.ParseHelpersFromArgs(gameArgs, cellFactory);
var cells = CreateMatrix(cellFactory, matrixSettings);

var hashProvider = new HashProvider();
var listener = CreateEventListener(verbose);
var resolver = new SimpleResolverImpl(listener, hashProvider);

Console.WriteLine($"Playing game with {cells.Length} cells, {helpers.Count()} helpers and {constraints.Count()} constraints...");

var result = resolver.Resolve(cells, matrixSettings, constraints, helpers);

if (result.Success && !EnsureMatrixResolved(result.Cells, matrixSettings))
{
    throw new InvalidOperationException("Resolver doesn't tell the truth: game not resolved!");
}

var exporter = CreateExporter(args.FirstArgOrDefault("-O"), verbose);
Console.WriteLine(exporter.Export(matrixSettings, result));

/********************************/

Cell[] CreateMatrix(CellFactory factory, MatrixSettings settings)
{
    return Enumerable.Range(0, settings.GroupCount)
        .SelectMany(groupIndex => factory.CreateGroupCells(groupIndex))
        .ToArray();
}

bool EnsureMatrixResolved(IEnumerable<CellStatus> cells, MatrixSettings settings)
{
    var possibilities = settings.GetPossibilities().Order().ToArray();

    var notResolvedGroups = cells.GroupBy(c => c.Cell.GroupIndex)
        .Where(group => !group.ContainsAllValues(possibilities))
        .Select(group => group.Key);

    var notResolvedRows = cells.GroupBy(c => c.Cell.MatrixRow)
        .Where(group => !group.ContainsAllValues(possibilities))
        .Select(group => group.Key);

    var notResolvedCols = cells.GroupBy(c => c.Cell.MatrixCol)
        .Where(group => !group.ContainsAllValues(possibilities))
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

bool PrintUsage(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} <filename> [--verbose]");
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
        Console.WriteLine(@"
Options:
--verbose    Print extended logs
");
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

IExporter CreateExporter(string? exporterType, bool verbose)
{
    return exporterType switch
    {
        _ => new SimpleExporter(verbose)
    };
}

IEventListener CreateEventListener(bool verbose)
{
    var actorFormatter = new DefaultActorFormatter();
    var listener = new ConsoleEventListener(actorFormatter, verbose);
    return listener;
}
