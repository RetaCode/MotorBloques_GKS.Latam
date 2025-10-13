using System.Collections.Generic;

namespace MotorBloques.Models
{
    public class Resultado
    {
        public List<Bloque> Bloques { get; set; } = new();
        public double AreaTotal { get; set; }
        public double PorcentajeOcupado { get; set; }
    }
}
