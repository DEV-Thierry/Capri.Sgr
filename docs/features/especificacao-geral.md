# Especificação Geral do Produto — Capri.Sgr

**Status:** consolidada das especificações de Associado, Animal, Coberturas, UX/UI, contratos e ADRs; implementação pendente.  
**Linguagem canônica:** [CONTEXT.md](../../CONTEXT.md).  
**Precedência:** esta é a visão integrada. Em conflito, prevalecem o ADR aplicável e a especificação funcional específica.

## Problem Statement

Associações de equinos precisam administrar o relacionamento com Associados, Plantéis, Animais, Registro genealógico, Coberturas, Crias, Documentos associados, Cobranças, Pendências e comunicações em processos rastreáveis. O legado mistura regras de negócio, códigos técnicos e operações manuais, dificultando a compreensão das responsabilidades, a aplicação coerente de bloqueios e a preservação de histórico.

O Capri.Sgr deve oferecer um produto web que permita a interessados, Associados, Responsáveis e à Equipe da associação operar esses processos com linguagem clara, controles de acesso e Auditoria imutável. A primeira implantação atende uma associação, preservando uma direção de isolamento multi-tenant futuro.

## Solution

Construir um CRM para associações de equinos com Portal público institucional, Área do Associado e Área Administrativa. O produto deve organizar responsabilidades em módulos de autoridade própria — Cadastro de Associado, Animal, Coberturas, Transferência de Animal, Resenha, Registro genealógico, Documentos, Cobranças, Pendências, Notificações e Auditoria — conectados por fatos de negócio e referências auditáveis, sem duplicar a fonte de verdade.

A solução preserva identidade, genealogia, titularidade, decisões e obrigações ao longo do tempo. A Situação de um processo permanece distinta de Pendências: a primeira explica a etapa do fluxo; a segunda registra uma causa, seu bloqueio e sua condição de resolução. A interface sempre apresenta causa, efeito e caminho de regularização.

## User Stories

1. Como visitante, quero consultar Publicações institucionais e páginas públicas, para conhecer a associação sem autenticação.
2. Como interessado, quero criar, salvar e enviar uma Solicitação de associação, para ingressar na associação de forma acompanhável.
3. Como interessado pessoa física, quero registrar dados, contatos e Documentos associados obrigatórios, para demonstrar os requisitos de adesão.
4. Como interessado pessoa jurídica, quero vincular um Responsável principal ativo e Responsáveis adicionais, para permitir atuação autorizada em nome do Associado.
5. Como interessado ou Responsável, quero aceitar explicitamente o termo de adesão, para formalizar o envio e o consentimento registrado.
6. Como interessado, quero acessar provisoriamente a Área do Associado após enviar a solicitação, para acompanhar Pendências, documentos e Cobranças.
7. Como Equipe da associação, quero analisar documentos e solicitar correções pontuais, para resolver inconsistências sem encerrar a Solicitação de associação.
8. Como Equipe da associação, quero aprovar, rejeitar, cancelar, suspender, inativar e reativar com motivo, para decidir de maneira rastreável.
9. Como Associado, quero manter meus contatos e endereço atualizados, para receber comunicações e executar operações elegíveis.
10. Como Equipe da associação, quero retificar CPF ou CNPJ somente em processo excepcional auditado, para preservar a integridade da identidade.
11. Como Associado elegível, quero solicitar um Afixo único, para compor a identidade nominal de Animais dos quais sou Criador.
12. Como Administrador, quero administrar Usuários internos, perfis e Permissões administrativas granulares, para separar consulta, manutenção, decisão e configuração.
13. Como associação, quero emitir Cobranças iniciais e recorrentes conforme o Tipo de associado, para vincular obrigações financeiras ao fato gerador correto.
14. Como Equipe da associação, quero registrar pagamentos, cancelamentos e estornos manualmente com evidências, para operar o financeiro inicial sem gateway.
15. Como associação, quero gerar Propostas de reenquadramento de Tipo de associado sem alterar o tipo automaticamente, para submeter exceções à decisão interna.
16. Como Proprietário atual, quero cadastrar um Animal com identidade, Criador, genealogia e titularidade rastreáveis, para integrá-lo corretamente ao Plantel.
17. Como Criador, quero que o Nome composto do Animal use meu Afixo congelado no cadastro, para preservar a identidade do Animal mesmo após Transferência de Animal.
18. Como associação, quero impedir reutilização do Nome composto em todo o acervo, para preservar a rastreabilidade nominal histórica.
19. Como Proprietário atual, quero transferir um Animal sem modificar Criador, Afixo, nome ou Registro genealógico, para separar patrimônio de identidade.
20. Como associação, quero registrar Baixa de Plantel, morte e castração com seus efeitos próprios, para preservar o histórico e bloquear apenas operações incompatíveis.
21. Como associação, quero realizar Resenhas e conceder Registro genealógico segundo identidade, idade, Pendências e decisões, para reconhecer a genealogia de forma controlada.
22. Como Comunicante, quero comunicar uma Cobertura com Garanhão, Égua reprodutora, participantes, Modalidade, local e data, para preservar o evento reprodutivo.
23. Como Proprietário participante, quero aprovar ou reprovar apenas a decisão vinculada à minha propriedade no instante da comunicação, para manter a decisão histórica após Transferência de Animal.
24. Como Veterinário designado, quero realizar Validação veterinária autenticada, para confirmar Coberturas de Modalidades que a exigem.
25. Como associação, quero distinguir a Situação da Cobertura de seu Desfecho reprodutivo, para não confundir decisão administrativa com fato biológico.
26. Como Comunicante, quero confirmar a multa por comunicação tardia antes de persistir a Cobertura, para conhecer o valor congelado pela política vigente.
27. Como Pagador, quero receber a Cobrança principal após aprovação dos Proprietários participantes, para que a obrigação reflita uma Cobertura validada pelos participantes.
28. Como destinatário terceiro, quero assinar uma Declaração de transferência após quitar a cobrança própria, para formalizar meu Beneficiário de destino sobre futura Cria ou embrião.
29. Como associação, quero exigir Evidência documental antes de registrar Cria ou emitir documento oficial, sem perder uma Cobertura comunicada com documento pendente.
30. Como associação, quero registrar Crias atomicamente com o Desfecho reprodutivo, para garantir uma Cria em nasceu e duas ou mais em gêmeos.
31. Como Associado, quero encontrar situação, Plantel, Coberturas, Pendências, Cobranças, documentos e histórico na Área do Associado, para priorizar minhas ações.
32. Como Usuário interno, quero acessar apenas recursos e decisões permitidos, para trabalhar sem expor dados indevidos.
33. Como associação, quero notificar fatos relevantes e permitir reenvio após falhas de entrega, para comunicar sem reverter fatos de negócio.
34. Como controlador do produto, quero restringir e auditar acessos a documentos e dados sensíveis, para preparar o atendimento às obrigações de privacidade.
35. Como equipe de produto, quero preservar isolamento por associação como evolução futura, para viabilizar multi-tenancy sem reescrever o domínio.

## Implementation Decisions

### Arquitetura, módulos e contratos

- A base atual é uma solução .NET em camadas Domain, Application, Infrastructure e Web, com MediatR, validação, EF Core/Identity, endpoints mínimos e Aspire. Ela contém exemplos técnicos de Todo e previsão do tempo; os domínios do produto ainda não estão implementados.
- Cada módulo mantém sua autoridade e integra por fatos, decisões e referências auditáveis. Integrações não duplicam dados ou regras cuja autoridade pertença a outro módulo.
- Cadastro de Associado fornece Associado, Tipo de associado, elegibilidade, Solicitação de associação, Responsáveis e situação operacional. Animal fornece identidade, Criador, Proprietário atual, Plantel, Situação do Animal e Registro genealógico. Coberturas fornece evento reprodutivo, participantes, Modalidade, validações e Desfecho.
- Documentos, Cobranças, Pendências, Notificações e Auditoria são capacidades transversais com ciclo de vida próprio. Transferência de Animal, Resenha e Registro genealógico possuem fronteiras definidas, mas ainda exigem especificação funcional detalhada antes da implementação.

### Decisões normativas transversais

- O vocabulário de CONTEXT.md é obrigatório; não usar sinônimos ambíguos para Associado, Proprietário atual, Criador, Cobertura, Égua reprodutora, Égua receptora, Pendência ou Cobrança.
- Pendências são independentes do status principal, têm causa, estado, bloqueio e condição de resolução próprios. A decisão é registrada no ADR-0001.
- Auditoria é imutável e registra ator, instante com fuso/offset, canal, entidade, ação, motivo e antes/depois quando aplicável. Acessos sensíveis também são auditáveis.
- Notificações por e-mail e central interna não revertem transições de domínio em caso de falha; a falha permanece rastreável e reenviável.
- Jobs e efeitos derivados devem ser idempotentes, rastreáveis e reprocessáveis. As execuções registram identificador, início, fim, resultado e falhas parciais.
- America/Sao_Paulo é a referência operacional inicial de jobs, vencimentos e prazos. Instantes preservam fuso/offset e a interface exibe no fuso do navegador.
- A primeira implantação é para uma associação. A futura evolução multi-tenant deve resolver explicitamente a associação e isolar identidade, permissões, arquivos, cache, Auditoria, jobs e integrações.

### Seams de teste e integração

- O seam primário é a camada de aplicação: comandos e consultas recebem intenções de negócio, aplicam validações/autorização e retornam resultados observáveis.
- Invariantes puras de identidade, normalização, transições e cardinalidade residem no domínio e são testadas diretamente.
- Persistência, unicidade, auditoria, idempotência, jobs e documentos são verificados por integração de infraestrutura.
- Fluxos críticos e bloqueios explicados são verificados por testes de aceitação web. Os projetos existentes de testes de Domain, Application, Infrastructure e Web/Playwright devem ser ampliados; não criar uma hierarquia paralela de testes.

### Contratos de negócio relevantes

- Aprovação de Solicitação de associação exige Documentos associados obrigatórios aprovados e Cobrança aplicável quitada.
- Animal ativo tem exatamente um Proprietário atual e um vínculo de Plantel ativo. Transferência concluída troca ambos como uma única decisão de negócio.
- Registro definitivo exige Registro provisório válido, nascimento confirmado, idade mínima, Pendências impeditivas resolvidas e Resenha aprovada.
- Cobertura é fonte da genealogia da Cria; divergências abrem Pendência e exigem decisão auditada, sem apagar evidências.
- Módulos de origem emitem fatos geradores de Cobrança; Cobranças define valor, vencimento, quitação, cancelamento e estorno.

## Testing Decisions

- Bons testes verificam comportamento observável: decisão permitida ou bloqueada, transição, efeito gerado, dado preservado, validação e contrato. Não verificam métodos privados, ordem incidental de chamadas ou detalhes do ORM.
- O domínio cobre invariantes de Nome composto e Afixo, titularidade única, morte/castração, Registro genealógico, Solicitação de associação, Situação da Cobertura e cardinalidade de Crias.
- A aplicação cobre envio e aprovação de Solicitação, pagamento manual, correção documental, Transferência de Animal, Baixa de Plantel, comunicação/aprovação/cancelamento de Cobertura, Validação veterinária e composição de Crias.
- A infraestrutura cobre restrições de unicidade, transações de titularidade, versionamento de documentos, Auditoria, idempotência de Cobranças/Pendências/notificações/jobs e recuperação de falha parcial.
- A aceitação web cobre acesso provisório, regularização de Pendência, decisões administrativas, gestão de permissões, operações principais de Animal e Cobertura, acessibilidade e responsividade.
- Cada regra aceita nas especificações funcionais recebe ao menos uma evidência de teste no seam mais alto que a cubra com confiabilidade; testes de domínio complementam regras puras.

## Out of Scope

- Integração efetiva com PagSeguro, gateway, conciliação automática, pagamento parcial, parcelamento e pró-rata.
- Assinatura eletrônica avançada, autenticação em dois fatores obrigatória, chat em tempo real e edição livre de HTML/CSS no Portal público.
- Consultas governamentais de CPF/CNPJ, integração de CEP e expiração automática de Documentos associados.
- Política LGPD definitiva de retenção, anonimização e descarte automatizados; é pré-requisito de produção, mas não está decidida.
- Catálogo editável de Tipos de associado, fuso configurável por associação, permissões distintas por Responsável e prova de poderes de representação para pessoa jurídica.
- Implementação de multi-tenancy, schema por associação, subdomínios, provisionamento e operações globais.
- Fluxos ainda não detalhados de Transferência de Animal, atendimento, certificados e documentos oficiais.

## Further Notes

- Fontes consolidadas: Cadastro de Associado, Animal, Coberturas, UX/UI, Contratos entre módulos, Privacidade/retenção, Direção de multi-tenancy e ADR-0001.
- As especificações específicas permanecem a fonte detalhada de seus respectivos domínios; esta especificação organiza a visão geral e a decomposição de implementação.
- Pendências antes de produção: validação jurídica e operacional de privacidade/retenção/descarte; decisão final da estratégia de isolamento multi-tenant; contratos técnicos/versionamento de integrações e catálogo comum de eventos; especificações detalhadas de Transferência de Animal, Atendimento e documentos oficiais.
- A proveniência e a data da documentação legada de Animal ainda precisam de confirmação.
- A publicação no GitHub deve usar o título Especificação geral integrada do Capri.Sgr e aplicar o rótulo ready-for-agent.
