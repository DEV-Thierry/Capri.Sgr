# Coberturas

## Objetivo

Especificar a comunicação e o acompanhamento de Coberturas no Capri.Sgr. Uma Cobertura registra um evento reprodutivo entre um Garanhão e uma Égua reprodutora, coordena validações, obrigações e rastreabilidade, e permite vincular uma ou mais Crias à sua genealogia.

Esta especificação descreve regras de negócio independentes do sistema legado. Não define nomes de campos, tabelas, endpoints, provedores de armazenamento ou mecanismos de integração.

## Escopo e limites

O escopo abrange comunicação, validação pelos proprietários e pelo veterinário quando aplicável, documentos, Pendências, Cobranças, multa por comunicação tardia, Declaração de transferência, Desfecho reprodutivo, Crias, consulta, auditoria e notificações.

Associado, Animal, propriedade, cadastro de Cria, Pagamentos, Evidências documentais e notificações são domínios próprios. Cobertura define seus contratos e efeitos de negócio, mas não duplica suas regras nem seus dados. A implementação inicial depende de contratos explícitos com esses domínios.

## Linguagem e participantes

Usar os termos canônicos de [CONTEXT.md](../../CONTEXT.md), em especial Cobertura, Garanhão, Égua reprodutora, Comunicante, Pagador, Situação da Cobertura, Desfecho reprodutivo, Cria e Pendência.

Em cada Cobertura participam:

- **Garanhão** e seu Proprietário participante;
- **Égua reprodutora** e seu Proprietário participante;
- **Comunicante**, que é obrigatoriamente um dos Proprietários participantes;
- **Pagador**, que responde pelas Cobranças da Cobertura e, por padrão, é o Comunicante;
- **Veterinário designado**, quando a Modalidade exigir Validação veterinária;
- **Beneficiário de destino**, quando o Pagador for terceiro e houver Declaração de transferência.

O usuário autenticado deve estar autorizado a agir pelo Associado correspondente. Nenhuma autorização é concedida pela simples informação de identificadores em uma requisição.

## Contratos com outros domínios

Na comunicação, Cobertura consulta os contratos de Associado e Animal para confirmar identidade, situação operacional, propriedade atual, sexo, situação registral, genealogia e eventuais bloqueios. Os Proprietários atuais naquele instante tornam-se Proprietários participantes e são imutáveis para autorizar decisões dessa Cobertura, mesmo que o Animal seja transferido depois. O snapshot de participantes, direitos e fatos relevantes é preservado para auditoria, sem substituir a fonte de verdade dos domínios de origem.

O domínio de Cobranças recebe solicitações idempotentes para criar, quitar, cancelar ou estornar obrigações vinculadas à Cobertura ou à Declaração. Evidências documentais são versionadas e validadas pelo domínio de Documentos. Notificações recebem fatos de domínio e falhas de entrega nunca revertem uma operação já concluída.

## Modalidades reprodutivas

O catálogo inicial é fechado e seus requisitos são regras de negócio, ainda que o modelo permita evoluir para configuração futura por associação:

| Modalidade | Exige veterinário | Exige Égua receptora | Evidência exigida |
| --- | --- | --- | --- |
| Natural | Não | Não | GTA quando os proprietários forem distintos |
| Natural a campo | Não | Não | GTA quando os proprietários forem distintos |
| Natural com transferência de embrião | Sim | Sim | GTA quando os proprietários forem distintos |
| Inseminação artificial | Sim | Não | Nota fiscal |
| Inseminação artificial com transferência de embrião | Sim | Sim | Nota fiscal |

Transferência de embrião exige Égua receptora identificada e data da transferência entre 6 e 12 dias após o início da Cobertura. O identificador complementar da Égua receptora pode ser informado, mas não é requisito para comunicar.

## Comunicação e validações

### Requisitos

Para comunicar uma Cobertura são obrigatórios Garanhão, Égua reprodutora, seus Proprietários participantes, Comunicante, Modalidade, data inicial e local do evento. Os animais devem existir, estar aptos, ter sexo compatível com seu papel e não possuir impedimento operacional aplicável.

A Cobertura aceita uma data única ou um período. A data inicial é obrigatória e orienta a janela estimada de parição: de 310 a 365 dias após essa data. Não são aceitas datas futuras. O período, quando informado, tem no máximo 90 dias inicialmente; esse limite é uma política configurável.

O Pagador é o Comunicante quando não informado. Pode ser terceiro. Pagador terceiro não pode ser confundido com proprietário: ele poderá se tornar Beneficiário de destino somente pelas regras de Declaração de transferência.

### Duplicidade e nova tentativa

Uma Cobertura ativa não cancelada nem reprovada é única pela combinação de Garanhão, Égua reprodutora, Modalidade e data inicial. Uma Cobertura inativada deixa de participar da validação de duplicidade, mas preserva integralmente seus fatos e sua Auditoria. Em modalidades com transferência de embrião, a Égua receptora integra essa chave de negócio, comparada por sua identidade normalizada.

Uma Cobertura reprovada encerra sua tentativa. Nova comunicação para a mesma combinação requer justificativa e decisão interna explícita, aprovada ou rejeitada na própria solicitação de comunicação; ambas as decisões são auditadas antes da criação, preservando o histórico da tentativa anterior.

### Aprovação inicial

A equivalência entre comunicação e aprovação é uma política do canal. A especificação inicial não presume aprovação automática: cada aprovação deve ser uma decisão explícita e auditada do respectivo Proprietário participante. A operação interna pode registrar a decisão em nome do proprietário somente com autorização aplicável.

## Situação da Cobertura

Situação da Cobertura é administrativa e distinta do Desfecho reprodutivo. Ela é derivada das decisões registradas, nunca inferida de um texto de resultado. A ordem de precedência é: Cancelada, Reprovada pelo Garanhão, Reprovada pela Égua reprodutora, Aguardando decisão do Garanhão, Aguardando decisão da Égua reprodutora, Aguardando Validação veterinária e Aprovada.

- **Aguardando decisão do Garanhão/Égua reprodutora:** falta manifestação válida do respectivo Proprietário participante.
- **Aguardando Validação veterinária:** ambos os proprietários aprovaram e a Modalidade exige veterinário ainda não validado.
- **Aprovada:** ambos os proprietários aprovaram e, quando necessária, a Validação veterinária foi concluída.
- **Reprovada:** um Proprietário participante recusou a parte sob sua responsabilidade.
- **Cancelada:** a Cobertura foi encerrada sem prosseguir.

Quando ambos os progenitores pertencem ao mesmo Associado na comunicação, uma decisão válida deste Associado aprova ou reprova ambos os lados. Para Proprietários participantes distintos, cada um só decide sua própria parte. Não há nova decisão sobre uma parte já decidida, e Coberturas canceladas ou reprovadas não recebem decisões.

Aprovação, reprovação e cancelamento exigem ator, momento, canal e motivo quando aplicável. Reprovação encerra o fluxo sem alterar o Desfecho reprodutivo.

## Cancelamento

Enquanto houver decisão pendente de outro Proprietário participante, somente o Comunicante pode cancelar. Após a aprovação de ambos os proprietários, cancelamento é ação exclusiva da equipe interna, exige motivo e deve abrir tratamento financeiro para Cobranças emitidas, inclusive estorno, crédito ou retenção conforme política financeira.

Exclusão física não é permitida. A inativação administrativa, quando autorizada, preserva todos os fatos, obrigações e a Auditoria, mas retira a Cobertura da validação de duplicidade para permitir uma nova comunicação.

## Validação veterinária

Modalidades que a exigem só alcançam Situação Aprovada após Validação veterinária. O veterinário deve estar ativo, autenticado e ser o profissional designado para a Cobertura. Não existe aprovação por token público.

A validação não pode se repetir e gera Auditoria, encerra sua Pendência correspondente e registra o marco veterinário aplicável. Seu atraso não altera o marco de geração da Cobrança principal. Cobertura cancelada ou reprovada não aceita Validação veterinária, Declaração, Desfecho ou vínculo de Cria; Pendências sem causa são canceladas ou resolvidas com motivo, e Cobranças abertas seguem para decisão financeira auditada.

## Documentos e Pendências

Evidências exigidas não bloqueiam a comunicação. A ausência de GTA nas modalidades naturais aplicáveis ou de nota fiscal nas modalidades de inseminação abre Pendência documental. Apenas PDF validado, seguro e dentro do limite de tamanho configurado pode ser evidência válida.

O envio de uma Evidência válida resolve automaticamente sua Pendência. Ausência documental não bloqueia aprovações, Validação veterinária ou Cobrança principal, mas bloqueia registro de Cria e emissão de documentos oficiais até a regularização.

A Cobertura também avalia Pendências para irregularidades dos progenitores, incluindo Égua reprodutora provisória, Garanhão provisório ou irregular e dados insuficientes de doadora em transferência de embrião. Toda Pendência identifica causa, alvo, estado e condição objetiva de resolução.

## Situação operacional e bloqueios

A matriz inicial de bloqueios é:

| Ação | Regra |
| --- | --- |
| Comunicar Cobertura | Bloqueada para Associado não apto a iniciar operação, incluindo impedimentos cadastrais, documentais, financeiros ou suspensão aplicáveis |
| Registrar Cria | Bloqueado por impedimentos do participante aplicável e por Evidência documental pendente |
| Aprovar, reprovar ou cancelar | Permitido ao ator autorizado apenas enquanto a Cobertura não estiver cancelada ou reprovada, respeitada sua Situação |
| Pagar Cobrança e assinar Declaração | Permitido somente quando o contrato de situação operacional autorizar a regularização específica |
| Iniciar nova operação | Nunca permitido a Associado inativo ou cancelado |

A matriz será refinada pelo domínio de Associados sem duplicar sua definição em Cobertura.

## Multa por comunicação tardia

A multa é calculada na comunicação pela política de multa vigente e só é incorporada após confirmação explícita do Comunicante. A tentativa sem confirmação retorna o valor e a memória de cálculo, sem persistir a Cobertura.

A política inicial define prazo até 31 de agosto do mesmo ano para eventos de janeiro a junho, e até 28 de fevereiro do ano seguinte para eventos de julho a dezembro. Após o prazo, calcula-se o atraso proporcional em ciclos de 180 dias sobre o valor-base; quando superar o limite inicial, aplica-se o fator acumulado do índice econômico definido pela política, excluído o mês da comunicação. Se a fonte do índice falhar, usa-se fator 1,0 e o fato é auditado. A política versionada define obrigatoriamente prazo, calendário, valor-base, faixas, índice, período de apuração, regra de arredondamento, vigência e fallback.

A comunicação confirmada congela versão da política, parâmetros, referência temporal, memória de cálculo e valor. O valor não é recalculado quando a Cobrança principal for criada.

## Cobranças e Declaração de transferência

Quando ambos os Proprietários aprovam, o sistema solicita uma única Cobrança principal de pré-registro ao Pagador, com vencimento inicial de 15 dias e vínculo inequívoco à Cobertura. A operação é idempotente por Cobertura, tipo de Cobrança e Pagador. A Validação veterinária não posterga essa geração.

Se existir multa congelada, é criada Cobrança de multa independente, também com vencimento inicial de 15 dias e idempotência por Cobertura. Cobranças abertas originam Pendências financeiras vinculadas e estas são resolvidas conforme os fatos de quitação, cancelamento ou estorno do domínio financeiro.

Quando o Pagador é terceiro, diferente dos dois Proprietários, e ambos aprovaram, cria-se uma Declaração de transferência. A origem é o Proprietário da Égua reprodutora; a destinação é o Pagador. A declaração é de transferência de produto sem transferência de embrião, ou de embrião quando aplicável.

A Declaração gera Cobrança de transferência, Pendência financeira e Pendência de assinatura. Somente o destinatário pode assiná-la, uma única vez, após a quitação da Cobrança de transferência. A assinatura resolve as Pendências da declaração. A emissão de seu documento oficial requer simultaneamente assinatura válida, quitação da Cobrança de transferência e Evidências documentais obrigatórias regularizadas.

A assinatura não transfere antecipadamente um Animal inexistente. Ela estabelece o Beneficiário de destino, a ser aplicado no registro de cada Cria, salvo decisão interna justificada e auditada.

## Desfecho reprodutivo e Crias

O Desfecho é informado após a aprovação dos Proprietários participantes. Proprietários participantes ou equipe interna podem informar ou corrigir os desfechos sem Cria — em acompanhamento, vazia e morreu — antes de haver vínculo. Os desfechos nasceu e gêmeos somente são consolidados pela composição atômica de Crias. Após o primeiro vínculo, apenas a equipe interna pode corrigir o Desfecho, com motivo e Auditoria.

Desfechos suportados inicialmente incluem em acompanhamento, vazia, nasceu, morreu e gêmeos. Eles obedecem às seguintes invariantes:

- **Nasceu** exige exatamente uma Cria vinculada;
- **Gêmeos** exige duas ou mais Crias vinculadas;
- os demais desfechos não permitem Crias vinculadas.

O registro de Cria é uma composição atômica: vincula uma ou mais Crias e consolida o Desfecho na mesma decisão. Para gêmeos, todas as Crias da composição são vinculadas conjuntamente. Cada Cria é registrada no fluxo próprio de Animais, recebe a Cobertura e os dois progenitores como origem genealógica, e preserva criador, proprietário e Beneficiário de destino conforme suas regras. Uma Cobertura pode ter múltiplas Crias somente nas hipóteses compatíveis com seu Desfecho.

## Consulta, relatórios e resenha provisória

A listagem é paginada, com tamanho entre 1 e 100, ordenação estável e filtros por identificador, progenitores, data de ocorrência, janela de parição, data de comunicação, Modalidade, Desfecho, Situação, participantes, Pagador, pré-registro, atividade e marcos veterinários. Filtros de data devem declarar explicitamente qual data consultam; data de ocorrência e data de comunicação não são sinônimos.

A resenha provisória reúne associação, Cobertura, janela prevista de parição, progenitores, Proprietários atuais à época do evento e Crias já vinculadas. A emissão deve respeitar as Pendências documentais e as permissões de acesso.

## Auditoria, notificações e resiliência

A Auditoria é imutável e registra comunicação, alterações, decisões dos proprietários, Validação veterinária, documentos, Pendências, Cobranças, Declarações, Desfechos, Crias, cancelamentos, inativações e acessos sensíveis. Cada fato contém ator, momento com fuso, canal, entidade, ação, motivo e antes/depois quando aplicável.

Notificar os Proprietários, Comunicante, Pagador, Veterinário e destinatário conforme o fato e a autorização. Destinatários repetidos recebem uma única notificação por evento. Falhas de entrega ficam pendentes para reenvio e não revertem fatos de domínio.

Processos assíncronos, geração de Cobranças, Declarações, abertura/resolução de Pendências, vínculos de Cria e notificações devem ser idempotentes, rastreáveis e reprocessáveis. Há no máximo uma Declaração por Cobertura e destinatário; uma Pendência aberta por Cobertura, tipo e causa; uma notificação por fato de domínio, destinatário e canal; e um vínculo por Cobertura e Cria. A operação usa o fuso operacional inicial America/Sao_Paulo para prazos e vencimentos e armazena instantes com offset.

## Critérios de aceite

- Comunicar Cobertura apenas com participantes válidos, Comunicante proprietário, Modalidade, local e data inicial válidos.
- Diferenciar Situação da Cobertura de Desfecho reprodutivo em todos os fluxos e relatórios.
- Aplicar requisitos de Modalidade, Égua receptora, veterinário e Evidências documentais.
- Impedir duplicidade ativa pela identidade de negócio definida, e exigir análise interna para nova tentativa após reprovação.
- Aplicar aprovação individual por proprietário ou decisão única quando houver proprietário comum.
- Gerar Cobrança principal após a aprovação dos dois Proprietários, inclusive quando houver Validação veterinária pendente.
- Exigir confirmação antes de persistir comunicação com multa e congelar seu cálculo confirmado.
- Abrir e resolver Pendências documental, veterinária e financeira de modo automático e idempotente.
- Bloquear registro de Cria e documentos oficiais enquanto faltar Evidência obrigatória.
- Criar Declaração somente para Pagador terceiro aprovado; exigir quitação de sua Cobrança de transferência para assinatura pelo destinatário.
- Suportar uma Cria para nasceu e duas ou mais para gêmeos, mantendo a genealogia.
- Preservar Auditoria e histórico completos, sem exclusão física dos fatos.
