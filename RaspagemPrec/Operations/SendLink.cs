using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaspagemPrec.Operations
{
    public class SendLink
    {

        public static void EnviarEmail(string nomeMail, string nome, string precoLivre, string precoLuiza, string responseCompare)
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


                MailMessage mensagem = new MailMessage(remetente, nomeMail)
                {
                    Subject = "Resultado da comparação de preços",
                    Body = $"Mercado Livre\n" +
                           $"Produto: {nome}\n" +
                           $"Preço: R${precoLivre}\n" +
                           "\n" +
                           $"Magazine Luiza\n" +
                           $"Produto: {nome}\n" +
                           $"Preço: {precoLuiza}\n" +
                           "\n" +
                           $"{responseCompare}\n"+
                           "\n"+
                           "by BOT 001897 - João Pedro Brandão Almeida"


                };

 
                client.Send(mensagem);


            }


        }
        public static bool IsValidEmail(string email)
        {
            
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
        
    }
}

    

