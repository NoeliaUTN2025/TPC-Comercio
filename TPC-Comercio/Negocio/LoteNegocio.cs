using System;
using System.Collections.Generic;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class LoteNegocio
    {
        public int Crear(int idProducto, int idDetalleCompra, int cantidad, decimal precioCompra)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Lotes_Crear");
                datos.setearParametro("@IdProducto", idProducto);
                datos.setearParametro("@IdDetalleCompra", idDetalleCompra);
                datos.setearParametro("@Cantidad", cantidad);
                datos.setearParametro("@PrecioCompra", precioCompra);
                datos.setearParametroSalida("@NewId", System.Data.SqlDbType.Int);
                datos.ejecutarAccion();
                return Convert.ToInt32(datos.obtenerParametro("@NewId"));
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

        public List<Lote> ListarPorProducto(int idProducto)
        {
            List<Lote> lista = new List<Lote>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Lotes_ListarPorProducto");
                datos.setearParametro("@IdProducto", idProducto);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Lote aux = new Lote();
                    aux.Id              = (int)datos.Lector["Id"];
                    aux.IdProducto      = (int)datos.Lector["IdProducto"];
                    aux.IdDetalleCompra = (int)datos.Lector["IdDetalleCompra"];
                    aux.CantidadTotal   = (int)datos.Lector["CantidadTotal"];
                    aux.CantidadDisp    = (int)datos.Lector["CantidadDisp"];
                    aux.PrecioCompra    = (decimal)datos.Lector["PrecioCompra"];
                    aux.FechaIngreso    = (DateTime)datos.Lector["FechaIngreso"];
                    aux.Proveedor       = (string)datos.Lector["Proveedor"];
                    lista.Add(aux);
                }

                return lista;
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

        public void DescontarStock(int idLote, int cantidad)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Lotes_DescontarStock");
                datos.setearParametro("@Id", idLote);
                datos.setearParametro("@Cantidad", cantidad);
                datos.ejecutarAccion();
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
