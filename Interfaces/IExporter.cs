using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface IExporter
{
    void Export(MatrixSettings matrixSettings, Result result);
}
