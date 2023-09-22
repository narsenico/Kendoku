using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class CellFactory : ICellFactory
{
    private readonly MatrixSettings _settings;

    public CellFactory(MatrixSettings settings)
    {
        _settings = settings;
    }

    public Cell CreateCell(int groupIndex, int row, int col)
    {
        var groupMatrixRow = groupIndex / _settings.MatrixRowSize;
        var groupMatrixCol = groupIndex - (groupMatrixRow * _settings.MatrixRowSize);

        var matrixRow = row + (groupMatrixRow * _settings.RowPerGroup);
        var matrixCol = col + (_settings.GroupRowSize * groupMatrixCol);

        return new Cell(groupIndex, row, col, matrixRow, matrixCol);
    }

    public IEnumerable<Cell> CreateGroupCells(int groupIndex)
    {
        return Enumerable.Range(0, _settings.GroupSize)
              .Select(i =>
              {
                  var row = i / _settings.GroupRowSize;
                  var col = i - (_settings.GroupRowSize * row);
                  return CreateCell(groupIndex, row, col);
              })
              .ToArray();
    }
}
