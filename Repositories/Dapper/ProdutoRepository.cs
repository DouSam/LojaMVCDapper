using Dapper;
using Microsoft.Data.Sqlite;
using LojaMvcDapper.Models;
using LojaMvcDapper.Repositories.Interfaces;

namespace LojaMvcDapper.Repositories.Dapper;

/// <summary>
/// Implementação concreta do Repositório de Produtos utilizando o Micro-ORM Dapper e SQLite.
/// Todas as consultas e comandos utilizam queries estritamente parametrizadas com '@',
/// garantindo proteção total contra ataques de SQL Injection.
/// </summary>
public class ProdutoRepository : IProdutoRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// Construtor que recebe a string de conexão configurada no appsettings.json.
    /// Injetado automaticamente pelo container de DI do ASP.NET Core no Program.cs.
    /// </summary>
    public ProdutoRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Método auxiliar que instancia e retorna uma nova conexão com o SQLite.
    /// Importante: A conexão deve ser aberta e descartada dentro de um bloco 'using' em cada operação.
    /// </summary>
    private SqliteConnection ObterConexao()
    {
        return new SqliteConnection(_connectionString);
    }

    // =========================================================================
    // IMPLEMENTAÇÃO DOS MÉTODOS DE ESCRITA (INSERT, UPDATE, DELETE)
    // =========================================================================

    /// <summary>
    /// Insere um novo produto na tabela 'Produtos' e captura o Id auto-incrementado gerado pelo SQLite.
    /// </summary>
    public void Adicionar(Produto entidade)
    {
        using var connection = ObterConexao();

        // O comando SQL utiliza parametros (@Nome, @Preco...)
        // Em seguida, 'last_insert_rowid()' devolve o ID gerado pelo SQLite nesta conexao
        const string sql = @"
            INSERT INTO Produtos (Nome, Categoria, Preco, Estoque, DataCadastro)
            VALUES (@Nome, @Categoria, @Preco, @Estoque, @DataCadastro);
            SELECT last_insert_rowid();";

        // ExecuteScalar executa a query e retorna a 1ª coluna da 1ª linha como int
        // O Dapper extrai automaticamente as propriedades do objeto 'entidade' para os parametros @
        int idGerado = connection.ExecuteScalar<int>(sql, new
        {
            entidade.Nome,
            entidade.Categoria,
            entidade.Preco,
            entidade.Estoque,
            DataCadastro = entidade.DataCadastro.ToString("yyyy-MM-dd HH:mm:ss")
        });

        entidade.Id = idGerado;
    }

    /// <summary>
    /// Atualiza as informações de um produto existente com base no seu Id.
    /// </summary>
    public void Atualizar(Produto entidade)
    {
        using var connection = ObterConexao();

        const string sql = @"
            UPDATE Produtos 
            SET Nome = @Nome,
                Categoria = @Categoria,
                Preco = @Preco,
                Estoque = @Estoque
            WHERE Id = @Id;";

        // connection.Execute retorna a quantidade de linhas afetadas no banco
        connection.Execute(sql, entidade);
    }

    /// <summary>
    /// Exclui um produto do banco de dados pelo seu identificador primário.
    /// </summary>
    public void Remover(int id)
    {
        using var connection = ObterConexao();

        const string sql = "DELETE FROM Produtos WHERE Id = @Id;";

        // Passamos um objeto anonimo com a propriedade 'Id' correspondente ao parametro @Id
        connection.Execute(sql, new { Id = id });
    }

    // =========================================================================
    // IMPLEMENTAÇÃO DOS MÉTODOS DE LEITURA (SELECT)
    // =========================================================================

    /// <summary>
    /// Busca um produto pelo seu Id primário.
    /// Retorna a instância de Produto preenchida ou null caso não exista.
    /// </summary>
    public Produto? ObterPorId(int id)
    {
        using var connection = ObterConexao();

        const string sql = @"
            SELECT Id, Nome, Categoria, Preco, Estoque, DataCadastro 
            FROM Produtos 
            WHERE Id = @Id;";

        // QueryFirstOrDefault retorna o primeiro registro mapeado ou default(Produto) = null
        return connection.QueryFirstOrDefault<Produto>(sql, new { Id = id });
    }

    /// <summary>
    /// Retorna todos os produtos ordenados alfabeticamente pelo nome.
    /// </summary>
    public IEnumerable<Produto> ObterTodos()
    {
        using var connection = ObterConexao();

        const string sql = @"
            SELECT Id, Nome, Categoria, Preco, Estoque, DataCadastro 
            FROM Produtos 
            ORDER BY Nome ASC;";

        // Query<T> itera o DataReader e instancia uma lista IEnumerable<Produto> de alta performance
        return connection.Query<Produto>(sql);
    }

    /// <summary>
    /// Retorna produtos com quantidade em estoque menor ou igual ao limite.
    /// </summary>
    public IEnumerable<Produto> ObterComEstoqueBaixo(int limite = 5)
    {
        using var connection = ObterConexao();

        const string sql = @"
            SELECT Id, Nome, Categoria, Preco, Estoque, DataCadastro 
            FROM Produtos 
            WHERE Estoque <= @Limite 
            ORDER BY Estoque ASC;";

        return connection.Query<Produto>(sql, new { Limite = limite });
    }

    /// <summary>
    /// Realiza busca filtrada por termo no nome ou na categoria (busca parcial case-insensitive).
    /// </summary>
    public IEnumerable<Produto> BuscarPorTermo(string termo)
    {
        using var connection = ObterConexao();

        const string sql = @"
            SELECT Id, Nome, Categoria, Preco, Estoque, DataCadastro 
            FROM Produtos 
            WHERE Nome LIKE @Termo OR Categoria LIKE @Termo 
            ORDER BY Nome ASC;";

        return connection.Query<Produto>(sql, new { Termo = $"%{termo}%" });
    }

    /// <summary>
    /// Calcula indicadores consolidados para o Dashboard inicial da aplicação.
    /// </summary>
    public (int TotalItens, decimal ValorTotalEstoque, int ItensCriticos) ObterMetricasDashboard()
    {
        using var connection = ObterConexao();

        const string sql = @"
            SELECT 
                COUNT(*) AS TotalItens,
                COALESCE(SUM(Preco * Estoque), 0) AS ValorTotalEstoque,
                COALESCE(SUM(CASE WHEN Estoque <= 5 THEN 1 ELSE 0 END), 0) AS ItensCriticos
            FROM Produtos;";

        var resultado = connection.QueryFirst(sql);
        return (
            TotalItens: (int)(long)resultado.TotalItens,
            ValorTotalEstoque: (decimal)resultado.ValorTotalEstoque,
            ItensCriticos: (int)(long)resultado.ItensCriticos
        );
    }
}
