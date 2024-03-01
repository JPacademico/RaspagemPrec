using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RaspagemPrec.Operations
{
    internal class SendLink
    {

        public static void EnviarEmail(string nome, string precoLuiza, string precoLivre, string responseCompare)
        {
            
            string smtpServer = "smtp-mail.outlook.com"; 
            int porta = 587; 
            string remetente = "jpsendtest@outlook.com"; 
            string senha = "bigode666"; 

            
            using (SmtpClient client = new SmtpClient(smtpServer, porta))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(remetente, senha);
                client.EnableSsl = true; 


                MailMessage mensagem = new MailMessage(remetente, "jpsendtest@outlook.com")
                {
                    Subject = "Resultado da comparação de preços",
                    Body = $"Mercado Livre\n" +
                           $"Produto: {nome}\n" +
                           $"Preço: {precoLivre}\n" +
                           "\n" +
                           $"Magazine Luiza\n" +
                           $"Produto: {nome}\n" +
                           $"Preço: R${precoLuiza}\n" +
                           "\n" +
                           $"{responseCompare}\n"+
                           "\n"+
                           "by BOT 001897 - João Pedro Brandão Almeida"


                };

 
                client.Send(mensagem);


            }


        }
    }

    
}
