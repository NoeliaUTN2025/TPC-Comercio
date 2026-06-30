using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    public static class Seguridad
    {
        public static bool SesionActiva (object usuario) // para saber si esta loguado
        {
            return (usuario != null);
        }
        public static bool EsAdmin (Usuario usuario)
        {
            if (usuario == null)
                return false;
            
                return usuario.perfil.NombrePerfil == "Administrador"; // solo cuando es administrador puede acceder a ciertas funciones
        }
        public static bool EsVendedor (Usuario usuario)
        {
            if (usuario == null)
                return false;

            return usuario.perfil.NombrePerfil == "Vendedor"; // solo cuando es vendedor puede acceder a ciertas funciones
        }

        public static bool EsCliente(Usuario usuario)
        {
            if (usuario == null)
                return false;

            return usuario.perfil.NombrePerfil == "Cliente"; // solo cuando es vendedor puede acceder a ciertas funciones
        }

        public static bool EsProveedor(Usuario usuario)
        {
            if (usuario == null)
                return false;

            return usuario.perfil.NombrePerfil == "Proveedor"; // solo cuando es vendedor puede acceder a ciertas funciones
        }
    }
}
