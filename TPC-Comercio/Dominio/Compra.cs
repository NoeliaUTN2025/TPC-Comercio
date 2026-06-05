using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public Proveedor Proveedor { get; set; }
        public Usuario Usuario { get; set; }
        public decimal Total { get; set; }
        public bool estado { get; set; }


    }
}
