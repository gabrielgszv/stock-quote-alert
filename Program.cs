using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

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

    public class StockService
    {
        private HttpClient httpClient = new HttpClient();

        public async Task<double> GetCurrentPrice(string ticker)
        {
            string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{ticker}.SA";

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozila/5.0");

            string json = await httpClient.GetStringAsync(url);

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                double price = doc.RootElement.GetProperty("chart").GetProperty("result")[0].GetProperty("meta").GetProperty("regularMarketPrice").GetDouble();    
                
                return price;
            }
            
        }
    }

    public class EmailService
    {
        public EmailSettings settings;

        public EmailService(EmailSettings setting)
        {
            settings = setting;
        }

        public void SendEmail(string ticker, double price, string action)
        {
            try
            {
                MailMessage email = new MailMessage(settings.SmtpUser, settings.DestinationEmail);

                email.Subject = $"Aviso de cotação: {ticker}";
                email.Body = $"O ativo {ticker} está com a cotação em R$ {price}.\nÉ aconselhado a {action} desse ativo.";

                SmtpClient smtp = new SmtpClient(settings.SmtpHost, settings.SmtpPort);

                smtp.Credentials = new NetworkCredential(settings.SmtpUser, settings.SmtpPass);
                smtp.EnableSsl = settings.EnableSsl;

                smtp.Send(email);

                smtp.Dispose();
                email.Dispose();

                Console.WriteLine($"Email enviado para {settings.DestinationEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
    }
    class Program
    {
        static async Task Main(string[] args)
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

            var StockService = new StockService();
            double preco = await StockService.GetCurrentPrice(ticker);

            Console.WriteLine($"Cotação atual de {ticker}: {preco}");

            var EmailService = new EmailService(config.EmailSettings);
            EmailService.SendEmail(ticker, preco, "teste");

        }
    }
}