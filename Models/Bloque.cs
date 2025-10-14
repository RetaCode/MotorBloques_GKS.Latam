namespace MotorBloques.Models
{
    public class Bloque
    {
        public int X { get; set; } // Coordenada X (inicio)
        public int Y { get; set; } // Coordenada Y (inicio)
        public int Ancho { get; set; }
        public int Alto { get; set; }
        public string Tipo { get; set; } // El tamaño del bloque como string ("300", "400")

        // Constructor para facilitar la creación en el motor
        public Bloque(int x, int y, int ancho, int alto, string tipo)
        {
            X = x;
            Y = y;
            Ancho = ancho;
            Alto = alto;
            Tipo = tipo;
        }
    }
}