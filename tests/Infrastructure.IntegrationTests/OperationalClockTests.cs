using Capri.Sgr.Infrastructure.Services;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Infrastructure.IntegrationTests;

public class OperationalClockTests
{
    [TestCase("2026-04-01T02:59:00+00:00", "2026-03-31")]
    [TestCase("2026-04-01T03:00:00+00:00", "2026-04-01")]
    public void OperationalDateUsesAmericaSaoPaulo(string instantText, string expectedDate)
    {
        var instant = DateTimeOffset.Parse(instantText, null, System.Globalization.DateTimeStyles.RoundtripKind);
        var clock = new OperationalClock(new FixedTimeProvider(instant));

        clock.GetOperationalDate().ShouldBe(DateOnly.Parse(expectedDate));
    }

    private sealed class FixedTimeProvider(DateTimeOffset instant) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => instant;
    }
}
