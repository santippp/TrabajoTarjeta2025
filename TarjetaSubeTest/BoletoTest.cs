using TarjetaSube;
using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class BoletoTest
    {
        [Test]
        public void BoletoSeCreaCorrectamenteTest()
        {
            Boleto boleto = new Boleto(
                monto: 1580,
                lineaColectivo: "120",
                saldoRestante: 420
            );

            Assert.IsNotNull(boleto);
            Assert.AreEqual(1580, boleto.Monto);
            Assert.AreEqual("120", boleto.LineaColectivo);
            Assert.AreEqual(420, boleto.SaldoRestante);
        }

        [Test]
        public void BoletoTieneFechaTest()
        {
            DateTime antes = DateTime.Now;
            Boleto boleto = new Boleto(1580, "120", 420);
            DateTime despues = DateTime.Now;

            Assert.GreaterOrEqual(boleto.Fecha, antes);
            Assert.LessOrEqual(boleto.Fecha, despues);
        }

        [Test]
        public void BoletoContieneInformacionTest()
        {
            Boleto boleto = new Boleto(1580, "120", 420);
            string texto = boleto.ToString();

            Assert.IsTrue(texto.Contains("120"));
            Assert.IsTrue(texto.Contains("1580"));
            Assert.IsTrue(texto.Contains("420"));
        }

        [Test]
        public void BoletoCreadoPorPagarConTest()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(5000);

            Colectivo colectivo = new Colectivo("133");
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual("133", boleto.LineaColectivo);
            Assert.AreEqual(1580, boleto.Monto);
            Assert.AreEqual(3420, boleto.SaldoRestante);
        }

        [Test]
        public void BoletosSecuencia()
        {
            Tarjeta tarjeta = new Tarjeta();
            tarjeta.Cargar(10000);

            Colectivo colectivo = new Colectivo();
            
            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Boleto boleto3 = colectivo.PagarCon(tarjeta);

            Assert.AreEqual(8420, boleto1.SaldoRestante);
            Assert.AreEqual(6840, boleto2.SaldoRestante);
            Assert.AreEqual(5260, boleto3.SaldoRestante);
        }
    }
}
