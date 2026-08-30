# Documentação do Capri.Sgr

Este diretório concentra a documentação funcional e técnica do projeto Capri.Sgr.

## Contexto do produto

O Capri.Sgr será um CRM para associações de equinos, como Mangalarga, Mangalarga Marchador e outras entidades com processos semelhantes de registro, gestão de associados, gestão de animais, transferências e relacionamento com criadores/proprietários.

No início, o sistema será desenvolvido para uma associação. A arquitetura deve evitar acoplamentos que dificultem uma migração futura para multi-tenant, onde cada associação poderá operar isoladamente por schema de banco de dados e ser identificada pelo subdomínio utilizado no acesso.

## Contexto de domínio

O projeto adota organização single-context. O [glossário de domínio](../CONTEXT.md) define a linguagem canônica compartilhada pelo produto.

## Especificações funcionais

- [Especificação Geral do Produto](features/especificacao-geral.md): visão integrada do produto, dos módulos, contratos, decisões e estratégia de testes.
- [Cadastro de Associado](features/cadastro-associado.md): especificação funcional do domínio de associados.
- [Animal](features/animal.md): regras de identidade, Registro genealógico, titularidade, Plantel e ciclo de vida de Animais. A especificação contém decisões abertas que ainda exigem validação de negócio.
- [Coberturas](features/coberturas.md): especificação funcional do domínio reprodutivo.
- [UX/UI — Portal público, Área do Associado e Área Administrativa](features/ux-ui-portal-e-area-administrativa.md): experiência, navegação, permissões, configuração visual e requisitos de qualidade das interfaces do produto.

Transferência de Animal e Atendimento possuem pontos de entrada descritos na especificação de UX/UI, mas ainda não têm especificação funcional detalhada.

## Documentos transversais

- [Contratos entre módulos](contracts.md): fronteiras e dependências de negócio entre os módulos documentados.
- [Privacidade, retenção e descarte](privacy-retention.md): política provisória e pré-requisitos LGPD para produção.
- [Direção de multi-tenancy](multi-tenancy.md): intenção futura, requisitos e decisões ainda abertas.

## ADRs

- [Decisões de arquitetura](adr/): decisões que afetam o sistema.
- [ADR-0001 — Status e pendências independentes no cadastro de associado](adr/0001-status-e-pendencias-independentes-no-cadastro-de-associado.md).

