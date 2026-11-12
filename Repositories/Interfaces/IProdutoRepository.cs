using LojaMvcDapper.Models;

namespace LojaMvcDapper.Repositories.Interfaces;

/// <summary>
/// Contrato de Repositório Especializado para a entidade Produto.
/// Herda todas as operações de CRUD da interface genérica IRepository&lt;Produto&gt;
/// e adiciona operações de negócio específicas para o catálogo de produtos.
/// </summary>
public interface IProdutoRepository : IRepository<Produto>
{
    /// <summary>
    /// Retorna produtos com quantidade em estoque menor ou igual ao limite especificado.
    /// Utilizado para alertas de estoque crítico na loja.
    /// </summary>
    IEnumerable<Produto> ObterComEstoqueBaixo(int limite = 5);

    /// <summary>
    /// Busca produtos filtrando pelo nome ou categoria (busca parcial case-insensitive).
    /// </summary>
    IEnumerable<Produto> BuscarPorTermo(string termo);

    /// <summary>
    /// Retorna métricas consolidadas do catálogo (Total de itens, valor total em estoque).
    /// </summary>
    (int TotalItens, decimal ValorTotalEstoque, int ItensCriticos) ObterMetricasDashboard();
}
