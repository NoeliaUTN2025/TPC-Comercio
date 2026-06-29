using System;
using System.Collections.Generic;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class CompraNegocio
    {
        public List<Compra> Listar()
        {
            List<Compra> lista = new List<Compra>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                // Usamos consulta directa para incluir la suma de cantidades y concatenar los códigos de productos
                datos.setearConsulta("SELECT c.Id, c.Fecha, c.IdProveedor, p.RazonSocial AS Proveedor, c.IdUsuario, c.Total, c.Estado, ISNULL((SELECT SUM(Cantidad) FROM DetalleCompras dc WHERE dc.IdCompra = c.Id), 0) AS CantidadTotal, ISNULL(STUFF((SELECT ', ' + pr.Codigo FROM DetalleCompras dc INNER JOIN Productos pr ON dc.IdProducto = pr.Id WHERE dc.IdCompra = c.Id FOR XML PATH('')), 1, 2, ''), '') AS CodigosProductos FROM Compras c INNER JOIN Proveedores p ON c.IdProveedor = p.ID WHERE c.Estado = 1 ORDER BY c.Fecha DESC");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Compra aux = new Compra();
                    aux.Id    = (int)datos.Lector["Id"];
                    aux.Fecha = (DateTime)datos.Lector["Fecha"];
                    aux.Total = (decimal)datos.Lector["Total"];
                    aux.CantidadTotal = (int)datos.Lector["CantidadTotal"];
                    aux.CodigosProductos = (string)datos.Lector["CodigosProductos"];
                    aux.estado = (bool)datos.Lector["Estado"];

                    aux.Proveedor = new Proveedor();
                    aux.Proveedor.ID          = (int)datos.Lector["IdProveedor"];
                    aux.Proveedor.RazonSocial = (string)datos.Lector["Proveedor"];

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public int Crear(Compra compra)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Compras_Insertar");
                datos.setearParametro("@IdProveedor", compra.Proveedor.ID);
                datos.setearParametro("@IdUsuario", compra.Usuario.Id);
                datos.setearParametroSalida("@NewId", System.Data.SqlDbType.Int);
                datos.ejecutarAccion();
                return Convert.ToInt32(datos.obtenerParametro("@NewId"));
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void ActualizarTotal(int idCompra)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Compras_ActualizarTotal");
                datos.setearParametro("@IdCompra", idCompra);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void RegistrarCompra(Compra compra, List<DetalleCompra> items)
        {
            int idCompra = Crear(compra);

            DetalleCompraNegocio detalleNegocio = new DetalleCompraNegocio();
            LoteNegocio loteNegocio = new LoteNegocio();
            ProductoNegocio productoNegocio = new ProductoNegocio();

            foreach (DetalleCompra d in items)
            {
                d.Compra = new Compra { Id = idCompra };
                int idDetalle = detalleNegocio.Insertar(d);
                loteNegocio.Crear(d.Producto.Id, idDetalle, d.Cantidad, d.PrecioUnitario);
                
                // Aumentar el stock actual global del producto
                productoNegocio.SumarStock(d.Producto.Id, d.Cantidad);
            }

            ActualizarTotal(idCompra);
        }
    }
}
