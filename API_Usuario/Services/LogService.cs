using API_Usuario.Models;
using System.Text.Json;

namespace API_Usuario.Services
{
    public class LogService
    {
        private readonly string _rutaArchivo;

        public LogService(IWebHostEnvironment env)
        {
            _rutaArchivo = Path.Combine(
                env.ContentRootPath,
                "Logs",
                "usuarios.txt");

            if (!Directory.Exists(Path.GetDirectoryName(_rutaArchivo)!))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_rutaArchivo)!);
            }
        }

        public async Task RegistrarUsuarioAsync(
            Usuario usuario)
        {
            var json =
                JsonSerializer.Serialize(usuario);

            await File.AppendAllTextAsync(
                _rutaArchivo,
                json + Environment.NewLine);
        }

        public async Task<List<Usuario>>
            ObtenerHistorialAsync()
        {
            if (!File.Exists(_rutaArchivo))
            {
                return new List<Usuario>();
            }

            var lineas =
                await File.ReadAllLinesAsync(
                    _rutaArchivo);

            var usuarios =
                new List<Usuario>();

            foreach (var linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                var usuario =
                    JsonSerializer.Deserialize<Usuario>(
                        linea);

                if (usuario != null)
                {
                    usuarios.Add(usuario);
                }
            }

            return usuarios;
        }

    }
}
