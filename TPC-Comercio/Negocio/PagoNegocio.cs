using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class PagoNegocio
    {
        public static List<Cuota> CalcularPlanDeCuotas(decimal totalFactura, int cantidadCuotas, decimal tasaMensual, DateTime fechaVenta)
        {
            decimal interesTotal = totalFactura * (tasaMensual / 100m) * cantidadCuotas;
            decimal capitalPorCuota = Math.Round(totalFactura / cantidadCuotas, 2, MidpointRounding.AwayFromZero);
            decimal interesPorCuota = Math.Round(interesTotal / cantidadCuotas, 2, MidpointRounding.AwayFromZero);

            List<Cuota> plan = new List<Cuota>();
            for (int nro = 1; nro <= cantidadCuotas; nro++)
            {
                bool esUltima = nro == cantidadCuotas;

                Cuota cuota = new Cuota();
                cuota.NroCuota = nro;
                cuota.Monto = esUltima ? totalFactura - capitalPorCuota * (cantidadCuotas - 1) : capitalPorCuota;
                cuota.Interes = esUltima ? interesTotal - interesPorCuota * (cantidadCuotas - 1) : interesPorCuota;
                cuota.Vencimiento = fechaVenta.AddMonths(nro);

                plan.Add(cuota);
            }

            return plan;
        }

        public int RegistrarPago(int idFactura, decimal totalFactura, string tipoPago, int cantidadCuotas)
        {
            List<Cuota> plan = new List<Cuota>();
            decimal montoPago = totalFactura;

            if (tipoPago == "Credito")
            {
                decimal tasaMensual = decimal.Parse(ConfigurationManager.AppSettings["TasaInteresMensual"], CultureInfo.InvariantCulture);
                plan = CalcularPlanDeCuotas(totalFactura, cantidadCuotas, tasaMensual, DateTime.Now);
                montoPago = totalFactura + plan.Sum(c => c.Interes);
            }

            int idPago = Crear(idFactura, tipoPago, montoPago, tipoPago == "Credito" ? cantidadCuotas : 0);

            foreach (Cuota cuota in plan)
            {
                InsertarCuota(idPago, cuota);
            }

            return idPago;
        }

        private int Crear(int idFactura, string tipo, decimal monto, int cantidadCuotas)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Pagos_Crear");
                datos.setearParametro("@IdFactura", idFactura);
                datos.setearParametro("@Tipo", tipo);
                datos.setearParametro("@Monto", monto);
                datos.setearParametro("@CantidadCuotas", cantidadCuotas);
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

        private void InsertarCuota(int idPago, Cuota cuota)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Cuotas_Crear");
                datos.setearParametro("@IdPago", idPago);
                datos.setearParametro("@NroCuota", cuota.NroCuota);
                datos.setearParametro("@Monto", cuota.Monto);
                datos.setearParametro("@Interes", cuota.Interes);
                datos.setearParametro("@Vencimiento", cuota.Vencimiento);
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

        public Pago ObtenerPorFactura(int idFactura)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Pagos_ObtenerPorFactura");
                datos.setearParametro("@IdFactura", idFactura);
                datos.ejecutarLectura();

                Pago pago = null;
                if (datos.Lector.Read())
                {
                    pago = new Pago();
                    pago.Id = (int)datos.Lector["Id"];
                    pago.IdFactura = (int)datos.Lector["IdFactura"];
                    pago.Tipo = (string)datos.Lector["Tipo"];
                    pago.Monto = (decimal)datos.Lector["Monto"];
                    pago.CantidadCuotas = (int)datos.Lector["CantidadCuotas"];
                    pago.Fecha = (DateTime)datos.Lector["Fecha"];
                }
                return pago;
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

        public List<Cuota> ListarCuotasPorPago(int idPago)
        {
            List<Cuota> lista = new List<Cuota>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Cuotas_ListarPorPago");
                datos.setearParametro("@IdPago", idPago);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Cuota aux = new Cuota();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.IdPago = idPago;
                    aux.NroCuota = (int)datos.Lector["NroCuota"];
                    aux.Monto = (decimal)datos.Lector["Monto"];
                    aux.Interes = (decimal)datos.Lector["Interes"];
                    aux.Vencimiento = (DateTime)datos.Lector["Vencimiento"];
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

        public decimal SumarEfectivoDesde(DateTime desde)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Pagos_SumarEfectivoDesde");
                datos.setearParametro("@Desde", desde);
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    return (decimal)datos.Lector["Total"];
                }
                return 0m;
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
