using System;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class DetalleFacturaNegocio
    {
        public int Insertar(DetalleFactura detalle)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO DetalleFacturas (IdFactura, IdProducto, Cantidad, PrecioCompra, PorcentajeGanancia, PrecioVenta, IdLote) VALUES (@IdFactura, @IdProducto, @Cantidad, @PrecioCompra, @PorcentajeGanancia, @PrecioVenta, @IdLote); SELECT SCOPE_IDENTITY();");
                datos.setearParametro("@IdFactura", detalle.Factura.Id);
                datos.setearParametro("@IdProducto", detalle.Producto.Id);
                datos.setearParametro("@Cantidad", detalle.Cantidad);
                datos.setearParametro("@PrecioCompra", detalle.PrecioCompra);
                datos.setearParametro("@PorcentajeGanancia", detalle.PorcentajeGanancia);
                datos.setearParametro("@PrecioVenta", detalle.PrecioVenta);
                datos.setearParametro("@IdLote", detalle.IdLote ?? (object)DBNull.Value);

                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    return Convert.ToInt32(datos.Lector[0]);
                }
                return 0;
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

        public System.Collections.Generic.List<DetalleFactura> ListarPorFactura(int idFactura)
        {
            System.Collections.Generic.List<DetalleFactura> lista = new System.Collections.Generic.List<DetalleFactura>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT DF.Id, DF.IdProducto, P.NombreProducto, DF.Cantidad, DF.PrecioCompra, DF.PorcentajeGanancia, DF.PrecioVenta, DF.Subtotal, DF.IdLote FROM DetalleFacturas DF INNER JOIN Productos P ON DF.IdProducto = P.Id WHERE DF.IdFactura = @IdFactura");
                datos.setearParametro("@IdFactura", idFactura);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    DetalleFactura aux = new DetalleFactura();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Cantidad = (int)datos.Lector["Cantidad"];
                    aux.PrecioCompra = (decimal)datos.Lector["PrecioCompra"];
                    aux.PorcentajeGanancia = (decimal)datos.Lector["PorcentajeGanancia"];
                    aux.PrecioVenta = (decimal)datos.Lector["PrecioVenta"];
                    aux.Subtotal = (decimal)datos.Lector["Subtotal"];
                    aux.IdLote = datos.Lector["IdLote"] != DBNull.Value ? (int?)datos.Lector["IdLote"] : null;
                    
                    aux.Producto = new Producto();
                    aux.Producto.Id = (int)datos.Lector["IdProducto"];
                    aux.Producto.NombreProducto = (string)datos.Lector["NombreProducto"];
                    
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
