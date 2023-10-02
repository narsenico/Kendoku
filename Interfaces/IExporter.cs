using Kendoku.Models;

namespace Kendoku.Interfaces;

public interface IExporter
{
    string Export(MatrixSettings matrixSettings, Result result);
}
