using System;
using System.Data;
using AccesoDatos;

namespace Negocio
{
    public class ReportesNegocio
    {
        public DataTable GenerarReporte(string entidad, DateTime? desde, DateTime? hasta)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            DataTable dt = new DataTable();

            try
            {
                string consulta = "";

                if (entidad == "Ventas")
                {
                    consulta = "SELECT F.NumeroFactura, F.Fecha, F.Total, C.Nombre + ' ' + C.Apellido as Cliente " +
                               "FROM Facturas F INNER JOIN Clientes C ON F.IdCliente = C.ID " +
                               "WHERE F.Estado = 1 ";
                    if (desde.HasValue) consulta += " AND CAST(F.Fecha AS DATE) >= @Desde ";
                    if (hasta.HasValue) consulta += " AND CAST(F.Fecha AS DATE) <= @Hasta ";
                    consulta += " ORDER BY F.Fecha DESC";
                }
                else if (entidad == "Compras")
                {
                    consulta = "SELECT C.Id as NumeroCompra, C.Fecha, C.Total, P.RazonSocial as Proveedor " +
                               "FROM Compras C INNER JOIN Proveedores P ON C.IdProveedor = P.ID " +
                               "WHERE C.Estado = 1 ";
                    if (desde.HasValue) consulta += " AND CAST(C.Fecha AS DATE) >= @Desde ";
                    if (hasta.HasValue) consulta += " AND CAST(C.Fecha AS DATE) <= @Hasta ";
                    consulta += " ORDER BY C.Fecha DESC";
                }
                else if (entidad == "LoteMasCostoso")
                {
                    consulta = "SELECT TOP 10 L.Id as Lote, P.NombreProducto, L.PrecioCompra, L.CantidadTotal, L.FechaIngreso " +
                               "FROM Lotes L INNER JOIN Productos P ON L.IdProducto = P.Id " +
                               "ORDER BY L.PrecioCompra DESC";
                }
                else if (entidad == "ProductoMasVendido")
                {
                    consulta = "SELECT TOP 10 P.NombreProducto, SUM(DF.Cantidad) as CantidadVendida, SUM(DF.PrecioVenta * DF.Cantidad) as TotalRecaudado " +
                               "FROM DetalleFacturas DF INNER JOIN Productos P ON DF.IdProducto = P.Id " +
                               "INNER JOIN Facturas F ON DF.IdFactura = F.Id " +
                               "WHERE F.Estado = 1 ";
                    if (desde.HasValue) consulta += " AND CAST(F.Fecha AS DATE) >= @Desde ";
                    if (hasta.HasValue) consulta += " AND CAST(F.Fecha AS DATE) <= @Hasta ";
                    consulta += " GROUP BY P.NombreProducto ORDER BY CantidadVendida DESC";
                }
                else
                {
                    throw new Exception("Entidad no válida para reporte.");
                }

                datos.setearConsulta(consulta);
                if (desde.HasValue) datos.setearParametro("@Desde", desde.Value);
                if (hasta.HasValue) datos.setearParametro("@Hasta", hasta.Value);

                datos.ejecutarLectura();

                // Cargar el Lector directamente en un DataTable
                dt.Load(datos.Lector);

                return dt;
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
