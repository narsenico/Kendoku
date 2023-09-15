using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface IEventListener
{
    public void OnCellResolved(CellStatus cell);
}