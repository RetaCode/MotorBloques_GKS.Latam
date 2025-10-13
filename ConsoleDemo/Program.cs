using System;
using System.IO;
using System.Text.Json;
using MotorBloques.Models;
using MotorBloques.Motor;

namespace ConsoleDemo
{
    class Program
    {
        static void Main()
        {
            var configText = File.ReadAllText("../config/config.json");
            var config = JsonSerializer.Deserialize<Configuracion>(configText)!;
            var resultado = MotorBloques.Motor.MotorBloques.Ejecutar(config);
            Directory.CreateDirectory("../web");
            Utils.GuardarJson(resultado, "../web/output.json");
            Utils.GuardarSVG(resultado, "../web/output.svg");
            Console.WriteLine($"Bloques generados: {resultado.Bloques.Count}");
            Console.WriteLine($"Ocupación: {resultado.PorcentajeOcupado:F2}%");
        }
    }
}
