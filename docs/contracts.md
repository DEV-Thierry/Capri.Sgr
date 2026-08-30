# Contratos entre módulos

**Status:** referência de fronteiras; não define APIs nem substitui decisões de negócio.

Este documento consolida os contratos de negócio atualmente expressos nas especificações. Onde não há evidência suficiente, a lacuna permanece explícita.

## Princípios

- Cada módulo é responsável por suas próprias regras e decisões.
- Integrações devem transportar fatos, decisões e referências auditáveis, sem duplicar a autoridade de outro módulo.
- Pendências podem restringir operações específicas; não alteram automaticamente o estado principal de outro fluxo.
- Divergências entre cadastro e fatos de origem devem gerar Pendência e Auditoria.

## Fronteiras confirmadas

| Módulo | Fornece | Consome / depende |
|---|---|---|
| Cadastro de Associado | Associado, Tipo de associado, elegibilidade, Solicitação de associação, Documentos associados e Cobrança inicial | Documentos, Cobranças, Notificações, Pendências e Auditoria |
| Animal | identidade, Criador, Proprietário atual, Plantel, Registro genealógico, Situação do Animal e fatos geradores | Cadastro, Transferência de Animal, Reprodução, Resenha, Cobranças, Pendências e Auditoria |
| Coberturas | evento reprodutivo, participantes, modalidade, validações e desfecho | Animal, Cadastro, Documentos, Cobranças, Pendências, Notificações e Auditoria |
| Transferência de Animal | mudança formal de Proprietário atual e histórico de titularidade | Animal, Cadastro, Documentos, Cobranças, Pendências e Auditoria |
| Documentos | identidade, versão, vigência e análise de evidências | regras do módulo que exige o documento |
| Cobranças | valor, vencimento, quitação, cancelamento e estorno | fatos geradores e elegibilidade do módulo de origem |
| Notificações | entrega e histórico de comunicação | eventos publicáveis dos módulos |
| Auditoria | registro imutável de fatos relevantes | todos os módulos que produzem decisões ou alterações sensíveis |

## Estados e pré-condições

- Aprovação de Solicitação de associação depende dos Documentos obrigatórios aprovados e da Cobrança aplicável quitada.
- Registro definitivo depende de Registro Provisório válido, idade, data confirmada, Pendências resolvidas e Resenha aprovada.
- Transferência concluída altera Proprietário atual e Plantel, preservando identidade, Criador, Afixo, nome, Registro e histórico.
- Cobertura é a fonte do vínculo reprodutivo; divergência cadastral gera Pendência.

## Lacunas para contratos futuros

- Formato técnico e versionamento das integrações.
- Catálogo comum de eventos e idempotência.
- Autoridade formal para decisões registrais e reapresentações.
- Formato técnico, versionamento e idempotência da Cobrança única consolidada.
- O dispatcher/outbox durável dos efeitos derivados e o catálogo de Permissões administrativas granulares serão tratados nos tickets específicos; esta fundação mantém somente o registro idempotente reprocessável e a autenticação/autorização de plataforma.
