using Microsoft.AspNetCore.Mvc;
using TapiocaManager.API.Data;
using TapiocaManager.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TapiocaManager.API.Controllers
{
    [ApiController]
    [Route("api[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly TapiocaDbContext _context;

        public PedidosController(TapiocaDbContext context)
        {
            _context = context;
        }
        
        // GET: api/pedidos

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Itens)
                .ToListAsync();
            return Ok(pedidos);
        }


        //GET: api/pedido/5
       [HttpGet("{id}")]
       public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if(pedido == null)
            {
                return NotFound(new{ mensagem = "Pedido não encontrado"});
            }

            return Ok(pedido);
        }

        //POST: api/pedidos
        [HttpPost]
        public async Task<ActionResult<Pedido>> CreatePedido(Pedido pedido)
        {
            if(string.IsNullOrEmpty(pedido.NomeCliente))
            {
                return BadRequest(new { mensagem = "Nome do cliente é obrigatório"});
            }

            if(string.IsNullOrEmpty(pedido.Telefone))
            {
                return BadRequest(new { mensagem = "Telefone é obrigatório"});
            }

            if(pedido.Itens == null || pedido.Itens.Count == 0)
            {
                return BadRequest(new { mensagem = " pedido deve ter pelo menos um intem "});
            }

            pedido.CriadoEm =DateTime.Now;
            pedido.Status = "Pendete";
            pedido.Total = pedido.Itens.Sum(i => i.Subtotal);

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id}, pedido);
        }

        //PUT: api/pedidos/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpadateStatusPedido(int id, [FromBody] string novoStatus)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound(new { mensagem = "pedido não encontrado"});
            }

            var statusValidos = new[] {"Pendete", "Em Preparo", "Pronto", " Entregue"};
            if (!statusValidos.Contains(novoStatus))
            {
                return BadRequest(new { mensagem = "status inválido. Use: Pendente, Em Preparo, Pronto ou Entregue"});
            }

            pedido.Status = novoStatus;
            pedido.AtualizadoEm  = DateTime.Now;

            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = $"pedido atualizado para: {novoStatus}", pedido});

        } 

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if(pedido == null)
            {
                return NotFound(new { mensagem = " Pedido nâo encontrado"});
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return Ok(new {mensagem = "Pedido não encontrado"});
            
        }

        // GET: api/pedidos/relatorios/dia
        [HttpGet("relatorio/dia")]
        public async Task<IActionResult> GetRelatorioDia()
        {
            var hoje = DateTime.Today;
            var pedidoHoje = await _context.Pedidos
                .Where(p => p.CriadoEm.Date == hoje)
                .ToListAsync();
            var total = pedidoHoje.Sum(p => p.Total);
            var quantidade = pedidoHoje.Count;
            return Ok(new
            {
                data = hoje,
                totalPedidos = quantidade,
                totalVendas = total,
                pedidos = pedidoHoje
            });
        }
        
    }
}