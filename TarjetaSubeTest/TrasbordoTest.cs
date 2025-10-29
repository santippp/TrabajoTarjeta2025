using TarjetaSube;
using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TrasbordoTest
    {
        private Tarjeta tarjeta;
        private TiempoFalso tiempo;
        private Colectivo colectivo120;
        private Colectivo colectivo133;

        [SetUp]
        public void Setup()
        {
            tiempo = new TiempoFalso(2024, 10, 14);
            tiempo.AgregarDias(0); // Lunes
            tiempo.AgregarMinutos(10 * 60); // 10:00 AM

            tarjeta = new Tarjeta(10000, tiempo);
            colectivo120 = new Colectivo("120");
            colectivo133 = new Colectivo("133");
        }

        #region Tests de Trasbordo Básico

        [Test]
        public void PrimerViajeNoEsTrasbordo()
        {
            Boleto boleto = colectivo120.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.IsFalse(boleto.EsTrasbordo);
            Assert.AreEqual(1580, boleto.MontoAbonado);
            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 1580
        }

        [Test]
        public void SegundoViajeEnLineaDiferenteEsTrasbordo()
        {
            // Viaje 1: Línea 120
            colectivo120.PagarCon(tarjeta);

            // Avanzar 10 minutos
            tiempo.AgregarMinutos(10);

            // Viaje 2: Línea 133 (diferente)
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsTrue(boleto2.EsTrasbordo); // ⭐ Es trasbordo
            Assert.AreEqual(0, boleto2.MontoAbonado); // ⭐ Gratis
            Assert.AreEqual(8420, tarjeta.Saldo); // No se descuenta
        }

        [Test]
        public void SegundoViajeEnMismaLineaNoEsTrasbordo()
        {
            // Viaje 1: Línea 120
            colectivo120.PagarCon(tarjeta);

            // Avanzar 10 minutos
            tiempo.AgregarMinutos(10);

            // Viaje 2: Línea 120 (misma línea)
            Boleto boleto2 = colectivo120.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.IsFalse(boleto2.EsTrasbordo); // NO es trasbordo
            Assert.AreEqual(1580, boleto2.MontoAbonado); // Cobra normal
            Assert.AreEqual(6840, tarjeta.Saldo); // 10000 - 1580 - 1580
        }

        #endregion

        #region Tests de Límite de 1 Hora

        [Test]
        public void TrasborAfter59MinutosEsValidoTest()
        {
            // Viaje 1: Línea 120
            colectivo120.PagarCon(tarjeta);

            // Avanzar 59 minutos (justo antes de la hora)
            tiempo.AgregarMinutos(59);

            // Viaje 2: Línea 133
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.AreEqual(0, boleto2.MontoAbonado);
        }

        [Test]
        public void TrasbordoDespuesDe1HoraNoEsValidoTest()
        {
            // Viaje 1: Línea 120
            colectivo120.PagarCon(tarjeta);

            // Avanzar 60 minutos (1 hora exacta)
            tiempo.AgregarMinutos(60);

            // Viaje 2: Línea 133
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.AreEqual(1580, boleto2.MontoAbonado); // Cobra normal
        }

        [Test]
        public void TrasbordoDespuesDe61MinutosNoEsValidoTest()
        {
            // Viaje 1
            colectivo120.PagarCon(tarjeta);

            // Avanzar 61 minutos
            tiempo.AgregarMinutos(61);

            // Viaje 2
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.AreEqual(1580, boleto2.MontoAbonado);
        }

        #endregion

        #region Tests de Horario (Lunes a Sábado, 7:00 - 22:00)

        [Test]
        public void TrasbordoALas7AMEsValidoTest()
        {
            // Configurar a las 7:00 AM del lunes
            tiempo.AgregarMinutos(7 * 60); // 7:00 AM
            tarjeta = new Tarjeta(10000, tiempo);

            colectivo120.PagarCon(tarjeta);
            tiempo.AgregarMinutos(10);
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void TrasbordoALas21_59EsValidoTest()
        {
            // Configurar a las 21:59
            tiempo.AgregarMinutos(21 * 60 + 59); // 21:59
            tarjeta = new Tarjeta(10000, tiempo);

            colectivo120.PagarCon(tarjeta);
            tiempo.AgregarMinutos(10);
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void TrasbordoALas22_00NoEsValidoTest()
        {
            // Configurar a las 22:00
            TiempoFalso tiempo1 = new TiempoFalso(2024, 10, 14);
            Tarjeta tarjeta1 = new Tarjeta(10000, tiempo1);
            tiempo1.AgregarMinutos(22 * 60); // 22:00

            colectivo120.PagarCon(tarjeta1);
            tiempo1.AgregarMinutos(10);
            Boleto boleto2 = colectivo133.PagarCon(tarjeta1);

            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.AreEqual(1580, boleto2.MontoAbonado);
        }

        [Test]
        public void TrasbordoALas6_59EsValidoTest()
        {
            // Configurar a las 6:59 AM
            TiempoFalso tiempo1 = new TiempoFalso(2024, 10, 14);
            tiempo1.AgregarMinutos(6 * 60 + 59); // 6:59 AM
            Tarjeta tarjeta1 = new Tarjeta(10000, tiempo1);

            colectivo120.PagarCon(tarjeta1);
            tiempo1.AgregarMinutos(10); // 7:09 AM - Si se puede ya
            Boleto boleto2 = colectivo133.PagarCon(tarjeta1);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        [Test]
        public void TrasbordoEnDomingoNoEsValidoTest()
        {
            // Configurar domingo a las 10:00 AM
            tiempo.AgregarDias(6); // Domingo (14/10 + 6 días = 20/10 domingo)
            tiempo.AgregarMinutos(10 * 60); // 10:00 AM
            tarjeta = new Tarjeta(10000, tiempo);

            colectivo120.PagarCon(tarjeta);
            tiempo.AgregarMinutos(10);
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            Assert.IsFalse(boleto2.EsTrasbordo);
            Assert.AreEqual(1580, boleto2.MontoAbonado);
        }

        [Test]
        public void TrasbordoEnSabadoEsValidoTest()
        {
            // Configurar sábado a las 10:00 AM
            tiempo.AgregarDias(5); // Sábado (14/10 + 5 días = 19/10 sábado)
            tiempo.AgregarMinutos(10 * 60); // 10:00 AM
            tarjeta = new Tarjeta(10000, tiempo);

            colectivo120.PagarCon(tarjeta);
            tiempo.AgregarMinutos(10);
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            Assert.IsTrue(boleto2.EsTrasbordo);
        }

        #endregion

        #region Tests de Trasbordos Múltiples

        [Test]
        public void TresTrasbordosSinLimiteTest()
        {
            // Viaje 1: Línea 120
            colectivo120.PagarCon(tarjeta);
            decimal saldoInicial = tarjeta.Saldo; // 8420

            tiempo.AgregarMinutos(10);

            // Viaje 2: Línea 133 (Trasbordo 1)
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);
            Assert.IsTrue(boleto2.EsTrasbordo);

            tiempo.AgregarMinutos(10);

            // Viaje 3: Línea 142 (Trasbordo 2 - desde la línea 133)
            Colectivo colectivo142 = new Colectivo("142");
            Boleto boleto3 = colectivo142.PagarCon(tarjeta);
            Assert.IsTrue(boleto3.EsTrasbordo);

            // Solo se cobró el primer viaje
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void CuatroTrasbordosDentroDeUnaHoraTest()
        {
            colectivo120.PagarCon(tarjeta); // Paga

            tiempo.AgregarMinutos(10);
            colectivo133.PagarCon(tarjeta); // Trasbordo

            tiempo.AgregarMinutos(10);
            Colectivo colectivo142 = new Colectivo("142");
            colectivo142.PagarCon(tarjeta); // Trasbordo

            tiempo.AgregarMinutos(10);
            Colectivo colectivo150 = new Colectivo("150");
            colectivo150.PagarCon(tarjeta); // Trasbordo

            // Solo se cobró el primer viaje
            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 1580
        }

        #endregion

        #region Tests con Diferentes Tipos de Tarjetas

        [Test]
        public void FranquiciaParcialPuedeTrasbordarTest()
        {
            FranquiciaParcial tarjetaFP = new FranquiciaParcial(10000, tiempo);

            // Viaje 1: Paga medio boleto (790)
            colectivo120.PagarCon(tarjetaFP);
            Assert.AreEqual(9210, tarjetaFP.Saldo);

            tiempo.AgregarMinutos(10);

            // Viaje 2: Trasbordo gratis
            Boleto boleto2 = colectivo133.PagarCon(tarjetaFP);
            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.AreEqual(9210, tarjetaFP.Saldo); // No se descuenta
        }

        [Test]
        public void FranquiciaCompletaPuedeTrasbordarTest()
        {
            FranquiciaCompleta tarjetaFC = new FranquiciaCompleta(10000, tiempo);

            // Viaje 1: Gratis
            colectivo120.PagarCon(tarjetaFC);
            Assert.AreEqual(10000, tarjetaFC.Saldo);

            tiempo.AgregarMinutos(10);

            // Viaje 2: Trasbordo gratis
            Boleto boleto2 = colectivo133.PagarCon(tarjetaFC);
            Assert.IsTrue(boleto2.EsTrasbordo);
            Assert.AreEqual(10000, tarjetaFC.Saldo);
        }

        #endregion

        #region Tests de ToString

        [Test]
        public void BoletoToStringIndicaTrasbordoTest()
        {
            colectivo120.PagarCon(tarjeta);
            tiempo.AgregarMinutos(10);
            Boleto boleto2 = colectivo133.PagarCon(tarjeta);

            string texto = boleto2.ToString();
            Assert.IsTrue(texto.Contains("[TRASBORDO]"));
        }

        [Test]
        public void BoletoNormalNoIndicaTrasbordoTest()
        {
            Boleto boleto = colectivo120.PagarCon(tarjeta);
            string texto = boleto.ToString();
            Assert.IsFalse(texto.Contains("[TRASBORDO]"));
        }

        #endregion
    }
}