using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface ICellFactory
{
    Cell CreateCell(int groupIndex, int row, int col);
    IEnumerable<Cell> CreateGroupCells(int groupIndex);
}