using System;
using System.Collections.Generic;

namespace TarjetaSube
{
    public class Tarjeta
    {
        private decimal saldo;
        private const decimal LIMITE_SALDO = 40000;
        
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
       
        public bool Cargar(int monto)
        {
            // Valida que el monto sea valido
            if (!MontosValidos.Contains(monto))
            {
                return false;
            }

            // Valida que no supere el limite
            if (saldo + monto > LIMITE_SALDO)
            {
                // Cargar solo hasta el límite
                saldo = LIMITE_SALDO;
                return true;
            }

            saldo += monto;
            return true;
        }

       
        public bool DescontarSaldo(decimal monto)
        {
            if (saldo >= monto)
            {
                saldo -= monto;
                return true;
            }
            return false;
        }
    }
}
