using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LojaMvcDapper.Models;

/// <summary>
/// Entidade de Domínio: Produto
/// Representa uma mercadoria gerenciada no catálogo da loja.
/// Demonstra os pilares de POO (encapsulamento, validação de invariantes e construtores)
/// combinado com Data Annotations do ASP.NET Core para validação client e server-side.
/// </summary>
public class Produto
{
    // =========================================================================
    // PROPRIEDADES (Colunas da tabela 'Produtos' no banco de dados SQLite)
    // =========================================================================

    /// <summary>
    /// Identificador único (Chave Primária / Auto Increment no banco de dados).
    /// </summary>
    [DisplayName("Código (ID)")]
    public int Id { get; set; }

    /// <summary>
    /// Nome comercial do produto.
    /// Validações: Obrigatório e tamanho entre 3 e 100 caracteres.
    /// </summary>
    [Required(ErrorMessage = "O nome do produto é de preenchimento obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 100 caracteres.")]
    [DisplayName("Nome do Produto")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Categoria comercial (ex: Periféricos, Hardware, Monitores, Acessórios).
    /// </summary>
    [Required(ErrorMessage = "Informe a categoria do produto.")]
    [StringLength(50, ErrorMessage = "A categoria não pode ultrapassar 50 caracteres.")]
    [DisplayName("Categoria")]
    public string Categoria { get; set; } = string.Empty;

    /// <summary>
    /// Preço de venda unitário em Reais.
    /// Validações: Deve ser estritamente positivo (entre 0.05 e 999.999,00).
    /// </summary>
    [Required(ErrorMessage = "O preço unitário deve ser informado.")]
    [Range(0.05, 999999.00, ErrorMessage = "O preço deve ser maior que R$ 0,05.")]
    [DataType(DataType.Currency)]
    [DisplayName("Preço Unitário (R$)")]
    public decimal Preco { get; set; }

    /// <summary>
    /// Quantidade física disponível em estoque.
    /// Validações: Não pode ser um valor negativo.
    /// </summary>
    [Required(ErrorMessage = "A quantidade em estoque deve ser informada.")]
    [Range(0, 100000, ErrorMessage = "A quantidade de estoque não pode ser negativa.")]
    [DisplayName("Qtd. em Estoque")]
    public int Estoque { get; set; }

    /// <summary>
    /// Data e horário do cadastramento no sistema.
    /// </summary>
    [DisplayName("Data de Cadastro")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    // =========================================================================
    // PROPRIEDADES CALCULADAS DE DOMÍNIO (Não mapeadas como coluna no banco)
    // =========================================================================

    /// <summary>
    /// Calcula o valor financeiro total imobilizado neste produto no estoque.
    /// </summary>
    [DisplayName("Valor Total em Estoque")]
    public decimal ValorTotalEmEstoque => Preco * Estoque;

    /// <summary>
    /// Indica se o produto está com estoque crítico (menor ou igual a 5 unidades).
    /// </summary>
    public bool EstoqueCritico => Estoque <= 5;

    // =========================================================================
    // CONSTRUTORES
    // =========================================================================

    /// <summary>
    /// Construtor sem parâmetros:
    /// OBRIGATÓRIO para o Dapper e para o Model Binder do ASP.NET Core MVC.
    /// Ambos utilizam reflexão para criar instâncias vazias e atribuir as propriedades.
    /// </summary>
    public Produto() { }

    /// <summary>
    /// Construtor rico:
    /// Utilizado quando queremos garantir a instanciação de um objeto válido e seguro em código.
    /// Aplica o princípio Fail-Fast validando os argumentos.
    /// </summary>
    public Produto(string nome, string categoria, decimal preco, int estoque)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do produto não pode ser nulo ou vazio.", nameof(nome));

        if (preco <= 0)
            throw new ArgumentOutOfRangeException(nameof(preco), "O preço deve ser estritamente positivo.");

        if (estoque < 0)
            throw new ArgumentOutOfRangeException(nameof(estoque), "A quantidade de estoque não pode ser negativa.");

        Nome = nome;
        Categoria = categoria;
        Preco = preco;
        Estoque = estoque;
        DataCadastro = DateTime.Now;
    }
}
