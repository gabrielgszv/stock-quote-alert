using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.CompilerServices;
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

    public class TickerItem
    {
        public string Ticker {get; set;}
        public double SellPrice {get; set;}
        public double BuyPrice {get; set;}
    }

    public class StockService
    {
        private HttpClient httpClient = new HttpClient();
    
        // funcao para consultar a cotacao de uma acao usando o Yahoo Finance
        public async Task<double> GetCurrentPrice(string ticker)
        {
            string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{ticker}.SA";

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

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

        // funcao para mandar o email de alerta
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

                Console.WriteLine($"É aconselhado a {action} desse ativo");
                Console.WriteLine($"Email enviado para {settings.DestinationEmail}");
                Console.WriteLine("=========================================");
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

            // verificando a entrada
            if (args.Length < 3 || args.Length%3 != 0)
            {
                Console.WriteLine("A entrada tem que estar nesse formato: stock-quote-alert.exe <ATIVO1> <PRECO_VENDA1> <PRECO_COMPRA1> ... ");
                return;
            }

            // monitorando mais de uma acao
            List<TickerItem> tickers = new List<TickerItem>();

            for(int i = 0; i < args.Length; i+=3)
            {
                string ticker = args[i].ToUpper();

                if (!double.TryParse(args[i+1], out double sellPrice) || !double.TryParse(args[i+2], out double buyPrice))
                {
                    Console.WriteLine("Erro: Os preços devem ser valores numéricos válidos.");
                    return;
                }

                TickerItem item = new TickerItem{Ticker = ticker, SellPrice = sellPrice, BuyPrice= buyPrice};

                tickers.Add(item);
            }   

            // leitura do arquivo de configuracao
            string jsonText = File.ReadAllText("appsettings.json");
            AppConfig config = JsonSerializer.Deserialize<AppConfig>(jsonText);

            // monitoramento da acao
            var StockService = new StockService();
            var EmailService = new EmailService(config.EmailSettings);

            while (true)
            {
                foreach(var ticker in tickers)
                {
                    try
                    {
                        // preco atual da acao
                        double preco = await StockService.GetCurrentPrice(ticker.Ticker);
                        Console.WriteLine($"Horário: {DateTime.Now:HH:mm:ss}");
                        Console.WriteLine($"Cotação de {ticker.Ticker}: R$ {preco}");

                        if (preco >= ticker.SellPrice)
                        {
                            EmailService.SendEmail(ticker.Ticker, preco, "venda");
                        }
                        else if (preco <= ticker.BuyPrice)
                        {
                            EmailService.SendEmail(ticker.Ticker, preco, "compra");
                        }
                        else
                        {
                            Console.WriteLine("Cotação dentro do intervalo");
                            Console.WriteLine("=========================================");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro: {ex}");

                        await Task.Delay(10000); // intervalo de 10 segundos para tentar novamente
                    }    
                }

                Console.WriteLine("Intervalo para a próxima consulta");
                await Task.Delay(1800000); // intervalo de 30 minutos para a próxima consulta
                
            }

        }
    }
}