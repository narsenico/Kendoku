using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class ConsoleEventListener : IEventListener
{
    private readonly IFormatter _actorFormatter;

    public ConsoleEventListener(IFormatter actorFormatter)
    {
        _actorFormatter = actorFormatter;
    }

    public void OnCellResolved(Cell cell, object actor)
    {
        Console.WriteLine($"Cell {cell} resolved with {_actorFormatter.Format(actor)}");
    }

    public void OnEndIteration(int iteration, int cellResolvedCount)
    {
        Console.WriteLine($"End iteration #{iteration}: resolved={cellResolvedCount}");
    }

    public void OnNothingChanged(int iteration)
    {
        Console.WriteLine($"End iteration #{iteration}: noting changed from previous iteration");
    }
}
