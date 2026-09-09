# Stock Quote Alert 

Aplicação console em C# .NET que monitora cotações de ativos da B3 em tempo real via Yahoo Finance e dispara alertas por email quando os preços atingem os limites de compra ou venda definidos pelo usuário.


## Pré-requisitos

* .NET SDK
* Conta SMTP para o envio de emails

## Configuração

Antes de executar, é necessário editar o arquivo `appsettings.json` com as credenciais SMTP.

```json
{
  "EmailSettings": {
    "DestinationEmail": "email-destino@exemplo.com",
    "SmtpHost": "smtp.exemplo.com",
    "SmtpPort": 587,
    "SmtpUser": "email-remetente@exemplo.com",
    "SmtpPass": "senha-ou-token",
    "EnableSsl": true
  }
}
```

## Como Executar

Para rodar é preciso passar 3 parâmetros no terminal: Ativo, Preço de Venda e Preço de Compra.

**Exemplos de uso:**
* Via codigo fonte: `dotnet run -- PETR4 22.67 22.59`
* Via executável: `stock-quote-alert.exe PETR4 22.67 22.59`

<br>

Após a inicialização, o sistema monitora o ativo e assim que a cotação atinge um dos limites definidos, um email é enviado com a recomendação de ação.