using System.IO;
using System.Text.Json;
using MotorBloques.Models;

namespace MotorBloques.Motor
{
    public static class Utils
    {
        public static void GuardarJson(Resultado resultado, string ruta)
        {
            var json = JsonSerializer.Serialize(resultado, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ruta, json);
        }

        public static void GuardarSVG(Resultado resultado, string ruta)
        {
            using var writer = new StreamWriter(ruta);
            writer.WriteLine($"<svg xmlns='http://www.w3.org/2000/svg' width='800' height='600'>");
            foreach (var b in resultado.Bloques)
                writer.WriteLine($"<rect x='{b.X}' y='{b.Y}' width='{b.Ancho}' height='{b.Alto}' fill='lightblue' stroke='black' />");
            writer.WriteLine("</svg>");
        }
    }
}
