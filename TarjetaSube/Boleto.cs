using System;

namespace TarjetaSube
{
    public class Boleto
    {
        public DateTime Fecha { get; private set; }
        public decimal Monto { get; private set; }
        public string LineaColectivo { get; private set; }
        public decimal SaldoRestante { get; private set; }

        public Boleto(decimal monto, string lineaColectivo, decimal saldoRestante)
        {
            Fecha = DateTime.Now;
            Monto = monto;
            LineaColectivo = lineaColectivo;
            SaldoRestante = saldoRestante;
        }

        public override string ToString()
        {
            return $"Boleto - Fecha: {Fecha:dd/MM/yyyy HH:mm:ss}, Línea: {LineaColectivo}, " +
                   $"Monto: ${Monto}, Saldo Restante: ${SaldoRestante}";
        }
    }
}
