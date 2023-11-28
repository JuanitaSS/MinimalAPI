using System;

namespace MinimalAPI.Modelos
{
    public class Producto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }

        // Otras propiedades

        public Producto()
        {
            Id = Guid.NewGuid();
        }
    }
}
