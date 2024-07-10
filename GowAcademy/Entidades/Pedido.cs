using GowAcademy.Shared.Enums;


namespace PedidoService.Entidades
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public string ValorTotalPedido { get; set; }
        public string EmailCliente { get; set; }
        public StatusPedido Status { get; set; }
    }
}
