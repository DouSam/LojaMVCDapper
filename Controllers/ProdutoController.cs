using Microsoft.AspNetCore.Mvc;
using LojaMvcDapper.Models;
using LojaMvcDapper.Repositories.Interfaces;

namespace LojaMvcDapper.Controllers;

/// <summary>
/// Controller responsável por orquestrar todo o ciclo de vida do catálogo de Produtos.
/// Demonstra Injeção de Dependência via construtor, ações HTTP GET e POST,
/// validação com ModelState e o padrão Post-Redirect-Get (PRG).
/// </summary>
public class ProdutoController : Controller
{
    private readonly IProdutoRepository _produtoRepo;

    /// <summary>
    /// Injeção de Dependência no Construtor:
    /// O ASP.NET Core entrega automaticamente a implementação de ProdutoRepository
    /// registrada no Program.cs com o ciclo Scoped. O Controller NUNCA faz 'new'.
    /// </summary>
    public ProdutoController(IProdutoRepository produtoRepo)
    {
        _produtoRepo = produtoRepo ?? throw new ArgumentNullException(nameof(produtoRepo));
    }

    // =========================================================================
    // 1. TELA DE LISTAGEM (Index) - HTTP GET
    // =========================================================================

    /// <summary>
    /// Exibe a página principal com a tabela de produtos cadastrados.
    /// Suporta filtros opcionais via Query String: busca por termo ou apenas estoque crítico.
    /// Exemplo de URLs: /Produto, /Produto?termoBusca=gamer, /Produto?apenasCriticos=true
    /// </summary>
    [HttpGet]
    public IActionResult Index(string? termoBusca, bool? apenasCriticos)
    {
        IEnumerable<Produto> produtos;

        if (apenasCriticos == true)
        {
            produtos = _produtoRepo.ObterComEstoqueBaixo(5);
            ViewBag.FiltroAtivo = "Produtos com Estoque Crítico (<= 5 unidades)";
        }
        else if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            produtos = _produtoRepo.BuscarPorTermo(termoBusca);
            ViewBag.TermoBusca = termoBusca;
            ViewBag.FiltroAtivo = $"Filtrado por: '{termoBusca}'";
        }
        else
        {
            produtos = _produtoRepo.ObterTodos();
        }

        // Passamos a coleção 'produtos' como Model fortemente tipado para Views/Produto/Index.cshtml
        return View(produtos);
    }

    // =========================================================================
    // 2. TELA DE CADASTRO (Criar) - HTTP GET & POST
    // =========================================================================

    /// <summary>
    /// GET: /Produto/Criar
    /// Apenas carrega a página com o formulário em branco para digitação.
    /// </summary>
    [HttpGet]
    public IActionResult Criar()
    {
        return View();
    }

    /// <summary>
    /// POST: /Produto/Criar
    /// Recebe os dados preenchidos no formulário pelo usuário via corpo da requisição HTTP.
    /// </summary>
    /// <param name="produto">Objeto Produto montado pelo Model Binding do ASP.NET</param>
    [HttpPost]
    [ValidateAntiForgeryToken] // Prevenção contra ataques CSRF (Cross-Site Request Forgery)
    public IActionResult Criar(Produto produto)
    {
        // 1. O framework inspeciona se todas as Data Annotations de Produto.cs foram satisfeitas
        if (!ModelState.IsValid)
        {
            // Se houver erros, retorna a mesma View com os dados digitados e mensagens de erro
            return View(produto);
        }

        // 2. Persiste a entidade no banco de dados SQLite através do Dapper
        _produtoRepo.Adicionar(produto);

        // Feedback visual amigável usando TempData (persiste durante um único redirecionamento)
        TempData["Sucesso"] = $"Produto '{produto.Nome}' cadastrado com sucesso!";

        // 3. Padrão Post-Redirect-Get (PRG):
        // Retorna HTTP 302 redirecionando para a Action Index, evitando reenvios ao apertar F5
        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // 3. TELA DE DETALHES (Detalhes) - HTTP GET
    // =========================================================================

    /// <summary>
    /// GET: /Produto/Detalhes/{id}
    /// Exibe um painel visual com todas as informações e métricas de um produto específico.
    /// </summary>
    [HttpGet]
    public IActionResult Detalhes(int id)
    {
        var produto = _produtoRepo.ObterPorId(id);

        if (produto == null)
        {
            // Se o ID não existe no banco, devolve status HTTP 404 amigável
            return NotFound($"Nenhum produto com ID #{id} foi localizado no sistema.");
        }

        return View(produto);
    }

    // =========================================================================
    // 4. TELA DE EDIÇÃO (Editar) - HTTP GET & POST
    // =========================================================================

    /// <summary>
    /// GET: /Produto/Editar/{id}
    /// Busca o produto no banco e carrega o formulário pré-preenchido para edição.
    /// </summary>
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var produto = _produtoRepo.ObterPorId(id);

        if (produto == null)
            return NotFound($"Produto #{id} não localizado para edição.");

        return View(produto);
    }

    /// <summary>
    /// POST: /Produto/Editar/{id}
    /// Processa a submissão dos dados atualizados com validação.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, Produto produto)
    {
        if (id != produto.Id)
            return BadRequest("Inconsistência entre o ID da URL e o ID do formulário.");

        if (!ModelState.IsValid)
            return View(produto);

        _produtoRepo.Atualizar(produto);

        TempData["Sucesso"] = $"Produto '{produto.Nome}' atualizado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    // =========================================================================
    // 5. TELA DE EXCLUSÃO (Excluir) - HTTP GET & POST
    // =========================================================================

    /// <summary>
    /// GET: /Produto/Excluir/{id}
    /// Exibe a tela de confirmação de segurança antes da remoção definitiva.
    /// </summary>
    [HttpGet]
    public IActionResult Excluir(int id)
    {
        var produto = _produtoRepo.ObterPorId(id);

        if (produto == null)
            return NotFound($"Produto #{id} não localizado.");

        return View(produto);
    }

    /// <summary>
    /// POST: /Produto/Excluir/{id}
    /// Confirma e executa a exclusão definitiva no SQLite via Dapper.
    /// </summary>
    [HttpPost, ActionName("Excluir")]
    [ValidateAntiForgeryToken]
    public IActionResult ExcluirConfirmado(int id)
    {
        var produto = _produtoRepo.ObterPorId(id);

        if (produto != null)
        {
            _produtoRepo.Remover(id);
            TempData["Sucesso"] = $"Produto '{produto.Nome}' excluído com sucesso!";
        }

        return RedirectToAction(nameof(Index));
    }
}
