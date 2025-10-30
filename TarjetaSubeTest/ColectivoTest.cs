using TarjetaSube;
using NUnit.Framework;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class ColectivoTest
    {
        private Colectivo colectivo;
        private Interurbano interurbano;
        private Tarjeta tarjeta;

        [SetUp]
        public void Setup()
        {
            colectivo = new Colectivo("120");
            interurbano = new Interurbano("Galvez");
            tarjeta = new Tarjeta();
        }

        #region Tests de Pago Exitoso

        [Test]
        public void PagarConSaldoSuficienteTest()
        {
            tarjeta.Cargar(2000);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(420, tarjeta.Saldo);
        }

        [Test]
        public void PagarMultiplesVecesTest()
        {
            tarjeta.Cargar(5000);
            
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto1);
            Assert.IsNotNull(boleto2);
            Assert.AreEqual(1840, tarjeta.Saldo);
        }

        [Test]
        public void PagarConSaldoJustoTest()
        {
            tarjeta.Cargar(2000);
            tarjeta.DescontarSaldo(420); // Dejar exactamente 1580
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        #endregion

        #region Tests de Pago Fallido

        [Test]
        public void PagarSinSaldoTest()
        {
            // Tarjeta sin cargar (saldo = 0)
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNull(boleto);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void PagarConSaldoInsuficienteTest()
        {
            tarjeta.Cargar(2000);
            tarjeta.DescontarSaldo(1000);
            
            Boleto boleto = colectivo.PagarCon(tarjeta);
            
            Assert.IsNotNull(boleto); // Queda en negativo
            Assert.AreEqual(-580, tarjeta.Saldo); // 2000 - 1000 - 1580 = -580
        }

        [Test]
        public void PagarConTarjetaNulaTest()
        {
            Boleto boleto = colectivo.PagarCon(null);
            Assert.IsNull(boleto);
        }

        #endregion

        #region Tests de Linea de Colectivo

        [Test]
        public void ColectivoTieneLineaTest()
        {
            Colectivo cole = new Colectivo("133");
            Assert.AreEqual("133", cole.Linea);
        }
        #endregion

        #region Test de interurbano
        [Test]
        public void Interurbano_TieneTarifaBasicaDe3000()
        {
            Assert.AreEqual(3000, interurbano.ObtenerTarifa());
        }

        [Test]
        public void PagarCon_SaldoSuficiente_GeneraBoletoCorrecto()
        {
            tarjeta.Cargar(5000);

            Boleto boleto = interurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(3000, boleto.MontoAbonado);
            Assert.AreEqual("Galvez", boleto.LineaColectivo);
        }

        [Test]
        public void PagarCon_SaldoInsuficiente_NoGeneraBoleto()
        {

            Boleto boleto = interurbano.PagarCon(tarjeta);

            Assert.IsNull(boleto);
        }

        [Test]
        public void PagarCon_Trasbordo_DeberiaSerGratis()
        {
            TiempoFalso tiempo = new TiempoFalso(2025, 10, 29);
            tiempo.AgregarMinutos(7 * 60);

            Tarjeta tarjetaAux = new Tarjeta (8000, tiempo);

            Boleto boleto = interurbano.PagarCon(tarjetaAux);

            Interurbano inter = new Interurbano("Arroyito");
            Boleto boleto1 = inter.PagarCon(tarjetaAux);

            Assert.IsNotNull(boleto);
            Assert.IsTrue(boleto1.EsTrasbordo);
            Assert.AreEqual(0, boleto1.MontoAbonado);
        }

        [Test]
        public void Interurbano_AceptaFranquicias_IgualQueUrbano()
        {
            tarjeta = new FranquiciaCompleta();
            interurbano = new Interurbano("Baigorria");

            Boleto boleto = interurbano.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, boleto.MontoAbonado);
        }

        #endregion
    }
}
