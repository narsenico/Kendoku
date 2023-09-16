using Kendoku;
using Kendoku.Models;
using Kendoku.Implementations;

// TODO: leggere tutte le configurazioni da args
/** INIZIO CONF **/
var _args = new string[] {
    "-c", "6",
    "-s", "6",
    "-r", "3",
    "-m", "2",
    "-t", "0:0:0,0:0:1,0:1:1,13",
    "-t", "0:1:0,2:0:0,3",
    "-l", "0:0:1,3",
    "-l", "1:1:1,4",
    "-l", "4:0:1,5",
    "-l", "5:1:1,5",
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
Console.WriteLine("Resolving...");

var resolved = resolver.Resolve(cells, constraints, helpers);

Console.WriteLine($"...matrix is {(resolved ? "resolved!" : "not resolved!")}");

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