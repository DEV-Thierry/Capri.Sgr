namespace Capri.Sgr.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class CadastroAssociadoStepDefinitions
{
    [Given("que o sistema possui os tipos de associado iniciais")]
    public void DadoQueOSistemaPossuiOsTiposDeAssociadoIniciais()
    {
        Pendente();
    }

    [Given("que existe o tipo de associado {string} que permite sufixo")]
    [Given("que existe o tipo de associado {string} que permite prefixo")]
    [Given("que existe o tipo de associado {string} que não permite prefixo nem sufixo")]
    [Given("que existe o tipo de associado {string} exclusivo de pessoa física")]
    public void DadoQueExisteUmTipoDeAssociado(string nome)
    {
        Pendente();
    }

    [Given("o tipo de associado {string} exige os documentos para pessoa física")]
    [Given("o tipo de associado {string} exige os documentos para pessoa jurídica")]
    public void DadoOTipoDeAssociadoExigeOsDocumentos(string nome, DataTable documentos)
    {
        Pendente();
    }

    [When("o interessado solicita cadastro como pessoa física")]
    [When("o interessado solicita cadastro como pessoa jurídica")]
    public void QuandoOInteressadoSolicitaCadastro(DataTable dadosCadastrais)
    {
        Pendente();
    }

    [When("informa exatamente um telefone principal")]
    [When("informa os telefones")]
    public void QuandoInformaTelefones(DataTable telefones)
    {
        Pendente();
    }

    [When("informa exatamente um endereço para correspondência")]
    [When("informa os endereços")]
    public void QuandoInformaEnderecos(DataTable enderecos)
    {
        Pendente();
    }

    [When("envia todos os documentos obrigatórios")]
    public void QuandoEnviaTodosOsDocumentosObrigatorios()
    {
        Pendente();
    }

    [When("envia apenas o documento obrigatório {string}")]
    public void QuandoEnviaApenasODocumentoObrigatorio(string documento)
    {
        Pendente();
    }

    [When("vincula o responsável operacional")]
    public void QuandoVinculaOResponsavelOperacional(DataTable responsavel)
    {
        Pendente();
    }

    [Then("o cadastro do associado deve ser criado com status {string}")]
    public void EntaoOCadastroDoAssociadoDeveSerCriadoComStatus(string status)
    {
        Pendente();
    }

    [Then("o tipo de associado {string} deve possuir cobrança {string}")]
    public void EntaoOTipoDeAssociadoDevePossuirCobranca(string tipoAssociado, string cobranca)
    {
        Pendente();
    }

    [Then("o tipo de associado {string} deve possuir desconto de {string} nos serviços")]
    public void EntaoOTipoDeAssociadoDevePossuirDescontoNosServicos(string tipoAssociado, string desconto)
    {
        Pendente();
    }

    [Then("o tipo de associado {string} deve permitir no mínimo {int} animais")]
    public void EntaoOTipoDeAssociadoDevePermitirNoMinimoAnimais(string tipoAssociado, int minimo)
    {
        Pendente();
    }

    [Then("o tipo de associado {string} deve permitir no máximo {string} animais")]
    public void EntaoOTipoDeAssociadoDevePermitirNoMaximoAnimais(string tipoAssociado, string maximo)
    {
        Pendente();
    }

    [Then("o tipo de associado {string} deve possuir permissão de afixo {string}")]
    public void EntaoOTipoDeAssociadoDevePossuirPermissaoDeAfixo(string tipoAssociado, string permiteAfixo)
    {
        Pendente();
    }

    [Then("o tipo de associado {string} deve permitir pessoa jurídica {string}")]
    public void EntaoOTipoDeAssociadoDevePermitirPessoaJuridica(string tipoAssociado, string permitePessoaJuridica)
    {
        Pendente();
    }

    [Then("deve existir pendência aberta de {string}")]
    [Then("não deve existir pendência aberta de {string}")]
    public void EntaoDeveValidarPendenciaAberta(string tipoPendencia)
    {
        Pendente();
    }

    [Then("o responsável operacional deve estar vinculado ao associado")]
    public void EntaoOResponsavelOperacionalDeveEstarVinculadoAoAssociado()
    {
        Pendente();
    }

    [Then("o cadastro deve ser recusado porque deve existir exatamente um telefone principal")]
    [Then("o cadastro deve ser recusado porque deve existir exatamente um endereço para correspondência")]
    [Then("o cadastro deve ser recusado porque o tipo de associado não permite prefixo ou sufixo")]
    [Then("o cadastro deve ser recusado porque o tipo de associado não permite afixo")]
    [Then("o cadastro deve ser recusado porque pessoa jurídica não pode usar tipo de associado exclusivo de pessoa física")]
    public void EntaoOCadastroDeveSerRecusado()
    {
        Pendente();
    }

    private static void Pendente()
    {
        Assert.Ignore("Cadastro de associado ainda não possui UI/API/domínio implementado. Este cenário define o comportamento esperado a partir da documentação.");
    }
}
