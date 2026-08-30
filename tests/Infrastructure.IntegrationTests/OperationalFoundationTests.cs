using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Infrastructure.IntegrationTests;

public class OperationalFoundationTests
{
    [Test]
    public void AuditRecordPreservesRequiredAuditEvidence()
    {
        var occurredAt = new DateTimeOffset(2026, 4, 1, 9, 30, 0, TimeSpan.FromHours(-3));
        var record = new AuditRecord("operator-7", occurredAt, "web", "Associado", "42", "Aprovado", "documentos validados", "{\"situacao\":\"Pendente\"}", "{\"situacao\":\"Ativo\"}");

        record.ActorId.ShouldBe("operator-7");
        record.OccurredAt.ShouldBe(occurredAt);
        record.Channel.ShouldBe("web");
        record.EntityType.ShouldBe("Associado");
        record.EntityId.ShouldBe("42");
        record.Action.ShouldBe("Aprovado");
        record.Reason.ShouldBe("documentos validados");
        record.Before.ShouldBe("{\"situacao\":\"Pendente\"}");
        record.After.ShouldBe("{\"situacao\":\"Ativo\"}");
    }

    [Test]
    public void DerivedEffectExecutionKeyHasDatabaseUniqueIndex()
    {
        using var context = CreateContext();
        var index = context.Model.FindEntityType(typeof(DerivedEffectExecution))!
            .GetIndexes()
            .Single(candidate => candidate.Properties.Single().Name == nameof(DerivedEffectExecution.ExecutionKey));

        index.IsUnique.ShouldBeTrue();
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=capri_test;Username=capri;Password=capri")
            .Options;
        return new ApplicationDbContext(options);
    }
}
