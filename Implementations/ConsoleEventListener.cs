using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class ConsoleEventListener : IEventListener
{
    public void OnCellResolved(CellStatus cell)
    {
        Console.WriteLine($"Resolved {cell.ToHumanString()}");
    }
}