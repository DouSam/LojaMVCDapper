# 🛒 TechStore MVC — Projeto Base de Referência (P2)
## Disciplina: Programação Orientada a Objetos (ILP007)
### FATEC Ribeirão Preto — Curso Superior de Tecnologia em ADS
**Professor:** Prof. Me. Douglas Samuel Gonçalves  
**Semestre:** 2º Semestre / 2026  

---

## 📌 1. Visão Geral do Projeto
Este repositório é um **projeto modelo e funcional de referência** construído para apoiar as equipes no desenvolvimento do **Projeto Prático Integrador (Avaliação Oficial — P2)**.

A aplicação implementa um portal web em **ASP.NET Core MVC**, persistindo dados em um banco relacional **SQLite** através do Micro-ORM **Dapper**, aplicando rigorosamente os conceitos de **POO**, separação de responsabilidades em camadas e boas práticas de engenharia de software.

---

## 🎯 2. Principais Conceitos e Requisitos Contemplados

| Ref. | Conceito Aplicado | Onde Encontrar no Projeto? |
| :--- | :--- | :--- |
| **POO** | **Encapsulamento e Invariantes** | `Models/Produto.cs` (Construtores ricos, validações *fail-fast* e propriedades calculadas). |
| **Generics** | **Padrão Repository Genérico** | `Repositories/Interfaces/IRepository.cs` com restrição `where T : class`. |
| **Herança** | **Especialização de Interfaces** | `Repositories/Interfaces/IProdutoRepository.cs` herdando de `IRepository<Produto>`. |
| **Persistência** | **Micro-ORM Dapper** | `Repositories/Dapper/ProdutoRepository.cs` com queries SQL 100% parametrizadas (anti SQL Injection). |
| **Banco Local** | **SQLite Serverless** | Arquivo `loja_mvc.db` criado automaticamente na inicialização via `Database/DatabaseInitializer.cs`. |
| **MVC** | **Controllers e Routing** | `Controllers/ProdutoController.cs` e `Controllers/HomeController.cs`. |
| **Inversão de Controle** | **Injeção de Dependência (DI)** | Configurada no `Program.cs` com ciclo `builder.Services.AddScoped<IProdutoRepository, ...>()`. |
| **Validação** | **Data Annotations & ModelState** | Validação client-side com `_ValidationScriptsPartial` e server-side com `ModelState.IsValid`. |
| **UX Web** | **Padrão Post-Redirect-Get (PRG)** | Ações de cadastro, alteração e exclusão redirecionando para a listagem para evitar duplicações no F5. |

---

## 📂 3. Estrutura de Pastas e Arquivos

```text
LojaMvcDapper/
|-- Controllers/
|   |-- HomeController.cs          --> Dashboard inicial com métricas e alertas de estoque
|   \-- ProdutoController.cs       --> Orquestrador completo das ações HTTP de Produto
|-- Models/
|   |-- Produto.cs                 --> Entidade de domínio rica com Data Annotations
|   \-- ErrorViewModel.cs          --> Modelo para telas de erro
|-- Repositories/
|   |-- Interfaces/
|   |   |-- IRepository.cs         --> Interface genérica base (IRepository<T>)
|   |   \-- IProdutoRepository.cs  --> Contrato especializado com métodos de negócio
|   \-- Dapper/
|       \-- ProdutoRepository.cs   --> Implementação do CRUD e consultas via Micro-ORM Dapper
|-- Database/
|   |-- script.sql                 --> Script DDL e DML para consulta e referência
|   \-- DatabaseInitializer.cs     --> Criação automática da tabela e dados de exemplo no SQLite
|-- Views/
|   |-- Home/
|   |   \-- Index.cshtml           --> Dashboard visual com cards e alertas em Bootstrap 5
|   |-- Produto/
|   |   |-- Index.cshtml           --> Tela 1: Catálogo com filtros, busca e badges de estoque
|   |   |-- Criar.cshtml           --> Tela 2: Formulário de cadastro com validação
|   |   |-- Detalhes.cshtml        --> Tela 3: Visualização individual e patrimônio imobilizado
|   |   |-- Editar.cshtml          --> Tela 4: Formulário de atualização
|   |   \-- Excluir.cshtml         --> Tela 5: Confirmação de exclusão segura
|   \-- Shared/
|       |-- _Layout.cshtml         --> Layout mestre com navbar, alerts e rodapé FATEC
|       |-- _ValidationScriptsPartial.cshtml --> Scripts jQuery para validação client-side
|       \-- _ViewImports.cshtml    --> Importações globais e Tag Helpers
|-- appsettings.json               --> String de conexão centralizada ("Data Source=loja_mvc.db")
|-- Program.cs                     --> Inicialização do app, DI, middlewares e rotas
\-- LojaMvcDapper.csproj           --> Arquivo de projeto e pacotes NuGet (Dapper e SQLite)
```

---

## 🚀 4. Como Executar o Projeto na sua Máquina

### Pré-requisitos:
- [.NET SDK 8.0 ou superior](https://dotnet.microsoft.com/download) instalado.
- Visual Studio Code, Visual Studio 2022 ou Rider.

### Passo a Passo no Terminal:

1. **Abra o terminal** na pasta do projeto:
   ```bash
   cd codigo/LojaMvcDapper
   ```

2. **Restaure as dependências NuGet:**
   ```bash
   dotnet restore
   ```

3. **Execute a aplicação:**
   ```bash
   dotnet run
   ```

4. **Acesse no navegador:**
   - Acesse: `http://localhost:5000` ou a porta indicada no terminal (ex: `https://localhost:7001`).
   - O arquivo SQLite `loja_mvc.db` será criado **automaticamente** e já virá populado com 6 produtos de teste!

---

## 🖥️ 5. Telas Implementadas

1. **Dashboard Inicial (`/`):**
   - Cards com indicadores em tempo real: Total de produtos cadastrados, Patrimônio total em estoque (R$) e Quantidade de itens críticos.
   - Tabela de alertas para reposição imediata de produtos com estoque crítico ($\le 5$ un).

2. **Catálogo de Produtos (`/Produto`):**
   - Tabela responsiva com badges visuais de estoque (verde/azul para níveis normais, vermelho para crítico).
   - Barra de pesquisa para busca rápida por nome ou categoria.
   - Botões de ação rápida para Detalhes, Edição e Exclusão.

3. **Cadastro de Produto (`/Produto/Criar`):**
   - Formulário com Tag Helpers modernos (`asp-for`, `asp-action`).
   - Validação imediata no navegador (sem recarregar) e proteção server-side via `ModelState.IsValid`.

4. **Painel de Detalhes (`/Produto/Detalhes/{id}`):**
   - Card completo com cálculos de domínio (ex: valor total do estoque imobilizado).

5. **Edição e Exclusão Segura (`/Produto/Editar` e `/Produto/Excluir`):**
   - Atualização de dados e tela de confirmação antes de deletar registros.

---

## 💡 6. Dicas para o Projeto Integrador (P2)
Para o projeto oficial da sua equipe, adapte esta arquitetura para o tema escolhido:
1. **Modelos:** Crie suas 3 ou mais entidades com relacionamentos (1:N ou N:N) dentro da pasta `Models/`.
2. **Repositórios:** Cada entidade de domínio deve ter sua interface em `Repositories/Interfaces/` e sua classe Dapper em `Repositories/Dapper/`.
3. **Injeção de Dependência:** Registre cada repositório no `Program.cs` usando `builder.Services.AddScoped<I...Repository, ...Repository>()`.
4. **Controllers e Views:** Crie um Controller para cada entidade e sua respectiva subpasta dentro de `Views/`.
