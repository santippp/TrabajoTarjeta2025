using System;
using System.Collections.Generic;

namespace TarjetaSube
{
    public class Tarjeta
    {
        protected decimal saldo;
        protected decimal saldoPendienteAcreditacion;
        private const decimal LIMITE_SALDO = 56000;
        
        // Diferentes Montos validos
        private static readonly HashSet<int> MontosValidos = new HashSet<int>
        {
            2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000
        };

        public Tarjeta(decimal saldoInicial = 0)
        {
            this.saldo = saldoInicial;
        }

        public decimal Saldo
        {
            get { return saldo; }
        }

        public decimal SaldoPendiente
        {
            get { return saldoPendienteAcreditacion; }
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
        //Esta linea llama al constructor de tarjeta de la clase principal con ese parametro de entrada
        public FranquiciaParcial(decimal saldoInicial = 0) : base(saldoInicial) { }

        public override bool DescontarSaldo(decimal monto)
        {
            return base.DescontarSaldo(monto / 2);
        }
    }

    public class FranquiciaCompleta : Tarjeta
    {
        public FranquiciaCompleta(decimal saldoInicial = 0) : base(saldoInicial) { }

        public override bool DescontarSaldo(decimal monto)
        {
            // Siempre devuelve verdadero porque siempre lo puede pagar
            return true;
        }
    }
}
