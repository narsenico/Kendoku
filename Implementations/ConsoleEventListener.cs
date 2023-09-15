using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class ConsoleEventListener : IEventListener
{
    public void OnCellResolved(CellStatus cell)
    {
        Console.WriteLine($"Resolved {cell.ToHumanString()}");
    }

    public void OnEndIteration(int iteration)
    {
        Console.WriteLine($"End iteration #{iteration}");
    }

    public void OnNothingChanged()
    {
        Console.WriteLine("Noting changed from previous iteration");
    }
}