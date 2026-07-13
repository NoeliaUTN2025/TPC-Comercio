using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class ProductoNegocio
    {
        public List<Producto> Listar()
        {
            List<Producto> lista = new List<Producto>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();

            try
            {
                datos.setearProcedimiento("SP_Productos_Listar");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Producto aux = new Producto();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.NombreProducto = (string)datos.Lector["NombreProducto"];

                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.Descripcion = (string)datos.Lector["Descripcion"];

                    aux.StockActual = (int)datos.Lector["StockActual"];
                    aux.StockMinimo = (int)datos.Lector["StockMinimo"];
                    aux.Precio = (decimal)datos.Lector["Precio"];
                    aux.PorcentajeGanancia = (decimal)datos.Lector["PorcentajeGanancia"];

                    aux.marca = new Marca();
                    aux.marca.Id = (int)datos.Lector["IdMarca"];
                    aux.marca.Descripcion = (string)datos.Lector["Marca"];

                    aux.categoria = new Categoria();
                    aux.categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.categoria.Descripcion = (string)datos.Lector["Categoria"];

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

        ///Listar Paginado 
        public List<Producto> ListarPaginado(int pageNumber, int pageSize, out int totalRegistros, string busqueda = null)
        {
            List<Producto> lista = new List<Producto>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            totalRegistros = 0;

            try
            {
                datos.setearProcedimiento("SP_Productos_ListarPaginado");
                datos.setearParametro("@PageNumber", pageNumber);
                datos.setearParametro("@PageSize", pageSize);

                if (string.IsNullOrWhiteSpace(busqueda))
                    datos.setearParametro("@Busqueda", DBNull.Value);
                else
                    datos.setearParametro("@Busqueda", busqueda);

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Producto aux = new Producto();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.NombreProducto = (string)datos.Lector["NombreProducto"];

                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.Descripcion = (string)datos.Lector["Descripcion"];

                    aux.StockActual = (int)datos.Lector["StockActual"];
                    aux.StockMinimo = (int)datos.Lector["StockMinimo"];
                    aux.Precio = (decimal)datos.Lector["Precio"];
                    aux.PorcentajeGanancia = (decimal)datos.Lector["PorcentajeGanancia"];

                    aux.marca = new Marca();
                    aux.marca.Id = (int)datos.Lector["IdMarca"];
                    aux.marca.Descripcion = (string)datos.Lector["Marca"];

                    aux.categoria = new Categoria();
                    aux.categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.categoria.Descripcion = (string)datos.Lector["Categoria"];

                    totalRegistros = (int)datos.Lector["TotalRegistros"];

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


        public void Agregar(Producto nuevo)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();

            try
            {
                datos.setearProcedimiento("SP_Productos_Insertar");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@NombreProducto", nuevo.NombreProducto);
                datos.setearParametro("@Descripcion", nuevo.Descripcion ?? (object)DBNull.Value);
                datos.setearParametro("@Precio", nuevo.Precio);
                datos.setearParametro("@StockMinimo", nuevo.StockMinimo);
                datos.setearParametro("@PorcentajeGanancia", nuevo.PorcentajeGanancia);
                datos.setearParametro("@IdMarca", nuevo.marca.Id);
                datos.setearParametro("@IdCategoria", nuevo.categoria.Id);

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

        public void Modificar(Producto modificar)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();

            try
            {
                datos.setearProcedimiento("SP_Productos_Actualizar");
                datos.setearParametro("@Id", modificar.Id);
                datos.setearParametro("@Codigo", modificar.Codigo);
                datos.setearParametro("@NombreProducto", modificar.NombreProducto);
                datos.setearParametro("@Descripcion", modificar.Descripcion ?? (object)DBNull.Value);
                datos.setearParametro("@Precio", modificar.Precio);
                datos.setearParametro("@StockMinimo", modificar.StockMinimo);
                datos.setearParametro("@PorcentajeGanancia", modificar.PorcentajeGanancia);
                datos.setearParametro("@IdMarca", modificar.marca.Id);
                datos.setearParametro("@IdCategoria", modificar.categoria.Id);

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
                datos.setearProcedimiento("SP_Productos_BajaLogica");
                datos.setearParametro("@Id", id);
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

        public void SumarStock(int idProducto, int cantidad)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE Productos SET StockActual = StockActual + @Cantidad WHERE Id = @Id");
                datos.setearParametro("@Cantidad", cantidad);
                datos.setearParametro("@Id", idProducto);
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
