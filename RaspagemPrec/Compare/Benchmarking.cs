using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspagemPrec.Compare
{
    internal class Benchmarking
    {
        public static string Compare(string precoLivre, string precoLuiza, string linkMerc, string linkMag)
        {

            char[] charsToTrim = { 'R', '$', ' ' };


            MercadoLivreScraper mercado = new();
            MagazineLuizaScraper magazine = new();

            decimal livreDecimal = Convert.ToDecimal(precoLivre.Trim(charsToTrim));
            decimal luizaDecimal = Convert.ToDecimal(precoLuiza.Trim(charsToTrim));


            if (luizaDecimal > livreDecimal)
            {

                return $"No mercado livre está R${luizaDecimal - livreDecimal} mais barato\n"+$"Link: {linkMerc}";
            }
            else if(luizaDecimal < livreDecimal)
            {

                return $"No magazine Luiza está R${livreDecimal - luizaDecimal} mais barato\n" +$"Link: {linkMag}";
            }
            else
            {
                return "Preço igual";
            }
        }
    }
}
