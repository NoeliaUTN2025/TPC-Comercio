using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Factura
    {
         public int Id { get; set; }
        public string  NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public Cliente Cliente { get; set; }
        public Usuario Usuario { get; set; }
        public decimal Total { get; set; }
        public bool estado { get; set; }
    }
}
