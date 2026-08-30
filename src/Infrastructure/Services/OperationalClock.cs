using Capri.Sgr.Application.Common.Interfaces;

namespace Capri.Sgr.Infrastructure.Services;

public sealed class OperationalClock(TimeProvider timeProvider) : IOperationalClock
{
    private static readonly TimeZoneInfo SaoPaulo = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public DateTimeOffset GetCurrentInstant() => timeProvider.GetUtcNow();

    public DateOnly GetOperationalDate()
    {
        var saoPauloNow = TimeZoneInfo.ConvertTime(GetCurrentInstant(), SaoPaulo);
        return DateOnly.FromDateTime(saoPauloNow.DateTime);
    }
}
