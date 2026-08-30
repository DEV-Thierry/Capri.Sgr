# Especificação Funcional — Módulo Animal

**Versão:** 1.0  
**Status:** decisões de negócio consolidadas; implementação pendente  
**Fonte:** Especificação Funcional Detalhada — Domínio Animal (legado), versão 1.0, 05/09/2026 (proveniência a confirmar)

A linguagem canônica está no [glossário de domínio](../../CONTEXT.md).

## 1. Objetivo e escopo

Definir exclusivamente regras de negócio do domínio Animal: cadastro, identidade nominal, Criador, Proprietário atual, Plantel, genealogia, Registro genealógico, reprodução, Baixa de Plantel, morte, castração, Transferência de Animal, Pendências, Cobranças e Auditoria.

Ficam fora: endpoints, códigos HTTP, classes, tabelas, consultas, locks, armazenamento de fotos, PDF, autenticação, permissões, telas, infraestrutura, provedores externos e detalhes de implementação. Reprodução, Resenha, Cobranças e Pendências mantêm suas regras próprias; este documento descreve apenas as fronteiras.

## 2. Modelo de negócio

- Um Animal possui identidade, genealogia, Criador e titularidade rastreáveis.
- Um Animal operacionalmente ativo possui exatamente um Proprietário atual e integra um Plantel.
- Registro genealógico, Situação do Animal e titularidade são dimensões independentes.
- Situações do Animal: Normal, Em seguimento, Castrado e Morto.
- Estados do Registro: Sem Registro, Em regularização, Provisório, Definitivo e Castrado.
- Ausência de Proprietário atual é condição de titularidade/regularização, não Situação.
- Suspensão registral é condição operacional causada por Pendência impeditiva, não novo estado.

## 3. Regras de negócio

### Cadastro e identidade

**RN-AN-001 — Dados mínimos:** exigir Nome-base, sexo, data de nascimento, Criador e Proprietário atual. Criador omitido assume o Proprietário atual.

**RN-AN-002 — Sexo:** Animal ativo possui sexo macho ou fêmea. Não informado só é aceito em saneamento histórico e bloqueia Registro e Reprodução dependentes.

**RN-AN-003 — Nascimento:** data pode ser confirmada, estimada ou desconhecida. Somente a confirmada satisfaz idade para Registro definitivo.

**RN-AN-004 — Genealogia:** pai, mãe ou ambos podem estar ausentes no cadastro inicial; ausência deve ser distinguida entre desconhecida e ainda não informada.

**RN-AN-005 — Unicidade:** Nome composto deve ser único em todo o acervo, inclusive Mortos, sem titularidade e excluídos logicamente. Exclusão não libera nome ou número de Registro.

**RN-AN-006 — Normalização:** Nome-base, Afixo e Nome composto são gravados em letras maiúsculas, preservando acentuação. Remover espaços externos e compor Afixo e Nome-base com um espaço; preservar espaços internos legítimos. JOÃO e JOAO são diferentes.

**RN-AN-007 — Caracteres:** aceitar letras, acentos, espaços internos, hífen e apóstrofo. Rejeitar controles, números e símbolos sem função nominal.

**RN-AN-008 — Afixo:** é herdado exclusivamente do Criador, conforme prefixo ou sufixo. Sem Afixo, o Nome composto coincide com o Nome-base e nenhum processo é bloqueado.

**RN-AN-009 — Imutabilidade:** congelar Afixo no cadastro. Transferência, mudança do Proprietário ou alteração posterior do Afixo do Criador não o modificam.

**RN-AN-010 — Nome:** antes do Registro, permitir correção do Nome-base se o novo composto for único, mantendo o anterior na Auditoria. Após o Registro, Nome-base, Afixo e Nome composto são imutáveis no fluxo ordinário. Erro comprovado pode ser corrigido excepcionalmente, preservando número e histórico.

**RN-AN-011 — Identificação oficial:** Nome composto normalizado é a identificação principal em Registro e documentos; Nome-base é auxiliar.

### Titularidade, Plantel e Transferência

**RN-AN-012 — Titularidade única:** Animal ativo tem um Proprietário atual e um Plantel. Transferência em andamento não muda titularidade.

**RN-AN-013 — Transferência concluída:** encerra vínculo no Plantel anterior, cria vínculo no novo e preserva ambos os históricos. Não pode produzir dois titulares nem ausência de titularidade para Animal operacional.

**RN-AN-014 — Novo proprietário:** deve estar apto a manter Plantel; Associado inativo/irregular não recebe titularidade.

**RN-AN-015 — Preservação:** Transferência não altera Criador, Afixo, Nome-base, Nome composto ou número de Registro.

**RN-AN-016 — Sem titular:** Animal sem Proprietário atual fica em regularização, não integra Plantel e não executa operações dependentes de titularidade.

**RN-AN-017 — Baixa:** deve existir catálogo de Motivos de Baixa de Plantel, com exigências documentais, de Transferência e manutenção operacional. Venda é motivo; morte é fato terminal.

**RN-AN-018 — Venda:** encerra propriedade ativa, não encerra Animal nem Registro, e pode deixar o Animal aguardando Transferência.

**RN-AN-019 — Reativação:** exige decisão administrativa justificada e Auditoria; não é automática e não se aplica a morte ou exclusão lógica.

**RN-AN-020 — Contagem:** limites e Cobrança de Plantel contam somente Animais ativos no Plantel no cálculo. Excluir Mortos, vendidos, excluídos e sem titularidade; Pendência registral não exclui enquanto propriedade estiver ativa.

### Situação, morte e castração

**RN-AN-021 — Seguimento:** deriva de Cobertura que requer acompanhamento e retorna a Normal quando cessar, salvo Castrado ou Morto.

**RN-AN-022 — Morte:** é terminal, prevalece na Situação, impede operações incompatíveis e encerra Plantel se existente. Pode ser registrada sem titular.

**RN-AN-023 — Registro após morte:** preservar Registro, genealogia, identidade e número.

**RN-AN-024 — Castração:** impede novas Coberturas, exige nova Resenha para todo Animal que possuía Registro e estabelece Castrado para progressão. A castração não suspende nem revoga o Registro existente.

**RN-AN-025 — Registro anterior:** preservar Registro provisório/definitivo histórico. Definitivo castrado exibe Castrado, mantendo Definitivo como reconhecimento histórico.

**RN-AN-026 — Cobertura ativa:** castração não cancela Cobertura já comunicada; ela segue o fluxo próprio.

### Registro, Resenha e genealogia

**RN-AN-027 — Estados:** usar Sem Registro, Em regularização, Provisório, Definitivo e Castrado. Apto ao Definitivo não é estado.

**RN-AN-028 — Número:** atribuir número único, sequencial e imutável somente na aprovação do Provisório. Um único Provisório vigente; correções preservam número.

**RN-AN-029 — Pendência inicial:** criar Pendência de regularização no cadastro, exceto com Registro externo reconhecido.

**RN-AN-030 — Externo:** reconhecimento exige decisão explícita de equivalência provisória ou definitiva, entidade, identificador e evidência. Equivalência definitiva habilita Reprodução se demais condições estiverem regulares.

**RN-AN-031 — Resenha provisória:** exige cadastro/propriedade regulares, ausência de processo provisório concorrente e ausência de Registro incompatível. Pendências impeditivas podem impedir aprovação; não impeditivas não bloqueiam início.

**RN-AN-032 — Resenha definitiva:** exige Provisório válido, três anos completos na solicitação, ausência de processo definitivo concorrente/concluído e Pendências impeditivas resolvidas. Data estimada/desconhecida não satisfaz idade.

**RN-AN-033 — Concorrência:** no máximo uma Resenha de cada tipo; definitiva só inicia após conclusão da provisória.

**RN-AN-034 — Aprovação:** aprovação gera uma Cobrança consolidada, identificando todos os fatos geradores e a Pendência correspondente, salvo política financeira. A Cobrança nasce após aprovação; valores, isenções e estornos pertencem a Cobranças.

**RN-AN-035 — Reprovação/cancelamento:** preservam estado anterior, não removem Provisório e não geram Cobrança de Registro. Resenha reprovada não pode ser reapresentada; deve ser criada uma nova Resenha.

**RN-AN-036 — Nascimento fora do prazo:** gera Pendência que bloqueia progressão registral até decisão da Equipe da associação, sem bloquear cadastro.

**RN-AN-037 — Correção:** identidade, genealogia, Criador, nascimento e sexo após Registro exigem processo justificado, evidência, decisão da Equipe da associação e Auditoria. Nenhuma correção suspende ou revoga o Registro; a correção preserva número e histórico.

**RN-AN-038 — Reprodução:** progenitor exige Definitivo ou Registro externo definitivo, sexo/situação compatíveis e ausência de Pendências impeditivas. Modalidades pertencem a Reprodução.

**RN-AN-039 — Cria:** Cobertura é fonte do vínculo e progenitores. Divergência gera Pendência e Cobertura prevalece até decisão. Alterações exigem evidência e Auditoria.

**RN-AN-040 — Cobertura cancelada:** não remove genealogia automaticamente; Cria entra em regularização, preservando vínculo histórico, e pode permanecer no Plantel se propriedade válida.

### Exclusão, Pendências, Cobranças e Auditoria

**RN-AN-041 — Exclusão lógica:** torna Animal indisponível aos fluxos, preserva relações, nome, número, genealogia e Auditoria, não permite reativação ordinária e não libera nome/número.

**RN-AN-042 — Pendências:** restringem operações conforme política; não implicam bloqueio total por padrão.

**RN-AN-043 — Morte e Cobranças:** impede novas Cobranças posteriores; existentes seguem política financeira.

**RN-AN-044 — Isenção:** não há limite nem exceção para a isenção de Plantel.

**RN-AN-045 — Auditoria:** alterações, decisões, Transferências, Baixas, reativações, exclusões, correções e suspensões geram Auditoria imutável com ator, momento, canal, ação, motivo e alterações. Consulta simples não, salvo dado sensível, lei ou investigação.

**RN-AN-046 — Histórico:** toda mudança de Proprietário e Plantel preserva início, término, motivo e decisão; correção não apaga fatos.

## 4. Transições

### Situação
Cadastro inicia Normal, salvo Cobertura que exija Em seguimento. Seguimento retorna a Normal ao cessar. Normal/Seguimento podem tornar-se Castrado. Qualquer situação não terminal pode tornar-se Morto. Morto é terminal e prevalece.

### Registro
Sem Registro pode ir a Em regularização e depois Provisório. Provisório pode ir a Definitivo após idade, Pendências, Resenha e decisões aplicáveis. Castração impede progressão e torna Castrado a condição atual, preservando estados anteriores. Suspensão é condição operacional.

### Titularidade
Cadastro inicia com Proprietário e Plantel. Baixa encerra vínculo. Transferência concluída cria vínculo novo. Reativação exige decisão, exceto morte e exclusão.

## 5. Fronteiras entre módulos

- Reprodução fornece Cobertura, vínculo, progenitores, acompanhamento e desfecho; valida modalidades.
- Resenha decide avaliações; Animal fornece identidade, idade, sexo, situação, Registro e Pendências.
- Registro reconhece estados, concede número e controla validade.
- Pendências define severidade, resolução e bloqueios; Animal cria/consome Pendências do ciclo.
- Cobranças calcula valores, vencimentos, isenções e estornos; Animal fornece fatos geradores.
- Transferência formaliza titularidade e preserva histórico; não altera nome/Afixo.
- Auditoria preserva fatos imutáveis.

## 6. Decisões aceitas nesta especificação

Venda é Baixa, não terminal; Afixo é do Criador, opcional e congelado; ausência de Afixo não bloqueia; nome é maiúsculo com acentuação e nome composto único; Transferência preserva identidade registral; morte é terminal; castração preserva registros anteriores; Registro externo exige equivalência explícita; Cobertura prevalece em divergência; Pendências não bloqueiam tudo por padrão.

## 7. Decisões resolvidas

- **Códigos legados:** `N` Plantel, `B` Bloqueado, `C` Castrado, `D` Doado, `I` Inativado, `L` Cancelado, `M` Morto, `P` Perdido, `V` Vendido, `E` Égua comum e `S` Pendente de Registro. O domínio pode substituir códigos por um enum com nomes explícitos; os códigos permanecem como compatibilidade legada/migração.
- **Morte:** o motivo é classificado como `Morto`.
- **Isenção de Plantel:** não há limite nem exceção.
- **Autoridade:** somente a Equipe da associação decide nascimento fora do prazo e correções registrais.
- **Reprovação:** Resenha reprovada não pode ser reapresentada; deve ser criada uma nova Resenha.
- **Baixa:** o motivo é informado em texto livre pelo operador.
- **Nome:** números não são permitidos; Afixo e Nome-base são separados por espaço.
- **Correções e castração:** correções não suspendem nem revogam o Registro. Animal castrado que possuía Registro deve realizar nova Resenha.
- **Cobrança:** múltiplos fatos geradores resultam em uma Cobrança única consolidada, identificando todos os fatos geradores.

## 8. Decisões pendentes

Nenhuma das dez decisões de negócio originais permanece pendente. Permanecem apenas validações de implementação, incluindo a migração segura dos códigos legados para um enum e a definição do formato técnico da Cobrança consolidada.

### Proveniência a confirmar

- A referência à versão legada datada de 05/09/2026 é mantida como informação recebida, mas sua data e origem ainda precisam ser confirmadas.

### Implementação

- O código atual ainda não implementa o domínio Animal descrito nesta especificação; a divergência é esperada e deve ser tratada no planejamento de implementação, não resolvida silenciosamente nesta documentação.

## 9. Cenários de aceitação

- Nome Príncipe é gravado como PRÍNCIPE, com acento; Afixo, se houver, também é maiúsculo.
- Criador sem Afixo não bloqueia cadastro nem Registro; nome composto é o Nome-base.
- Nome composto já existente bloqueia cadastro, inclusive se o Animal anterior estiver Morto ou excluído.
- Transferência troca Proprietário e Plantel, preservando Criador, Afixo, nome e Registro.
- Baixa por venda encerra Plantel e preserva Animal e Registro.
- Morte altera Situação para Morto, impede novas operações incompatíveis e preserva Registro.
- Registro definitivo exige Provisório válido, três anos completos, data confirmada, Pendências resolvidas e Resenha aprovada.
- Castração de Animal que possuía Registro exige nova Resenha, não suspende nem revoga o Registro existente e impede novas Coberturas.
- Divergência de genealogia entre cadastro e Cobertura gera Pendência e mantém Cobertura como fonte.
- Cobertura cancelada não apaga genealogia automaticamente; Cria entra em regularização.

## 10. Rastreabilidade

Documento derivado da documentação legada fornecida. Esta especificação é a referência normativa de negócio e prevalece sobre comportamentos técnicos conflitantes do legado.
