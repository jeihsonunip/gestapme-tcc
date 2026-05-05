# Roteiro de testes funcionais — GestaPME

**Objetivo:** validar o comportamento individual de cada módulo.

## 1. Cadastro de empresa e login
Pré: banco limpo.
- [ ] Acessar `/`. Deve redirecionar para `/Conta/Login`.
- [ ] Clicar "Cadastre sua empresa". Preencher e submeter.
- [ ] Esperado: redirect para `/` logado + alert de boas-vindas.

## 2. Departamentos — CRUD
- [ ] Criar 3 departamentos (TI, RH, Financeiro).
- [ ] Editar descrição de TI.
- [ ] Tentar excluir sem funcionários: ok.
- [ ] Criar funcionário em TI, tentar excluir TI: esperado erro "não é possível excluir".

## 3. Cargos — CRUD
- [ ] Repetir os mesmos passos de 2 para Cargos.

## 4. Funcionários — CRUD + filtros
- [ ] Criar 5 funcionários em 2 deptos diferentes.
- [ ] Usar filtro por departamento: só 5 filtrados.
- [ ] Usar busca por nome parcial: retorna só os que casam.
- [ ] Inativar um funcionário: sai do filtro "ativos".

## 5. Férias
- [ ] Solicitar férias de um funcionário ativo.
- [ ] Solicitar nova, sobrepondo datas: esperado erro.
- [ ] Aprovar (como Admin). Ver no painel inicial "em curso" (se datas incluem hoje).

## 6. Usuários
- [ ] Criar um Gestor com senha `senha123`.
- [ ] Logar com ele. Ver sidebar sem "Usuários".
- [ ] Tentar URL direta `/Usuarios`: esperado redirecionar para `/Conta/AcessoNegado`.

## 7. Assistente IA
- [ ] Clicar o chip "Funcionários ativos?". Verificar resposta coerente com o cadastro.

## 8. Minha conta
- [ ] Alterar senha com senha atual errada: esperado erro.
- [ ] Alterar com senha certa: sucesso. Deslogar e logar com a nova.
