using Dapper;
using Microsoft.Data.Sqlite;

namespace LojaMvcDapper.Database;

/// <summary>
/// Utilitário de inicialização do banco de dados SQLite.
/// Executado na inicialização da aplicação (Program.cs) para assegurar
/// que o arquivo loja_mvc.db, suas tabelas e os dados de exemplo existam.
/// </summary>
public static class DatabaseInitializer
{
    public static void Inicializar(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        // 1. Criação idempotente da tabela
        const string sqlCriarTabela = @"
            CREATE TABLE IF NOT EXISTS Produtos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome TEXT NOT NULL,
                Categoria TEXT NOT NULL,
                Preco REAL NOT NULL,
                Estoque INTEGER NOT NULL,
                DataCadastro TEXT NOT NULL
            );";
        connection.Execute(sqlCriarTabela);

        // 2. Verifica se a tabela já possui registros
        int totalRegistros = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Produtos;");

        // 3. Se estiver vazia, popula com dados de teste
        if (totalRegistros == 0)
        {
            const string sqlSeed = @"
                INSERT INTO Produtos (Nome, Categoria, Preco, Estoque, DataCadastro) VALUES 
                ('Teclado Mecânico RGB Switch Blue', 'Periféricos', 289.90, 15, '2026-11-01 10:00:00'),
                ('Mouse Gamer Óptico 16000 DPI', 'Periféricos', 179.50, 22, '2026-11-02 11:30:00'),
                ('Monitor UltraWide 29 IPS Full HD', 'Monitores', 1199.00, 4, '2026-11-03 14:15:00'),
                ('Headset Gamer 7.1 Surround', 'Áudio', 349.90, 3, '2026-11-04 09:20:00'),
                ('SSD NVMe M.2 1TB Leitura 3500MB/s', 'Hardware', 459.00, 30, '2026-11-05 16:45:00'),
                ('Cadeira Gamer Ergonômica Reclinável', 'Mobiliário', 899.90, 2, '2026-11-06 17:00:00');";

            connection.Execute(sqlSeed);
        }
    }
}
