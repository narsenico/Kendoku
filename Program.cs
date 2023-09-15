using Kendoku;
using Kendoku.Models;
using Kendoku.Resolvers;

// TODO: leggere tutte le configurazioni da args
/** INIZIO CONF **/
var matrixSettings = new MatrixSettings
{
  GroupCount = 6,
  GroupSize = 6,
  GroupRowSize = 3,
};

var constraint = new Constraint(new Cell[] {
  new(0, 1, 0),
  new(2, 0, 0)
}, 3);

var helpCell = new HelpCell(0, 1, 1, 6);

var cells = Create(matrixSettings);
/** FINE CONF **/

Console.WriteLine($"Cells {cells.Length}");
Console.WriteLine(cells.ToHumanString());
Console.WriteLine(constraint.ToHumanString());

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