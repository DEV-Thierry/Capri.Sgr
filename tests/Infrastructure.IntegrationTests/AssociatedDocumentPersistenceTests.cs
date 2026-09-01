using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Infrastructure.IntegrationTests;

public class AssociatedDocumentPersistenceTests
{
    [Test]
    public void DocumentVersionNumberIsUniqueWithinItsDossierDocument()
    {
        using var context = CreateContext();
        var index = context.Model.FindEntityType(typeof(AssociatedDocumentVersion))!
            .GetIndexes()
            .Single(candidate => candidate.Properties.Select(property => property.Name)
                .SequenceEqual(["AssociatedDocumentId", nameof(AssociatedDocumentVersion.Number)]));

        index.IsUnique.ShouldBeTrue();
    }

    [Test]
    public void VersionsUseRestrictiveDeleteToPreserveDossierHistory()
    {
        using var context = CreateContext();
        var foreignKey = context.Model.FindEntityType(typeof(AssociatedDocumentVersion))!
            .GetForeignKeys()
            .Single(candidate => candidate.PrincipalEntityType.ClrType == typeof(AssociatedDocument));

        foreignKey.DeleteBehavior.ShouldBe(DeleteBehavior.Restrict);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=capri_test;Username=capri;Password=capri")
            .Options;
        return new ApplicationDbContext(options);
    }
}
