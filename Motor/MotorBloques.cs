using System;
using System.Collections.Generic;
using System.Linq;
using MotorBloques.Models; // Necesario para usar Bloque y Configuracion

namespace MotorBloques.Motor
{
    public class MotorBloques
    {
        private readonly Configuracion _config;
        private List<Bloque> _bloquesColocados;

        // -----------------------------------------------------------------
        // CONSTRUCTOR CORREGIDO (Llama a la conversión de unidades)
        // -----------------------------------------------------------------
        public MotorBloques(Configuracion config)
        {
            // Convertir inmediatamente todos los valores a MM
            _config = ConvertToInternalUnits(config); 
            _bloquesColocados = new List<Bloque>();
        }

        // -----------------------------------------------------------------
        // LÓGICA DE CONVERSIÓN DE UNIDADES (Tarea de la conversación con René)
        // -----------------------------------------------------------------
        
        /// <summary>
        /// Obtiene el factor de multiplicación necesario para convertir la unidad de entrada a milímetros (mm).
        /// </summary>
        private double GetConversionFactor(string unidad)
        {
            return unidad.ToLowerInvariant() switch
            {
                "m" => 1000.0, 
                "cm" => 10.0,  
                "mm" => 1.0,   
                _ => 1.0,      // Si no se reconoce, asume milímetros.
            };
        }

        /// <summary>
        /// Convierte todas las dimensiones de la configuración de entrada a milímetros.
        /// </summary>
        private Configuracion ConvertToInternalUnits(Configuracion originalConfig)
        {
            var factor = GetConversionFactor(originalConfig.UnidadEntrada);
            if (factor == 1.0) return originalConfig;

            var convertedConfig = new Configuracion
            {
                // Conversión de propiedades individuales
                AnchoArea = (int)(originalConfig.AnchoArea * factor),
                AltoArea = (int)(originalConfig.AltoArea * factor),
                Junta = (int)(originalConfig.Junta * factor),
                Tolerancia = (int)(originalConfig.Tolerancia * factor),
                OffsetMataJunta = (int)(originalConfig.OffsetMataJunta * factor),
                AltoBloque = (int)(originalConfig.AltoBloque * factor),
                GrosorParedFinal = (int)(originalConfig.GrosorParedFinal * factor),
                
                // Conversión de la lista de tamaños
                TamanosBloque = originalConfig.TamanosBloque.Select(t => (int)(t * factor)).ToList(),
                UnidadEntrada = "mm" // La nueva unidad interna (estándar)
            };
            return convertedConfig;
        }


        // -----------------------------------------------------------------
        // LÓGICA PRINCIPAL DEL MOTOR (Método Run)
        // -----------------------------------------------------------------
        
        /// <summary>
        /// Genera la disposición de bloques fila por fila, aplicando las reglas de No-Corte y Mata-Junta.
        /// </summary>
        public List<Bloque> Run()
        {
            _bloquesColocados.Clear();

            // Desestructurar configuración (ya en MM)
            int anchoArea = _config.AnchoArea;
            int altoArea = _config.AltoArea;
            int junta = _config.Junta;
            int tolerancia = _config.Tolerancia;
            int altoBloque = _config.AltoBloque;
            int offsetMataJunta = _config.OffsetMataJunta;

            // Ordenar la lista para la lógica Greedy
            var anchosDisponibles = _config.TamanosBloque.OrderByDescending(a => a).ToArray();
            if (!anchosDisponibles.Any()) return _bloquesColocados;

            int anchoMinimoRequerido = anchosDisponibles.Last() + junta;

            int yActual = 0; // Coordenada Y de la fila actual
            int numFila = 0;
            
            // Bucle principal para iterar por filas
            while (yActual + altoBloque <= altoArea)
            {
                int xActual = 0; // Coordenada X (inicio) de la fila

                // 1. Implementar Mata-Junta (Offset)
                int offset = (numFila % 2 != 0) ? offsetMataJunta : 0;
                xActual += offset;
                
                // Si el espacio restante es demasiado pequeño para el bloque más chico, terminamos.
                if (anchoArea - xActual < anchoMinimoRequerido) break;

                var bloquesFila = new List<Bloque>();

                // Bucle para llenar la fila
                while (xActual < anchoArea)
                {
                    double espacioRestanteAlMuro = anchoArea - xActual;

                    // 2. PRIMERA BÚSQUEDA: Encontrar ajuste perfecto (Regla No-Corte)
                    int? mejorBloqueNoCorte = null;
                    int mejorSobrante = tolerancia + 1; 

                    foreach (var anchoBloque in anchosDisponibles)
                    {
                        int anchoTotalBloque = anchoBloque + junta;
                        int sobrante = (int)(espacioRestanteAlMuro - anchoTotalBloque);

                        // Condición No-Corte
                        if (sobrante >= 0 && sobrante <= tolerancia)
                        {
                            if (sobrante < mejorSobrante)
                            {
                                mejorBloqueNoCorte = anchoBloque;
                                mejorSobrante = sobrante;
                                if (sobrante == 0) break;
                            }
                        }
                    }

                    // 3. COLOCACIÓN
                    if (mejorBloqueNoCorte.HasValue)
                    {
                        // Caso A: Ajuste perfecto/tolerable
                        bloquesFila.Add(new Bloque(
                            xActual, 
                            yActual, 
                            mejorBloqueNoCorte.Value, 
                            altoBloque, 
                            mejorBloqueNoCorte.Value.ToString()
                        ));

                        xActual = anchoArea; // Mueve X al final del muro
                    }
                    else
                    {
                        // Caso B: Greedy para seguir llenando
                        int? anchoGreedy = null;

                        // Buscamos el bloque MÁS GRANDE que cabe
                        foreach (var anchoBloque in anchosDisponibles)
                        {
                            int anchoTotalBloque = anchoBloque + junta;
                            if (xActual + anchoTotalBloque <= anchoArea)
                            {
                                anchoGreedy = anchoBloque;
                                break; 
                            }
                        }

                        if (anchoGreedy.HasValue)
                        {
                            int anchoTotal = anchoGreedy.Value + junta;
                            bloquesFila.Add(new Bloque(
                                xActual, 
                                yActual, 
                                anchoGreedy.Value, 
                                altoBloque, 
                                anchoGreedy.Value.ToString()
                            ));
                            xActual += anchoTotal;
                        }
                        else
                        {
                            // Espacio restante demasiado pequeño
                            break;
                        }
                    }
                } // Fin del bucle de llenado de fila

                _bloquesColocados.AddRange(bloquesFila);
                yActual += altoBloque + junta;
                numFila++;
            }

            return _bloquesColocados;
        }
    }
}