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
        public bool EsTrasbordo { get; private set; }

        public Boleto(decimal tarifa, string lineaColectivo, Tarjeta tarjeta, bool esTrasbordo = false)
        {
            contador++;
            Id = contador;
            
            Fecha = tarjeta.GetTiempo().Now();
            TipoTarjeta = tarjeta.GetType().Name;
            LineaColectivo = lineaColectivo;
            SaldoRestante = tarjeta.Saldo;
            MontoAbonado = esTrasbordo ? 0 : tarifa;
            EsTrasbordo = esTrasbordo;

        }

        public override string ToString()
        {
            string trasbordo = EsTrasbordo ? " [TRASBORDO]" : "";

            return $"--------------------------------------\n" +
                   $"  BOLETO: {Id}{trasbordo}\n" +
                   $"  Fecha:       {Fecha:dd/MM/yyyy HH:mm:ss}\n" +
                   $"  Línea:       {LineaColectivo}\n" +
                   $"  Tipo:        {TipoTarjeta}\n" +
                   $"  Abonado:     ${MontoAbonado:F2}\n" +
                   $"  Saldo:       ${SaldoRestante:F2}\n" +
                   $"--------------------------------------";
        }

    }
}
