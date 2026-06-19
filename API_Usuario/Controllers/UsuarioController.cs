using API_Usuario.Data;
using API_Usuario.Models;
using API_Usuario.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Usuario.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController:ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public UsuarioController(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/usuarios/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // POST: api/usuarios
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Validar correo duplicado
            bool existeCorreo = await _context.Usuarios
                .AnyAsync(u => u.Correo == usuario.Correo);

            if (existeCorreo)
            {
                return BadRequest(new
                {
                    mensaje = "El correo electrónico ya está en uso."
                });
            }
            
            usuario.Contrasena = HashService.ComputeSha256(usuario.Contrasena);
            
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            await _logService.RegistrarUsuarioAsync(usuario);

            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.Id },
                usuario);
        }

        // PUT: api/usuarios/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            var correoDuplicado = await _context.Usuarios
                .AnyAsync(u => u.Correo == usuario.Correo
                            && u.Id != id);

            if (correoDuplicado)
            {
                return BadRequest(new
                {
                    mensaje = "El correo electrónico ya está en uso."
                });
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(e => e.Id == id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/usuarios/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("historial")]
        public async Task<IActionResult> GetHistorial()
        {
            var historial = await _logService.ObtenerHistorialAsync();

            if (!historial.Any())
            {
                return NotFound("No existen registros en el archivo.");
            }

            return Ok(historial.Select(u => new {u.Id,u.Nombre, u.Correo, u.FechaDeNacimiento}));
        }
    }
}
