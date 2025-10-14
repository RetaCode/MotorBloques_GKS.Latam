using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using MotorBloques.Models; // Para usar Configuracion y Bloque
using MotorBloques.Motor; // Para usar MotorBloques

namespace MotorBloques.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- MotorBloques GKS: Demo de Consola C# ---");

            // 1. Definir la ruta del archivo de configuración
            // Busca el archivo en la misma carpeta donde se ejecuta el programa (.exe)
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

            if (!File.Exists(configPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: Archivo de configuración no encontrado en: {configPath}");
                Console.WriteLine("Asegúrese de que el archivo config/config.json exista.");
                Console.ResetColor();
                return;
            }

            try
            {
                // 2. Cargar y deserializar la configuración
                string jsonString = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<Configuracion>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Permite leer PascalCase o camelCase
                });
                
                if (config == null)
                {
                    Console.WriteLine("Error al deserializar la configuración.");
                    return;
                }

                // 3. Mostrar la configuración cargada
                Console.WriteLine($"Muro a generar: {config.AnchoArea} x {config.AltoArea} mm");
                Console.WriteLine($"Tamaños de bloque: {string.Join(", ", config.TamanosBloque)}");
                Console.WriteLine($"Reglas: Junta={config.Junta}, Offset={config.OffsetMataJunta}, Tolerancia={config.Tolerancia}");
                Console.WriteLine("-------------------------------------------------");

                // VALIDACIÓN DE UNIDAD (Para simular la alerta en el Add-in)
                if (config.UnidadEntrada.ToLowerInvariant() != "mm")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("ADVERTENCIA: La unidad de entrada NO es 'mm'. El motor convertirá todos los valores a milímetros internamente.");
                    Console.ResetColor();
                }
                // -------------------------------------------------

                // 4. Ejecutar el Motor
                var motor = new MotorBloques.Motor.MotorBloques(config);
                var bloques = motor.Run();

                // 5. Mostrar y Exportar Resultados
                Console.WriteLine($"Motor ejecutado. Total de Bloques Colocados: {bloques.Count}");

                // Mostrar tabla de las primeras filas
                Console.WriteLine("\n--- Primeros Bloques Generados (X, Y, Ancho) ---");
                Console.WriteLine("X \t Y \t Ancho\t Tipo");
                Console.WriteLine("-------------------------------------------");
                
                foreach (var bloque in bloques.Take(10))
                {
                    Console.WriteLine($"{bloque.X}\t{bloque.Y}\t{bloque.Ancho}\t{bloque.Tipo}");
                }
                
                // Exportar el JSON final (útil para la futura API de Revit)
                string outputJson = JsonSerializer.Serialize(bloques, new JsonSerializerOptions { WriteIndented = true });
                string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
                Directory.CreateDirectory(outputDir);
                string outputFilePath = Path.Combine(outputDir, "output.json");
                File.WriteAllText(outputFilePath, outputJson);
                
                Console.WriteLine($"\nResultados completos exportados a: {outputFilePath}");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
        }
    }
}