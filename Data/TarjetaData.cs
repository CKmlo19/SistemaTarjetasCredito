using Microsoft.Data.SqlClient;
using SistemaTarjetasCredito.Models;
using System.Data;
using System.Collections.Generic;

namespace SistemaTarjetasCredito.Data
{
    public class TarjetaData
    {
        //Este método lista todas las tarjetas en orden alfabético
        public List<TarjetaModel> ListarTodasLasTarjetas()
        {
            var listaTarjetas = new List<TarjetaModel>();
            var cn = new Conexion();

            // Abre la conexión
            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                // Llama al procedimiento almacenado para listar todas las tarjetas
                SqlCommand cmd = new SqlCommand("dbo.ObtenerTodasLasTarjetas", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        listaTarjetas.Add(new TarjetaModel
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            NumeroTarjeta = dr["NumeroTarjeta"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            TipoCuenta = dr["TipoCuenta"].ToString(),
                            FechaVencimiento = Convert.ToDateTime(dr["FechaVencimiento"])
                        });
                    }
                }
            }

            return listaTarjetas;
        }

        public List<EstadoCuentaModel> ObtenerEstadosCuentaTCM(int idTCM)
        {
            var listaEstados = new List<EstadoCuentaModel>();

            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                SqlCommand cmd = new SqlCommand("dbo.ObtenerEstadosCuentaTCM", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@idTCM", idTCM);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        listaEstados.Add(new EstadoCuentaModel
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            IdTCM = Convert.ToInt32(dr["idTCM"]),
                            SaldoActual = Convert.ToDecimal(dr["SaldoActual"]),
                            PagoMinimo = Convert.ToDecimal(dr["PagoMinimo"]),
                            FechaEstadoCuenta = Convert.ToDateTime(dr["FechaEstadoCuenta"]),
                            InteresesCorrientes = Convert.ToDecimal(dr["InteresesCorrientes"]),
                            InteresesMoratorios = Convert.ToDecimal(dr["InteresesMoratorios"]),
                            CantidadOperacionesATM = Convert.ToInt32(dr["CantidadOperacionesATM"]),
                            CantidadOperacionesVentanilla = Convert.ToInt32(dr["CantidadOperacionesVentanilla"])
                        });
                    }
                }
            }

            return listaEstados;
        }

        public List<SubestadoCuentaModel> ObtenerSubestadosCuentaTCA(int idTCA)
        {
            var listaSubestados = new List<SubestadoCuentaModel>();

            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                SqlCommand cmd = new SqlCommand("dbo.ObtenerSubestadosCuentaTCA", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@idTCA", idTCA);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        listaSubestados.Add(new SubestadoCuentaModel
                        {
                            FechaEstadoCuenta = Convert.ToDateTime(dr["FechaEstadoCuenta"]),
                            CantidadOperacionesATM = Convert.ToInt32(dr["CantidadOperacionesATM"]),
                            CantidadOperacionesVentanilla = Convert.ToInt32(dr["CantidadOperacionesVentanilla"]),
                            CantidadCompras = Convert.ToInt32(dr["CantidadCompras"]),
                            SumaCompras = Convert.ToDecimal(dr["SumaCompras"]),
                            CantidadRetiros = Convert.ToInt32(dr["CantidadRetiros"]),
                            SumaRetiros = Convert.ToDecimal(dr["SumaRetiros"])
                        });
                    }
                }
            }

            return listaSubestados;
        }


        public List<MovimientoModel> ObtenerMovimientosPorEstado(int idTarjetaFisica)
        {
            var listaMovimientos = new List<MovimientoModel>();

            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                SqlCommand cmd = new SqlCommand("dbo.ObtenerMovimientosPorEstado", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Asegúrate de agregar el parámetro @idTarjetaFisica
                cmd.Parameters.AddWithValue("@idTarjetaFisica", idTarjetaFisica);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        listaMovimientos.Add(new MovimientoModel
                        {
                            FechaOperacion = Convert.ToDateTime(dr["FechaOperacion"]),
                            TipoMovimiento = dr["TipoMovimiento"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Referencia = dr["Referencia"].ToString(),
                            Monto = Convert.ToDecimal(dr["Monto"]),
                            NuevoSaldo = Convert.ToDecimal(dr["NuevoSaldo"])
                        });
                    }
                }
            }

            return listaMovimientos;
        }

        public List<TarjetaModel> ListarTarjetasPorUsuario(int idUsuario)
        {
            var listaTarjetas = new List<TarjetaModel>();

            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.getCadenaSQL()))
            {
                conexion.Open();

                SqlCommand cmd = new SqlCommand("dbo.ObtenerTarjetasPorTarjetaHabiente", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        listaTarjetas.Add(new TarjetaModel
                        {
                            Id = Convert.ToInt32(dr["id"]),
                            NumeroTarjeta = dr["NumeroTarjeta"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            TipoCuenta = dr["TipoCuenta"].ToString(),
                            FechaVencimiento = Convert.ToDateTime(dr["FechaVencimiento"])
                        });
                    }
                }
            }

            return listaTarjetas;
        }

        public int ObtenerIdTipoUsuario(string username, string password)
        {
            int idTipoUsuario = -1; // Valor por defecto en caso de no encontrar datos

            try
            {
                var cn = new Conexion();

                // Abre la conexión
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();

                    // Llama al procedimiento almacenado
                    SqlCommand cmd = new SqlCommand("dbo.ObtenerIdTipoUsuario", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agrega los parámetros de entrada
                    cmd.Parameters.AddWithValue("@Nombre", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    // Ejecuta el procedimiento almacenado
                    using (var dr = cmd.ExecuteReader())
                    {
                        // Si hay datos, asigna el idTipoUsuario
                        if (dr.Read())
                        {
                            idTipoUsuario = (int)dr["idTipoUsuario"];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Maneja errores y devuelve -1 en caso de fallo
                Console.WriteLine($"Error: {e.Message}");
                idTipoUsuario = -1;
            }

            return idTipoUsuario;
        }

        public int ObtenerIdUsuario(string username, string password)
        {
            int idUsuario = -1; // Valor por defecto si no se encuentra el usuario o ocurre un error

            try
            {
                var cn = new Conexion();

                // Abre la conexión
                using (var conexion = new SqlConnection(cn.getCadenaSQL()))
                {
                    conexion.Open();

                    // Llama al procedimiento almacenado
                    SqlCommand cmd = new SqlCommand("dbo.ObtenerIdUsuario", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agrega los parámetros de entrada
                    cmd.Parameters.AddWithValue("@Nombre", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    // Parámetro de salida para el id del usuario
                    SqlParameter outputParam = new SqlParameter("@OutId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    // Ejecuta el procedimiento almacenado
                    cmd.ExecuteNonQuery();

                    // Obtiene el valor del parámetro de salida
                    idUsuario = (int)cmd.Parameters["@OutId"].Value;
                }
            }
            catch (Exception e)
            {
                // Maneja errores y deja el idUsuario con el valor por defecto (-1)
                Console.WriteLine($"Error: {e.Message}");
            }

            return idUsuario;
        }





    }
}


