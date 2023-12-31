using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class ConsoleEventListener : IEventListener
{
    private readonly IFormatter _actorFormatter;
    private readonly bool _verbose;

    public ConsoleEventListener(IFormatter actorFormatter,
                                bool verbose)
    {
        _actorFormatter = actorFormatter;
        _verbose = verbose;
    }

    public void OnCellResolved(Cell cell, object actor)
    {
        if (!_verbose) return;
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
