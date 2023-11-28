using System;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPI.Modelos
{
    public class Producto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La cantidad del producto es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a cero.")]
        public int Cantidad { get; set; }

        // Puedes agregar más propiedades según tus necesidades

        public Producto()
        {
            Id = Guid.NewGuid();
        }
    }
}
