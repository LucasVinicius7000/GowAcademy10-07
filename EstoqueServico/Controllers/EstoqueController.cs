using Microsoft.AspNetCore.Mvc;

namespace EstoqueService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstoqueController 
    {
        [HttpGet]
        public Dictionary<string, int> Get()
        {
            return Estoque.EstoqueProdutos;
        }

        [HttpPost("{nomeProduto}/{quantidadeNoEstoque}")] 
        public void Post([FromRoute] string nomeProduto, int quantidadeNoEstoque)
        {
            Estoque.EstoqueProdutos.Add(nomeProduto.ToLower(), quantidadeNoEstoque);
        }
    }
}
