using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface IEventListener
{
    public void OnNothingChanged(int iteration);

    public void OnEndIteration(int iteration, int cellResolvedCount);

    public void OnCellResolved(Cell cell, object actor);
}
