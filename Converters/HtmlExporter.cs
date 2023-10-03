using Kendoku.Interfaces;
using Kendoku.Models;

using System.Text;

namespace Kendoku.Converters;

internal class HtmlExporter : IExporter
{
    private readonly string _fileName;

    public HtmlExporter(string fileName)
    {
        _fileName = fileName;
    }

    public void Export(MatrixSettings matrixSettings, Result result)
    {
        var buff = new StringBuilder();
        buff.AppendLine("<html>");
        buff = WriteHead(buff, matrixSettings);
        buff = WriterBody(buff, result);
        buff.Append("</html>");

        File.WriteAllText(_fileName, buff.ToString());
    }

    private StringBuilder WriterBody(StringBuilder buff, Result result)
    {
        buff.AppendLine("<body>");
        buff.AppendLine("""<div class="matrix">""");
        buff = result.Cells
            .GroupBy(cell => cell.Cell.GroupIndex)
            .OrderBy(g => g.Key)
            .Aggregate(buff, WriteGroup);
        buff.AppendLine("</div>");
        WriteScript(buff);
        buff.AppendLine("</body>");
        return buff;
    }

    private StringBuilder WriteHead(StringBuilder buff, MatrixSettings matrixSettings)
    {
        buff.AppendLine("<head>");
        buff.AppendLine("<style>");

        buff.Append($@"
            * {{
                box-sizing: border-box;
            }}
            body {{
                font-family: sans-serif;
            }}
            .matrix {{
                --cell-size: 60px;
                display: grid;
                grid-template-columns: repeat({matrixSettings.MatrixRowSize}, 1fr);
                grid-template-rows: repeat({matrixSettings.RowPerMatrix}, 1fr);
                grid-column-gap: 0px;
                grid-row-gap: 0px;
                width: fit-content;
            }}
            .group {{
                display: grid;
                grid-template-columns: repeat({matrixSettings.GroupRowSize}, 1fr);
                grid-template-rows: repeat({matrixSettings.RowPerGroup}, 1fr);
                grid-column-gap: 0px;
                grid-row-gap: 0px;
                border: solid 1px #333;
            }}
            .cell {{
                width: var(--cell-size);
                height: var(--cell-size);
                border: solid 1px #ddd;
                display: flex;
                flex-wrap: wrap;
                align-items: center;
                justify-content: center;
            }}
            .cell.resolved > div {{
                font-size: 2rem;
                height: calc(var(--cell-size) - 4px);
                width: calc(var(--cell-size) - 4px);
                line-height: var(--cell-size);
                text-align: center;
            }} 
           .cell:not(.resolved) > div {{
                border: solid 1px #7e8e96;
                border-radius: 50%;
                padding: 0 3px;
                font-size: 0.7em;
                margin: 0 1px;
                font-weight: bold;
            }}
           .cell>div.sel {{
                background: #FFC107;
           }}
            ");

        buff.AppendLine("</style>");
        buff.AppendLine("</head>");
        return buff;
    }


    private StringBuilder WriteScript(StringBuilder buff)
    {
        buff.AppendLine("<script>");
        buff.AppendLine("""
            function addHighlight(num) {{
                for (let cell of [...document.querySelectorAll(`.cell>div`)].filter(c => c.innerText == num)) {{
                    cell.classList.add('sel');
                }}
            }}
            function clearHighlight() {{
                for (let cell of [...document.querySelectorAll(`.cell>div`)]) {{
                    cell.classList.remove('sel');
                }}
            }}
            document.querySelectorAll('.cell>div').forEach(c => c.addEventListener('mouseenter', e => {{
                clearHighlight();
                addHighlight(e.target.innerText);
            }}));            
            document.querySelectorAll('.cell>div').forEach(c => c.addEventListener('mouseout', e => {{
                clearHighlight();
            }}));            

        """);
        buff.AppendLine("</script>");
        return buff;
    }

    private StringBuilder WriteGroup(StringBuilder buff, IEnumerable<CellStatus> cells)
    {
        buff.Append("""<div class="group">""");
        buff = cells.Aggregate(buff, WriteCell);
        buff.Append("</div>");
        return buff;
    }

    private StringBuilder WriteCell(StringBuilder buff, CellStatus cell)
    {
        buff.Append($"""<div class="cell {(cell.IsResolved ? "resolved" : "")}">""");
        buff = cell.Possibilities.Aggregate(buff, (buff, v) => buff.Append($"""<div>{v}</div>"""));
        buff.Append("</div>");
        return buff;
    }
}
