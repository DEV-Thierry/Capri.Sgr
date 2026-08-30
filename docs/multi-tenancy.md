# Direção de multi-tenancy

**Status:** direção arquitetural futura; não implementada nem aprovada como decisão final.

## Intenção registrada

O sistema começa atendendo uma associação. A documentação do produto orienta evitar acoplamentos que dificultem uma futura operação multi-tenant, com isolamento por schema de banco de dados e identificação pelo subdomínio.

## Requisitos a preservar

- Um contexto de associação não pode ler ou alterar dados de outro contexto.
- Toda operação deve resolver explicitamente a associação antes de acessar dados do domínio.
- Identidade, permissões, cache, arquivos, auditoria, jobs e integrações devem respeitar o contexto.
- Migrações, backups, restauração e observabilidade devem permitir comprovar isolamento.
- Subdomínio inválido, desconhecido ou ausente deve ter comportamento definido antes da adoção.

## Decisões abertas

- Schema por associação versus outra estratégia de isolamento.
- Autoridade para resolver subdomínio e associação.
- Administração global e operações de suporte.
- Provisionamento, migração, exclusão e recuperação de um contexto.
- Política de nomes, domínios personalizados e ambientes locais.
- Estratégia de testes de isolamento e prevenção de vazamento.

Nenhum requisito desta página deve ser interpretado como implementação existente.
