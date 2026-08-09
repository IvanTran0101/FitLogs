using System;

namespace FitLogs.ExternalServices.OpenFoodFacts;

/// <summary>Stops barcode traffic briefly after repeated transient upstream failures.</summary>
public sealed class OpenFoodFactsCircuitBreaker
{
    private readonly object _gate = new();
    private int _consecutiveFailures;
    private DateTimeOffset _openUntil;

    public bool TryEnter()
    {
        lock (_gate)
        {
            return DateTimeOffset.UtcNow >= _openUntil;
        }
    }

    public void RecordSuccess()
    {
        lock (_gate)
        {
            _consecutiveFailures = 0;
            _openUntil = DateTimeOffset.MinValue;
        }
    }

    public void RecordTransientFailure()
    {
        lock (_gate)
        {
            _consecutiveFailures++;
            if (_consecutiveFailures >= 3)
            {
                _openUntil = DateTimeOffset.UtcNow.AddSeconds(30);
            }
        }
    }
}
