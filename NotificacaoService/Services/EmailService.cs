using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace NotificacaoService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        public EmailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            this._emailConfiguration = emailConfiguration.Value;
        }
        public async Task<bool> SendMail(string mailTO, string body, string subject)
        {

            MailMessage msg = new()
            {
                From = new MailAddress(_emailConfiguration.Remetente, "Gow soluções"),
                ReplyTo = new MailAddress(_emailConfiguration.Remetente, "Gow soluções")
            };

            msg.To.Add(mailTO);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;

            SmtpClient client = new()
            {
                UseDefaultCredentials = false,
                Host = _emailConfiguration.Host,
                Port = Convert.ToInt32(_emailConfiguration.Porta),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_emailConfiguration.Remetente, _emailConfiguration.Password),
                Timeout = 120000
            };

            try
            {
                client.Send(msg);
                return true;
            }
            catch (SmtpException smtpNotFound)
            {
                return false;

            }
            catch (System.Net.Sockets.SocketException socketException)
            {
                return false;

            }
            catch (System.ComponentModel.Win32Exception win32Exception)
            {
                return false;

            }
            catch (System.Exception exception)
            {

                return false;
            }
            finally
            {

                msg.Dispose();
            }
        }
    }

    public class EmailConfiguration
    {
        public string Remetente { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
        public string Porta { get; set; }
    }
}
