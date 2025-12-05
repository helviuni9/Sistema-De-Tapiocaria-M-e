using Microsoft.AspNetCore.Mvc;
using TapiocaManager.API.Data;
using TapiocaManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace TapiocaManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly TapiocaDbContext _context;

        public ProdutosController(TapiocaDbContext context)
        {
            _context = context;
        }

        // GET: api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produtos>>> GetProdutos()
        {
            var produtos = await _context.Produtos.Where(p => p.Ativo).ToListAsync();
            return Ok(produtos);
        }

        // GET: api/produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produtos>> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound(new { mensagem = "Produto não encontrado" });
            }
            return Ok(produto);
        }

        // POST: api/produtos
        [HttpPost]
        public async Task<ActionResult<Produtos>> CreateProduto(Produtos produto)
        {
            if (string.IsNullOrEmpty(produto.Nome))
            {
                return BadRequest(new { mensagem = "Nome do produto é obrigatório" });
            }

            produto.CriadoEm = DateTime.Now;
            produto.Ativo = true;
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        // PUT: api/produtos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduto(int id, Produtos produtoAtualizado)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound(new { mensagem = "Produto não encontrado" });
            }

            produto.Nome = produtoAtualizado.Nome;
            produto.Descricao = produtoAtualizado.Descricao;
            produto.Preco = produtoAtualizado.Preco;
            produto.Ativo = produtoAtualizado.Ativo;

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Produto atualizado com sucesso" });
        }

        // DELETE: api/produtos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound(new { mensagem = "Produto não encontrado" });
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return Ok(new { mensagem = "Produto deletado com sucesso" });
        }
    }
}