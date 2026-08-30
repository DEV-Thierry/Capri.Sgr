# Cadastro de Associado

## Objetivo

Especificar o cadastro de associado do Capri.Sgr para implementação inicial. Associado é o vínculo de uma pessoa física (PF) ou jurídica (PJ) com a associação, com dados cadastrais, responsáveis, documentos, cobrança, pendências e permissões operacionais.

A linguagem canônica deste documento é definida pelo [glossário de domínio](../../CONTEXT.md).

## Escopo e fora de escopo

O escopo cobre autoatendimento, cadastro assistido, análise e aprovação documental, cobrança manual, afixo, reenquadramento, notificações, auditoria e permissões granulares.

Ficam fora de escopo: gateway/integração PagSeguro, conciliação automática, parcelamento, pró-rata, assinatura eletrônica avançada, consultas governamentais de CPF/CNPJ, integração de CEP, expiração de documentos, retenção/anonimização/descarte automático LGPD, timezone configurável por associação, catálogo editável de tipos, permissões distintas por responsável e prova de poderes de representação de PJ. Antes da produção, é obrigatória uma política LGPD de retenção, anonimização e descarte.

## Canais, identidade e dados

Há dois canais: autoatendimento do interessado e cadastro assistido pela equipe interna.

CPF identifica PF e CNPJ identifica PJ. O documento é único por associação (tenant); bloqueia-se nova solicitação para documento já associado. Validar formato e dígitos verificadores, sem consultas externas. Comparar o documento normalizado e preservar a grafia para exibição. Alteração de CPF/CNPJ aprovado só ocorre em retificação excepcional interna, com motivo, evidência, validação de unicidade, auditoria e histórico do valor anterior.

### Campos obrigatórios

**PF:** CPF, nome completo, data de nascimento, e-mail válido, exatamente um telefone principal, exatamente um endereço de correspondência, documento de identidade, comprovante de residência e termo de adesão. Nome social e campos estruturados do documento são opcionais.

**PJ:** CNPJ, razão social, e-mail institucional válido, exatamente um telefone principal, exatamente um endereço de correspondência, responsável principal ativo, contrato/estatuto, comprovante de endereço, termo de adesão e identidade do responsável principal. Nome fantasia e inscrições estadual/municipal são opcionais.

O associado pode ter vários telefones e endereços, mas exatamente um de cada deve ser principal/correspondência. E-mails e telefones podem repetir entre associados e responsáveis. Validar telefone com país, DDD e número; CEP e UF estruturalmente. Normalizar espaços; limites técnicos de interface/persistência não são regras de negócio.

## Responsáveis de PJ

PJ exige exatamente um responsável principal ativo para aprovação e pode ter adicionais. Responsável é pessoa identificada por CPF válido e único no tenant, com nome, e-mail e telefone. Uma PF pode ser responsável de várias PJs sem duplicar identidade.

Não há exigência de prova de poderes de representação. Todo responsável ativo autenticado pode consultar e operar em nome da PJ nas ações permitidas pelo status. O cadastro da pessoa é separado de seu acesso: só opera com usuário autenticado vinculado. Trocar o responsável principal requer aprovação interna; remover/desativar adicional tem efeito imediato.

## Tipos de associado

O catálogo é fechado. Mudanças de nomes, elegibilidade, limites, valores, descontos ou regras de afixo exigem mudança de regra de negócio.

| Tipo | Pessoa | Mínimo | Máximo | Cobrança | Desconto | Afixo |
| --- | --- | ---: | ---: | --- | ---: | --- |
| Contribuinte Junior | PF ou PJ | 6 | 59 | Trimestralidade R$ 396,00 | 50% | Permitido |
| Contribuinte Senior | PF ou PJ | 60 | Ilimitado | Trimestralidade R$ 3.150,00 | 50% | Permitido |
| Jovem | PF | 1 | Ilimitado | Trimestralidade R$ 193,00 | 50% | Permitido |
| Nao Socio Criador | PF | 1 | Ilimitado | Sem cobrança | 0% | Proibido |
| Remido | PF ou PJ | 1 | Ilimitado | Taxa única de 100 salários mínimos | 50% | Permitido |
| Usuario | PF | 0 | 5 | Anuidade R$ 209,00 | 50% | Proibido |

PJ não pode usar Jovem, Nao Socio Criador ou Usuario. A equipe configura matriz documental, salário mínimo de Remido, agenda financeira, prazos e notificações. Cada cobrança Remido congela salário mínimo, referência/data-base e valor monetário.

## Documentos e aceite

Matriz inicial: **PF:** CPF, identidade, comprovante de residência e termo. **PJ:** CNPJ, contrato/estatuto, comprovante de endereço, termo e identidade do responsável principal. A matriz é configurável pela equipe.

Envio formal exige todos os documentos obrigatórios anexados; aprovação exige todos aprovados. Documentos podem ser aprovados, rejeitados ou devolvidos para reenvio. Cada reenvio preserva versões com autor, data/hora, status e motivo; somente a versão vigente participa da validação. Arquivos não são excluídos fisicamente: são invalidados ou substituídos, preservando o dossiê.

Aceitar PDF, JPG e PNG, com tamanho máximo configurável e validação de segurança. Falha no armazenamento impede concluir o anexo; ele somente é válido após persistência e auditoria confirmadas.

Cadastro assistido pode ser criado pela equipe, mas só é enviado mediante aceite explícito do interessado ou responsável principal. O aceite requer termo anexado/marcado como aceito e registra versão, data/hora, identidade do aceitante e canal.

## Status, fluxo e decisões

Fluxo normal: `Rascunho` → `Aguardando documentos` → `Aguardando pagamento` → `Aguardando aprovação` → `Aprovado`. Existem também `Correção solicitada`, `Rejeitado`, `Cancelado`, `Suspenso` e `Inativo`. Pendências são independentes do status e podem coexistir.

- **Rascunho:** pode estar incompleto.
- **Envio:** exige dados válidos, telefone principal, endereço de correspondência e todos os documentos obrigatórios anexados.
- **Aguardando documentos:** análise documental ou correção pendente.
- **Aguardando pagamento:** cobrança aplicável ainda não paga.
- **Aguardando aprovação:** documentos obrigatórios aprovados e cobrança aplicável paga.
- **Correção solicitada:** solicitação continua ativa e libera apenas itens apontados.
- **Rejeitado/Cancelado:** encerram solicitação, impedindo alterações e aprovação.
- **Suspenso/Inativo:** aplicáveis somente a associado aprovado.

A equipe pode analisar documentos em paralelo ao pagamento. Nao Socio Criador não gera cobrança e segue à análise/aprovação. Antes da aprovação, são permitidos consulta, correção liberada, contatos, documentos e pagamento; animais, plantel, serviços e solicitações operacionais ficam bloqueados.

Aprovar, rejeitar, cancelar, suspender, reativar e inativar exigem motivo, autor, data/hora e histórico; uma aprovação autorizada é suficiente. Após rejeição/cancelamento, uma nova solicitação começa em rascunho e pode copiar dados/contatos, mas requer novo aceite e não reaproveita automaticamente documentos/pagamentos.

## Financeiro

No envio, gerar cobrança inicial integral para tipos cobrados; aprovação requer quitação. Documentos e pagamento podem tramitar em paralelo. Estados: `Pendente`, `Paga`, `Vencida`, `Cancelada`, `Estornada`.

A equipe registra pagamento, cancelamento e estorno manualmente, com valor, data, meio, comprovante opcional, autor e observação. Pagamento duplicado da mesma cobrança é bloqueado. Estorno torna a cobrança pendente e aplica bloqueios financeiros. Reservar provedor e identificador externo para PagSeguro futuro.

Cobrança vencida mantém a solicitação em Aguardando pagamento, abre pendência e notifica; cancelamento é manual e auditado. Job gera recorrências pela agenda financeira configurável. Não há pró-rata: a inicial é integral e as futuras obedecem o próximo ciclo. Remido não gera recorrência após taxa única; Nao Socio Criador não gera cobrança. Rejeição/cancelamento de solicitação paga abre pendência interna para o Financeiro decidir estorno, crédito ou retenção, sempre com motivo/auditoria.

## Afixo e alterações

Há no máximo um afixo (prefixo ou sufixo) por associado. É opcional para Contribuinte Junior, Contribuinte Senior, Jovem e Remido; proibido para Nao Socio Criador e Usuario. Deve ser único por associação após normalização sem diferença de caixa/acentuação, preservando-se a grafia de exibição. Criar, trocar ou remover afixo exige aprovação interna; o afixo vigente permanece até decisão. O nome de animal preserva o afixo copiado no momento do registro.

E-mail, telefones e endereços podem ser alterados diretamente, preservando invariantes. Nome/razão social, responsável principal, tipo e afixo requerem análise interna. Alterações que afetem documentos só vigoram após documentação atualizada aprovada.

## Pendências e bloqueios

Pendências são cadastrais, documentais, financeiras, operacionais ou de reenquadramento e possuem os estados `Aberta`, `Em análise`, `Resolvida` e `Cancelada`. O sistema as encerra automaticamente quando possível; a equipe pode criá-las manualmente.

| Situação | Efeito |
| --- | --- |
| Documentação pendente/rejeitada | Bloqueia criação/transferência de animais, solicitações operacionais e serviços; permite regularização. |
| Pendência financeira | Bloqueia criação, envio/recebimento de transferência, serviços e emissão de documentos; permite pagamento/regularização. |
| Reenquadramento pendente | Não bloqueia operações neste escopo. |
| Telefone principal/endereço de correspondência ausente | Bloqueia novas operações e aprovação de alterações relevantes; permite correção. |
| Suspenso | Bloqueia operações operacionais e financeiras iniciadas pelo associado; permite consulta e regularização definida pela equipe. |

## Plantel e reenquadramento

Os limites são inclusivos, mas não bloqueiam associação, aquisição nem transferência. Job diário avalia associados criados há mais de 30 dias corridos. Conta apenas propriedade atual ativa e confirmada; exclui transferências pendentes e animais sem propriedade ativa. Em copropriedade, cada animal conta integralmente para cada coproprietário.

O job nunca muda tipo automaticamente: cria proposta e pendência. Há no máximo uma proposta aberta por associado; ela é atualizada quando o destino muda, ou cancelada (com resolução da pendência) se houver retorno à faixa atual, preservando-se histórico.

- Junior com 60+ animais: propor Senior.
- Senior com 6–59: propor Junior.
- Jovem, Remido, Nao Socio Criador, Usuario e casos sem destino: criar pendência para decisão interna e manter tipo atual.

A equipe revalida o plantel e pode aprovar, manter exceção, alterar manualmente, suspender ou inativar, sempre com motivo. Tipo aprovado produz efeitos no ciclo financeiro seguinte, sem retroatividade/pró-rata.

## Notificações, permissões, privacidade e auditoria

E-mail é obrigatório no envio. Notificar por e-mail e central interna: envio, cobranças criadas/vencidas/pagas/estornadas, documentos analisados, correções, aprovação/rejeição/cancelamento, suspensão/inativação/reativação, abertura/resolução de pendência e proposta/decisão de reenquadramento. PF recebe em seu e-mail; para PJ, o principal é prioritário e todos os responsáveis ativos recebem. Falha de e-mail não reverte a operação; é registrada para reenvio, e a central é a contingência oficial.

Existe o perfil inicial **Administrador**. Administradores podem criar/editar/desativar perfis, atribuir permissões granulares por operação e gerir vínculos de usuários. É proibido remover o último Administrador ativo ou perder a própria permissão administrativa na mesma ação.

Documentos/comprovantes são acessíveis apenas ao interessado, responsáveis autorizados e usuários internos com permissão cadastral/financeira. Visualização e download são auditados. Dados, arquivos e histórico são mantidos enquanto o cadastro existir, inclusive inativado.

A auditoria é imutável e cobre criação, alteração, decisões, bloqueios, consultas/downloads sensíveis e permissões. Cada evento registra ator, data/hora, canal, entidade, ação, antes/depois quando aplicável e motivo. Nenhum perfil pode editar ou excluir a auditoria.

## Jobs, tempo e resiliência

Jobs de reenquadramento e recorrência são idempotentes, rastreáveis e reprocessáveis sem duplicar cobranças, pendências, propostas ou notificações. Cada execução registra identificador, início, término e resultado. Falhas parciais alertam Administradores e permitem reprocessar itens pendentes/falhos com segurança.

`America/Sao_Paulo` é o fuso operacional inicial para jobs, vencimentos, referências de dia e cálculo dos 30 dias. Dados temporais devem armazenar timestamp com fuso/offset; a interface converte a exibição para o fuso do navegador do usuário. Falha de notificação registra estado pendente/falha para reenvio sem reverter a operação de domínio.

## Modelo de domínio sugerido

- Associado, TipoAssociado, ResponsavelAssociado, TelefoneAssociado, EnderecoAssociado.
- DocumentoAssociado, TipoDocumentoAssociado, PendenciaAssociado.
- CobrancaAssociado, PagamentoCobrancaAssociado, PropostaReenquadramentoAssociado.
- NotificacaoAssociado, EventoAuditoria, Animal e HistoricoPropriedadeAnimal.
- Value objects/enums: TipoPessoa, TipoNomeAnimal, StatusAssociado, StatusDocumentoAssociado, StatusPendencia, StatusCobranca, Cpf, Cnpj, Email, Telefone, Endereco e TimestampComFuso.

## Critérios de aceite

- Cadastrar PF/PJ com obrigatoriedade, validação e unicidade de CPF/CNPJ por tenant.
- Aplicar os seis tipos fechados e suas regras de elegibilidade, cobrança, limites, desconto e afixo.
- Exigir responsável principal ativo para PJ e permitir reutilizá-lo entre PJs.
- Manter exatamente um telefone principal e endereço de correspondência.
- Exigir anexos para envio e aprovação documental antes de aprovar associado.
- Suportar correção, rejeição e cancelamento com efeitos, motivo e histórico definidos.
- Gerar cobrança inicial no envio, impedir aprovação sem pagamento aplicável e permitir registro manual auditado.
- Aplicar matriz de bloqueios por pendência/status.
- Controlar afixo e preservar nomes de animais registrados.
- Executar reenquadramento diário idempotente após 30 dias, sem troca automática de tipo.
- Notificar por e-mail e central interna sem reverter operação em falha de e-mail.
- Aplicar permissões granulares administradas por Administrador e proteger o último Administrador.
- Manter auditoria imutável e acesso controlado a dados sensíveis.
- Armazenar temporalidade com fuso/offset, calcular em America/Sao_Paulo e exibir no fuso do navegador.
