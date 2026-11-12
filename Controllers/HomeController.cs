using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LojaMvcDapper.Models;
using LojaMvcDapper.Repositories.Interfaces;

namespace LojaMvcDapper.Controllers;

/// <summary>
/// Controller da Página Inicial (Dashboard da Loja)
/// Demonstra a injeção do repositório para carregar métricas consolidadas em tempo real.
/// </summary>
public class HomeController : Controller
{
    private readonly IProdutoRepository _produtoRepo;

    public HomeController(IProdutoRepository produtoRepo)
    {
        _produtoRepo = produtoRepo ?? throw new ArgumentNullException(nameof(produtoRepo));
    }

    /// <summary>
    /// GET: / (Raiz da aplicação)
    /// Carrega as métricas de catálogo e estoque para exibição em cards visuais.
    /// </summary>
    public IActionResult Index()
    {
        var metricas = _produtoRepo.ObterMetricasDashboard();
        ViewBag.TotalItens = metricas.TotalItens;
        ViewBag.ValorTotalEstoque = metricas.ValorTotalEstoque;
        ViewBag.ItensCriticos = metricas.ItensCriticos;

        // Lista recente de produtos com estoque baixo para alerta na Home
        var produtosCriticos = _produtoRepo.ObterComEstoqueBaixo(5);
        return View(produtosCriticos);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
