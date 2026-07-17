using System;
using System.Collections.Generic;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class PlantillaNegocio
    {
        public void Guardar(ReportePlantilla plantilla)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Plantillas_Insertar");
                datos.setearParametro("@Nombre", plantilla.Nombre);
                datos.setearParametro("@Entidad", plantilla.Entidad);
                
                if (plantilla.FechaDesde.HasValue)
                    datos.setearParametro("@FechaDesde", plantilla.FechaDesde.Value);
                else
                    datos.setearParametro("@FechaDesde", DBNull.Value);

                if (plantilla.FechaHasta.HasValue)
                    datos.setearParametro("@FechaHasta", plantilla.FechaHasta.Value);
                else
                    datos.setearParametro("@FechaHasta", DBNull.Value);

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

        public List<ReportePlantilla> Listar()
        {
            List<ReportePlantilla> lista = new List<ReportePlantilla>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Plantillas_Listar");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    ReportePlantilla aux = new ReportePlantilla();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Entidad = (string)datos.Lector["Entidad"];
                    
                    if (!(datos.Lector["FechaDesde"] is DBNull))
                        aux.FechaDesde = (DateTime)datos.Lector["FechaDesde"];
                    
                    if (!(datos.Lector["FechaHasta"] is DBNull))
                        aux.FechaHasta = (DateTime)datos.Lector["FechaHasta"];

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
    }
}
