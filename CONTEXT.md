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

**Tipo de associado**:
Categoria fechada que define elegibilidade, cobrança, desconto, limites de Plantel e permissão de Afixo.
_Avoid_: plano, perfil, modalidade

**Solicitação de associação**:
Pedido de ingresso de um interessado, iniciado como rascunho e encerrado por aprovação, rejeição ou cancelamento.
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
Identificador textual único de um Associado, classificado como prefixo ou sufixo, usado na formação do nome de Animal.
_Avoid_: marca, apelido

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
