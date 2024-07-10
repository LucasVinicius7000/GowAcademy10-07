using System.ComponentModel.DataAnnotations;

namespace PagamentoService.Models
{

    public class ProcessarPagamentoRequest
    {
        [Required]
        public Guid PedidoId { get; set; }

        [Required]
        public CartaoCredito Cartao { get; set; }
        [Required]
        public string EmailCliente { get; set; }
    }

    public class CartaoCredito
    {
        [Required]
        public string Numero { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CVV { get; set; }

        [Required]
        public string NomeNoCartao { get; set; }
    }

}
