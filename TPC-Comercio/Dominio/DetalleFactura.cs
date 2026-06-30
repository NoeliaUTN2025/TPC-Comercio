using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class DetalleFactura
    {
        public int Id { get; set; }
        public Factura Factura { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioCompra{ get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal { get; set; }
        
        // Propiedad clave para la atomización/trazabilidad
        public int? IdLote { get; set; }
    }
}
