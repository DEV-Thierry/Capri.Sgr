# Cadastro de Associado

## Objetivo

Definir o comportamento inicial do cadastro de associado do Capri.Sgr.

O associado representa uma pessoa física ou pessoa jurídica vinculada à associação. Ele poderá possuir animais, criar animais, solicitar operações como transferência de equino e manter dados cadastrais, documentos, telefones, endereços, pendências e informações financeiras relacionadas ao seu tipo de associação.

## Escopo inicial

O cadastro de associado deve suportar:

- Pessoa física.
- Pessoa jurídica, como haras, condomínio ou outra organização.
- Responsável operacional pelo associado.
- CPF ou CNPJ conforme o tipo de pessoa.
- Upload de documentos cadastrais.
- Validação de documentos obrigatórios por tipo de associado/pessoa.
- Processo de aprovação pela equipe da associação.
- Prefixo ou sufixo usado na formação do nome dos animais criados.
- Lista de telefones com exatamente um telefone principal.
- Lista de endereços com exatamente um endereço para correspondência.
- Autoassociação, onde o próprio interessado solicita associação informando o tipo desejado.
- Tipos de associado com cobrança por trimestralidade, anuidade, taxa única ou sem cobrança.
- Tipos de associado que podem ou não possuir prefixo/sufixo.
- Pendências cadastrais, documentais, financeiras ou operacionais.

## Conceitos principais

### Associado

Entidade que representa o vínculo de uma pessoa física ou jurídica com a associação.

Um associado pode ser:

- Pessoa física: identificado por CPF.
- Pessoa jurídica: identificado por CNPJ.

O associado deve ter um status cadastral e pode possuir pendências que impedem ou restringem determinadas operações.

### Responsável

Pessoa autorizada a realizar operações em nome do associado.

Para pessoa jurídica, o responsável é obrigatório para operações como:

- Solicitar transferência de equino.
- Atualizar dados cadastrais, quando permitido.
- Enviar documentos.
- Resolver pendências.
- Interagir com a associação em processos administrativos.

Para pessoa física, o próprio associado pode ser o responsável. Ainda assim, o modelo deve permitir que uma pessoa física tenha representantes ou procuradores no futuro, caso essa regra seja necessária.

### Tipo de associado

Categoria escolhida ou atribuída ao associado.

Cada tipo de associado deve definir:

- Nome.
- Descrição.
- Tipo de pessoa permitido.
- Se permite prefixo/sufixo.
- Se exige aprovação pela equipe da associação.
- Quantidade mínima de animais.
- Quantidade máxima de animais, quando houver limite.
- Valor de trimestralidade, quando aplicável.
- Valor de anuidade, quando aplicável.
- Valor de taxa única, quando aplicável.
- Percentual de desconto nos serviços da associação.
- Documentos obrigatórios para pessoa física.
- Documentos obrigatórios para pessoa jurídica.
- Permissões ou restrições operacionais.

Tipos de associado iniciais do sistema:

| Tipo de associado | Tipo de pessoa permitido | Mínimo de animais | Máximo de animais | Cobrança | Desconto nos serviços | Permite afixo |
| --- | --- | ---: | ---: | --- | ---: | --- |
| Contribuinte Junior | Pessoa física ou jurídica | 6 | 59 | Trimestralidade de R$ 396,00 | 50% | Sim |
| Contribuinte Senior | Pessoa física ou jurídica | 60 | Ilimitado | Trimestralidade de R$ 3.150,00 | 50% | Sim |
| Jovem | Somente pessoa física | 1 | Ilimitado | Trimestralidade de R$ 193,00 | 50% | Sim |
| Nao Socio Criador | Somente pessoa física | 1 | Ilimitado | Sem cobrança recorrente | 0% | Não |
| Remido | Pessoa física ou jurídica | 1 | Ilimitado | Taxa única de 100 salários mínimos | 50% | Sim |
| Usuario | Somente pessoa física | 0 | 5 | Anuidade de R$ 209,00 | 50% | Não |

O sistema deve cadastrar e validar exatamente esses tipos de associado no escopo inicial. Alterações futuras nos tipos, valores ou limites devem ser tratadas como mudança de regra de negócio.

### Afixo: prefixo ou sufixo

Identificador textual usado para compor o nome de animais criados pelo associado.

Exemplo:

- Nome base do animal: `Diamante`
- Sufixo do associado criador: `do Gadu`
- Nome final do animal: `Diamante do Gadu`

Regra inicial:

- O associado pode ter um prefixo ou um sufixo.
- Prefixo e sufixo são tratados como afixo do associado.
- `Nao Socio Criador` não pode ter prefixo nem sufixo.
- `Usuario` não pode ter prefixo nem sufixo.
- A obrigatoriedade de prefixo/sufixo deve ser definida pelo tipo de associado ou por regra da associação.
- O prefixo/sufixo pertence ao associado criador.
- Ao criar um animal, o sistema deve copiar o prefixo/sufixo vigente do associado criador para formar o nome registrado do animal.

Observação importante: o nome do animal não deve depender dinamicamente do cadastro atual do associado. Se o associado alterar seu sufixo no futuro, os animais já registrados não devem mudar automaticamente sem um processo formal de retificação.

### Criador e proprietário do animal

Para o cadastro de animal, devem existir dois vínculos com associado:

- Associado criador: associado que criou o animal.
- Associado proprietário: associado que é dono do animal no momento atual.

Com isso, o associado terá um plantel de animais, formado principalmente pelos animais em que ele é proprietário atual.

Um animal pode ter histórico de proprietários, mas o cadastro inicial deve deixar claro qual associado é o proprietário atual.

## Dados cadastrais do associado

### Campos comuns

Campos aplicáveis a pessoa física e pessoa jurídica:

- Identificador interno.
- Tipo de pessoa: física ou jurídica.
- Tipo de associado.
- Documento principal: CPF ou CNPJ.
- Nome de exibição.
- Status cadastral.
- Status de aprovação.
- Prefixo ou sufixo, quando permitido.
- Telefones.
- Endereços.
- Documentos enviados.
- Responsáveis.
- Pendências.
- Data de criação.
- Data da última atualização.

### Pessoa física

Campos recomendados:

- CPF.
- Nome completo.
- Nome social, se aplicável.
- Data de nascimento.
- E-mail.
- Documento de identidade, se aplicável.

### Pessoa jurídica

Campos recomendados:

- CNPJ.
- Razão social.
- Nome fantasia.
- Inscrição estadual ou municipal, se aplicável.
- E-mail institucional.
- Responsável principal.

Exemplos de pessoa jurídica:

- Haras.
- Condomínio.
- Empresa.
- Associação ou entidade parceira.

Regras:

- Pessoa jurídica não pode se associar como `Jovem`.
- Pessoa jurídica não pode se associar como `Nao Socio Criador`.
- Pessoa jurídica não pode se associar como `Usuario`.
- Pessoa jurídica pode se associar como `Contribuinte Junior`, `Contribuinte Senior` ou `Remido`, desde que atenda às demais regras do tipo de associado.

## Telefones

O associado pode ter vários telefones.

Regras:

- Deve existir exatamente um telefone principal.
- Não pode existir mais de um telefone principal.
- Não deve ser possível aprovar um associado sem telefone principal.
- O telefone principal deve ser usado como contato preferencial pela associação.

Campos recomendados:

- Tipo: celular, comercial, residencial, WhatsApp ou outro.
- Código do país.
- DDD.
- Número.
- Ramal, quando aplicável.
- Indica se é principal.
- Observação.

## Endereços

O associado pode ter vários endereços.

Regras:

- Deve existir exatamente um endereço para correspondência.
- Não pode existir mais de um endereço para correspondência.
- Não deve ser possível aprovar um associado sem endereço para correspondência.

Campos recomendados:

- Tipo: residencial, comercial, fazenda, haras, cobrança, correspondência ou outro.
- CEP.
- Logradouro.
- Número.
- Complemento.
- Bairro.
- Cidade.
- UF.
- País.
- Indica se é endereço para correspondência.
- Observação.

## Documentos

O associado deve realizar upload dos documentos exigidos para seu tipo de pessoa e tipo de associado.

### Regras gerais

- Cada tipo de associado pode exigir documentos diferentes.
- Documentos obrigatórios podem variar entre pessoa física e pessoa jurídica.
- O sistema deve identificar pendência quando faltar documento obrigatório.
- O associado pode enviar documentos durante o autoatendimento.
- A equipe da associação deve poder aprovar, rejeitar ou solicitar reenvio de documentos.
- Documento rejeitado deve gerar pendência.
- Documento aprovado deve compor o dossiê cadastral do associado.

### Exemplos de documentos para pessoa física

- CPF.
- Documento de identidade.
- Comprovante de residência.
- Termo de adesão.

### Exemplos de documentos para pessoa jurídica

- CNPJ.
- Contrato social ou estatuto.
- Documento do responsável legal.
- Comprovante de endereço.
- Termo de adesão.

Os documentos definitivos devem ser configuráveis, pois cada associação pode ter exigências próprias.

## Aprovação cadastral

O associado passa por um processo de aprovação pela equipe da associação.

### Fluxo sugerido

1. Interessado informa o tipo de pessoa e o tipo de associado desejado.
2. Sistema calcula os documentos obrigatórios.
3. Interessado preenche os dados cadastrais.
4. Interessado informa telefones e endereços.
5. Interessado envia os documentos obrigatórios.
6. Sistema cria pendências automáticas, quando necessário.
7. Equipe da associação analisa o cadastro.
8. Equipe aprova, rejeita ou solicita correção.
9. Após aprovação, o associado fica apto a realizar operações permitidas pelo seu tipo de associado.

### Status cadastral sugeridos

- Rascunho: cadastro iniciado, mas ainda incompleto.
- Aguardando documentos: faltam documentos obrigatórios.
- Aguardando aprovação: documentação mínima enviada e cadastro pendente de análise.
- Aprovado: cadastro aprovado pela equipe da associação.
- Rejeitado: cadastro recusado.
- Suspenso: associado temporariamente impedido de operar.
- Inativo: associado não está mais ativo na associação.

## Pendências

Pendência é qualquer item que exige ação do associado ou da equipe da associação.

### Pendências iniciais

- Documentação incompleta: faltam documentos obrigatórios.
- Aguardando aprovação: cadastro ainda não aprovado pela equipe.
- Documento rejeitado: documento precisa ser reenviado.
- Dados cadastrais incompletos: campos obrigatórios não preenchidos.
- Telefone principal ausente: nenhum telefone principal definido.
- Endereço de correspondência ausente: nenhum endereço de correspondência definido.
- Pendência financeira: mensalidade, anuidade ou taxa em aberto.

### Regras gerais

- Um associado pode ter várias pendências abertas.
- Uma pendência deve ter status.
- Uma pendência pode bloquear operações específicas.
- Ao resolver a causa da pendência, o sistema deve encerrá-la automaticamente quando possível.
- Pendências manuais podem ser abertas pela equipe da associação.

### Status de pendência sugeridos

- Aberta.
- Em análise.
- Resolvida.
- Cancelada.

## Autoassociação

O sistema deve permitir que o interessado solicite sua própria associação.

Dados mínimos:

- Tipo de pessoa.
- CPF ou CNPJ.
- Tipo de associado desejado.
- Dados cadastrais obrigatórios.
- Telefone principal.
- Endereço para correspondência.
- Documentos obrigatórios.

Após o envio:

- O cadastro não deve ser considerado aprovado automaticamente, salvo regra explícita da associação.
- O associado deve receber status compatível com sua situação, como `Aguardando documentos` ou `Aguardando aprovação`.
- Pendências automáticas devem ser criadas conforme dados/documentos ausentes.

## Financeiro do tipo de associado

Cada tipo de associado define sua regra financeira.

Regras iniciais:

- Um tipo de associado pode ter trimestralidade.
- Um tipo de associado pode ter anuidade.
- Um tipo de associado pode ter taxa única.
- Um tipo de associado pode não ter cobrança recorrente.
- `Contribuinte Junior` paga trimestralidade de R$ 396,00.
- `Contribuinte Senior` paga trimestralidade de R$ 3.150,00.
- `Jovem` paga trimestralidade de R$ 193,00.
- `Nao Socio Criador` não paga mensalidade, trimestralidade, anuidade ou taxa única.
- `Remido` paga taxa única de 100 salários mínimos.
- `Usuario` paga anuidade de R$ 209,00.
- Todos os tipos têm 50% de desconto nos serviços da associação, exceto `Nao Socio Criador`, que não possui desconto e paga 100% das taxas de serviços.
- A existência de cobrança pode gerar pendência financeira quando houver débito em aberto.
- Pendência financeira pode bloquear operações, conforme configuração da associação.

## Limite de animais por tipo de associado

O tipo de associado define a quantidade mínima e máxima de animais que o associado pode manter.

Regras iniciais:

- `Contribuinte Junior` deve possuir no mínimo 6 e no máximo 59 animais.
- `Contribuinte Senior` deve possuir no mínimo 60 animais e não possui limite máximo.
- `Jovem` deve possuir no mínimo 1 animal e não possui limite máximo.
- `Nao Socio Criador` deve possuir no mínimo 1 animal e não possui limite máximo.
- `Remido` deve possuir no mínimo 1 animal e não possui limite máximo.
- `Usuario` pode possuir de 0 a 5 animais.
- O sistema deve validar os limites antes de aprovar operações que alterem a quantidade de animais do associado.

## Operações condicionadas ao status do associado

Determinadas operações devem depender da situação cadastral do associado.

Exemplos:

- Criar animal.
- Solicitar transferência de equino.
- Receber transferência de equino.
- Alterar dados do plantel.
- Solicitar segunda via de documentos.

Regra recomendada:

- Associado não aprovado não deve executar operações críticas.
- Associado com pendência bloqueante não deve executar operações bloqueadas pela pendência.
- Associado suspenso não deve executar operações operacionais até regularização.

## Modelo de domínio sugerido

Entidades e objetos recomendados para implementação inicial:

- `Associado`
- `TipoAssociado`
- `ResponsavelAssociado`
- `TelefoneAssociado`
- `EnderecoAssociado`
- `DocumentoAssociado`
- `TipoDocumentoAssociado`
- `PendenciaAssociado`
- `Animal`
- `HistoricoPropriedadeAnimal`, quando o histórico de proprietários for implementado

Enums ou value objects sugeridos:

- `TipoPessoa`: física ou jurídica.
- `TipoNomeAnimal`: prefixo ou sufixo.
- `StatusAssociado`.
- `StatusAprovacaoAssociado`.
- `StatusDocumentoAssociado`.
- `StatusPendencia`.
- `TipoPendenciaAssociado`.
- `Cpf`.
- `Cnpj`.
- `Email`.
- `Telefone`.
- `Endereco`.

## Invariantes de domínio

As seguintes regras devem ser protegidas no domínio ou na camada de aplicação:

- Associado pessoa física deve possuir CPF válido.
- Associado pessoa jurídica deve possuir CNPJ válido.
- CPF/CNPJ deve ser único dentro da associação.
- Tipo de associado deve ser um dos tipos iniciais definidos pelo sistema: `Contribuinte Junior`, `Contribuinte Senior`, `Jovem`, `Nao Socio Criador`, `Remido` ou `Usuario`.
- Pessoa jurídica não pode usar tipo de associado exclusivo de pessoa física.
- `Jovem`, `Nao Socio Criador` e `Usuario` são tipos exclusivos de pessoa física.
- Associado não deve ultrapassar o limite máximo de animais definido pelo tipo de associado.
- Associado deve atender ao mínimo de animais definido pelo tipo de associado para permanecer enquadrado nesse tipo, salvo regra operacional temporária de regularização.
- Associado deve ter exatamente um telefone principal para ser aprovado.
- Associado deve ter exatamente um endereço de correspondência para ser aprovado.
- Tipo de associado define se prefixo/sufixo é permitido.
- `Nao Socio Criador` e `Usuario` não podem possuir afixo.
- Associado não pode possuir prefixo/sufixo quando seu tipo não permite.
- Associado não pode ser aprovado com documentos obrigatórios pendentes.
- Documento obrigatório rejeitado mantém ou cria pendência.
- Animal deve ter associado criador.
- Animal deve ter associado proprietário atual.
- Nome registrado do animal deve preservar o prefixo/sufixo aplicado no momento do registro.

## Multi-tenant futuro

O desenvolvimento inicial será single-tenant, mas deve evitar decisões que dificultem o suporte futuro a múltiplas associações.

Diretrizes:

- Evitar regras fixas de uma associação específica dentro do código.
- Tipos de associado começam como catálogo fechado no sistema, mas a modelagem deve evitar impedir parametrização futura por associação.
- Tipos de documentos obrigatórios devem ser configuráveis.
- Regras de bloqueio por pendência devem ser configuráveis.
- Nomes, valores e permissões devem considerar o contexto da associação.
- Não assumir que CPF/CNPJ é globalmente único em todo o sistema; a unicidade deve ser pelo tenant/associação.

Arquitetura futura prevista:

- Identificação do tenant por subdomínio.
- Separação dos dados por schema no banco de dados.
- Cada associação terá suas próprias configurações operacionais.

## Questões em aberto

Pontos que ainda precisam de decisão:

- Quais documentos serão obrigatórios para pessoa física?
- Quais documentos serão obrigatórios para pessoa jurídica?
- Quais tipos poderão ter prefixo/sufixo?
- Prefixo/sufixo será obrigatório para algum tipo?
- Quais pendências bloqueiam quais operações?
- A aprovação será feita por um único usuário ou exigirá múltiplos aprovadores?
- O associado poderá alterar CPF/CNPJ após aprovado?
- A transferência de animal exigirá assinatura ou documento anexado?
- Haverá integração com pagamento para mensalidade/anuidade?

## Critérios de aceite iniciais

- Deve ser possível cadastrar associado pessoa física com CPF.
- Deve ser possível cadastrar associado pessoa jurídica com CNPJ.
- Deve ser possível vincular responsável a pessoa jurídica.
- Deve ser possível definir o tipo de associado.
- O tipo de associado deve ser um entre `Contribuinte Junior`, `Contribuinte Senior`, `Jovem`, `Nao Socio Criador`, `Remido` e `Usuario`.
- Deve ser possível consultar a cobrança, o desconto em serviços e os limites de animais de cada tipo de associado.
- Deve ser possível validar o limite mínimo e máximo de animais por tipo de associado.
- Deve ser possível impedir associação de pessoa jurídica nos tipos exclusivos de pessoa física: `Jovem`, `Nao Socio Criador` e `Usuario`.
- Deve ser possível configurar se o tipo de associado permite prefixo/sufixo.
- Deve ser possível cadastrar prefixo ou sufixo quando permitido.
- Deve ser possível cadastrar múltiplos telefones mantendo exatamente um principal.
- Deve ser possível cadastrar múltiplos endereços mantendo exatamente um de correspondência.
- Deve ser possível exigir documentos obrigatórios por tipo de pessoa e tipo de associado.
- Deve ser possível enviar documentos.
- Deve ser possível identificar pendência de documentação incompleta.
- Deve ser possível identificar pendência de aprovação.
- Deve ser possível aprovar ou rejeitar cadastro pela equipe da associação.
- Associado aprovado deve ficar apto às operações permitidas pelo seu tipo.
- Associado com pendência bloqueante não deve executar operações bloqueadas.
