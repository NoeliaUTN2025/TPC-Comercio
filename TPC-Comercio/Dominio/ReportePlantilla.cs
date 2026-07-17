using System;

namespace Dominio
{
    public class ReportePlantilla
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Entidad { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        // Propiedad calculada útil para mostrar en la lista
        public string DescripcionCombo
        {
            get
            {
                string desc = $"{Nombre} ({Entidad})";
                if (FechaDesde.HasValue || FechaHasta.HasValue)
                {
                    desc += " [Con filtro de fecha]";
                }
                return desc;
            }
        }
    }
}
