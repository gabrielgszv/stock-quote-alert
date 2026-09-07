using System;
using System.Globalization;

namespace StockQuoteAlert
{
    class Program
    {
        static void Main(string[] args)
        {

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            // Lendo os 3 valores
            if (args.Length != 3)
            {
                Console.WriteLine("A entrada tem que estar nesse formato: stock-quote-alert.exe <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>");
                return;
            }

            string ticker = args[0].ToUpper();

            // Garantindo que os precos sejam valores numericos
            if (!double.TryParse(args[1], out double sellPrice) || !double.TryParse(args[2], out double buyPrice))
            {
                Console.WriteLine("Erro: Os preços devem ser valores numéricos válidos.");
                return;
            }

            Console.WriteLine($"Ativo: {ticker}");
            Console.WriteLine($"Preco de venda: {sellPrice}");
            Console.WriteLine($"Preço de compra: {buyPrice}");
        }
    }
}