using PedidoService.Entidades;
using System.ComponentModel.DataAnnotations;

namespace PedidosServico.Models
{
    public class CriarPedidoRequest
    {
        [Required]
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public string ValorTotalPedido { get; set; }
        [Required]
        public string EmailCliente { get; set; }

        public Pedido ToPedido()
        {
            return new Pedido
            {
                Id = Guid.NewGuid(),
                NomeProduto = NomeProduto.ToLower(),
                Quantidade = Quantidade,
                ValorTotalPedido = ValorTotalPedido,
                EmailCliente = EmailCliente
            };
        }
    }
}
