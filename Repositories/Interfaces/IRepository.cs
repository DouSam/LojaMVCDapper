namespace LojaMvcDapper.Repositories.Interfaces;

/// <summary>
/// Contrato de Repositório Genérico (Padrão Repository com Generics)
/// Aplica a restrição 'where T : class' para garantir que T seja um tipo de referência.
/// Atende ao Requisito R5 do Edital da Avaliação P2.
/// </summary>
/// <typeparam name="T">Entidade de domínio a ser persistida</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Insere uma nova entidade no banco de dados.
    /// </summary>
    void Adicionar(T entidade);

    /// <summary>
    /// Atualiza os dados de uma entidade já existente.
    /// </summary>
    void Atualizar(T entidade);

    /// <summary>
    /// Remove um registro pelo seu identificador único.
    /// </summary>
    void Remover(int id);

    /// <summary>
    /// Localiza uma entidade pelo seu Id ou retorna null se não existir.
    /// </summary>
    T? ObterPorId(int id);

    /// <summary>
    /// Retorna todos os registros cadastrados na tabela correspondente.
    /// </summary>
    IEnumerable<T> ObterTodos();
}
