using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Infrastructure.IntegrationTests;

public class LegalEntityMembershipApplicationPersistenceTests
{
    [Test]
    public void CnpjIsUniquePerTenantAndPrincipalConstraintIsDatabaseBacked()
    {
        using var context = CreateContext();
        var application = context.Model.FindEntityType(typeof(LegalEntityMembershipApplication))!;
        application.GetIndexes().Single(index => index.Properties.Select(property => property.Name).SequenceEqual(["TenantId", "Cnpj"])).IsUnique.ShouldBeTrue();

        var responsible = context.Model.FindEntityType(typeof(LegalEntityResponsible))!;
        var index = responsible.GetIndexes().Single(index => index.IsUnique);
        index.Properties.Single().Name.ShouldBe(nameof(LegalEntityResponsible.LegalEntityMembershipApplicationId));
        index.GetFilter().ShouldBe("\"IsPrincipal\" = true AND \"IsActive\" = true");
    }

    [Test]
    public void ResponsibleCpfIsUniqueAndUserLinkUsesCompositeIdentity()
    {
        using var context = CreateContext();
        var person = context.Model.FindEntityType(typeof(ResponsiblePerson))!;
        person.GetIndexes().Single(index => index.Properties.Single().Name == nameof(ResponsiblePerson.Cpf)).IsUnique.ShouldBeTrue();
        context.Model.FindEntityType(typeof(ResponsibleUserLink))!.FindPrimaryKey()!.Properties.Select(property => property.Name)
            .ShouldBe([nameof(ResponsibleUserLink.ResponsiblePersonId), nameof(ResponsibleUserLink.UserId)]);
    }

    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseNpgsql("Host=localhost;Database=capri_test;Username=capri;Password=capri")
        .Options);
}