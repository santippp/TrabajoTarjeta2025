using System;

namespace TarjetaSube
{
    public class Boleto
    {
        private static int contador = 0;
        
        public int Id { get; private set; }
        public DateTime Fecha { get; private set; }
        public string TipoTarjeta { get; private set; }
        public string LineaColectivo { get; private set; }
        public decimal MontoAbonado { get; private set; }
        public decimal SaldoRestante { get; private set; }

        public Boleto(decimal tarifa, string lineaColectivo, Tarjeta tarjeta)
        {
            contador++;
            Id = contador;
            
            Fecha = DateTime.Now;
            TipoTarjeta = tarjeta.GetType().Name;
            LineaColectivo = lineaColectivo;
            SaldoRestante = tarjeta.Saldo;
            MontoAbonado = tarifa;

        }

        public override string ToString()
        {
            return $"--------------------------------------\n" +
                   $"  BOLETO: {Id}\n" +
                   $"  Fecha:       {Fecha:dd/MM/yyyy HH:mm:ss}\n" +
                   $"  Línea:       {LineaColectivo}\n" +
                   $"  Tipo:        {TipoTarjeta}\n" +
                   $"  Abonado:     ${MontoAbonado:F2}\n" +
                   $"  Saldo:       ${SaldoRestante:F2}\n" +
                   $"--------------------------------------";
        }

    }
}
