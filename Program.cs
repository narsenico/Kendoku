using Kendoku;

var cells = Create(groupCount: 6,
                   cellPerGroup: 6,
                   cellPerGroupRow: 3);

cells.Find(group: 0, row: 1, col: 1)?.Resolve(6);

var constraint = new Constraint(new Cell[] {
  cells.Find(0, 1, 0),
  cells.Find(2, 0, 0)
}, 3);

Console.WriteLine($"Cells {cells.Length}");
Console.WriteLine(cells.ToHumanString());
Console.WriteLine(constraint.ToHumanString());

Cell[] Create(int groupCount, int cellPerGroup, int cellPerGroupRow)
{
  var possibilities = Enumerable.Range(1, cellPerGroup).ToArray();

  return Enumerable.Range(0, groupCount)
    .Select(g => new Group(g, cellPerGroup, cellPerGroupRow))
    .SelectMany(group => group.CreateCells(possibilities))
    .ToArray();
}