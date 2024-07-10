using Azure.Messaging.ServiceBus;
using GowAcademy.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NotificacaoService.Services;

namespace NotificacaoService
{
    public class NotificaPedidoCriado : IHostedService, IDisposable
    {
        private readonly ILogger<NotificaPedidoCriado> _logger;
        private readonly IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> _messageService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private Timer _timer;

        public NotificaPedidoCriado(ILogger<NotificaPedidoCriado> logger, IConfiguration configuration, IOptions<EmailConfiguration> emailConfig)
        {
            _logger = logger;
            _configuration = configuration;
            _messageService = new MessageService(_configuration.GetConnectionString("ServiceBusNamespace"));
            _emailService = new EmailService(emailConfig);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Hosted Service is working.");
            var message = _messageService.GetQueueMessage("pedido-criado").Result;
            if (message?.Body != null)
            {
                var content = JObject.Parse(message.Body.ToString());
                var emailCliente = content["EmailCliente"]?.ToString();
                _emailService.SendMail(emailCliente, "Seu pedido foi criado com sucesso", "Pedido criado");
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
