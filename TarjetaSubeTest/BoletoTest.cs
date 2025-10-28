using TarjetaSube;
using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class BoletoTest
    {
        private Tarjeta tarjeta;
        private FranquiciaParcial tarjetaParcial;
        private FranquiciaCompleta tarjetaCompleta;

        [SetUp]
        public void Setup()
        {
            tarjeta = new Tarjeta();
            tarjetaParcial = new FranquiciaParcial();
            tarjetaCompleta = new FranquiciaCompleta();
        }

        [Test]
        public void BoletoSeCreaCorrectamenteTest()
        {
            tarjeta.Cargar(2000);

            Boleto boleto = new Boleto(
                tarifa: 1580,
                lineaColectivo: "120",
                tarjeta: tarjeta
            );


            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.MontoAbonado);
            Assert.AreEqual("120", boleto.LineaColectivo);
            Assert.AreEqual(2000, boleto.SaldoRestante);
        }

        [Test]
        public void BoletoTieneFechaTest()
        {
            DateTime antes = DateTime.Now;
            Boleto boleto = new Boleto(1580, "120", tarjeta);
            DateTime despues = DateTime.Now;

            Assert.GreaterOrEqual(boleto.Fecha, antes);
            Assert.LessOrEqual(boleto.Fecha, despues);
        }

        [Test]
        public void BoletoContieneInformacionTest()
        {
            Boleto boleto = new Boleto(1580, "120", tarjeta);
            string texto = boleto.ToString();

            Assert.IsTrue(texto.Contains("120"));
            Assert.IsTrue(texto.Contains("1580"));
        }

        [Test]
        public void BoletoCreadoPorPagarConTest()
        {
            tarjeta.Cargar(5000);

            Colectivo colectivo = new Colectivo("133");
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("133", boleto.LineaColectivo);
            Assert.AreEqual(1580, boleto.MontoAbonado);
            Assert.AreEqual(3420, boleto.SaldoRestante);
        }

        [Test]
        public void BoletosSecuencia()
        {
            tarjeta.Cargar(10000);

            Colectivo colectivo = new Colectivo();
            
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Boleto boleto3 = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(8420, boleto1.SaldoRestante);
            Assert.AreEqual(6840, boleto2.SaldoRestante);
            Assert.AreEqual(5260, boleto3.SaldoRestante);
        }

        #region Tests de Property Id

        [Test]
        public void BoletoTieneIdUnicoTest()
        {
            tarjeta.Cargar(5000);

            Boleto boleto1 = new Boleto(1580, "120", tarjeta);
            Boleto boleto2 = new Boleto(1580, "120", tarjeta);

            // Verifica que los IDs sean diferentes y consecutivos
            Assert.AreNotEqual(boleto1.Id, boleto2.Id);
            Assert.Greater(boleto2.Id, boleto1.Id);
        }

        [Test]
        public void BoletoIdSeIncrementaCorrectamenteTest()
        {
            tarjeta.Cargar(10000);

            Boleto boleto1 = new Boleto(1580, "120", tarjeta);
            int idAnterior = boleto1.Id;

            Boleto boleto2 = new Boleto(1580, "120", tarjeta);

            // El segundo boleto debe tener ID = anterior + 1
            Assert.AreEqual(idAnterior + 1, boleto2.Id);
        }

        [Test]
        public void BoletoIdEsMayorQueCeroTest()
        {
            Boleto boleto = new Boleto(1580, "120", tarjeta);
            Assert.Greater(boleto.Id, 0);
        }

        #endregion

        #region Tests de Property TipoTarjeta

        [Test]
        public void BoletoConTarjetaNormal()
        {
            tarjeta.Cargar(2000);
            Boleto boleto = new Boleto(1580, "120", tarjeta);

            Assert.AreEqual("Tarjeta", boleto.TipoTarjeta);
        }

        [Test]
        public void BoletoConFranquiciaParcial()
        {
            tarjetaParcial.Cargar(2000);
            Boleto boleto = new Boleto(1580, "120", tarjetaParcial);

            Assert.AreEqual("FranquiciaParcial", boleto.TipoTarjeta);
        }

        [Test]
        public void BoletoConFranquiciaCompleta()
        {
            tarjetaCompleta.Cargar(2000);
            Boleto boleto = new Boleto(1580, "120", tarjetaCompleta);

            Assert.AreEqual("FranquiciaCompleta", boleto.TipoTarjeta);
        }

        #endregion

        #region Tests de ToString Completo

        [Test]
        public void BoletoToStringContieneIdTest()
        {
            Boleto boleto = new Boleto(1580, "120", tarjeta);
            string texto = boleto.ToString();

            Assert.IsTrue(texto.Contains(boleto.Id.ToString()));
        }

        [Test]
        public void BoletoToStringContieneTipoTarjetaTest()
        {
            tarjetaParcial.Cargar(2000);
            Boleto boleto = new Boleto(1580, "133", tarjetaParcial);
            string texto = boleto.ToString();

            Assert.IsTrue(texto.Contains("FranquiciaParcial"));
        }

        [Test]
        public void BoletoToStringContieneTodasLasPropertiesTest()
        {
            tarjeta.Cargar(5000);
            Boleto boleto = new Boleto(1580, "120", tarjeta);
            string texto = boleto.ToString();

            // Verifica que contenga las propiedades
            Assert.IsTrue(texto.Contains(boleto.Id.ToString()), "Falta Id");
            Assert.IsTrue(texto.Contains("120"), "Falta LineaColectivo");
            Assert.IsTrue(texto.Contains("Tarjeta"), "Falta TipoTarjeta");
            Assert.IsTrue(texto.Contains("1580"), "Falta MontoAbonado");
            Assert.IsTrue(texto.Contains("5000"), "Falta SaldoRestante");
        }

        #endregion

        #region Tests de Escenarios Diferentes

        [Test]
        public void BoletoConDiferentesLineasDeColectivoTest()
        {
            tarjeta.Cargar(5000);

            Boleto boleto1 = new Boleto(1580, "120", tarjeta);
            Boleto boleto2 = new Boleto(1580, "133", tarjeta);
            Boleto boleto3 = new Boleto(1580, "142", tarjeta);

            Assert.AreEqual("120", boleto1.LineaColectivo);
            Assert.AreEqual("133", boleto2.LineaColectivo);
            Assert.AreEqual("142", boleto3.LineaColectivo);
        }

        [Test]
        public void BoletoConDiferentesMontosTest()
        {
            tarjeta.Cargar(5000);

            Boleto boleto1 = new Boleto(1580, "120", tarjeta);
            Boleto boleto2 = new Boleto(790, "120", tarjeta); // Medio boleto

            Assert.AreEqual(1580, boleto1.MontoAbonado);
            Assert.AreEqual(790, boleto2.MontoAbonado);
        }


        [Test]
        public void BoletoConTarjetaDiferentesSaldosTest()
        {
            Tarjeta tarjeta1 = new Tarjeta();
            tarjeta1.Cargar(2000);

            Tarjeta tarjeta2 = new Tarjeta();
            tarjeta2.Cargar(10000);

            Boleto boleto1 = new Boleto(1580, "120", tarjeta1);
            Boleto boleto2 = new Boleto(1580, "120", tarjeta2);

            Assert.AreEqual(2000, boleto1.SaldoRestante);
            Assert.AreEqual(10000, boleto2.SaldoRestante);
        }

        #endregion

        #region Tests de Integracion con Colectivo

        [Test]
        public void BoletoCreadoPorColectivoTieneTodosLosDatosTest()
        {
            tarjeta.Cargar(5000);
            Colectivo colectivo = new Colectivo("142");

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.Greater(boleto.Id, 0);
            Assert.IsNotNull(boleto.Fecha);
            Assert.AreEqual("142", boleto.LineaColectivo);
            Assert.AreEqual("Tarjeta", boleto.TipoTarjeta);
            Assert.AreEqual(1580, boleto.MontoAbonado);
            Assert.AreEqual(3420, boleto.SaldoRestante);
        }

        [Test]
        public void BoletosDiferentesTarjetasTienenDiferentesTiposTest()
        {
            tarjeta.Cargar(5000);
            tarjetaParcial.Cargar(5000);
            tarjetaCompleta.Cargar(5000);

            Colectivo colectivo = new Colectivo();

            Boleto boletoNormal = colectivo.PagarCon(tarjeta);
            Boleto boletoParcial = colectivo.PagarCon(tarjetaParcial);
            Boleto boletoCompleto = colectivo.PagarCon(tarjetaCompleta);

            Assert.AreEqual("Tarjeta", boletoNormal.TipoTarjeta);
            Assert.AreEqual("FranquiciaParcial", boletoParcial.TipoTarjeta);
            Assert.AreEqual("FranquiciaCompleta", boletoCompleto.TipoTarjeta);
        }

        #endregion
    }
}
