using System;
using System.Collections.Generic;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class ProveedorNegocio
    {
        public List<Proveedor> Listar()
        {
            List<Proveedor> lista = new List<Proveedor>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();

            try
            {
                datos.setearProcedimiento("SP_Proveedores_Listar");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Proveedor aux = new Proveedor();
                    aux.ID = (int)datos.Lector["ID"];
                    
                    if (!(datos.Lector["RazonSocial"] is DBNull))
                        aux.RazonSocial = (string)datos.Lector["RazonSocial"];
                        
                    if (!(datos.Lector["Cuit"] is DBNull))
                        aux.Cuit = (string)datos.Lector["Cuit"];

                    if (!(datos.Lector["Direccion"] is DBNull))
                        aux.Direccion = (string)datos.Lector["Direccion"];

                    if (!(datos.Lector["Telefono"] is DBNull))
                        aux.Telefono = (string)datos.Lector["Telefono"];

                    if (!(datos.Lector["Email"] is DBNull))
                        aux.Email = (string)datos.Lector["Email"];

                    aux.Estado = (bool)datos.Lector["Estado"];

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

        public int Agregar(Proveedor nuevo)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();

            try
            {
                datos.setearProcedimiento("SP_Proveedores_Insertar");
                datos.setearParametro("@RazonSocial", nuevo.RazonSocial ?? (object)DBNull.Value);
                datos.setearParametro("@Cuit", nuevo.Cuit ?? (object)DBNull.Value);
                datos.setearParametro("@Direccion", nuevo.Direccion ?? (object)DBNull.Value);
                datos.setearParametro("@Telefono", nuevo.Telefono ?? (object)DBNull.Value);
                datos.setearParametro("@Email", nuevo.Email ?? (object)DBNull.Value);
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

        public void Modificar(Proveedor modificar)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();

            try
            {
                datos.setearProcedimiento("SP_Proveedores_Actualizar");
                datos.setearParametro("@ID", modificar.ID);
                datos.setearParametro("@RazonSocial", modificar.RazonSocial ?? (object)DBNull.Value);
                datos.setearParametro("@Cuit", modificar.Cuit ?? (object)DBNull.Value);
                datos.setearParametro("@Direccion", modificar.Direccion ?? (object)DBNull.Value);
                datos.setearParametro("@Telefono", modificar.Telefono ?? (object)DBNull.Value);
                datos.setearParametro("@Email", modificar.Email ?? (object)DBNull.Value);

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

        public void EliminarLogico(int id)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Proveedores_BajaLogica");
                datos.setearParametro("@ID", id);
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
