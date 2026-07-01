using System;
using System.Collections.Generic;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class PropuestaNegocio
    {
        public void Insertar(PropuestaProveedor propuesta)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Propuestas_Insertar");
                datos.setearParametro("@IdProveedor",    propuesta.Proveedor.ID);
                datos.setearParametro("@IdProducto",     propuesta.Producto.Id);
                datos.setearParametro("@Cantidad",       propuesta.Cantidad);
                datos.setearParametro("@PrecioUnitario", propuesta.PrecioUnitario);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public List<PropuestaProveedor> ListarPorProveedor(int idProveedor)
        {
            List<PropuestaProveedor> lista = new List<PropuestaProveedor>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Propuestas_ListarPorProveedor");
                datos.setearParametro("@IdProveedor", idProveedor);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    PropuestaProveedor p = new PropuestaProveedor();
                    p.Id             = (int)datos.Lector["Id"];
                    p.Cantidad       = (int)datos.Lector["Cantidad"];
                    p.PrecioUnitario = (decimal)datos.Lector["PrecioUnitario"];
                    p.Estado         = datos.Lector["Estado"].ToString();
                    p.Fecha          = (DateTime)datos.Lector["Fecha"];
                    p.Producto       = new Producto { NombreProducto = datos.Lector["NombreProducto"].ToString() };
                    lista.Add(p);
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public List<PropuestaProveedor> ListarPendientes()
        {
            List<PropuestaProveedor> lista = new List<PropuestaProveedor>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Propuestas_ListarPendientes");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    PropuestaProveedor p = new PropuestaProveedor();
                    p.Id             = (int)datos.Lector["Id"];
                    p.Cantidad       = (int)datos.Lector["Cantidad"];
                    p.PrecioUnitario = (decimal)datos.Lector["PrecioUnitario"];
                    p.Estado         = datos.Lector["Estado"].ToString();
                    p.Fecha          = (DateTime)datos.Lector["Fecha"];
                    p.Producto       = new Producto { Id = (int)datos.Lector["IdProducto"], NombreProducto = datos.Lector["NombreProducto"].ToString() };
                    p.Proveedor      = new Proveedor { ID = (int)datos.Lector["IdProveedor"], RazonSocial = datos.Lector["RazonSocial"].ToString() };
                    lista.Add(p);
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public PropuestaProveedor ObtenerPorId(int id)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Propuestas_ObtenerPorId");
                datos.setearParametro("@Id", id);
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    PropuestaProveedor p = new PropuestaProveedor();
                    p.Id             = (int)datos.Lector["Id"];
                    p.Cantidad       = (int)datos.Lector["Cantidad"];
                    p.PrecioUnitario = (decimal)datos.Lector["PrecioUnitario"];
                    p.Estado         = datos.Lector["Estado"].ToString();
                    p.Fecha          = (DateTime)datos.Lector["Fecha"];
                    p.Producto       = new Producto { Id = (int)datos.Lector["IdProducto"], NombreProducto = datos.Lector["NombreProducto"].ToString() };
                    p.Proveedor      = new Proveedor { ID = (int)datos.Lector["IdProveedor"], RazonSocial = datos.Lector["RazonSocial"].ToString() };
                    return p;
                }
                return null;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void Aprobar(int id)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Propuestas_Aprobar");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }
}
