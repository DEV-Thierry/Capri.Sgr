@CadastroAssociado
Feature: Cadastro de associado
    Cadastro inicial de pessoa física e pessoa jurídica vinculada à associação.

Scenario Outline: Tipo de associado possui cobrança desconto e limite de animais
    Given que o sistema possui os tipos de associado iniciais
    Then o tipo de associado "<Tipo>" deve possuir cobrança "<Cobrança>"
    And o tipo de associado "<Tipo>" deve possuir desconto de "<Desconto>" nos serviços
    And o tipo de associado "<Tipo>" deve permitir no mínimo <Minimo> animais
    And o tipo de associado "<Tipo>" deve permitir no máximo "<Maximo>" animais
    And o tipo de associado "<Tipo>" deve possuir permissão de afixo "<PermiteAfixo>"
    And o tipo de associado "<Tipo>" deve permitir pessoa jurídica "<PermitePessoaJuridica>"

    Examples:
        | Tipo                  | Minimo | Maximo     | Cobrança                                | Desconto | PermiteAfixo | PermitePessoaJuridica |
        | Contribuinte Junior   | 6      | 59         | Trimestralidade de R$ 396,00            | 50%      | Sim           | Sim                    |
        | Contribuinte Senior   | 60     | Ilimitado  | Trimestralidade de R$ 3.150,00          | 50%      | Sim           | Sim                    |
        | Jovem                 | 1      | Ilimitado  | Trimestralidade de R$ 193,00            | 50%      | Sim           | Não                    |
        | Nao Socio Criador     | 1      | Ilimitado  | Sem cobrança                            | 0%       | Não           | Não                    |
        | Remido                | 1      | Ilimitado  | Taxa única de 100 salários mínimos      | 50%      | Sim           | Sim                    |
        | Usuario               | 0      | 5          | Anuidade de R$ 209,00                   | 50%      | Não           | Não                    |

Scenario: Interessado solicita cadastro de associado pessoa física com CPF
    Given que existe o tipo de associado "Contribuinte Junior" que permite sufixo
    And o tipo de associado "Contribuinte Junior" exige os documentos para pessoa física
        | Documento                  |
        | CPF                        |
        | Documento de identidade    |
        | Comprovante de residência  |
        | Termo de adesão            |
    When o interessado solicita cadastro como pessoa física
        | Campo              | Valor                 |
        | Tipo de associado  | Contribuinte Junior   |
        | CPF                | 123.456.789-09        |
        | Nome completo      | Maria Silva           |
        | Nome de exibição   | Maria Silva           |
        | E-mail             | maria@example.com     |
        | Sufixo             | do Gadu               |
    And informa exatamente um telefone principal
        | Tipo     | País | DDD | Número    | Principal |
        | WhatsApp | 55   | 11  | 999999999 | Sim       |
    And informa exatamente um endereço para correspondência
        | Tipo         | CEP       | Cidade    | UF | Correspondência |
        | Residencial  | 01001000  | São Paulo | SP | Sim             |
    And envia todos os documentos obrigatórios
    Then o cadastro do associado deve ser criado com status "Aguardando aprovação"
    And deve existir pendência aberta de "Aguardando aprovação"
    And não deve existir pendência aberta de "Documentação incompleta"

Scenario: Interessado solicita cadastro de associado pessoa jurídica com CNPJ e responsável
    Given que existe o tipo de associado "Contribuinte Senior" que permite prefixo
    And o tipo de associado "Contribuinte Senior" exige os documentos para pessoa jurídica
        | Documento                         |
        | CNPJ                              |
        | Contrato social ou estatuto       |
        | Documento do responsável legal    |
        | Comprovante de endereço           |
        | Termo de adesão                   |
    When o interessado solicita cadastro como pessoa jurídica
        | Campo              | Valor                   |
        | Tipo de associado  | Contribuinte Senior     |
        | CNPJ               | 12.345.678/0001-95      |
        | Razão social       | Haras Gadu Ltda         |
        | Nome fantasia      | Haras Gadu              |
        | Nome de exibição   | Haras Gadu              |
        | E-mail             | contato@harasgadu.test  |
        | Prefixo            | HG                       |
    And vincula o responsável operacional
        | Nome completo | CPF            | E-mail                  |
        | João Pereira  | 987.654.321-00 | joao@harasgadu.test     |
    And informa exatamente um telefone principal
        | Tipo      | País | DDD | Número    | Principal |
        | Comercial | 55   | 31  | 33334444  | Sim       |
    And informa exatamente um endereço para correspondência
        | Tipo       | CEP       | Cidade         | UF | Correspondência |
        | Comercial  | 30140071  | Belo Horizonte | MG | Sim             |
    And envia todos os documentos obrigatórios
    Then o cadastro do associado deve ser criado com status "Aguardando aprovação"
    And deve existir pendência aberta de "Aguardando aprovação"
    And o responsável operacional deve estar vinculado ao associado

Scenario Outline: Não deve permitir pessoa jurídica em tipo de associado exclusivo de pessoa física
    Given que existe o tipo de associado "<Tipo>" exclusivo de pessoa física
    When o interessado solicita cadastro como pessoa jurídica
        | Campo              | Valor                   |
        | Tipo de associado  | <Tipo>                  |
        | CNPJ               | 12.345.678/0001-95      |
        | Razão social       | Haras Gadu Ltda         |
        | Nome fantasia      | Haras Gadu              |
        | Nome de exibição   | Haras Gadu              |
        | E-mail             | contato@harasgadu.test  |
    Then o cadastro deve ser recusado porque pessoa jurídica não pode usar tipo de associado exclusivo de pessoa física

    Examples:
        | Tipo              |
        | Jovem             |
        | Nao Socio Criador |
        | Usuario           |

Scenario: Sistema identifica pendência quando documento obrigatório não é enviado
    Given que existe o tipo de associado "Usuario" que não permite prefixo nem sufixo
    And o tipo de associado "Usuario" exige os documentos para pessoa física
        | Documento                  |
        | CPF                        |
        | Comprovante de residência  |
    When o interessado solicita cadastro como pessoa física
        | Campo              | Valor                  |
        | Tipo de associado  | Usuario                |
        | CPF                | 111.444.777-35         |
        | Nome completo      | Ana Costa              |
        | Nome de exibição   | Ana Costa              |
        | E-mail             | ana@example.com        |
    And informa exatamente um telefone principal
        | Tipo     | País | DDD | Número    | Principal |
        | Celular  | 55   | 21  | 988887777 | Sim       |
    And informa exatamente um endereço para correspondência
        | Tipo         | CEP       | Cidade          | UF | Correspondência |
        | Residencial  | 20040002  | Rio de Janeiro  | RJ | Sim             |
    And envia apenas o documento obrigatório "CPF"
    Then o cadastro do associado deve ser criado com status "Aguardando documentos"
    And deve existir pendência aberta de "Documentação incompleta"

Scenario: Não deve permitir mais de um telefone principal
    Given que existe o tipo de associado "Jovem" que permite sufixo
    When o interessado solicita cadastro como pessoa física
        | Campo              | Valor                  |
        | Tipo de associado  | Jovem                  |
        | CPF                | 529.982.247-25         |
        | Nome completo      | Pedro Almeida          |
        | Nome de exibição   | Pedro Almeida          |
        | E-mail             | pedro@example.com      |
    And informa os telefones
        | Tipo      | País | DDD | Número    | Principal |
        | Celular   | 55   | 11  | 977776666 | Sim       |
        | WhatsApp  | 55   | 11  | 966665555 | Sim       |
    Then o cadastro deve ser recusado porque deve existir exatamente um telefone principal

Scenario: Não deve permitir mais de um endereço para correspondência
    Given que existe o tipo de associado "Remido" que permite sufixo
    When o interessado solicita cadastro como pessoa física
        | Campo              | Valor                  |
        | Tipo de associado  | Remido                 |
        | CPF                | 390.533.447-05         |
        | Nome completo      | Carla Souza            |
        | Nome de exibição   | Carla Souza            |
        | E-mail             | carla@example.com      |
    And informa exatamente um telefone principal
        | Tipo     | País | DDD | Número    | Principal |
        | Celular  | 55   | 41  | 955554444 | Sim       |
    And informa os endereços
        | Tipo         | CEP       | Cidade    | UF | Correspondência |
        | Residencial  | 80010000  | Curitiba  | PR | Sim             |
        | Fazenda      | 84130000  | Palmeira  | PR | Sim             |
    Then o cadastro deve ser recusado porque deve existir exatamente um endereço para correspondência

Scenario Outline: Não deve permitir afixo para tipo de associado sem permissão
    Given que existe o tipo de associado "<Tipo>" que não permite prefixo nem sufixo
    When o interessado solicita cadastro como pessoa física
        | Campo              | Valor                  |
        | Tipo de associado  | <Tipo>                 |
        | CPF                | 153.509.460-56         |
        | Nome completo      | Rafael Lima            |
        | Nome de exibição   | Rafael Lima            |
        | E-mail             | rafael@example.com     |
        | Sufixo             | da Serra               |
    Then o cadastro deve ser recusado porque o tipo de associado não permite afixo

    Examples:
        | Tipo              |
        | Nao Socio Criador |
        | Usuario           |
