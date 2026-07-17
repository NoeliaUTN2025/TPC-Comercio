using System;

namespace Dominio
{
    public class Caja
    {
        public int Id { get; set; }
        public DateTime FechaApertura { get; set; }
        public decimal MontoApertura { get; set; }
        public int IdUsuarioApertura { get; set; }
        public string UsuarioApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal? MontoCierreDeclarado { get; set; }
        public decimal? MontoCierreCalculado { get; set; }
        public int? IdUsuarioCierre { get; set; }
        public string UsuarioCierre { get; set; }
        public bool Estado { get; set; }

        public decimal? Diferencia
        {
            get
            {
                if (MontoCierreDeclarado.HasValue && MontoCierreCalculado.HasValue)
                {
                    return MontoCierreDeclarado.Value - MontoCierreCalculado.Value;
                }
                return null;
            }
        }
    }
}
