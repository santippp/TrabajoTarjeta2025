using TarjetaSube;
using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class UsoFrecuenteTest
    {
        private Tarjeta tarjeta;
        private TiempoFalso tiempo;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            // Iniciar en 1 de enero 2024, 10:00 AM
            tiempo = new TiempoFalso(2024, 1, 1);
            tiempo.AgregarMinutos(10 * 60); // 10:00 AM

            tarjeta = new Tarjeta(50000, tiempo);
            colectivo = new Colectivo("120");
        }

        #region Tests de Viajes 1-29 (Sin Descuento)

        [Test]
        public void PrimerViajeDelMesSinDescuentoTest()
        {
            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(48420, tarjeta.Saldo); // 50000 - 1580
            Assert.AreEqual(1, tarjeta.BoletosMes);
        }

        [Test]
        public void Viaje29SinDescuentoTest()
        {
            // Hacer 29 viajes
            for (int i = 0; i < 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            // Todos a tarifa normal: 29 * 1580 = 45820
            Assert.AreEqual(4180, tarjeta.Saldo); // 50000 - 45820
            Assert.AreEqual(29, tarjeta.BoletosMes);
        }

        #endregion

        #region Tests de Viajes 30-59 (20% Descuento)

        [Test]
        public void Viaje30Con20PorCientoDescuentoTest()
        {
            // Hacer 29 viajes sin descuento
            for (int i = 0; i < 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }
            decimal saldoAntes = tarjeta.Saldo; // 4180

            // Viaje 30: con 20% descuento
            // 1580 * 0.80 = 1264
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(2916, tarjeta.Saldo); // 4180 - 1264
            Assert.AreEqual(30, tarjeta.BoletosMes);
        }

        [Test]
        public void Viaje35Con20PorCientoDescuentoTest()
        {
            // Hacer 30 viajes
            for (int i = 0; i < 30; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            decimal saldoAntes = tarjeta.Saldo;

            // Viaje 31: con 20% descuento
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(saldoAntes - 1264, tarjeta.Saldo); // -1264 (1580 * 0.80)
        }

        [Test]
        public void Viaje59Con20PorCientoDescuentoTest()
        {
            // Hacer 30 viajes
            for (int i = 0; i < 30; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);

            // Hacer 28 viajes
            for (int i = 0; i < 28; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            decimal saldoAntes = tarjeta.Saldo;

            // Viaje 59: todavía con 20% descuento
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(59, tarjeta.BoletosMes);
            Assert.AreEqual(saldoAntes - 1264, tarjeta.Saldo);
        }

        [Test]
        public void Viajes30a59TodosConDescuento20Test()
        {
            tarjeta = new Tarjeta(100000, tiempo); // Más saldo para esta prueba

            // Viajes 1-29: sin descuento
            for (int i = 0; i < 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }
            // Gastado: 29 * 1580 = 45820
            // Saldo: 100000 - 45820 = 54180

            // Viajes 30-59: con 20% descuento (30 viajes)
            for (int i = 0; i < 30; i++)
            {
                colectivo.PagarCon(tarjeta);
            }
            // Gastado adicional: 30 * 1264 = 37920
            // Saldo final: 54180 - 37920 = 16260

            Assert.AreEqual(16260, tarjeta.Saldo);
            Assert.AreEqual(59, tarjeta.BoletosMes);
        }

        #endregion

        #region Tests de Viajes 60-79 (25% Descuento)

        [Test]
        public void Viaje60Con25PorCientoDescuentoTest()
        {
            tarjeta = new Tarjeta(100000, tiempo);

            // Hacer 59 viajes
            for (int i = 0; i < 59; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            decimal saldoAntes = tarjeta.Saldo;

            // Viaje 60: con 25% descuento
            // 1580 * 0.75 = 1185
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(saldoAntes - 1185, tarjeta.Saldo);
            Assert.AreEqual(60, tarjeta.BoletosMes);
        }

        [Test]
        public void Viaje70Con25PorCientoDescuentoTest()
        {
            tarjeta = new Tarjeta(150000, tiempo);

            // Hacer 69 viajes
            for (int i = 0; i < 69; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            decimal saldoAntes = tarjeta.Saldo;

            // Viaje 70: con 25% descuento
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(saldoAntes - 1185, tarjeta.Saldo);
        }

        [Test]
        public void Viaje79Con25PorCientoDescuentoTest()
        {
            tarjeta = new Tarjeta(150000, tiempo);

            // Hacer 78 viajes
            for (int i = 0; i < 78; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            decimal saldoAntes = tarjeta.Saldo;

            // Viaje 79: todavía con 25% descuento
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(saldoAntes - 1185, tarjeta.Saldo);
            Assert.AreEqual(79, tarjeta.BoletosMes);
        }

        #endregion

        #region Tests de Viaje 80+ (Sin Descuento)

        [Test]
        public void Viaje81SinDescuentoTest()
        {
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);

            // Hacer 79 viajes
            for (int i = 0; i < 80; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            decimal saldoAntes = tarjeta.Saldo;

            // Viaje 80: vuelve a tarifa normal
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(saldoAntes - 1580, tarjeta.Saldo); // Tarifa completa
            Assert.AreEqual(81, tarjeta.BoletosMes);
        }

        [Test]
        public void Viaje85SinDescuentoTest()
        {
            tarjeta = new Tarjeta(200000, tiempo);

            // Hacer 84 viajes
            for (int i = 0; i < 84; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            decimal saldoAntes = tarjeta.Saldo;

            // Viaje 85: tarifa normal
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(saldoAntes - 1580, tarjeta.Saldo);
        }

        #endregion

        #region Tests de Cambio de Mes

        [Test]
        public void ContadorSeReiniciaAlCambiarDeMesTest()
        {
            // Hacer 30 viajes en enero
            for (int i = 0; i < 30; i++)
            {
                colectivo.PagarCon(tarjeta);
            }
            Assert.AreEqual(30, tarjeta.BoletosMes);

            // Cambiar a febrero
            tiempo.AgregarDias(31); // 1 de febrero

            // Primer viaje de febrero - tarifa completa
            decimal saldoAntes = tarjeta.Saldo;
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(1, tarjeta.BoletosMes); // Contador reseteado
            Assert.AreEqual(saldoAntes - 1580, tarjeta.Saldo); // Sin descuento
        }

        [Test]
        public void TransicionEnero31aFebrero1Test()
        {
            // Hacer 30 viajes en enero
            for (int i = 0; i < 30; i++)
            {
                colectivo.PagarCon(tarjeta);
            }
            Assert.AreEqual(30, tarjeta.BoletosMes);

            // Cambiar a febrero
            tiempo.AgregarDias(31);

            tarjeta.Cargar(30000);
            tarjeta.Cargar(20000);

            // Primeros 29 viajes de febrero sin descuento
            for (int i = 0; i < 29; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            decimal saldoAntes = tarjeta.Saldo;

            // Viaje 30 de febrero: con 20% descuento
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(30, tarjeta.BoletosMes);
            Assert.AreEqual(saldoAntes - 1264, tarjeta.Saldo); // 20% descuento
        }

        [Test]
        public void CambioDeDiciembreaEneroTest()
        {
            // Configurar diciembre
            tiempo = new TiempoFalso(2024, 12, 1);
            tiempo.AgregarMinutos(10 * 60);
            tarjeta = new Tarjeta(200000, tiempo);

            // Hacer 60 viajes en diciembre
            for (int i = 0; i < 60; i++)
            {
                colectivo.PagarCon(tarjeta);
            }
            Assert.AreEqual(60, tarjeta.BoletosMes);

            // Cambiar a enero del año siguiente
            tiempo.AgregarDias(31); // 1 de enero 2025

            decimal saldoAntes = tarjeta.Saldo;
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(1, tarjeta.BoletosMes); // Reseteado
            Assert.AreEqual(saldoAntes - 1580, tarjeta.Saldo); // Sin descuento
        }

        #endregion

        #region Tests de Franquicias (NO aplica uso frecuente)

        [Test]
        public void FranquiciaParcialNoAplicaUsoFrecuenteTest()
        {
            FranquiciaParcial franquicia = new FranquiciaParcial(50000, tiempo);

            // Hacer 30 viajes
            for (int i = 0; i < 30; i++)
            {
                colectivo.PagarCon(franquicia);
            }

            decimal saldoAntes = franquicia.Saldo;

            // Viaje 31: debería cobrar medio boleto sin descuento adicional
            tiempo.AgregarDias(1); // Nuevo día para resetear contador diario
            tiempo.SetInicioDia();
            tiempo.AgregarMinutos(10 * 60);
            colectivo.PagarCon(franquicia);

            // Medio boleto: 1580 / 2 = 790 (SIN descuento de uso frecuente)
            Assert.AreEqual(saldoAntes - 790, franquicia.Saldo);
        }

        [Test]
        public void FranquiciaCompletaNoAplicaUsoFrecuenteTest()
        {
            FranquiciaCompleta franquicia = new FranquiciaCompleta(50000, tiempo);

            // Hacer 40 viajes
            for (int i = 0; i < 20; i++) // 20 días, 2 viajes por día
            {
                tiempo.AgregarDias(1);
                colectivo.PagarCon(franquicia);
                colectivo.PagarCon(franquicia);
            }

            decimal saldoAntes = franquicia.Saldo;

            // Tercer viaje del día (debería cobrar completo)
            tiempo.AgregarDias(1);
            colectivo.PagarCon(franquicia); // Gratis
            colectivo.PagarCon(franquicia); // Gratis
            colectivo.PagarCon(franquicia); // Completo

            // Cobra 1580 SIN descuento de uso frecuente
            Assert.AreEqual(saldoAntes - 1580, franquicia.Saldo);
        }

        #endregion

        #region Tests de Cálculo Completo

        [Test]
        public void CalculoCompletoDe100ViajesEnUnMesTest()
        {
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            tarjeta.Cargar(30000);
            //Saldo total: 140000

            // Viajes 1-29: 29 * 1580 = 45820
            // Viajes 30-59: 30 * 1264 = 37920
            // Viajes 60-80: 21 * 1185 = 24885
            // Viajes 81-100: 20 * 1580 = 31600
            // Total: 140225

            for (int i = 0; i < 100; i++)
            {
                colectivo.PagarCon(tarjeta);
            }

            Assert.AreEqual(-225, tarjeta.Saldo); // 140000 - 140225
            Assert.AreEqual(100, tarjeta.BoletosMes);
        }

        #endregion
    }
}
