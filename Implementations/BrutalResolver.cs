using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

public class BrutalResolver : IResolver
{
    private readonly IEventListener _listener;
    private readonly IResolver _innerResolver;

    public BrutalResolver(IEventListener listener,
                          IResolver innerResolver)
    {
        _listener = listener;
        _innerResolver = innerResolver;
    }

    public Result Resolve(CellStatus[] cells,
                          Constraint[] constraints,
                          Helper[] helpers)
    {
        // TODO: risolvo con un valore casuale la prima cella con poche possibilità rimaste
        //  sperando in bene
        //  occorre però gestire meglio il caso di celle con 0 possibilità
        //  che può capitare con una certa frequenza
        //  il programma deve uscire fallendo ma senza eccezioni
        //  quindi quando trovo il caso di 0 possibilità devo avere la possibilità 
        //  di tornare indietro e provare con un altro valore

        while (true) 
        {
            var dd = cells.Where(c => c.Possibilities.Count() == 2).FirstOrDefault();
            if (dd == null) {
                break;
            }

            dd.Resolve(dd.Possibilities.First());
            _listener.OnCellResolved(dd.Cell, "brutal ruels");

            var result = _innerResolver.Resolve(cells, constraints, helpers);
            if (result.Success) {
                break;
            }
        }

        return new Result(
                Success: cells.IsResolved(),
                Cells: cells,
                IterationCount: 1);
    }
}
