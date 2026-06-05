using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Cliente
    {
        public int ID { get; set; }
        public string DNI { get; set; } 
        public string Nombre { get; set; }
        public string Apellido { get; set; }
       
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
       
        public bool Estado { get; set; } 

    }
}
