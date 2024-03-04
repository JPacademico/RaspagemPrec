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


public class LogContext : DbContext
{
    public DbSet<Log> LOGROBO { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=SQL9001.site4now.net;" +"Initial Catalog=db_aa5b20_apialmoxarifado;" +"User id=db_aa5b20_apialmoxarifado_admin;" +"Password=master@123"); 
    }
}

public class Log
{
    [Key]
    public int iDlOG { get; set; }
    public string CodigoRobo { get; set; }
    public string UsuarioRobo { get; set; }
    public DateTime DateLog { get; set; }
    public string Etapa { get; set; }
    public string InformacaoLog { get; set; }
    public int IdProdutoAPI { get; set; }
}

class Program
{

    static List<Produto> produtosVerificados = new List<Produto>();


    static bool valorDigitado;
    static string numZap;
    static string nomeMail;
    static void Main(string[] args)
    {
        AskMail();
        
        Console.WriteLine("Quer receber uma mensagem com o resultado? (sim ou não)");
        string opt = Console.ReadLine();

        valorDigitado = SendZap.AskZap(opt);

        if (valorDigitado)
        {
            Console.WriteLine("Digite seu número (DDD + número) obs: apenas números");
            numZap = "+55" + Console.ReadLine();
        }

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

            using (HttpClient client = new HttpClient())
            {

                var byteArray = Encoding.ASCII.GetBytes($"{username}:{senha}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


                HttpResponseMessage response = await client.GetAsync(url);


                if (response.IsSuccessStatusCode)
                {

                    string responseData = await response.Content.ReadAsStringAsync();


                    List<Produto> novosProdutos = ObterNovosProdutos(responseData);
                    foreach (Produto produto in novosProdutos)
                    {
                        if (!produtosVerificados.Exists(p => p.Id == produto.Id))
                        {

                            Console.WriteLine($"Novo produto encontrado: ID {produto.Id}, Nome: {produto.Nome}");

                            produtosVerificados.Add(produto);

                           
                            if (!ProdutoJaRegistrado(produto.Id))
                            {
                                RegistrarLog("210703", "joaopedro", DateTime.Now, "Verify Product", "Success", produto.Id);

                                MercadoLivreScraper mercadoLivreScraper = new MercadoLivreScraper();
                                
                                string precoLivre = mercadoLivreScraper.ObterPreco(produto.Nome, produto.Id);
                                string linkMer = $"https://lista.mercadolivre.com.br/{produto.Nome}".Replace(' ', '+');

                                MagazineLuizaScraper magazineLuizaScraper = new MagazineLuizaScraper();
                                
                                string precoLuiza = magazineLuizaScraper.ObterPreco(produto.Nome, produto.Id);
                                string linkMag = $"https://www.magazineluiza.com.br/busca/{produto.Nome}".Replace(' ', '+');

                                string responseCompare = Benchmarking.Compare(precoLivre, precoLuiza, linkMer, linkMag);
                                RegistrarLog("210703", "joaopedro", DateTime.Now, "Benchmarking", "Success", produto.Id);

                                SendLink.EnviarEmail(nomeMail, produto.Nome, precoLivre, precoLuiza, responseCompare);
                                RegistrarLog("210703", "joaopedro", DateTime.Now, "SendEmail", "Success", produto.Id);


                                SendZap.EnviarZap(numZap, produto.Nome, precoLivre, precoLuiza, responseCompare);
                                RegistrarLog("210703", "joaopedro", DateTime.Now, "SendZap", "Success", produto.Id);

                                

                            }
                        }
                    }
                }
                else
                {

                    Console.WriteLine($"Erro: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Erro ao fazer a requisição: {ex.Message}");
        }
    }


    static List<Produto> ObterNovosProdutos(string responseData)
    {

        List<Produto> produtos = JsonConvert.DeserializeObject<List<Produto>>(responseData);
        return produtos;
    }

    static bool ProdutoJaRegistrado(int idProduto)
    {
        using (var context = new LogContext())
        {
            return context.LOGROBO.Any(log => log.IdProdutoAPI == idProduto);
        }
    }


    static void RegistrarLog(string codRob, string usuRob, DateTime dateLog, string processo, string infLog, int idProd)
    {
        using (var context = new LogContext())
        {
            var log = new Log
            {
                CodigoRobo = codRob,
                UsuarioRobo = usuRob,
                DateLog = dateLog,
                Etapa = processo,
                InformacaoLog = infLog,
                IdProdutoAPI = idProd
            };
            context.LOGROBO.Add(log);
            context.SaveChanges();
        }
    }
    static void AskMail()
    {
        Console.WriteLine("Informe um Email válido: ");
        string inputEmail = Console.ReadLine();

        if (SendLink.IsValidEmail(inputEmail))
        {
            nomeMail = inputEmail;
        }
        else
        {
            Console.WriteLine("\nInválido");
            AskMail();
        }
    }


    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}

