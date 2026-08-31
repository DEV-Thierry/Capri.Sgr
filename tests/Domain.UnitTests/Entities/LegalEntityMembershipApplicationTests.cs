using Capri.Sgr.Domain.Entities;
using Capri.Sgr.Domain.ValueObjects;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class LegalEntityMembershipApplicationTests
{
    [Test]
    public void SubmitRequiresExactlyOneActivePrincipalResponsibleAndPjDocuments()
    {
        var application = CreateApplication();
        var principal = CreatePerson("529.982.247-25", "Principal");
        application.AddResponsible(principal, true);
        application.AddResponsible(CreatePerson("111.444.777-35", "Adicional"), false);

        application.Submit(LegalEntityMembershipApplication.RequiredDocumentTypes.ToArray());

        application.Status.ShouldBe(LegalEntityApplicationStatus.Submitted);
    }

    [Test]
    public void SubmissionAndApprovalRejectMissingPrincipalOrRequiredDocuments()
    {
        var application = CreateApplication();
        application.AddResponsible(CreatePerson("529.982.247-25", "Principal"), true);
        application.DeactivateResponsible(application.Responsibles.Single().ResponsiblePersonId);

        Should.Throw<InvalidOperationException>(() => application.Submit(LegalEntityMembershipApplication.RequiredDocumentTypes.ToArray()))
            .Message.ShouldContain("Exactly one active principal");

        var complete = CreateApplication();
        complete.AddResponsible(CreatePerson("111.444.777-35", "Principal"), true);
        Should.Throw<InvalidOperationException>(() => complete.Submit(["CNPJ", "ContratoOuEstatuto"]))
            .Message.ShouldContain("TermoDeAdesao");
    }

    [Test]
    public void OnlyOneActivePrincipalCanBeLinkedAndResponsibleIdentityCanBeReused()
    {
        var person = CreatePerson("529.982.247-25", "Reutilizável");
        var first = CreateApplication();
        first.AddResponsible(person, true);
        var second = new LegalEntityMembershipApplication("tenant", "11.222.333/0001-81", "Outra Razão", "outra@empresa.test", "dossier-2");
        second.AddResponsible(person, false);

        Should.Throw<InvalidOperationException>(() => first.AddResponsible(CreatePerson("111.444.777-35", "Outro"), true));
        second.HasActiveResponsible(person.Id).ShouldBeTrue();
    }

    [Test]
    public void ApprovalRequiresAllCurrentPjDocumentsToBeApproved()
    {
        var application = CreateApplication();
        application.AddResponsible(CreatePerson("529.982.247-25", "Principal"), true);
        application.Submit(LegalEntityMembershipApplication.RequiredDocumentTypes.ToArray());

        Should.Throw<InvalidOperationException>(() => application.Approve(["CNPJ", "ContratoOuEstatuto", "ComprovanteDeEndereco", "TermoDeAdesao"]));
        application.Approve(LegalEntityMembershipApplication.RequiredDocumentTypes.ToArray());
        application.Status.ShouldBe(LegalEntityApplicationStatus.Approved);
    }

    private static LegalEntityMembershipApplication CreateApplication() => new("tenant", "04.252.011/0001-10", "Empresa Teste Ltda", "contato@empresa.test", "dossier-1");
    private static ResponsiblePerson CreatePerson(string cpf, string name) => new(new Cpf(cpf), name, name.ToLowerInvariant() + "@empresa.test", "+55 11 99999-9999");
}