using Kendoku;
using Kendoku.Implementations;
using Kendoku.Models;

// TODO: leggere tutte le configurazioni da args
/** INIZIO CONF **/
var _args = new string[] {
    "-c", "6",
    "-s", "6",
    "-r", "3",
    "-m", "2",
    "-l", "0:0:1,3",
    "-l", "1:1:1,4",
    "-l", "4:0:1,5",
    "-l", "5:1:1,5",
    "-t", "0:0:0,0:0:1,0:1:1,13",
    "-t", "0:1:0,2:0:0,3",
    "-t", "0:0:2,0:1:2,6",
    "-t", "1:0:0,1:0:1,8",
    "-t", "1:1:0,3:0:0,6",
    "-t", "1:0:2,1:1:1,1:1:2,12",
    "-t", "2:0:1,2:0:2,10",
    "-t", "2:1:0,2:1:1,7",
    "-t", "2:1:2,4:0:2,7",
    "-t", "3:0:1,3:0:2,5",
    "-t", "3:1:0,3:1:1,10"
};

var matrixSettings = SettingsParser.ParseMatrixSettingsFromArgs(_args);
var cellFactory = new CellFactory(matrixSettings);
var constraints = SettingsParser.ParseConstraintsFromArgs(_args, cellFactory);
var helpers = SettingsParser.ParseHelpersFromArgs(_args, cellFactory);
var cells = CreateMatrix(cellFactory, matrixSettings);
/** FINE CONF **/

Console.WriteLine($"Cells {cells.Length}");
Console.WriteLine(helpers.ToHumanString());
Console.WriteLine(constraints.ToHumanString());

var listener = new ConsoleEventListener();
var hashProvider = new HashProvider();
var resolver = new SimpleResolverImpl(listener, hashProvider);

Console.WriteLine();
Console.WriteLine("Start...");

var resolved = resolver.Resolve(cells, constraints, helpers);

if (resolved && !EnsureMatrixResolved(cells, matrixSettings))
{
    throw new InvalidOperationException("Resolver doesn't tell the truth: game not resolved!");
}

Console.WriteLine();
Console.WriteLine($"Game is {(resolved ? "resolved!" : "not resolved!")}");

Console.WriteLine();
Console.WriteLine("Last result:");
Console.WriteLine(cells.ToHumanString());

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