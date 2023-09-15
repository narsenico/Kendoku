using Kendoku.Interfaces;
using Kendoku.Models;

namespace Kendoku.Implementations;

class SimpleResolverImpl : IResolver
{
    private readonly IEventListener _listener;

    public SimpleResolverImpl(IEventListener listener)
    {
        _listener = listener;
    }

    public bool Resolve(CellStatus[] cells,
                        Constraint[] constraints,
                        HelpCell[] helpCells)
    {
        // 1. applico gli aiuti
        // 2. inizio iterazione
        //  - per ogni cella applico constraint
        //  - per ogni cella applico regole sudoku
        //  - se non ci sono state modifiche nello stato della matrice esco (altrimenti loop infinito)
        //  - reitero fino a che tutte le celle non sono risolte

        ApplyHelps(cells, helpCells);

        // throw new NotImplementedException();
        return false;
    }

    private void ApplyHelps(CellStatus[] cells,
                            HelpCell[] helpCells)
    {
        foreach (var help in helpCells)
        {
            var cell = cells.Find(help.Cell);
            cell.Resolve(help.Value);
            _listener.OnCellResolved(cell);
        }
    }
}