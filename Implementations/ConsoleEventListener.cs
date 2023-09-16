using Kendoku.Interfaces;

namespace Kendoku.Implementations;

public class ConsoleEventListener : IEventListener
{
    public void OnEndIteration(int iteration, int cellResolvedCount)
    {
        Console.WriteLine($"End iteration #{iteration}: resolved={cellResolvedCount}");
    }

    public void OnNothingChanged()
    {
        Console.WriteLine("Noting changed from previous iteration");
    }
}