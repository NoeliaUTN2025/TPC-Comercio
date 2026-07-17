using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Producto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string NombreProducto { get; set; }
       
        public string Descripcion { get; set; } 
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public decimal Precio { get; set; }
        public decimal PorcentajeGanancia { get; set; }

        public Marca marca { get; set; }
        public Categoria categoria { get; set; }    
        public String UrlImagen { get; set; }

    }
}
