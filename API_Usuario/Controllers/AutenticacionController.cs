using API_Usuario.Data;
using API_Usuario.DTOs;
using API_Usuario.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Usuario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController: ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JWTService _jwtService;

        public AutenticacionController(
            AppDbContext context,
            JWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDTO login)
        {
            var passwordHash =
                HashService.ComputeSha256(
                    login.Contrasena);

            var usuario =
                await _context.Usuarios
                .FirstOrDefaultAsync(
                    u => u.Correo == login.Correo
                    && u.Contrasena == passwordHash);

            if (usuario == null)
            {
                return Unauthorized(
                    "Credenciales incorrectas");
            }

            var token =
                _jwtService.GenerarToken(
                    usuario.Correo,
                    usuario.Id);

            return Ok(new { token });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(
            ResfTokenDTO dto)
        {
            var handler =
                new System.IdentityModel.Tokens.Jwt
                .JwtSecurityTokenHandler();

            var jwt =
                handler.ReadJwtToken(dto.Token);

            var correo =
                jwt.Claims
                .First(x => x.Type.Contains("name"))
                .Value;

            var id =
                int.Parse(
                    jwt.Claims
                    .First(x => x.Type == "Id")
                    .Value);

            var newToken =
                _jwtService.GenerarToken(
                    correo,
                    id);

            return Ok(new { token = newToken });
        }
    }
}
