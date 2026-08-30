using Capri.Sgr.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class DerivedEffectExecutionTests
{
    [Test]
    public void FailedExecutionCanBeRetriedAndThenCompleted()
    {
        var initial = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
        var execution = new DerivedEffectExecution("notification:42", initial);

        execution.MarkFailed(initial.AddMinutes(1), "SMTP unavailable");
        execution.Retry(initial.AddMinutes(2));
        execution.MarkSucceeded(initial.AddMinutes(3));

        execution.Status.ShouldBe(DerivedEffectExecutionStatus.Succeeded);
        execution.Failure.ShouldBeNull();
        execution.FinishedAt.ShouldBe(initial.AddMinutes(3));
    }

    [Test]
    public void SuccessfulExecutionCannotBeRetried()
    {
        var execution = new DerivedEffectExecution("notification:42", DateTimeOffset.UtcNow);
        execution.MarkSucceeded(DateTimeOffset.UtcNow);

        Should.Throw<InvalidOperationException>(() => execution.Retry(DateTimeOffset.UtcNow));
    }
}
