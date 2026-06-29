using System;
using System.Collections.Generic;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class FacturaNegocio
    {
        public List<Factura> Listar()
        {
            List<Factura> lista = new List<Factura>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT F.Id, F.NumeroFactura, F.Fecha, F.Total, F.Estado, C.ID as IdCliente, C.Nombre, C.Apellido FROM Facturas F INNER JOIN Clientes C ON F.IdCliente = C.ID ORDER BY F.Fecha DESC");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Factura aux = new Factura();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.NumeroFactura = (string)datos.Lector["NumeroFactura"];
                    aux.Fecha = (DateTime)datos.Lector["Fecha"];
                    aux.Total = (decimal)datos.Lector["Total"];
                    aux.estado = (bool)datos.Lector["Estado"];
                    
                    aux.Cliente = new Cliente();
                    aux.Cliente.ID = (int)datos.Lector["IdCliente"];
                    aux.Cliente.Nombre = (string)datos.Lector["Nombre"];
                    aux.Cliente.Apellido = (string)datos.Lector["Apellido"];
                    
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

        public int Crear(Factura factura)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                // Algoritmo para el número de factura
                if (string.IsNullOrEmpty(factura.NumeroFactura))
                {
                    factura.NumeroFactura = "F" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                datos.setearConsulta("INSERT INTO Facturas (NumeroFactura, IdCliente, IdUsuario, Total) VALUES (@Num, @IdCliente, @IdUsuario, 0); SELECT SCOPE_IDENTITY();");
                datos.setearParametro("@Num", factura.NumeroFactura);
                datos.setearParametro("@IdCliente", factura.Cliente.ID);
                datos.setearParametro("@IdUsuario", factura.Usuario.Id);
                
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

        public void ActualizarTotal(int idFactura)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE Facturas SET Total = (SELECT ISNULL(SUM(Subtotal), 0) FROM DetalleFacturas WHERE IdFactura = @Id) WHERE Id = @Id");
                datos.setearParametro("@Id", idFactura);
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

        public void RegistrarVenta(Factura factura, List<DetalleFactura> items)
        {
            int idFactura = Crear(factura);
            DetalleFacturaNegocio detalleNegocio = new DetalleFacturaNegocio();
            LoteNegocio loteNegocio = new LoteNegocio();
            ProductoNegocio productoNegocio = new ProductoNegocio();

            foreach (DetalleFactura item in items)
            {
                int cantidadRestante = item.Cantidad;
                
                // Traemos los lotes por orden de ingreso
                List<Lote> lotesDisponibles = loteNegocio.ListarPorProducto(item.Producto.Id);
                
                foreach (Lote lote in lotesDisponibles)
                {
                    if (cantidadRestante <= 0) break;

                    int cantidadATomar = (lote.CantidadDisp <= cantidadRestante) ? lote.CantidadDisp : cantidadRestante;
                    
                    DetalleFactura detalleVenta = new DetalleFactura();
                    detalleVenta.Factura = new Factura { Id = idFactura };
                    detalleVenta.Producto = item.Producto;
                    detalleVenta.Cantidad = cantidadATomar;
                    detalleVenta.PrecioCompra = lote.PrecioCompra;
                    detalleVenta.PorcentajeGanancia = item.PorcentajeGanancia;
                    
                    detalleVenta.PrecioVenta = lote.PrecioCompra + (lote.PrecioCompra * (item.PorcentajeGanancia / 100));
                    detalleVenta.IdLote = lote.Id;

                    detalleNegocio.Insertar(detalleVenta);
                    
                    loteNegocio.DescontarStock(lote.Id, cantidadATomar);
                    
                    cantidadRestante -= cantidadATomar;
                }
                
                if (cantidadRestante > 0)
                {
                    throw new Exception("Error crítico: Stock de lotes inconsistente con stock global. Faltaron " + cantidadRestante + " unidades de " + item.Producto.NombreProducto);
                }
                
                // 3. Descontamos stock global (SumarStock con cantidad negativa funciona perfecto)
                productoNegocio.SumarStock(item.Producto.Id, -item.Cantidad);
            }
            
            ActualizarTotal(idFactura);
        }
    }
}
