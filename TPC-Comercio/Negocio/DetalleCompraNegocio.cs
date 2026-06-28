using System;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class DetalleCompraNegocio
    {
        public int Insertar(DetalleCompra detalle)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_DetalleCompras_Insertar");
                datos.setearParametro("@IdCompra", detalle.Compra.Id);
                datos.setearParametro("@IdProducto", detalle.Producto.Id);
                datos.setearParametro("@Cantidad", detalle.Cantidad);
                datos.setearParametro("@PrecioUnitario", detalle.PrecioUnitario);
                datos.setearParametroSalida("@NewId", System.Data.SqlDbType.Int);
                datos.ejecutarAccion();
                return Convert.ToInt32(datos.obtenerParametro("@NewId"));
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }
}
