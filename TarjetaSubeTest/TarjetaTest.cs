using TarjetaSube;
using NUnit.Framework;

namespace TarjetaSubeTest
{
    [TestFixture]
    public class TarjetaTest
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

        #region Tests de Carga de Saldo

        [Test]
        public void CargarMonto2000Test()
        {
            bool resultado = tarjeta.Cargar(2000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(2000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto3000Test()
        {
            bool resultado = tarjeta.Cargar(3000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(3000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto4000Test()
        {
            bool resultado = tarjeta.Cargar(4000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(4000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto5000Test()
        {
            bool resultado = tarjeta.Cargar(5000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(5000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto8000Test()
        {
            bool resultado = tarjeta.Cargar(8000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(8000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto10000Test()
        {
            bool resultado = tarjeta.Cargar(10000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(10000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto15000Test()
        {
            bool resultado = tarjeta.Cargar(15000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(15000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto20000Test()
        {
            bool resultado = tarjeta.Cargar(20000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(20000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto25000Test()
        {
            bool resultado = tarjeta.Cargar(25000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(25000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMonto30000Test()
        {
            bool resultado = tarjeta.Cargar(30000);
            Assert.IsTrue(resultado);
            Assert.AreEqual(30000, tarjeta.Saldo);
        }

        [Test]
        public void CargarMontoInvalidoTest()
        {
            // Intenta cargar un monto no valido
            bool resultado = tarjeta.Cargar(1000);
            Assert.IsFalse(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void CargarMontosMultiplesTest()
        {
            tarjeta.Cargar(5000);
            tarjeta.Cargar(3000);
            Assert.AreEqual(8000, tarjeta.Saldo);
        }

        [Test]
        public void CargarSaldoDespDeDeuda()
        {
            tarjeta.DescontarSaldo(1200);
            tarjeta.Cargar(2000);

            Assert.AreEqual(800, tarjeta.Saldo);
        }

        #endregion

        #region Tests de Límite de Saldo

        [Test]
        public void NoSuperaLimiteDeSaldoTest()
        {
            // Cargar hasta casi el límite
            tarjeta.Cargar(30000);
            tarjeta.Cargar(8000);
            
            // Intentar cargar más (debería llegar al límite de 40000)
            tarjeta.Cargar(5000);
            
            Assert.AreEqual(40000, tarjeta.Saldo);
        }

        [Test]
        public void LimiteExactoTest()
        {
            tarjeta.Cargar(20000);
            tarjeta.Cargar(20000);
            Assert.AreEqual(40000, tarjeta.Saldo);
        }

        [Test]
        public void TarjetaConSaldoInicialTest()
        {
            Tarjeta tarjetaConSaldo = new Tarjeta(5000);
            Assert.AreEqual(5000, tarjetaConSaldo.Saldo);
        }

        #endregion

        #region Tests de Descuento de Saldo

        [Test]
        public void DescontarSaldoExitosoTest()
        {
            tarjeta.Cargar(5000);
            bool resultado = tarjeta.DescontarSaldo(1580);
            
            Assert.IsTrue(resultado);
            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void DescontarSaldoInsuficienteTest()
        {
            bool resultado = tarjeta.DescontarSaldo(1000); // Saldo inicial = 0
            Assert.IsTrue(resultado);
            Assert.AreEqual(-1000, tarjeta.Saldo);
        }

        [Test]
        public void DescontarTodoElSaldoTest()
        {
            tarjeta.Cargar(2000);
            tarjeta.DescontarSaldo(2000);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void NoPuedeSuperarLimiteNegativoTest()
        {
            tarjeta.DescontarSaldo(1200);
            bool resultado = tarjeta.DescontarSaldo(100);
            Assert.IsFalse(resultado);
            Assert.AreEqual(-1200, tarjeta.Saldo);
        }

        #endregion

        #region Franquicias especiales
        [Test]
        public void MedioBoletoPagaMitadDelValorTest()
        {
            tarjetaParcial.Cargar(2000);
            tarjetaParcial.DescontarSaldo(1580);
            Assert.AreEqual(1210, tarjetaParcial.Saldo);
        }

        [Test]
        public void FranquiciaCompletaSiemprePuedePagarTest()
        {
            bool resultado1 = tarjetaCompleta.DescontarSaldo(1580);
            bool resultado2 = tarjetaCompleta.DescontarSaldo(1580);

            Assert.IsTrue(resultado1);
            Assert.IsTrue(resultado2);
            Assert.AreEqual(0, tarjetaCompleta.Saldo);
        }

        #endregion
    }
}
