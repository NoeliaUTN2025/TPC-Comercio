using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class CajaNegocio
    {
        public int AbrirCaja(decimal montoApertura, int idUsuario)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Caja_Abrir");
                datos.setearParametro("@MontoApertura", montoApertura);
                datos.setearParametro("@IdUsuario", idUsuario);
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

        public Caja ObtenerCajaAbierta()
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Caja_ObtenerAbierta");
                datos.ejecutarLectura();
                Caja caja = null;
                if (datos.Lector.Read())
                {
                    caja = MapearCaja(datos.Lector);
                }
                return caja;
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

        public void CerrarCaja(int idCaja, decimal montoCierreDeclarado, int idUsuario)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Caja_Cerrar");
                datos.setearParametro("@Id", idCaja);
                datos.setearParametro("@MontoCierreDeclarado", montoCierreDeclarado);
                datos.setearParametro("@IdUsuario", idUsuario);
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

        public List<Caja> Listar()
        {
            List<Caja> lista = new List<Caja>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Caja_Listar");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(MapearCaja(datos.Lector));
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

        private Caja MapearCaja(SqlDataReader lector)
        {
            Caja caja = new Caja();
            caja.Id = (int)lector["Id"];
            caja.FechaApertura = (DateTime)lector["FechaApertura"];
            caja.MontoApertura = (decimal)lector["MontoApertura"];
            caja.IdUsuarioApertura = (int)lector["IdUsuarioApertura"];
            caja.UsuarioApertura = (string)lector["UsuarioApertura"];
            caja.FechaCierre = lector["FechaCierre"] != DBNull.Value ? (DateTime?)lector["FechaCierre"] : null;
            caja.MontoCierreDeclarado = lector["MontoCierreDeclarado"] != DBNull.Value ? (decimal?)lector["MontoCierreDeclarado"] : null;
            caja.MontoCierreCalculado = lector["MontoCierreCalculado"] != DBNull.Value ? (decimal?)lector["MontoCierreCalculado"] : null;
            caja.IdUsuarioCierre = lector["IdUsuarioCierre"] != DBNull.Value ? (int?)lector["IdUsuarioCierre"] : null;
            caja.UsuarioCierre = lector["UsuarioCierre"] != DBNull.Value ? (string)lector["UsuarioCierre"] : null;
            caja.Estado = (bool)lector["Estado"];
            return caja;
        }
    }
}
