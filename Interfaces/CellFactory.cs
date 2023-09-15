using Kendoku;
using Kendoku.Models;

namespace kendoku.Interfaces;

public class CellFactory
{
    private readonly MatrixSettings _settings;

    public CellFactory(MatrixSettings settings)
    {
        _settings = settings;
    }

    public Cell CreateCell(int groupIndex, int row, int col)
    {
        var groupMatrixRow = groupIndex / _settings.GroupPerMatrixRow;
        var groupMatrixCol = groupIndex - (groupMatrixRow * _settings.GroupPerMatrixRow);

        var matrixRow = row + (groupMatrixRow * _settings.RowPerGroup);
        var matrixCol = col + (_settings.GroupRowSize * groupMatrixCol);
        // var cellIndex = groupRowSize * row + col;
        //Console.WriteLine($"G{groupIndex} R{row} C{col} GMR{groupMatrixRow} MR{matrixRow} GMC{groupMatrixCol} MC{matrixCol}");
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
