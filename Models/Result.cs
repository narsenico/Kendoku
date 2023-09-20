namespace Kendoku.Models;

public record Result(bool Success,
                     int ResolvedCount,
                     int TotalCount,
                     int IterationCount,
                     TimeSpan ExecutionTime);
