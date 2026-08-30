# UX/UI — Portal público, Área do Associado e Área Administrativa

## Precedência documental

As regras específicas de cada módulo e as decisões registradas em ADR prevalecem sobre esta especificação de experiência quando houver conflito. Para o domínio Animal, consulte [Animal](animal.md); para os demais módulos, consulte a especificação funcional correspondente. Esta documentação define a experiência, a descoberta e a apresentação, não substituindo regras de negócio.

## Objetivo

Definir a experiência, arquitetura de informação, navegação, permissões, estados de interface e requisitos de qualidade para as três superfícies do Capri.Sgr:

1. **Portal público institucional**, acessível sem autenticação;
2. **Área do Associado**, autenticada, destinada ao Associado e, quando aplicável, ao seu Responsável;
3. **Área Administrativa**, destinada à Equipe da associação.

Este documento complementa as regras de negócio de [Cadastro de Associado](cadastro-associado.md) e [Coberturas](coberturas.md). A terminologia segue o [glossário de domínio](../../CONTEXT.md). Quando houver conflito, prevalecem as regras específicas dessas features e a ADR de [Status e Pendências independentes](../adr/0001-status-e-pendencias-independentes-no-cadastro-de-associado.md).

## Princípios de experiência

- **Clareza operacional:** toda ação, situação, Pendência, Cobrança e decisão informa seu efeito e próximo passo.
- **Separação explícita de contextos:** informação pública, operação autenticada do Associado e operação interna nunca se confundem.
- **Autonomia com controle:** a associação configura marca, conteúdos, navegação e modelos pré-validados sem editar estrutura ou código.
- **Rastreabilidade por padrão:** decisões, publicações, mudanças sensíveis, documentos formais e acessos sensíveis preservam Auditoria.
- **Mobile-first para Associados:** fluxos públicos e da Área do Associado priorizam celular; a Área Administrativa prioriza produtividade em desktop, mantendo responsividade.
- **Acessibilidade e linguagem clara:** WCAG 2.2 nível AA e português do Brasil são obrigatórios.

## Públicos, identidades e acessos

| Público | Contexto | Acesso e objetivo |
| --- | --- | --- |
| Visitante | Portal público institucional | Consulta informações institucionais e Publicações institucionais sem login. |
| Interessado | Área do Associado provisória | Após enviar uma Solicitação de associação, recebe acesso autenticado para acompanhar e regularizar o processo. |
| Associado | Área do Associado | Consulta sua situação e Plantel, acompanha processos e executa operações autorizadas. |
| Responsável | Área do Associado | Atua em nome do Associado pessoa jurídica. Todo Responsável ativo autenticado possui o mesmo alcance operacional permitido pela situação do Associado nesta versão. |
| Usuário interno | Área Administrativa | Opera recursos conforme Permissões administrativas granulares. |
| Administrador | Área Administrativa | Usuário interno que inicialmente recebe todas as Permissões administrativas e administra usuários e permissões. |

### Autenticação

A primeira versão oferece e-mail e senha, convite/ativação de acesso e recuperação de senha. A autenticação em dois fatores fica preparada como evolução e não é obrigatória neste escopo.

Após autenticar-se no Portal público institucional, a pessoa permanece no contexto público. Quando tiver vínculo ou Solicitação de associação, vê o botão **Área do Associado**, que muda explicitamente para o contexto autenticado. A interface deve identificar o Associado em cujo nome a pessoa opera; para pessoa jurídica, também identifica o Responsável autenticado.

## Arquitetura de informação

### Portal público institucional

O Portal público institucional é aberto a qualquer pessoa. Sua navegação inicial contém:

- Início;
- Notícias;
- Comunicados;
- Atas;
- Eventos;
- Páginas institucionais;
- Associar-se;
- Acessar;
- Área do Associado, exibido para pessoa autenticada elegível.

Todas as Publicações institucionais são públicas e completas nesta versão. Dados pessoais, documentos, Pendências, Cobranças e processos individuais não podem ser exibidos neste contexto.

### Área do Associado

A Área do Associado tem as seções abaixo. Em celular, prioriza **Início**, **Operações** e **Minha situação** em navegação compacta; em telas maiores pode usar menu lateral persistente.

1. **Início** — painel de prioridades;
2. **Minha situação** — dados, Situação, Pendências, Documentos associados e Cobranças;
3. **Plantel** — lista e detalhe dos Animais;
4. **Operações** — Coberturas, Transferências de Animal e serviços disponíveis;
5. **Publicações** — leitura de notícias, comunicados, atas e eventos públicos;
6. **Notificações e histórico** — atualizações, comprovantes e acompanhamento;
7. **Atendimento** — abertura e acompanhamento de solicitações estruturadas.

### Área Administrativa

A navegação é orientada por objetivos de trabalho. Cada item, dado e ação aparece somente com a Permissão administrativa correspondente.

1. **Painel administrativo** — filas, indicadores, prazos e prioridades;
2. **Associação** — Solicitações de associação, Associados, Responsáveis, Documentos associados, Pendências e reenquadramentos;
3. **Registro** — Animais, Plantéis, Coberturas, Transferências de Animal e validações;
4. **Financeiro** — Cobranças, pagamentos, estornos e restrições financeiras;
5. **Comunicação** — Publicações institucionais, categorias, mídia e calendário editorial;
6. **Configurações** — Usuários internos, Permissões administrativas, identidade visual, Modelos de layout, blocos, menus e políticas editoriais;
7. **Auditoria** — consulta de fatos relevantes e rastreabilidade.

## Jornadas essenciais

### Visitante, autenticação e entrada na Área do Associado

1. O visitante navega pelo Portal público institucional e consulta Publicações institucionais.
2. Pode iniciar a Solicitação de associação pela ação **Associar-se**.
3. Após o envio da Solicitação, o sistema disponibiliza acesso autenticado e apresenta **Área do Associado**.
4. A pessoa escolhe o botão para acessar a Área do Associado. A entrada explicita: **“Sua Solicitação de associação está em análise”**.
5. Após aprovação, o mesmo espaço passa a disponibilizar as operações autorizadas, sem exigir mudança de produto ou nova identidade de acesso.

### Área do Associado provisória

Antes da aprovação, o painel destaca Situação, Pendências, documentos obrigatórios, Cobranças, prazos e próximas ações. Estão disponíveis:

- consulta da Situação da Solicitação de associação;
- correção e reenvio de itens liberados em Correção solicitada;
- envio ou substituição de Documentos associados;
- consulta e regularização de Cobranças vinculadas;
- notificações, comprovantes e Atendimento;
- alteração de dados e contatos quando permitida pelo fluxo.

Plantel, Coberturas, Transferências de Animal, serviços e documentos oficiais continuam visíveis na navegação, mas bloqueados até a aprovação. O bloqueio informa causa, impacto e atalho de regularização; jamais promete liberação automática fora das regras de negócio.

### Painel de prioridades do Associado

O Início é uma área de trabalho, não apenas um resumo. Ordena blocos por impacto e prazo:

1. Pendências e Cobranças que bloqueiam ou vencem primeiro;
2. decisões, correções e ações que dependem do Associado;
3. operações em andamento;
4. resumo do Plantel;
5. atalhos para comunicar Cobertura, iniciar Transferência de Animal e serviços autorizados;
6. Publicações institucionais recentes;
7. notificações e histórico recente.

### Plantel

A seção apresenta lista pesquisável e filtrável de Animais, com cartões responsivos no celular. Cada detalhe reúne identificação, genealogia, situação, histórico de propriedade disponível, Coberturas relacionadas e operações permitidas. Ações indisponíveis seguem o padrão de bloqueio explicado neste documento.

### Coberturas

A comunicação de Cobertura deve guiar o Comunicante por etapas, preservando as exigências da feature de Coberturas: progenitores, Proprietários participantes, modalidade, data inicial, local, Pagador, exigências de Evidência documental e, se aplicável, Égua receptora e Validação veterinária.

A experiência deve distinguir visualmente a Situação da Cobertura do Desfecho reprodutivo. Decisões de Proprietários participantes, marcos veterinários, Cobranças, Pendências e histórico precisam estar acessíveis no detalhe. Quando a Cobertura não puder ser comunicada, informar a condição impeditiva e o caminho de regularização.

### Transferência de Animal

A Área do Associado oferece ponto de entrada para o Proprietário atual informar intenção de transferir um ou mais Animais para outro Associado. O fluxo detalhado, suas decisões e elegibilidades pertencem à feature específica de Transferência de Animal. Esta documentação determina apenas a descoberta, o acesso pelo menu Operações, o histórico e a apresentação de bloqueios aplicáveis.

### Atendimento

Atendimento é uma solicitação estruturada, não chat em tempo real. A pessoa informa categoria/assunto e descrição, pode anexar arquivos informais quando cabível, acompanha Situação e histórico de respostas. Um anexo de atendimento nunca substitui Documento associado ou Evidência documental exigidos formalmente.

## Situação, Pendências e bloqueios

Situação e Pendências são independentes. A interface deve mostrar ambas sem fundi-las em um único rótulo de status.

Quando uma Pendência bloquear uma operação, a operação permanece visível e desabilitada. O componente de bloqueio apresenta:

- operação afetada;
- causa específica;
- efeito da restrição;
- ação de regularização disponível;
- atalho para o contexto correto, quando existir.

Exemplo: **“Transferência de Animal temporariamente indisponível: há uma Pendência documental aberta. Regularize o Documento associado indicado para continuar.”**

A Área Administrativa não pode ter uma ação genérica de “liberar operação”. Usuários internos resolvem ou cancelam a Pendência pela causa aplicável, respeitando permissão, evidência, justificativa e Auditoria.

## Área Administrativa: permissões e ações sensíveis

Permissões administrativas são granulares por recurso e ação. A matriz deve permitir, no mínimo:

- consultar;
- criar e editar rascunho;
- submeter para decisão;
- aprovar, rejeitar ou publicar;
- configurar;
- consultar Auditoria.

Portanto, um Usuário interno pode consultar Solicitações de associação sem aprová-las, ou redigir uma Publicação institucional sem publicá-la. O Administrador começa com todas as permissões. Não é permitido remover o último Administrador ativo nem fazer alguém perder a própria permissão administrativa na mesma ação.

Ações sensíveis — incluindo aprovar, rejeitar, cancelar, inativar, alterar excepcionalmente identificadores, resolver/cancelar Pendências, publicar conteúdo e publicar configuração visual — exigem confirmação explícita, motivo quando aplicável, prévia clara de efeito e Auditoria com ator, data/hora, canal e alterações.

## Publicações institucionais e governança editorial

As categorias iniciais são notícias, comunicados, atas, páginas institucionais, eventos, banners e chamadas da página inicial.

A governança é configurável por categoria: uma categoria pode permitir publicação direta, enquanto outra pode requerer criador e revisor distintos. O ciclo editorial contempla rascunho, agendamento, publicação e arquivamento. Deve preservar autor, revisor quando existente, versões, datas e Auditoria.

O Portal público oferece busca de Publicações institucionais e páginas. A Área Administrativa fornece calendário editorial, filtros e busca por conteúdo, categoria, autor, período e situação.

## Personalização do Portal público institucional

A associação pode configurar:

- logo, cores de marca, tipografia, favicon e imagens;
- banners, destaques, páginas institucionais e blocos editoriais;
- menu, visibilidade e ordem das seções públicas;
- um Modelo de layout pré-validado.

O modelo oferece uma biblioteca fechada de blocos responsivos e acessíveis — por exemplo, banner, notícias, comunicados, atalhos de serviços, eventos, números institucionais e chamadas de associação. A equipe controla conteúdo, ordem, visibilidade e quantidades permitidas pelo modelo, mas não edita livremente HTML, CSS ou a estrutura de navegação.

Mudanças de identidade visual, menu, blocos ou Modelo de layout são preparadas em rascunho, pré-visualizadas em celular e desktop e só passam a vigorar com publicação explícita. Versões anteriores permanecem rastreáveis e reversíveis. A Área Administrativa usa logo e elementos de marca da associação, porém mantém componentes, contraste, layout e navegação administrativos controlados pelo produto.

## Busca, filtros, notificações e histórico

- **Portal público:** busca de Publicações institucionais e páginas.
- **Área do Associado:** busca e filtros para Plantel, operações, Cobranças, Pendências e histórico.
- **Área Administrativa:** busca por Associado, Animal, Solicitação de associação, Cobertura, Cobrança e Publicação institucional; filtros por situação, período, responsável e Pendência.

Eventos relevantes geram notificação no centro do portal e e-mail complementar: alteração de Situação, nova Pendência, Cobrança, decisão, Correção solicitada, resposta de Atendimento, publicação e prazo próximo. O centro de notificações é consultável, filtrável e permite abrir o contexto de origem. Falha de e-mail não reverte o fato de negócio e mantém a possibilidade de reenvio.

Cada operação e solicitação deve oferecer histórico e comprovantes consultáveis conforme autorização. Consultas e downloads sensíveis são auditados.

## Componentes e estados mínimos

| Componente | Estados esperados |
| --- | --- |
| Cartão de prioridade | informativo, requer ação, bloqueante, vencido/próximo do vencimento, resolvido |
| Situação | rótulo semântico, descrição, data de atualização e vínculo ao histórico |
| Pendência | aberta, em análise, resolvida, cancelada; causa, impacto e regularização |
| Cobrança | pendente, paga, vencida, cancelada, estornada; valor, vencimento e comprovante quando houver |
| Operação | disponível, indisponível por situação, bloqueada por Pendência, em andamento, concluída, cancelada |
| Documento formal | pendente, enviado, em análise, aprovado, rejeitado, devolvido para reenvio, substituído/inválido |
| Publicação institucional | rascunho, agendada, publicada, arquivada; revisão quando exigida |
| Confirmação sensível | resumo do efeito, motivo quando aplicável, confirmação explícita e registro de Auditoria |
| Estado vazio | explica ausência de dados e oferece ação somente quando autorizada |
| Erro | linguagem clara, preservação do preenchimento quando possível e caminho de recuperação |

## Requisitos de qualidade

- Conformidade com **WCAG 2.2 AA**, inclusive foco visível, navegação por teclado, semântica, contraste, alternativas textuais e mensagens de erro compreensíveis.
- Fluxos públicos e da Área do Associado projetados primeiro para celular; Área Administrativa otimizada para desktop e responsiva.
- Português do Brasil com linguagem clara, sem jargão técnico e com explicação de Pendências, restrições e próximos passos.
- Não depender exclusivamente de cor para comunicar situação, bloqueio ou urgência.
- Preservar Auditoria e rastreabilidade sem expor informações pessoais ou documentos a públicos não autorizados.

## Fora do escopo

Esta feature não define:

- processo detalhado, decisões e regras da Transferência de Animal;
- emissão de certificados e documentos oficiais;
- pagamento integrado;
- chat em tempo real;
- conteúdo exclusivo autenticado;
- edição livre de HTML/CSS;
- autenticação em dois fatores obrigatória.

Também permanecem aplicáveis os limites funcionais definidos nas features de Cadastro de Associado e Coberturas.

## Critérios de aceite de UX/UI

- O visitante navega e consulta todas as Publicações institucionais sem autenticação.
- Uma pessoa autenticada com Solicitação de associação enviada vê o botão Área do Associado e encontra o acesso provisório com ações de regularização disponíveis.
- Operações que exigem aprovação permanecem visíveis e bloqueadas, com causa, impacto e próximo passo.
- Associado e Responsável autorizado encontram painel de prioridades, Minha situação, Plantel, Operações, Publicações, Notificações e histórico e Atendimento.
- Usuários internos acessam somente recursos e ações permitidos; consultar não concede decidir ou publicar.
- Configurações públicas são publicadas somente após prévia e confirmação, com histórico de versão reversível.
- Situação e Pendências são apresentadas separadamente em todas as experiências.
- Ações sensíveis apresentam confirmação e registram Auditoria.
- Busca e filtros atendem aos três contextos definidos.
- Interfaces cumprem requisitos mobile, responsivos, em português do Brasil e WCAG 2.2 AA.
