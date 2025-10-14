using System.Collections.Generic;

namespace MotorBloques.Models
{
    public class Configuracion
    {
        public int AnchoArea { get; set; }
        public int AltoArea { get; set; }
        public int Junta { get; set; }
        public int Tolerancia { get; set; }
        public List<int> TamanosBloque { get; set; } // Lista de [300, 400, 500]
        public int OffsetMataJunta { get; set; }
        public int AltoBloque { get; set; }
        
        public int GrosorParedFinal { get; set; } // Ejemplo: 150mm para 15cm

        public string UnidadEntrada { get; set; } = string.Empty; // "mm", "cm", "m"

        public Configuracion()
        {
            TamanosBloque = new List<int>(); 
        }
    }
}