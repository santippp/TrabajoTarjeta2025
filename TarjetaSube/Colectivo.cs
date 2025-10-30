using System;

namespace TarjetaSube
{
    public class Colectivo
    {
        public string Linea { get; private set; }
        protected virtual decimal TarifaBasica => 1580;


        public Colectivo(string linea = "120")
        {
            Linea = linea;
        }

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null)
            {
                return null;
            }

            bool esTrasbordo = tarjeta.PuedeHacerTrasbordo(Linea);
            decimal montoACobrar = esTrasbordo ? 0 : TarifaBasica;

            // Intenta descontar saldo
            decimal saldoAntes = tarjeta.Saldo;
            bool pagoExitoso = tarjeta.DescontarSaldo(montoACobrar);

            if (!pagoExitoso)
            {
                // No hay saldo suficiente
                return null;
            }

            decimal saldoDespues = tarjeta.Saldo;
            decimal montoRealCobrado = saldoAntes - saldoDespues;

            // Crear y devolver el boleto
            Boleto boleto = new Boleto(
                tarifa: montoRealCobrado,
                lineaColectivo: Linea,
                tarjeta: tarjeta,
                esTrasbordo: esTrasbordo
            );

            tarjeta.RegistrarBoleto(boleto);

            return boleto;
        }

        public decimal ObtenerTarifa()
        {
            return TarifaBasica;
        }
    }

    public class Interurbano : Colectivo
    {
        protected override decimal TarifaBasica => 3000;
        public Interurbano(string linea) : base(linea) { }


    }
}
