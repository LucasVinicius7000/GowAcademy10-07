using Azure.Messaging.ServiceBus;
using GowAcademy.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NotificacaoService.Services;

namespace NotificacaoService
{
    public class NotificaPagamentoProcessado : IHostedService, IDisposable
    {

        private readonly IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> _messageService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificaPagamentoProcessado> _logger;
        private Timer _timer;

        public NotificaPagamentoProcessado(ILogger<NotificaPagamentoProcessado> logger, IConfiguration configuration, IOptions<EmailConfiguration> emailConfig)
        {
            _logger = logger;
            _configuration = configuration;
            _messageService = new MessageService(_configuration.GetConnectionString("ServiceBusNamespace"));
            var connectionString = _configuration.GetSection("EmailConfiguration");
            _emailService = new EmailService(emailConfig);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Hosted Service is working.");
            var message = _messageService.GetTopicMessage("pagamento-processado", "notifica-pagamento");
            if (message?.Body != null)
            {
                var content = JObject.Parse(message.Body.ToString());
                var emailCliente = content["EmailCliente"]?.ToString();
                _emailService.SendMail(emailCliente, "Pagamento aprovado", "Parabéns! Seu pagamento acabou de ser processado e seu pedido será enviado em breve.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
