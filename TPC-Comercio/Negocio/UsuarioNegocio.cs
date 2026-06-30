using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using AccesoDatos;
using System.CodeDom;


namespace Negocio
{
    public class UsuarioNegocio
    {
        public Usuario Login(string user, string contraseña)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            Usuario usuario = null;

            try
            {
                datos.setearProcedimiento("SP_Usuarios_Login");
                datos.setearParametro("@User", user);
                datos.setearParametro("@Contrasena", contraseña);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    usuario = new Usuario();

                    usuario.Id = (int)datos.Lector["Id"];
                    usuario.User = datos.Lector["User"].ToString();
                    usuario.Contraseña = datos.Lector["Contrasena"].ToString();
                    usuario.Estado = (bool)datos.Lector["Estado"];

                    usuario.perfil = new Perfil();
                    usuario.perfil.Id = (int)datos.Lector["IdPerfil"];
                    usuario.perfil.NombrePerfil = datos.Lector["NombrePerfil"].ToString();
                }
                return usuario;

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

        public void AgregarUsuario(Usuario usuario)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Usuarios_Agregar");
                datos.setearParametro("@User", usuario.User);
                datos.setearParametro("@Contrasena", usuario.Contraseña);
                datos.setearParametro("@IdPerfil", usuario.perfil.Id);
                datos.setearParametro("@Estado", usuario.Estado);
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

        public bool ExisteUsuario(string user)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT [User] FROM [Usuarios] WHERE [User] = @User");
                datos.setearParametro("@User", user);
                datos.ejecutarLectura();
                return datos.Lector.Read(); // Si hay un registro, el usuario existe
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

        public bool CambiarContraseña(int IdUsuario, string contraseñaActual, string contraseñaNueva)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();

            try
            {
                datos.setearProcedimiento("SP_Usuarios_CambiarContrasena");
                datos.setearParametro(@"Id", IdUsuario);
                datos.setearParametro("ContrasenaActual", contraseñaActual);
                datos.setearParametro("ContrasenaNueva", contraseñaNueva);

                datos.ejecutarAccion();

                return true; 
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

