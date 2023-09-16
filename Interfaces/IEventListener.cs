namespace Kendoku.Interfaces;

public interface IEventListener
{
    public void OnNothingChanged();

    public void OnEndIteration(int iteration, int cellResolvedCount);
}