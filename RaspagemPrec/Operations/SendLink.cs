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
        //Enviar email com o resultado da comparação
        public static void EnviarEmail(string nome, string precoLuiza, string precoLivre, string responseCompare)
        {
            // Configurações do servidor SMTP do Gmail
            string smtpServer = "smtp-mail.outlook.com"; // Servidor SMTP do Gmail
            int porta = 587; // Porta SMTP do Gmail para TLS/STARTTLS
            string remetente = "jpsendtest@outlook.com"; // Seu endereço de e-mail do Gmail
            string senha = "bigode666"; // Sua senha do Gmail

            // Configurar cliente SMTP
            using (SmtpClient client = new SmtpClient(smtpServer, porta))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(remetente, senha);
                client.EnableSsl = true; // Habilitar SSL/TLS

                // Construir mensagem de e-mail
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
                           $"{responseCompare}\n"


                };

                // Enviar e-mail
                client.Send(mensagem);


            }


        }
    }

    
}
