using LojaMvcDapper.Database;
using LojaMvcDapper.Repositories.Dapper;
using LojaMvcDapper.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// 1. CONFIGURAÇÃO DE SERVIÇOS (CONTAINER DE INJEÇÃO DE DEPENDÊNCIA - DI)
// =============================================================================

// Adiciona o suporte a Controllers e Views Razor do ASP.NET Core MVC
builder.Services.AddControllersWithViews();

// Recupera a String de Conexão definida no arquivo 'appsettings.json'
string connectionString = builder.Configuration.GetConnectionString("SqliteConnection")
    ?? "Data Source=loja_mvc.db";

// Inicializa o banco de dados SQLite (cria o arquivo loja_mvc.db, tabelas e dados iniciais caso não existam)
DatabaseInitializer.Inicializar(connectionString);

// Registra a Injeção de Dependência do Repositório de Produtos:
// Ciclo Scoped: Uma única instância de ProdutoRepository é instanciada por requisição HTTP.
// Sempre que um Controller solicitar 'IProdutoRepository' em seu construtor, o ASP.NET Core entrega esta instância.
builder.Services.AddScoped<IProdutoRepository>(provider => new ProdutoRepository(connectionString));

var app = builder.Build();

// =============================================================================
// 2. CONFIGURAÇÃO DO PIPELINE HTTP (MIDDLEWARES)
// =============================================================================

if (!app.Environment.IsDevelopment())
{
    // Em produção, intercepta exceções não tratadas e redireciona para a página de erro amigável
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redireciona requisições HTTP para HTTPS de forma segura
app.UseHttpsRedirection();

// Habilita o roteamento de arquivos estáticos da pasta wwwroot (CSS, JS, imagens, Bootstrap)
app.UseRouting();

// Middleware de autorização e controle de acesso
app.UseAuthorization();

// Compatibilidade de assets estáticos (.NET 8+)
app.MapStaticAssets();

// Configura a rota convencional padrão do MVC: /{controller}/{action}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Inicia o servidor web Kestrel e aguarda requisições HTTP dos navegadores
app.Run();
