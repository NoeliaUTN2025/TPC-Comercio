using System;

namespace Dominio
{
    public class Lote
    {
        public int Id { get; set; }
        public int IdProducto { get; set; }
        public int IdDetalleCompra { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadDisp { get; set; }
        public decimal PrecioCompra { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Proveedor { get; set; }
 
    

    public decimal PrecioTotal
        {
            get
            {
                return CantidadTotal * PrecioCompra;
            }
        }

    public decimal ValorDisponible
       {
            get
            {
                return CantidadDisp * PrecioCompra;
            }
        }
    }
}


