using TarjetaSube;
using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class FranquiciaCompletaTest
    {
        private FranquiciaCompleta tarjeta;
        private TiempoFalso tiempo;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            tiempo = new TiempoFalso(2024, 10, 14);
            tiempo.AgregarMinutos(60 * 6);

            tarjeta = new FranquiciaCompleta(5000, tiempo);

            colectivo = new Colectivo();
        }

        #region Tests de Viajes Gratuitos

        [Test]
        public void PrimerViajeEsGratuitoTest()
        {
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(5000, tarjeta.Saldo);
            Assert.AreEqual(1, tarjeta.ViajesGratuitosHoy);
        }

        [Test]
        public void SegundoViajeEsGratuitoTest()
        {
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(5000, tarjeta.Saldo);

            Boleto boleto2 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.AreEqual(5000, tarjeta.Saldo);
            Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
        }

        [Test]
        public void PuedeHacerDosViajesGratuitosInmediatosTest()
        {
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(5000, tarjeta.Saldo);
            Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
        }

        [Test]
        public void TarjetaSinSaldoPuedeViajarGratisTest()
        {
            FranquiciaCompleta tarjetaSinSaldo = new FranquiciaCompleta(0, tiempo);

            Boleto boleto1 = colectivo.PagarCon(tarjetaSinSaldo);
            Boleto boleto2 = colectivo.PagarCon(tarjetaSinSaldo);

            Assert.IsNotNull(boleto1);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(0, tarjetaSinSaldo.Saldo);
            Assert.AreEqual(2, tarjetaSinSaldo.ViajesGratuitosHoy);
        }

        #endregion

        #region Tests de Tercer Viaje (Tarifa Completa)

        [Test]
        public void TercerViajeCobraTarifaCompletaTest()
        {
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);

            Boleto boleto3 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto3);
            Assert.AreEqual(3420, tarjeta.Saldo); // 5000 - 1580
            Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
        }

        [Test]
        public void CuartoViajeCobraTarifaCompletaTest()
        {
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);

            colectivo.PagarCon(tarjeta); // -1580

            Boleto boleto4 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto4);
            Assert.AreEqual(1840, tarjeta.Saldo); // 5000 - 1580 - 1580
            Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
        }

        [Test]
        public void CincoViajesEnUnDiaTest()
        {
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta); 

            colectivo.PagarCon(tarjeta); // 3420
            colectivo.PagarCon(tarjeta); // 1840
            colectivo.PagarCon(tarjeta); // 260

            Assert.AreEqual(260, tarjeta.Saldo); // 5000 - 3*1580
            Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
        }

        [Test]
        public void TercerViajeRequiereSaldoSuficienteTest()
        {
            FranquiciaCompleta tarjetaPoca = new FranquiciaCompleta(1000, tiempo);

            colectivo.PagarCon(tarjetaPoca);
            colectivo.PagarCon(tarjetaPoca);
            Assert.AreEqual(1000, tarjetaPoca.Saldo);


            Boleto boleto3 = colectivo.PagarCon(tarjetaPoca);

            Assert.IsNotNull(boleto3);
            Assert.AreEqual(-580, tarjetaPoca.Saldo); // 1000 - 1580
        }

        [Test]
        public void TercerViajeNoPermitidoSiSuperaSaldoNegativoTest()
        {
            FranquiciaCompleta tarjetaPoca = new FranquiciaCompleta(200, tiempo);

            colectivo.PagarCon(tarjetaPoca);
            colectivo.PagarCon(tarjetaPoca);

            Boleto boleto3 = colectivo.PagarCon(tarjetaPoca);

            Assert.IsNull(boleto3);
            Assert.AreEqual(200, tarjetaPoca.Saldo);
        }

        #endregion

        #region Tests de Reseteo Diario

        [Test]
        public void ContadorSeReiniciaAlCambiarDeDiaTest()
        {
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
            Assert.AreEqual(5000, tarjeta.Saldo);

            tiempo.AgregarDias(1);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(1, tarjeta.ViajesGratuitosHoy);
            Assert.AreEqual(5000, tarjeta.Saldo);
        }

        [Test]
        public void CuatroViajesGratuitosEnDosDiasTest()
        {
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);

            tiempo.AgregarDias(1);

            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(5000, tarjeta.Saldo);
            Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
        }

        [Test]
        public void TresViajesPorDiaDuranteDosDiasTest()
        {
            colectivo.PagarCon(tarjeta); // Gratis
            colectivo.PagarCon(tarjeta); // Gratis
            colectivo.PagarCon(tarjeta); // -1580
            Assert.AreEqual(3420, tarjeta.Saldo);

            tiempo.AgregarDias(1);
            colectivo.PagarCon(tarjeta); // Gratis
            colectivo.PagarCon(tarjeta); // Gratis
            colectivo.PagarCon(tarjeta); // -1580

            Assert.AreEqual(1840, tarjeta.Saldo); // 5000 - 1580 - 1580
            Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
        }

        [Test]
        public void VariosDiasSeguidos_VerificaContadorTest()
        {
            for (int dia = 0; dia < 3; dia++)
            {
                colectivo.PagarCon(tarjeta);
                colectivo.PagarCon(tarjeta);

                Assert.AreEqual(2, tarjeta.ViajesGratuitosHoy);
                Assert.AreEqual(5000, tarjeta.Saldo);

                tiempo.AgregarDias(1);
            }

            Assert.AreEqual(5000, tarjeta.Saldo);
        }

        #endregion

        #region Tests de Boleto Generado

        [Test]
        public void BoletoDeViajeGratuitoTieneTipoCorrectoTest()
        {
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("FranquiciaCompleta", boleto.TipoTarjeta);
        }

        [Test]
        public void BoletoDeViajeGratuitoMuestraSaldoCorrectoTest()
        {
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(5000, boleto.SaldoRestante); 
        }

        [Test]
        public void BoletoDelTercerViajeMuestraSaldoDescontadoTest()
        {
            // Dos gratuitos
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);

            // Tercero pagado
            Boleto boleto3 = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(3420, boleto3.SaldoRestante); // 5000 - 1580
        }

        #endregion

        #region Tests Edge Cases

        [Test]
        public void DiezViajesEnCincoDiasTest()
        {
            for (int dia = 0; dia < 5; dia++)
            {
                colectivo.PagarCon(tarjeta);
                colectivo.PagarCon(tarjeta);

                if (dia < 4) tiempo.AgregarDias(1);
            }

            Assert.AreEqual(5000, tarjeta.Saldo);
        }

        #endregion

        #region Tests de Comparación con Tarjeta Normal

        [Test]
        public void ComparacionConTarjetaNormalTest()
        {
            Tarjeta tarjetaNormal = new Tarjeta(5000);


            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);
            colectivo.PagarCon(tarjeta);


            colectivo.PagarCon(tarjetaNormal);
            colectivo.PagarCon(tarjetaNormal);
            colectivo.PagarCon(tarjetaNormal);

            Assert.AreEqual(3420, tarjeta.Saldo); // FranquiciaCompleta
            Assert.AreEqual(260, tarjetaNormal.Saldo); // Normal
            Assert.Greater(tarjeta.Saldo, tarjetaNormal.Saldo);
        }

        #endregion
    }
}