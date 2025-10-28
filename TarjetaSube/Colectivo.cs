using System;

namespace TarjetaSube
{
    public class Colectivo
    {
        public string Linea { get; private set; }
        private const decimal TARIFA_BASICA = 1580;

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

            // Intenta descontar saldo
            bool pagoExitoso = tarjeta.DescontarSaldo(TARIFA_BASICA);

            if (!pagoExitoso)
            {
                // No hay saldo suficiente
                return null;
            }

            // Crear y devolver el boleto
            Boleto boleto = new Boleto(
                tarifa: TARIFA_BASICA,
                lineaColectivo: Linea,
                tarjeta: tarjeta
            );

            return boleto;
        }

        public decimal ObtenerTarifa()
        {
            return TARIFA_BASICA;
        }
    }
}
