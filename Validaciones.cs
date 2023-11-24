using System;
using System.Collections.Generic;
using MinimalAPI.Modelos;

namespace MinimalAPI.Validaciones
{
    public static class Validaciones
    {
        public static List<string> ValidarProducto(Producto producto)
        {
            var errores = new List<string>();

            if (string.IsNullOrEmpty(producto.Nombre))
            {
                errores.Add("El nombre del producto es obligatorio.");
            }

            if (producto.Cantidad < 0)
            {
                errores.Add("La cantidad debe ser mayor o igual a cero.");
            }


            return errores;
        }
    }
}
