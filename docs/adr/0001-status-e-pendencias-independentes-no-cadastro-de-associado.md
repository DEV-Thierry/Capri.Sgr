# Status e pendências independentes no cadastro de associado

Status: aceito

A Solicitação de associação terá um único status principal que representa sua etapa no fluxo — como Rascunho, Aguardando pagamento, Aguardando aprovação, Aprovado, Rejeitado ou Cancelado — enquanto Pendências serão entidades independentes, categorizadas por causa e com ciclo de vida próprio. Esta separação foi escolhida em vez de combinar todos os motivos de bloqueio em um único status, porque uma solicitação ou Associado pode ter simultaneamente pendências documentais, financeiras, cadastrais, operacionais e de Reenquadramento, cada uma com resolução e efeitos distintos.

## Consequências

- A aprovação só é possível quando os pré-requisitos relevantes estiverem satisfeitos, incluindo documentos obrigatórios aprovados e quitação da Cobrança aplicável.
- Uma Pendência pode bloquear operações específicas sem alterar ou multiplicar os estados do fluxo principal.
- A causa, o estado, o bloqueio e a resolução de cada Pendência permanecem auditáveis independentemente da transição da Solicitação de associação.
- A especificação detalhada de estados, pendências e bloqueios está em [Cadastro de Associado](../features/cadastro-associado.md).
