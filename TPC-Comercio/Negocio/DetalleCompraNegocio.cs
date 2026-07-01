using System;
using System.Collections.Generic;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class DetalleCompraNegocio
    {
        public List<DetalleCompra> ListarPorCompra(int idCompra)
        {
            List<DetalleCompra> lista = new List<DetalleCompra>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_DetalleCompras_ListarPorCompra");
                datos.setearParametro("@IdCompra", idCompra);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    DetalleCompra d = new DetalleCompra();
                    d.Id            = (int)datos.Lector["Id"];
                    d.Cantidad      = (int)datos.Lector["Cantidad"];
                    d.PrecioUnitario = (decimal)datos.Lector["PrecioUnitario"];
                    d.Subtotal      = (decimal)datos.Lector["Subtotal"];
                    d.Producto      = new Producto { Id = (int)datos.Lector["IdProducto"], NombreProducto = datos.Lector["NombreProducto"].ToString() };
                    lista.Add(d);
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

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
