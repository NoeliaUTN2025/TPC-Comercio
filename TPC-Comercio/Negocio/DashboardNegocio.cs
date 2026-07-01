using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;

namespace Negocio
{
    public class DashboardStats
    {
        public decimal TotalVentasMes { get; set; }
        public decimal TotalComprasMes { get; set; }
        public int ProductosBajoStock { get; set; }
    }

    public class DashboardNegocio
    {
        public DashboardStats ObtenerEstadisticas()
        {
            DashboardStats stats = new DashboardStats();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();

            try
            {
                // 1. Total Ventas del Mes
                datos.setearConsulta("SELECT ISNULL(SUM(Total), 0) AS TotalVentas FROM Facturas WHERE MONTH(Fecha) = MONTH(GETDATE()) AND YEAR(Fecha) = YEAR(GETDATE()) AND Estado = 1");
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    stats.TotalVentasMes = (decimal)datos.Lector["TotalVentas"];
                }
                datos.cerrarConexion();

                // 2. Total Compras del Mes
                datos.setearConsulta("SELECT ISNULL(SUM(Total), 0) AS TotalCompras FROM Compras WHERE MONTH(Fecha) = MONTH(GETDATE()) AND YEAR(Fecha) = YEAR(GETDATE()) AND Estado = 1");
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    stats.TotalComprasMes = (decimal)datos.Lector["TotalCompras"];
                }
                datos.cerrarConexion();

                // 3. Productos Bajo Stock
                datos.setearConsulta("SELECT COUNT(*) AS BajoStock FROM Productos WHERE StockActual <= StockMinimo AND Estado = 1");
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    stats.ProductosBajoStock = (int)datos.Lector["BajoStock"];
                }
                datos.cerrarConexion();

                return stats;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
