using System;

namespace Dominio
{
    public class PropuestaProveedor
    {
        public int Id { get; set; }
        public Proveedor Proveedor { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }
}
