# Documentação do Capri.Sgr

Este diretório concentra a documentação funcional e técnica do projeto Capri.Sgr.

## Contexto do produto

O Capri.Sgr será um CRM para associações de equinos, como Mangalarga, Mangalarga Marchador e outras entidades com processos semelhantes de registro, gestão de associados, gestão de animais, transferências e relacionamento com criadores/proprietários.

No início, o sistema será desenvolvido para uma associação. A arquitetura deve evitar acoplamentos que dificultem uma migração futura para multi-tenant, onde cada associação poderá operar isoladamente por schema de banco de dados e ser identificada pelo subdomínio utilizado no acesso.

## Documentos

- [Cadastro de Associado](features/cadastro-associado.md)

