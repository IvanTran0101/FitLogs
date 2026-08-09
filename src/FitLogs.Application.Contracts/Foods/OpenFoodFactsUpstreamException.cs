using System;

namespace FitLogs.Foods;

public enum OpenFoodFactsFailureKind
{
    InvalidData,
    Timeout,
    Unavailable
}

public sealed class OpenFoodFactsUpstreamException : Exception
{
    public OpenFoodFactsFailureKind Kind { get; }

    public OpenFoodFactsUpstreamException(OpenFoodFactsFailureKind kind, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
    }
}
