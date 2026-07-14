using System;
using System.Collections.Generic;
using AccesoDatos;

namespace Negocio
{
    public class LoteTrazaRow
    {
        public int Id { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Proveedor { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadDisp { get; set; }
        public int CantidadVendida { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioTotal { get; set; }
        public decimal GananciaRealizada { get; set; }
        
    }

    public class VentaTrazaRow
    {
        public int IdLote { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime FechaVenta { get; set; }
        public string Cliente { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class TrazabilidadNegocio
    {
        public List<LoteTrazaRow> ObtenerLotes(int idProducto, decimal porcentajeGanancia)
        {
            List<LoteTrazaRow> lista = new List<LoteTrazaRow>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Trazabilidad_Lotes");
                datos.setearParametro("@IdProducto", idProducto);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    int total = (int)datos.Lector["CantidadTotal"];
                    int disp = (int)datos.Lector["CantidadDisp"];
                    int vendido = total - disp;
                    decimal pc = (decimal)datos.Lector["PrecioCompra"];
                    decimal pv = pc * (1 + porcentajeGanancia / 100m);

                    LoteTrazaRow fila = new LoteTrazaRow();
                    fila.Id = (int)datos.Lector["Id"];
                    fila.FechaIngreso = (DateTime)datos.Lector["FechaIngreso"];
                    fila.Proveedor = datos.Lector["Proveedor"].ToString();
                    fila.CantidadTotal = total;
                    fila.CantidadDisp = disp;
                    fila.CantidadVendida = vendido;
                    fila.PrecioCompra = pc;
                    fila.PrecioTotal = (decimal)datos.Lector["PrecioTotal"];
                    fila.PrecioVenta = pv;
                    fila.GananciaRealizada = (pv - pc) * vendido;
                    lista.Add(fila);
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public List<VentaTrazaRow> ObtenerVentas(int idProducto)
        {
            List<VentaTrazaRow> lista = new List<VentaTrazaRow>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Trazabilidad_Ventas");
                datos.setearParametro("@IdProducto", idProducto);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    VentaTrazaRow fila = new VentaTrazaRow();
                    fila.IdLote = (int)datos.Lector["IdLote"];
                    fila.NumeroFactura = datos.Lector["NumeroFactura"].ToString();
                    fila.FechaVenta = (DateTime)datos.Lector["FechaVenta"];
                    fila.Cliente = datos.Lector["Cliente"].ToString();
                    fila.Cantidad = (int)datos.Lector["Cantidad"];
                    fila.PrecioVenta = (decimal)datos.Lector["PrecioVenta"];
                    fila.Subtotal = (decimal)datos.Lector["Subtotal"];
                    lista.Add(fila);
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }
}
