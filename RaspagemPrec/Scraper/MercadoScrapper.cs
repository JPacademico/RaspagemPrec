using HtmlAgilityPack;
using System;

public class MercadoLivreScraper
{
    public string ObterPreco(string descricaoProduto, int idProduto)
    {
        // URL da pesquisa no Mercado Livre com base na descrição do produto
        string url = $"https://lista.mercadolivre.com.br/{descricaoProduto}";

        try
        {
            // Inicializa o HtmlWeb do HtmlAgilityPack
            HtmlWeb web = new HtmlWeb();

            // Carrega a página de pesquisa do Mercado Livre
            HtmlDocument document = web.Load(url);

            // Encontra o elemento que contém o preço do primeiro produto            
            HtmlNode firstProductPriceNode = document.DocumentNode.SelectSingleNode("//span[@class='andes-money-amount__fraction']");

            // Verifica se o elemento foi encontrado
            if (firstProductPriceNode != null)
            {
                // Obtém o preço do primeiro produto
                string firstProductPrice = firstProductPriceNode.InnerText.Trim();

                // Registra o log com o ID do produto
                RegistrarLog("210703", "joaopedro", DateTime.Now, "Scraping - Mercado Livre", "Sucesso", idProduto);

                // Retorna o preço
                return firstProductPrice;
            }
            else
            {
                Console.WriteLine("Preço não encontrado.");

                // Registra o log com o ID do produto
                RegistrarLog("210703", "joaopedro", DateTime.Now, "Scraping - Mercado Livre", "Preço não encontrado", idProduto);

                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao acessar a página: {ex.Message}");

            // Registra o log com o ID do produto
            RegistrarLog("210703", "joaopedro", DateTime.Now, "Scraping - Mercado Livre", $"Erro: {ex.Message}", idProduto);

            return null;
        }
    }

    private void RegistrarLog(string codRob, string usuRob, DateTime dateLog, string processo, string infLog, int idProd)
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
}
