using Kendoku;
using Kendoku.Models;
using Kendoku.Implementations;
using kendoku.Implementations;

// TODO: leggere tutte le configurazioni da args
/** INIZIO CONF **/
var _args = new string[] { 
    "-c", "6", 
    "-s", "6", 
    "-r", "3",
    "-t", "0:0:0,0:0:1,0:1:1,13", 
    "-t", "0:1:0,2:0:0,3", 
    "-l", "0:0:1,3",
    "-l", "1:1:1,4",
    "-l", "4:0:1,5",
    "-l", "5:1:1,5",
};

var constraints = SettingsParser.ParseConstraintsFromArgs(_args);
var helpCells = SettingsParser.ParseHelpCellsFromArgs(_args);
var matrixSettings = SettingsParser.ParseMatrixSettingsFromArgs(_args);
var cells = Create(matrixSettings);
/** FINE CONF **/

args.Dump();

Console.WriteLine($"Cells {cells.Length}");
Console.WriteLine(helpCells.ToHumanString());
Console.WriteLine(constraints.ToHumanString());
Console.WriteLine(cells.ToHumanString());

var listener = new ConsoleEventListener();
var hashProvider = new HashProvider();
var resolver = new SimpleResolverImpl(listener, hashProvider);

Console.WriteLine();
Console.WriteLine("Resolving...");

var resolved = resolver.Resolve(cells, constraints, helpCells);

Console.WriteLine($"...matrix is {(resolved ? "resolved!" : "not resolved!")}");
Console.WriteLine(cells.ToHumanString());

/********************************/

CellStatus[] Create(MatrixSettings settings)
{
  var possibilities = Enumerable.Range(1, settings.GroupSize).ToArray();

  return Enumerable.Range(0, settings.GroupCount)
    .SelectMany(groupIndex => CreateCells(groupIndex, settings.GroupSize, settings.GroupRowSize))
    .Select(cell => new CellStatus(cell, possibilities))
    .ToArray();
}

Cell[] CreateCells(int groupIndex, int groupSize, int groupRowSize)
{
  return Enumerable.Range(0, groupSize)
    .Select(i =>
    {
      var row = i / groupRowSize;
      var col = i - (groupRowSize * row);
      // var cellIndex = groupRowSize * row + col;
      return new Cell(groupIndex, row, col);
    })
    .ToArray();
}