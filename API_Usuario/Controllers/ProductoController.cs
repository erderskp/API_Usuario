using API_Usuario.Data;
using API_Usuario.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Usuario.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController:ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/producto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProducto()
        {
            return await _context.Productos
                                 .Include(p => p.Proveedor)
                                 .Include(p => p.Categoria)
                                 .ToListAsync();
        }

        // GET: api/producto/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                                         .Include(p => p.Proveedor)
                                         .Include(p => p.Categoria)
                                         .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound();

            return producto;
        }

        // POST: api/producto
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            var proveedorExiste = await _context.Proveedores
                                                 .AnyAsync(p => p.Id == producto.IdProveedor);

            var categoriaExiste = await _context.Categorias
                                                .AnyAsync(c => c.Id == producto.IdCategoria);

            if (!proveedorExiste || !categoriaExiste)
            {
                return BadRequest(
                    "Proveedor o categoría no existen.");
            }

            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProducto),
                new { id = producto.Id },
                producto);
        }

        // PUT: api/producto/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/producto/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("estadisticas")]
        public async Task<IActionResult> Estadisticas()
        {
            if (!await _context.Productos.AnyAsync())
                return NotFound("No hay productos.");

            var productoMasCaro =
                await _context.Productos
                    .OrderByDescending(
                        p => p.Precio)
                    .FirstAsync();

            var productoMasBarato =
                await _context.Productos
                    .OrderBy(
                        p => p.Precio)
                    .FirstAsync();

            var suma =
                await _context.Productos
                    .SumAsync(
                        p => p.Precio);

            var promedio =
                await _context.Productos
                    .AverageAsync(
                        p => p.Precio);

            return Ok(new
            {
                ProductoMasCaro = productoMasCaro,
                ProductoMasBarato = productoMasBarato,
                SumaPrecios = suma,
                PrecioPromedio = promedio
            });
        }

        [HttpGet("categoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorCategoria(int idCategoria)
        {
            return await _context.Productos
                .Where(
                    p => p.IdCategoria
                        == idCategoria)
                .Include(
                    p => p.Categoria)
                .Include(
                    p => p.Proveedor)
                .ToListAsync();
        }

        [HttpGet("proveedor/{idProveedor}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorProveedor(int idProveedor)
        {
            return await _context.Productos
                .Where(
                    p => p.IdProveedor
                        == idProveedor)
                .Include(
                    p => p.Categoria)
                .Include(
                    p => p.Proveedor)
                .ToListAsync();
        }

        [HttpGet("cantidad")]
        public async Task<IActionResult> CantidadProductos()
        {
            var cantidad =
                await _context.Productos
                    .CountAsync();

            return Ok(new
            {
                TotalProductos = cantidad
            });
        }
    }
}
