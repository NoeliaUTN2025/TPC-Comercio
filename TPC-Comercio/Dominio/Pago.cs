using System;

namespace Dominio
{
    public class Pago
    {
        public int Id { get; set; }
        public int IdFactura { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public int CantidadCuotas { get; set; }
        public DateTime Fecha { get; set; }
    }
}
