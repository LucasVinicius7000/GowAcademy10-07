using Azure.Messaging.ServiceBus;
using GowAcademy.Shared;
using Microsoft.AspNetCore.Mvc;
using PagamentoService.Models;
using System.Text.Json;

namespace PagamentoService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PagamentoController(ILogger<PagamentoController> logger, IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> messageService) : ControllerBase
    {
        private readonly static List<ProcessarPagamentoRequest> _pagamentosProcessados = [];
        private readonly IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> _messageService = messageService;

        [HttpPost]
        public Task ProcessarPagamento([FromBody] ProcessarPagamentoRequest processarPagamentoRequest)
        {
            if(!processarPagamentoRequest.EmailCliente.Contains('@'))
                throw new Exception("email invalido");
            if (processarPagamentoRequest.Cartao is not null)
            {
                _pagamentosProcessados.Add(processarPagamentoRequest);
                var message = new ServiceBusMessage(JsonSerializer.Serialize(processarPagamentoRequest));
                _messageService.PostTopicMessage(message, "pagamento-processado");
            }

            return Task.CompletedTask;
        }
    }
}
