using API_Usuario.Models;
using System.Text.Json;

namespace API_Usuario.Services
{
    public class LogService
    {
        private readonly string _rutaArchivo;

        public LogService()
        {
            _rutaArchivo = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Logs",
                "usuarios.txt");

            var directorio =
                Path.GetDirectoryName(_rutaArchivo);

            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio!);
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
