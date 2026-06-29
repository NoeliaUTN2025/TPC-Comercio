using System;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class DetalleFacturaNegocio
    {
        public int Insertar(DetalleFactura detalle)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                // Usamos consulta directa por pragmatismo (evitamos tocar SQL con nuevos SPs)
                datos.setearConsulta("INSERT INTO DetalleFacturas (IdFactura, IdProducto, Cantidad, PrecioCompra, PorcentajeGanancia, PrecioVenta, IdLote) VALUES (@IdFactura, @IdProducto, @Cantidad, @PrecioCompra, @PorcentajeGanancia, @PrecioVenta, @IdLote); SELECT SCOPE_IDENTITY();");
                datos.setearParametro("@IdFactura", detalle.Factura.Id);
                datos.setearParametro("@IdProducto", detalle.Producto.Id);
                datos.setearParametro("@Cantidad", detalle.Cantidad);
                datos.setearParametro("@PrecioCompra", detalle.PrecioCompra);
                datos.setearParametro("@PorcentajeGanancia", detalle.PorcentajeGanancia);
                datos.setearParametro("@PrecioVenta", detalle.PrecioVenta);
                datos.setearParametro("@IdLote", detalle.IdLote ?? (object)DBNull.Value);

                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    return Convert.ToInt32(datos.Lector[0]);
                }
                return 0;
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
