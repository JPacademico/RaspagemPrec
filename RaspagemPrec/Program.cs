using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using RaspagemPrec.Compare;
using RaspagemPrec.Operations;

// Classe de contexto do banco de dados
public class LogContext : DbContext
{
    public DbSet<Log> Logs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC03LAB2539\\SENAI;Initial Catalog=scrap_DB;User Id=sa;Password=senai.123;"); // Substitua "YourConnectionString" pela sua string de conexão
    }
}

// Classe de modelo para os logs
public class Log
{
    [Key]
    public int IdLog { get; set; }
    public string CodRob { get; set; }
    public string UsuRob { get; set; }
    public DateTime DateLog { get; set; }
    public string Processo { get; set; }
    public string InfLog { get; set; }
    public int IdProd { get; set; }
}

class Program
{
    // Lista para armazenar produtos já verificados
    static List<Produto> produtosVerificados = new List<Produto>();

    static void Main(string[] args)
    {
        // Definir o intervalo de tempo para 5 minutos (300.000 milissegundos)
        int intervalo = 300000;

        // Criar um temporizador que dispara a cada 5 minutos
        Timer timer = new Timer(VerificarNovoProduto, null, 0, intervalo);

        // Manter a aplicação rodando
        while (true)
        {
            Thread.Sleep(Timeout.Infinite);
        }
    }

    static async void VerificarNovoProduto(object state)
    {
        string username = "11164448";
        string senha = "60-dayfreetrial";
        string url = "http://regymatrix-001-site1.ktempurl.com/api/v1/produto/getall";

        try
        {
            // Criar um objeto HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Adicionar as credenciais de autenticação básica
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{senha}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Fazer a requisição GET à API
                HttpResponseMessage response = await client.GetAsync(url);

                // Verificar se a requisição foi bem-sucedida (código de status 200)
                if (response.IsSuccessStatusCode)
                {
                    // Ler o conteúdo da resposta como uma string
                    string responseData = await response.Content.ReadAsStringAsync();

                    // Processar os dados da resposta
                    List<Produto> novosProdutos = ObterNovosProdutos(responseData);
                    foreach (Produto produto in novosProdutos)
                    {
                        if (!produtosVerificados.Exists(p => p.Id == produto.Id))
                        {
                            // Se é um novo produto, faça algo com ele
                            Console.WriteLine($"Novo produto encontrado: ID {produto.Id}, Nome: {produto.Nome}");
                            // Adicionar o produto à lista de produtos verificados
                            produtosVerificados.Add(produto);

                            // Registra um log no banco de dados apenas se o produto for novo
                            if (!ProdutoJaRegistrado(produto.Id))
                            {
                                RegistrarLog("210703", "joaopedro", DateTime.Now, "Verify Product", "Success", produto.Id);

                                MercadoLivreScraper mercadoLivreScraper = new MercadoLivreScraper();
                                mercadoLivreScraper.ObterPreco(produto.Nome, produto.Id);
                                string precoLivre = mercadoLivreScraper.ObterPreco(produto.Nome, produto.Id);
                                string linkMer = $"https://lista.mercadolivre.com.br/{produto.Nome}".Replace(' ', '+');

                                MagazineLuizaScraper magazineLuizaScraper = new MagazineLuizaScraper();
                                magazineLuizaScraper.ObterPreco(produto.Nome, produto.Id);
                                string precoLuiza = magazineLuizaScraper.ObterPreco(produto.Nome, produto.Id);
                                string linkMag = $"https://www.magazineluiza.com.br/busca/{produto.Nome}".Replace(' ', '+');

                                string responseCompare = Benchmarking.Compare(precoLivre, precoLuiza, linkMer, linkMag);
                                RegistrarLog("210703", "joaopedro", DateTime.Now, "Benchmarking", "Success", produto.Id);

                                SendLink.EnviarEmail(produto.Nome, precoLivre, precoLuiza, responseCompare);
                                RegistrarLog("210703", "joaopedro", DateTime.Now, "SendEmail", "Success", produto.Id);

                                SendZap.EnviarZap(produto.Nome, precoLivre, precoLuiza, responseCompare);
                                RegistrarLog("210703", "joaopedro", DateTime.Now, "SendZap", "Success", produto.Id);

                            }
                        }
                    }
                }
                else
                {
                    // Imprimir mensagem de erro caso a requisição falhe
                    Console.WriteLine($"Erro: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            // Imprimir mensagem de erro caso ocorra uma exceção
            Console.WriteLine($"Erro ao fazer a requisição: {ex.Message}");
        }
    }

    // Método para processar os dados da resposta e obter produtos
    static List<Produto> ObterNovosProdutos(string responseData)
    {
        // Desserializar os dados da resposta para uma lista de produtos
        List<Produto> produtos = JsonConvert.DeserializeObject<List<Produto>>(responseData);
        return produtos;
    }

    // Método para verificar se o produto já foi registrado no banco de dados
    static bool ProdutoJaRegistrado(int idProduto)
    {
        using (var context = new LogContext())
        {
            return context.Logs.Any(log => log.IdProd == idProduto);
        }
    }

    // Método para registrar um log no banco de dados
    static void RegistrarLog(string codRob, string usuRob, DateTime dateLog, string processo, string infLog, int idProd)
    {
        using (var context = new LogContext())
        {
            var log = new Log
            {
                CodRob = codRob,
                UsuRob = usuRob,
                DateLog = dateLog,
                Processo = processo,
                InfLog = infLog,
                IdProd = idProd
            };
            context.Logs.Add(log);
            context.SaveChanges();
        }
    }

    // Classe para representar um produto
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}

