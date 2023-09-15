using Kendoku;
using Kendoku.Models;
using Kendoku.Implementations;

// TODO: leggere tutte le configurazioni da args
/** INIZIO CONF **/
var matrixSettings = new MatrixSettings
{
  GroupCount = 6,
  GroupSize = 6,
  GroupRowSize = 3,
};

var constraints = new Constraint[]
{
  new(new Cell[] {
    new(0, 1, 0),
    new(2, 0, 0)
  }, 3)
};

var helpCells = new HelpCell[]
{
  new(0, 1, 1, 6)
};

var cells = Create(matrixSettings);
/** FINE CONF **/

Console.WriteLine($"Cells {cells.Length}");
Console.WriteLine(cells.ToHumanString());
Console.WriteLine(constraints.ToHumanString());

var listener = new ConsoleEventListener();
var resolver = new SimpleResolverImpl(listener);

Console.WriteLine("Resolving...");

resolver.Resolve(cells, constraints, helpCells);

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