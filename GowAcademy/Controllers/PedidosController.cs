using Microsoft.AspNetCore.Mvc;
using PedidoService.Entidades;
using PedidosServico.Models;
using Azure.Messaging.ServiceBus;
using GowAcademy.Shared;
using System.Text.Json;
using GowAcademy.Shared.Enums;

namespace GowAcademy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidosController(ILogger<PedidosController> logger, IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> messageService) : ControllerBase
    {

        private readonly static List<Pedido> _pedidos = [];
        private readonly ILogger<PedidosController> _logger = logger;
        private readonly IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> _messageService = messageService;

        [HttpPost]
        public Pedido CriarPedido([FromBody] CriarPedidoRequest pedido)
        {
            if (!pedido.EmailCliente.Contains('@'))
                throw new Exception("email invalido");
            var novoPedido = pedido.ToPedido();
            novoPedido.Status = StatusPedido.Criado;
            _pedidos.Add(novoPedido);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(novoPedido));
            _messageService.PostQueueMessage(message, "pedido-criado");
            return novoPedido;
        }

        [HttpGet("{pedidoId}")]
        public Pedido GetPedido([FromRoute] string pedidoId)
        {
            return _pedidos.FirstOrDefault(p => p.Id == Guid.Parse(pedidoId));
        }
    }
}
