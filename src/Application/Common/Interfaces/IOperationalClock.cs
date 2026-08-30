namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>Provides instants and the association's operational date.</summary>
public interface IOperationalClock
{
    DateTimeOffset GetCurrentInstant();
    DateOnly GetOperationalDate();
}
