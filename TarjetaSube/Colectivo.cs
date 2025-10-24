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
        //Devuelve el boleto si se pudo, de lo contrario null
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
                monto: TARIFA_BASICA,
                lineaColectivo: Linea,
                saldoRestante: tarjeta.Saldo
            );

            return boleto;
        }

    }
}
