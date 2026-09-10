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

Para rodar é preciso passar os parâmetros em sequência de trios: `<ATIVO>`, `<PRECO_VENDA>` e `<PRECO_COMPRA>`. É possível passar mais de um ativo no mesmo comando.

**Exemplos de uso:**
* Monitorando um ativo: `dotnet run -- PETR4 22.67 22.59`
* Monitorando mais de um ativo: `dotnet run -- VALE3 77.51 77.34 ITUB4 41.81 41.39`
* Via executável: `.\stock-quote-alert.exe PETR4 22.67 22.59`

<br>

Após a inicialização, o sistema monitora os ativos e assim que a cotação atinge um dos limites definidos, um email é enviado com a recomendação de ação.