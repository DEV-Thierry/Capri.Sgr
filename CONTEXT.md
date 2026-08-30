# Capri.Sgr

Vocabulário compartilhado do CRM para associações de equinos. Estabelece conceitos de negócio comuns aos módulos atuais e futuros, sem decisões de implementação.

## Participantes e patrimônio

**Associado**:
Pessoa física ou jurídica vinculada à associação, apta a manter relações operacionais com animais e serviços conforme sua situação.
_Avoid_: cliente, usuário, conta, membro

**Responsável**:
Pessoa física vinculada a um Associado pessoa jurídica para atuar em seu nome.
_Avoid_: contato, representante, procurador

**Responsável principal**:
Único Responsável ativo obrigatório para aprovação de um Associado pessoa jurídica.
_Avoid_: dono, titular

**Animal**:
Equino registrado ou em processo de registro, cuja identidade, genealogia, criador e propriedade são rastreáveis.
_Avoid_: cavalo, equino cadastrado

**Proprietário atual**:
Associado que detém a propriedade ativa de um Animal em determinado momento.
_Avoid_: dono, titular

**Criador**:
Associado reconhecido como responsável pela criação de um Animal, distinto de seu Proprietário atual quando aplicável.
_Avoid_: proprietário de origem

**Proprietário participante**:
Associado que detinha a propriedade ativa de um progenitor no instante da comunicação de uma Cobertura e permanece vinculado às decisões desse evento.
_Avoid_: proprietário atual da cobertura, dono participante

**Plantel**:
Conjunto de Animais cuja propriedade atual está ativa e confirmada para um Associado.
_Avoid_: rebanho, inventário

## Associação

**Equipe da associação:**
Pessoas autorizadas a operar internamente os processos e conteúdos da associação, com permissões concedidas conforme sua função.
_Avoid_: staff, time interno, usuário administrativo

**Usuário interno:**
Identidade de acesso atribuída a uma pessoa da Equipe da associação para executar permissões administrativas em nome da associação.
_Avoid_: administrador, staff, usuário da associação

**Permissão administrativa:**
Autorização específica concedida a um Usuário interno para consultar, criar, alterar, submeter, decidir ou configurar determinado recurso administrativo, independentemente de um cargo predefinido.
_Avoid_: perfil fixo, função administrativa, capacidade

**Portal público institucional:**
Experiência digital acessível sem autenticação, pela qual qualquer pessoa consulta informações institucionais e conteúdos publicados pela associação.
_Avoid_: site público, site institucional

**Portal do Associado:**
Área autenticada acessível a partir do Portal público institucional, na qual o Associado e, quando aplicável, seu Responsável acessam informações, acompanham solicitações e realizam operações autorizadas em nome do Associado.
_Avoid_: área do cliente, portal do membro

**Transferência de Animal:**
Processo que formaliza a mudança de Proprietário atual de um ou mais Animais para outro Associado, preservando a rastreabilidade da titularidade.
_Avoid_: venda de animal, troca de proprietário

**Baixa de Plantel:**
Registro do encerramento da propriedade ativa de um Animal no Plantel de um Associado, com motivo e data; não encerra necessariamente o ciclo de vida do Animal.
_Avoid_: exclusão do animal, venda de animal

**Motivo de Baixa de Plantel:**
Classificação administrada que define o motivo do encerramento de uma propriedade ativa e as exigências documentais, de Transferência de Animal e de manutenção operacional associadas.
_Avoid_: situação do animal, motivo de exclusão

**Registro genealógico externo reconhecido:**
Registro emitido por outra entidade e aceito pela associação para efeitos de regularização do Registro genealógico de um Animal.
_Avoid_: registro provisório local

**Resenha:**
Processo de avaliação que subsidia a concessão de Registro genealógico provisório ou definitivo a um Animal.
_Avoid_: inspeção informal

**Registro genealógico:**
Reconhecimento da associação sobre a identidade e genealogia de um Animal, progressivo entre sem registro, em regularização, provisório, definitivo ou castrado. A elegibilidade etária para o registro definitivo é uma condição do processo, não um estado separado.
_Avoid_: cadastro do animal

**Situação do Animal:**
Condição operacional do ciclo de vida de um Animal, distinta de sua existência cadastral, de sua propriedade ativa e de seu Registro genealógico.
_Avoid_: status

**Publicação institucional:**
Conteúdo editorial mantido pela Equipe da associação e disponibilizado no Portal público institucional, sujeito a governança, versionamento e Auditoria.
_Avoid_: post, material, conteúdo publicado

**Modelo de layout:**
Composição pré-validada de navegação e blocos visuais que a associação pode escolher para o Portal público institucional, com personalização guiada e sem edição livre de estrutura.
_Avoid_: tema livre, template editável

**Tipo de associado**:
Categoria fechada que define elegibilidade, cobrança, desconto, limites de Plantel e permissão de Afixo.
_Avoid_: plano, perfil, modalidade

**Solicitação de associação**:
Pedido de ingresso de um interessado, iniciado como rascunho e encerrado por aprovação, rejeição ou cancelamento. Após seu envio, concede acesso provisório à Área do Associado apenas para acompanhamento e regularização, enquanto operações dependentes de aprovação permanecem bloqueadas.
_Avoid_: cadastro, proposta

**Correção solicitada**:
Situação em que uma Solicitação de associação permanece ativa, com itens específicos liberados para ajuste ou reenvio.
_Avoid_: rejeição, devolução

**Documento associado**:
Documento integrante do dossiê de uma Solicitação de associação ou de um Associado, sujeito a versionamento e análise.
_Avoid_: anexo, upload

**Documento vigente**:
Versão de Documento associado que participa da validação atual da Solicitação de associação.
_Avoid_: último anexo

**Afixo**:
Identificador textual único de um Associado, classificado como prefixo ou sufixo, herdado do Criador e usado na formação do nome composto de Animal quando existir. Como o Criador não muda, o Afixo do Animal permanece o mesmo; na ausência de Afixo do Criador, o nome composto coincide com o Nome-base.
_Avoid_: marca, apelido

**Nome-base do Animal**:
Nome próprio do Animal antes da composição com o Afixo herdado do Criador. O nome do Animal é sempre gravado em letras maiúsculas, preservando a acentuação original.
_Avoid_: nome exibido, nome completo

**Nome composto do Animal**:
Identificação nominal formada pelo Afixo do Criador, quando existente, e pelo Nome-base do Animal, sempre gravada em letras maiúsculas com preservação da acentuação.
_Avoid_: nome de proprietário, apelido

**Reenquadramento**:
Revisão do Tipo de associado diante da quantidade de Animais de seu Plantel.
_Avoid_: upgrade, downgrade, mudança automática de plano

**Proposta de reenquadramento**:
Sugestão gerada pelo sistema para alteração de Tipo de associado, que exige decisão interna.
_Avoid_: reenquadramento automático

## Reprodução

**Cobertura**:
Evento reprodutivo comunicado à associação que relaciona um garanhão, uma égua reprodutora e uma modalidade reprodutiva, preservando a rastreabilidade de seu processo e de suas crias.
_Avoid_: monta, cruzamento, registro de monta

**Garanhão**:
Animal macho participante de uma Cobertura como progenitor paterno.
_Avoid_: macho, pai

**Égua reprodutora**:
Animal fêmea participante de uma Cobertura como progenitora materna.
_Avoid_: matriz, fêmea, mãe

**Comunicante**:
Proprietário atual de um dos progenitores que comunica uma Cobertura à associação.
_Avoid_: solicitante, cadastrante

**Pagador**:
Associado responsável pelas cobranças originadas por uma Cobertura, que pode ser diferente dos proprietários dos progenitores.
_Avoid_: cobrando, responsável financeiro

**Modalidade reprodutiva**:
Classificação fechada da técnica utilizada em uma Cobertura; suas exigências variam conforme suas capacidades, como inseminação e transferência de embrião.
_Avoid_: tipo de monta

**Validação veterinária**:
Decisão emitida pelo veterinário designado que confirma uma Cobertura cuja modalidade a exige.
_Avoid_: aprovação do veterinário

**Situação da Cobertura**:
Estado administrativo do processo de uma Cobertura, determinado pelas decisões dos participantes e, quando aplicável, pela Validação veterinária.
_Avoid_: resultado, status de nascimento

**Desfecho reprodutivo**:
Fato observado sobre o resultado biológico de uma Cobertura, independente de sua Situação administrativa.
_Avoid_: status da cobertura

**Cria**:
Animal nascido e vinculado a uma Cobertura, com genealogia derivada de seus progenitores.
_Avoid_: produto, filhote

**Égua receptora**:
Égua que recebe um embrião em uma Cobertura com transferência de embrião, distinta da Égua reprodutora quando aplicável.
_Avoid_: receptora, matriz receptora

## Obrigações e rastreabilidade

**Pendência**:
Obrigação aberta, associada a uma causa específica, que pode restringir operações até ser resolvida ou cancelada.
_Avoid_: tarefa, alerta

**Cobrança**:
Obrigação financeira vinculada a um Associado e a um fato de negócio, com ciclo de vida próprio até sua quitação, cancelamento ou estorno.
_Avoid_: taxa, boleto

**Declaração de transferência**:
Documento que formaliza a destinação a terceiro dos direitos sobre uma Cria ou embrião derivados de uma Cobertura.
_Avoid_: declaração de cobertura

**Evidência documental**:
Documento apresentado para atender uma exigência de uma Cobertura e preservado com sua identidade, versão e histórico de validação.
_Avoid_: upload, anexo

**Auditoria**:
Registro imutável de um fato relevante, contendo ator, momento, canal, entidade, ação, motivo e alterações aplicáveis.
_Avoid_: log, histórico técnico
