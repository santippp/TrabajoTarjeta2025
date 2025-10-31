using TarjetaSube;
using NUnit.Framework;
using System;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class FranquiciaParcialTest
    {
        private FranquiciaParcial tarjeta;
        private TiempoFalso tiempo;
        private Colectivo colectivo;

        [SetUp]
        public void Setup()
        {
            //Nos va a permitir modificar el tiempo
            tiempo = new TiempoFalso(2024, 10, 14);
            tiempo.AgregarMinutos(6 * 60);

            tarjeta = new FranquiciaParcial(10000, tiempo);

            colectivo = new Colectivo();
        }

        #region Tests de Restricción de 5 Minutos

        [Test]
        public void PrimerViajeConMedioBoletoTest()
        {
            Boleto boleto = colectivo.PagarCon(tarjeta); // 1580/2 = 790

            Assert.IsNotNull(boleto);
            Assert.AreEqual(9210, tarjeta.Saldo); // 10000 - 790
            Assert.AreEqual(1, tarjeta.BoletosHoy);
            Assert.IsNotNull(tarjeta.UltimoViaje);
        }

        [Test]
        public void SegundoViajeAntesDe5MinutosPagaTarifaCompletaTest()
        {
            colectivo.PagarCon(tarjeta);
            decimal saldoDespuesPrimero = tarjeta.Saldo; // 9210

            // Avanzar solo 3 minutos
            tiempo.AgregarMinutos(3);

            // Viaje en menos de 5 minutos
            Boleto boleto2 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.AreEqual(7630, tarjeta.Saldo); // 9210 - 1580
            Assert.AreEqual(1, tarjeta.BoletosHoy); // Sigue en 1
        }

        [Test]
        public void SegundoViajeDespuesDe5MinutosPagaMedioBoletoTest()
        {
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(9210, tarjeta.Saldo);

            tiempo.AgregarMinutos(5);

            Boleto boleto2 = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto2);
            Assert.AreEqual(8420, tarjeta.Saldo); // 9210 - 790
            Assert.AreEqual(2, tarjeta.BoletosHoy); // Ahora son 2
        }

        [Test]
        public void ViajeA4MinutosPagaTarifaCompletaTest()
        {
            colectivo.PagarCon(tarjeta);

            tiempo.AgregarMinutos(4);

            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(7630, tarjeta.Saldo); // 10000 - 790 - 1580
        }

        [Test]
        public void ViajeA6MinutosPagaMedioBoletoTest()
        {
            colectivo.PagarCon(tarjeta);

            tiempo.AgregarMinutos(6);

            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 790 - 790
        }

        [Test]
        public void TresViajesCon5MinutosDeEsperaTest()
        {
            colectivo.PagarCon(tarjeta); // 10000 - 790 = 9210

            tiempo.AgregarMinutos(5);

            colectivo.PagarCon(tarjeta); // 9210 - 790 = 8420

            tiempo.AgregarMinutos(5);

            colectivo.PagarCon(tarjeta); // 8420 - 1580 = 6840

            Assert.AreEqual(6840, tarjeta.Saldo);
            Assert.AreEqual(2, tarjeta.BoletosHoy); // Solo cuenta los 2 primeros
        }

        #endregion

        #region Tests de Límite de 2 Viajes con Medio Boleto por Día

        [Test]
        public void PermiteDosViajesConMedioBoletoEnUnDiaTest()
        {
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(1, tarjeta.BoletosHoy);

            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta);
            Assert.AreEqual(2, tarjeta.BoletosHoy);

            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 790 x 2
        }

        [Test]
        public void TercerViajeDelDiaPagaTarifaCompletaTest()
        {
            colectivo.PagarCon(tarjeta); // -790
            tiempo.AgregarMinutos(5);

            colectivo.PagarCon(tarjeta); // -790
            tiempo.AgregarMinutos(5);

            colectivo.PagarCon(tarjeta); // -1580

            Assert.AreEqual(6840, tarjeta.Saldo); // 10000 - 790 - 790 - 1580
            Assert.AreEqual(2, tarjeta.BoletosHoy); // No incrementa en el tercero
        }

        [Test]
        public void CuartoViajeDelDiaTambienPagaTarifaCompletaTest()
        {
            colectivo.PagarCon(tarjeta);
            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta);

            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -1580

            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -1580

            Assert.AreEqual(5260, tarjeta.Saldo); // 10000 - 790 - 790 - 1580 - 1580
            Assert.AreEqual(2, tarjeta.BoletosHoy);
        }

        [Test]
        public void ContadorSeReiniciaAlCambiarDeDiaTest()
        {
            // Dia 1
            colectivo.PagarCon(tarjeta); // -790
            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -790
            Assert.AreEqual(2, tarjeta.BoletosHoy);
            Assert.AreEqual(8420, tarjeta.Saldo);

            tiempo.AgregarDias(1);

            // Dia 2
            colectivo.PagarCon(tarjeta);

            Assert.AreEqual(1, tarjeta.BoletosHoy); // Contador reiniciado
            Assert.AreEqual(7630, tarjeta.Saldo); // 8420 - 790
        }

        [Test]
        public void CuatroViajesEnDosDiasConMedioBoletoTest()
        {
            // Dia 1
            colectivo.PagarCon(tarjeta); // -790
            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -790

            tiempo.AgregarDias(1);

            // Dia 2
            colectivo.PagarCon(tarjeta); // -790
            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -790

            Assert.AreEqual(6840, tarjeta.Saldo); // 10000 - 4*790
            Assert.AreEqual(2, tarjeta.BoletosHoy); // Contador del dia 2
        }

        [Test]
        public void TresViajes_DosDias_VerificaReseteoTest()
        {
            // Dia 1
            colectivo.PagarCon(tarjeta); // -790
            Assert.AreEqual(1, tarjeta.BoletosHoy);

            // Dia 2
            tiempo.AgregarDias(1);
            colectivo.PagarCon(tarjeta); // -790
            Assert.AreEqual(1, tarjeta.BoletosHoy);

            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -790
            Assert.AreEqual(2, tarjeta.BoletosHoy);

            Assert.AreEqual(7630, tarjeta.Saldo); // 10000 - 3*790
        }

        #endregion

        #region Tests Combinados (5 minutos + Límite Diario)

        [Test]
        public void SegundoViajeRapidoYTerceroDespuesDe5MinutosTest()
        {
            colectivo.PagarCon(tarjeta); // -790

            tiempo.AgregarMinutos(2);
            colectivo.PagarCon(tarjeta); // -1580
            Assert.AreEqual(1, tarjeta.BoletosHoy); // Solo cuenta el primero

            tiempo.AgregarMinutos(6); // Total: 8 minutos desde el primero
            colectivo.PagarCon(tarjeta); // -790

            Assert.AreEqual(6840, tarjeta.Saldo); // 10000 - 790 - 1580 - 790
            Assert.AreEqual(2, tarjeta.BoletosHoy);
        }

        [Test]
        public void NoPermiteMasDe2MediosBoletoAunqueHayanPasado5MinutosTest()
        {
            colectivo.PagarCon(tarjeta); // -790
            tiempo.AgregarMinutos(5);

            colectivo.PagarCon(tarjeta); // -790
            tiempo.AgregarMinutos(5);

            colectivo.PagarCon(tarjeta); // -1580 (completo)

            Assert.AreEqual(6840, tarjeta.Saldo);
            Assert.AreEqual(2, tarjeta.BoletosHoy);
        }

        [Test]
        public void IntentoCincoViajesSeguidos_VerificaReglasTest()
        {
            colectivo.PagarCon(tarjeta); // -790

            tiempo.AgregarMinutos(2);
            colectivo.PagarCon(tarjeta); // -1580

            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -790

            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -1580

            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta); // -1580

            Assert.AreEqual(3680, tarjeta.Saldo); // 10000 - 790 - 1580 - 790 - 1580 - 1580
            Assert.AreEqual(2, tarjeta.BoletosHoy);
        }

        #endregion

        #region Tests de Registro de Último Viaje

        [Test]
        public void UltimoViajeSeRegistraCorrectamenteTest()
        {
            Assert.IsNull(tarjeta.UltimoViaje); // Al inicio no hay viajes

            colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(tarjeta.UltimoViaje);
            Assert.AreEqual(new DateTime(2024, 10, 14, 6, 0, 0), tarjeta.UltimoViaje.Value);
        }

        [Test]
        public void UltimoViajeSeActualizaEnCadaViajeConMedioBoletoTest()
        {
            colectivo.PagarCon(tarjeta);
            DateTime primerViaje = tarjeta.UltimoViaje.Value;

            tiempo.AgregarMinutos(5);
            colectivo.PagarCon(tarjeta);
            DateTime segundoViaje = tarjeta.UltimoViaje.Value;

            Assert.AreNotEqual(primerViaje, segundoViaje);
            Assert.Greater(segundoViaje, primerViaje);
        }

        [Test]
        public void UltimoViajeNOSeActualizaSiPagaTarifaCompletaTest()
        {
            colectivo.PagarCon(tarjeta);
            DateTime primerViaje = tarjeta.UltimoViaje.Value;

            tiempo.AgregarMinutos(2);
            colectivo.PagarCon(tarjeta);
            Assert.AreNotEqual(primerViaje, tiempo.Now());
        }

        #endregion

        #region Tests Edge Cases

        [Test]
        public void VariosDiasSeguidos_VerificaContadorTest()
        {
            for (int dia = 0; dia < 3; dia++)
            {
                colectivo.PagarCon(tarjeta);
                tiempo.AgregarMinutos(5);
                colectivo.PagarCon(tarjeta);

                Assert.AreEqual(2, tarjeta.BoletosHoy);

                tiempo.AgregarDias(1);
            }
            Assert.AreEqual(5260, tarjeta.Saldo); // 10000 - 4740
        }

        #endregion

        #region Tests de Restricción Horaria

        [Test]
        public void MedioBoletoALas6AMEsValidoTest()
        {
            // Lunes a las 6:00 AM
            tiempo.AgregarMinutos(6 * 60); // 6:00 AM
            tarjeta = new FranquiciaParcial(10000, tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            // Debe aplicar medio boleto
            Assert.AreEqual(9210, tarjeta.Saldo); // 10000 - 790
        }

        [Test]
        public void MedioBoletoALas21_59EsValidoTest()
        {
            // Lunes a las 21:59
            tiempo.AgregarMinutos(15 * 60 + 59); // 21:59
            tarjeta = new FranquiciaParcial(10000, tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(9210, tarjeta.Saldo); // Medio boleto
        }

        [Test]
        public void MedioBoletoALas5_59NoEsValidoTest()
        {
            TiempoFalso tiempo1 = new TiempoFalso(2024, 10, 14);
            tiempo1.AgregarMinutos(5 * 60 + 59); // 5:59 AM
            tarjeta = new FranquiciaParcial(10000, tiempo1);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            // Debe cobrar tarifa COMPLETA (no medio boleto)
            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 1580
        }

        [Test]
        public void MedioBoletoALas22_00NoEsValidoTest()
        {
            // Lunes a las 22:00 (fuera de horario)
            tiempo.AgregarMinutos(22 * 60); // 22:00
            tarjeta = new FranquiciaParcial(10000, tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            // Debe cobrar tarifa completa
            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 1580
        }

        [Test]
        public void MedioBoletoALas23_00NoEsValidoTest()
        {
            // Lunes a las 23:00 (fuera de horario)
            tiempo.AgregarMinutos(23 * 60); // 23:00
            tarjeta = new FranquiciaParcial(10000, tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(8420, tarjeta.Saldo); // Tarifa completa
        }

        [Test]
        public void MedioBoletoEnSabadoNoEsValidoTest()
        {
            // Sábado a las 10:00 AM
            tiempo.AgregarDias(5); // Sábado (14/10 + 5 = 19/10 sábado)
            tiempo.AgregarMinutos(10 * 60); // 10:00 AM
            tarjeta = new FranquiciaParcial(10000, tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            // Debe cobrar tarifa completa (no medio boleto en sábado)
            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 1580
        }

        [Test]
        public void MedioBoletoEnDomingoNoEsValidoTest()
        {
            // Domingo a las 10:00 AM
            tiempo.AgregarDias(6); // Domingo (14/10 + 6 = 20/10 domingo)
            tiempo.AgregarMinutos(4 * 60); // 10:00 AM
            tarjeta = new FranquiciaParcial(10000, tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            // Debe cobrar tarifa completa
            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 1580
        }

        [Test]
        public void MedioBoletoEnViernesEsValidoTest()
        {
            // Viernes a las 10:00 AM
            tiempo.AgregarDias(4); // Viernes (14/10 + 4 = 18/10 viernes)
            tiempo.AgregarMinutos(10 * 60); // 10:00 AM
            tarjeta = new FranquiciaParcial(10000, tiempo);

            Boleto boleto = colectivo.PagarCon(tarjeta);

            Assert.IsNotNull(boleto);
            Assert.AreEqual(9210, tarjeta.Saldo); // Medio boleto válido
        }

        [Test]
        public void TransicionHoraria_21_59_a_22_01Test()
        {
            // Primer viaje a las 21:59 (válido)
            tiempo.AgregarMinutos(15 * 60 + 59); // 21:59
            tarjeta = new FranquiciaParcial(10000, tiempo);

            Boleto boleto1 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(9210, tarjeta.Saldo); // Medio boleto

            // Avanzar a las 22:01 (inválido)
            tiempo.AgregarMinutos(5); // Ahora 22:04

            Boleto boleto2 = colectivo.PagarCon(tarjeta);
            Assert.AreEqual(7630, tarjeta.Saldo); // Tarifa completa (9210 - 1580)
        }

        #endregion
    }
}