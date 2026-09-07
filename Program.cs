using System;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace StockQuoteAlert
{
    public class AppConfig
    {
        public EmailSettings EmailSettings {get; set;}
    }

    public class EmailSettings
    {
        public string DestinationEmail {get; set;}
        public string SmtpHost {get; set;}
        public int SmtpPort {get; set;}
        public string SmtpUser {get; set;}
        public string SmtpPass {get; set;}
        public bool EnableSsl {get; set;}
    }
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

            string jsonText = File.ReadAllText("appsettings.json");
            AppConfig config = JsonSerializer.Deserialize<AppConfig>(jsonText);

            Console.WriteLine($"E-mail de destino: {config.EmailSettings.DestinationEmail}");

        }
    }
}