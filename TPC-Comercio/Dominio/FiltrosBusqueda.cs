using System;

namespace Dominio
{
    public class FiltrosBusqueda
    {
        public string Texto { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdMarca { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public FiltrosBusqueda()
        {
        }
    }
}
