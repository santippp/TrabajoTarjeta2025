using System;

namespace TarjetaSube
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Crea una tarjeta nueva
            Tarjeta miTarjeta = new Tarjeta();
            Console.WriteLine($"Tarjeta creada. su saldo es: ${miTarjeta.Saldo}");

            // Carga saldo
            Console.WriteLine("\nCargando saldo...");
            if (miTarjeta.Cargar(5000))
            {
                Console.WriteLine($"Se cargo exitosamente, el saldo nuevo es: ${miTarjeta.Saldo}");
            }

            // Crea un colectivo
            Colectivo colectivo120 = new Colectivo("120");
            Console.WriteLine($"\nColectivo línea {colectivo120.Linea}");

            // Realiza viajes
            Console.WriteLine("\nRealizando viajes...");
            
            Boleto boleto1 = colectivo120.PagarCon(miTarjeta);
            if (boleto1 != null)
            {
                Console.WriteLine($"Viaje 1:");
                Console.WriteLine($"  {boleto1}");
            }

            Boleto boleto2 = colectivo120.PagarCon(miTarjeta);
            if (boleto2 != null)
            {
                Console.WriteLine($"\nViaje 2:");
                Console.WriteLine($"  {boleto2}");
            }

            Boleto boleto3 = colectivo120.PagarCon(miTarjeta);
            if (boleto3 != null)
            {
                Console.WriteLine($"\nViaje 3:");
                Console.WriteLine($"  {boleto3}");
            }

            // Intenta viajar sin saldo
            Boleto boletoFallido = colectivo120.PagarCon(miTarjeta);
            if (boletoFallido == null)
            {
                Console.WriteLine($"\n No se pudo pagar, el saldo es insuficiente: ${miTarjeta.Saldo}");
            }

        }
    }
}
