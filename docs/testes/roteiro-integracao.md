# Roteiro de testes de integração — GestaPME

Foco: fluxos ponta-a-ponta.

## Fluxo A — Cadastro completo e uso
1. Cadastro público de Empresa + Admin.
2. Criação de depto e cargo.
3. Cadastro de 3 funcionários.
4. Solicitação e aprovação de férias.
5. Pergunta ao Assistente IA: "quantos ativos?".
6. Logout.

## Fluxo B — Isolamento multi-empresa
1. Cadastrar Empresa A + Admin A.
2. Cadastrar Empresa B + Admin B (deslogar antes).
3. Logar como A, criar 1 funcionário, copiar o ID da URL de edição.
4. Deslogar, logar como B.
5. Tentar `/Funcionarios/Edit/{id-do-funcionario-de-A}`.
6. Esperado: 404 "não encontrado".

## Fluxo C — Permissões por perfil
1. Como Admin, criar um Gestor.
2. Logar como Gestor.
3. Tentar criar departamento: 403.
4. Tentar aprovar férias: botões não aparecem; POST direto: 403.
5. Pode listar funcionários, ver detalhes, solicitar férias.
