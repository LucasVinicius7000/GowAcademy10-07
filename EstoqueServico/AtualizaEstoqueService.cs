using Azure.Messaging.ServiceBus;
using EstoqueService;
using GowAcademy.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class AtualizaEstoqueService : IHostedService, IDisposable
{

    private readonly IConfiguration _configuration;
    private readonly IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> _messageService;
    private readonly ILogger<AtualizaEstoqueService> _logger;
    private Timer _timer;

    public AtualizaEstoqueService(ILogger<AtualizaEstoqueService> logger, IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> messageService, IConfiguration configuration)
    {
        _logger = logger;
        _messageService = messageService;
        _configuration = configuration;
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
        var mensagem = _messageService.GetTopicMessage("pagamento-processado", "atualiza-estoque");
        if(mensagem?.Body != null)
        {
            var content = JObject.Parse(mensagem.Body.ToString());
            var pedidoId = content["PedidoId"]?.ToString();
            if(!string.IsNullOrEmpty(pedidoId))
            {
                var client = new HttpClient();
                var url = $"{_configuration.GetSection("ServicesEndpoints:PedidoService").Value}/Pedidos/{pedidoId}";
                try
                {
                    var result = client.GetAsync($"{url}").Result.Content.ReadAsStringAsync().Result;
                    var pedido = JObject.Parse(result);
                    if (pedido is not null)
                    {
                        var quantidade = pedido["quantidade"]?.ToString();
                        var nomeProduto = pedido["nomeProduto"]?.ToString();
                        Estoque.EstoqueProdutos[nomeProduto] = Estoque.EstoqueProdutos[nomeProduto] - int.Parse(quantidade);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao atualizar estoque: {ex.Message}");
                }
                
            }
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
