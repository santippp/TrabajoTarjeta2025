using System;
using System.Collections.Generic;

namespace TarjetaSube
{
    public class Tarjeta
    {
        protected decimal saldo;
        protected decimal saldoPendienteAcreditacion;
        private const decimal LIMITE_SALDO = 56000;
        protected Boleto ultimoBoleto; // Se guarda hast un (1) boleto anterior para calcular el trasbordo
        protected Tiempo tiempo;

        // Diferentes Montos validos
        private static readonly HashSet<int> MontosValidos = new HashSet<int>
        {
            2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000
        };

        public Tarjeta(decimal saldoInicial = 0, Tiempo tiempo = null)
        {
            this.saldo = saldoInicial;
            this.tiempo = tiempo ?? new Tiempo();
            this.ultimoBoleto = null;
        }

        public decimal Saldo
        {
            get { return saldo; }
        }

        public decimal SaldoPendiente
        {
            get { return saldoPendienteAcreditacion; }
        }
        public Boleto UltimoBoleto
        {
            get { return ultimoBoleto; }
        }
        public Tiempo GetTiempo()
        {
            return tiempo;
        }
        public void RegistrarBoleto(Boleto boleto)
        {
            ultimoBoleto = boleto;
        }

        public bool PuedeHacerTrasbordo(string lineaActual)
        {
            if (ultimoBoleto == null)
            {
                return false;
            }

            DateTime ahora = tiempo.Now();

            // Distinta linea
            if (ultimoBoleto.LineaColectivo == lineaActual)
            {
                return false;
            }

            // Dentro de 1 hora
            TimeSpan diferencia = ahora - ultimoBoleto.Fecha;
            if (diferencia.TotalHours >= 1)
            {
                return false;
            }

            // Dia de la semana
            DayOfWeek dia = ahora.DayOfWeek;
            int hora = ahora.Hour;

            // Domingo no se puede
            if (dia == DayOfWeek.Sunday)
            {
                return false;
            }

            // Dentro del horario
            if (hora < 7 || hora >= 22)
            {
                return false;
            }

            return true; // Cumple todas las condiciones
        }

        public bool Cargar(int monto)
        {
            // Valida que el monto sea valido
            if (!MontosValidos.Contains(monto))
            {
                return false;
            }

            decimal espacioDisponible = LIMITE_SALDO - saldo;

            // Valida que no supere el limite
            if (monto <= espacioDisponible)
            {
                // Cargar solo hasta el límite
                saldo += monto;
            }
            else
            {
                saldo = LIMITE_SALDO;
                // El resto del saldo queda pendiente
                saldoPendienteAcreditacion += monto - espacioDisponible;
            }

             return true;
        }

        public void AcreditarSaldoPendiente()
        {
            if (saldoPendienteAcreditacion <= 0)
            {
                return; // No hay nada pendiente
            }

            decimal espacioDisponible = LIMITE_SALDO - saldo;

            if (espacioDisponible <= 0)
            {
                return; // No se puede acreditar (maximo)
            }

            if (saldoPendienteAcreditacion <= espacioDisponible)
            {
                // Puedo acreditar todo el pendiente
                saldo += saldoPendienteAcreditacion;
                saldoPendienteAcreditacion = 0;
            }
            else
            {
                // Solo acredito hasta el límite
                saldo += espacioDisponible;
                saldoPendienteAcreditacion -= espacioDisponible;
            }

        }



        public virtual bool DescontarSaldo(decimal monto)
        {
            if (saldo - monto >= -1200)
            {
                saldo -= monto;
                AcreditarSaldoPendiente();
                return true;
            }
            return false;
        }
    }

    public class FranquiciaParcial : Tarjeta
    {
        private DateTime? ultimoViaje; // Registro ultimo viaje si existe
        private int boletosHoy; // Boletos del dia
        private DateTime? diaActual;

        public FranquiciaParcial(decimal saldoInicial = 0, Tiempo tiempo = null) : base(saldoInicial, tiempo)
        {
            this.ultimoViaje = null;
            this.boletosHoy = 0;
            this.diaActual = null;
        }

        public DateTime? UltimoViaje
        {
            get { return ultimoViaje; }
        }

        public int BoletosHoy
        {
            get { return boletosHoy; }
        }

        private void VerificarCambioDeDia()
        {
            DateTime ahora = tiempo.Now();

            // Si no habia dia registrado o si cambio el dia lo guarda
            if (!diaActual.HasValue || ahora.Date > diaActual.Value.Date)
            {
                boletosHoy = 0;
                diaActual = ahora.Date;
            }
        }

        private bool PuedePagarConMedioBoleto()
        {
            VerificarCambioDeDia();
            DateTime ahora = tiempo.Now();

            // Maximo 2 boletos por dia
            if (boletosHoy >= 2)
            {
                return false;
            }

            // 5m de diferencia entre viajes
            if (ultimoViaje.HasValue)
            {
                TimeSpan diferencia = ahora - ultimoViaje.Value;
                if (diferencia.TotalMinutes < 5)
                {
                    return false;
                }
            }

            return true;
        }

        private void RegistrarMedioBoleto()
        {
            ultimoViaje = tiempo.Now();
            boletosHoy++;
        }

        public override bool DescontarSaldo(decimal monto)
        {
            bool puedeMedioBoleto = PuedePagarConMedioBoleto();

            if (puedeMedioBoleto)
            {
                bool resultado = base.DescontarSaldo(monto / 2);

                if (resultado)
                {
                    RegistrarMedioBoleto();
                }

                return resultado;
            }
            else
            {
                // Si no pudo paga monto completo
                return base.DescontarSaldo(monto);
            }
        }
    }

    public class FranquiciaCompleta : Tarjeta
    {
        private int boletosHoy;
        private DateTime? diaActual;

        public FranquiciaCompleta(decimal saldoInicial = 0, Tiempo tiempo = null) : base(saldoInicial, tiempo)
        {
            this.boletosHoy = 0;
            this.diaActual = null;
        }

        public int ViajesGratuitosHoy
        {
            get { return boletosHoy; }
        }

        private void VerificarCambioDeDia()
        {
            DateTime ahora = tiempo.Now();

            if (!diaActual.HasValue || ahora.Date > diaActual.Value.Date)
            {
                boletosHoy = 0;
                diaActual = ahora.Date;
            }
        }

        private bool PuedeViajarGratis()
        {
            VerificarCambioDeDia();

            return boletosHoy < 2;
        }


        private void RegistrarViajeGratuito()
        {
            boletosHoy++;
        }

        public override bool DescontarSaldo(decimal monto)
        {
            bool puedeGratis = PuedeViajarGratis();

            if (puedeGratis)
            {
                RegistrarViajeGratuito();
                return true;
            }
            else
            {
                return base.DescontarSaldo(monto);
            }
        }
    }
}
