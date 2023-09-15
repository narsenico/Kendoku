using Kendoku.Interfaces;
using Kendoku.Models;

class SampleResolverImpl : IResolver
{
    public bool Resolve(CellStatus[] cells,
                        Constraint[] constraints,
                        HelpCell[] helpCells,
                        IEventListener listener)
    {
        throw new NotImplementedException();
    }
}