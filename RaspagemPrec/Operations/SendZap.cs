using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspagemPrec.Operations
{
    public class SendZap
    {
        public static async void EnviarZap(string nome, string precoLivre, string precoLuiza, string responseCompare)
        {

            try
            {
                Console.WriteLine("Quer receber uma mensagem com o resultado? (sim ou não)");
                string opt = Console.ReadLine();

                var parameters = new System.Collections.Specialized.NameValueCollection();
                var client = new System.Net.WebClient();
                var url = "https://app.whatsgw.com.br/api/WhatsGw/Send/";


                if (opt.ToLower().Equals("sim"))
                {
                    Console.WriteLine("Digite seu número (DDD + número) obs: apenas números");
                    string numero = "+55" + Console.ReadLine();

                    parameters.Add("apikey", "4e9691a8-744d-48a8-9375-beb52133c848"); //switch to your api key
                    parameters.Add("phone_number", "5579991302272"); //switch to your connected number
                    parameters.Add("contact_phone_number", numero); //switch to your number text to received message

                    parameters.Add("message_custom_id", "testejp");
                    parameters.Add("message_type", "text");
                    parameters.Add("message_body", $"Mercado Livre\n" +
                           $"Produto: {nome}\n" +
                           $"Preço: {precoLivre}\n" +
                           "\n" +
                           $"Magazine Luiza\n" +
                           $"Produto: {nome}\n" +
                           $"Preço: R${precoLuiza}\n" +
                           "\n" +
                           $"{responseCompare}\n");



                }


                string responseString = "";
                byte[] response_data;

                response_data = client.UploadValues(url, "POST", parameters);
                responseString = UnicodeEncoding.UTF8.GetString(response_data);

                Console.WriteLine(responseString);

            }
            catch (Exception ex)
            {

                Console.WriteLine($"FAIL: {ex.Message}");
            }


        }

    }
}
