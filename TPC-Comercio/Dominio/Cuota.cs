using System;

namespace Dominio
{
    public class Cuota
    {
        public int Id { get; set; }
        public int IdPago { get; set; }
        public int NroCuota { get; set; }
        public decimal Monto { get; set; }
        public decimal Interes { get; set; }
        public DateTime Vencimiento { get; set; }
    }
}
