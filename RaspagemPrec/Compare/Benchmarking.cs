using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspagemPrec.Compare
{
    internal class Benchmarking
    {
        public static string Compare(string nome, int idproduto)
        {

            string precoLivre;
            string precoLuiza;
            char[] charsToTrim = { 'R', '$', ' ' };

            MercadoLivreScraper mercado = new();
            MagazineLuizaScraper magazine = new();

            precoLivre = mercado.ObterPreco(nome, idproduto);
            precoLuiza = magazine.ObterPreco(nome, idproduto);

            decimal livreDecimal = Convert.ToDecimal(precoLivre.Trim(charsToTrim));
            decimal luizaDecimal = Convert.ToDecimal(precoLuiza.Trim(charsToTrim));


            if (luizaDecimal > livreDecimal)
            {

                return $"No mercado livre está R${luizaDecimal - livreDecimal} mais barato";
            }
            else if(luizaDecimal < livreDecimal)
            {

                return $"No magazine Luiza está R${livreDecimal - luizaDecimal} mais barato";
            }
            else
            {
                return "Preço igual";
            }
        }
    }
}
