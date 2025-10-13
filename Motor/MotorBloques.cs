using System;
using System.Collections.Generic;
using MotorBloques.Models;

namespace MotorBloques.Motor
{
    public static class MotorBloques
    {
        public static Resultado Ejecutar(Configuracion config)
        {
            var resultado = new Resultado();
            double y = 0;
            while (y + config.AltoBloque <= config.AltoArea + config.Tolerancia)
            {
                double x = 0;
                while (x + config.AnchoBloque <= config.AnchoArea + config.Tolerancia)
                {
                    resultado.Bloques.Add(new Bloque
                    {
                        X = x,
                        Y = y,
                        Ancho = config.AnchoBloque,
                        Alto = config.AltoBloque
                    });
                    x += config.AnchoBloque + config.Junta;
                }
                y += config.AltoBloque + config.Junta;
            }
            resultado.AreaTotal = config.AnchoArea * config.AltoArea;
            double areaBloques = resultado.Bloques.Count * config.AnchoBloque * config.AltoBloque;
            resultado.PorcentajeOcupado = areaBloques / resultado.AreaTotal * 100;
            return resultado;
        }
    }
}
