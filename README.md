# 🏖️ AutomacaoBeachTenis

Este projeto tem como objetivo automatizar o envio diário de notificações com os confrontos de Beach Tennis da ITF diretamente para um canal do **Telegram**.

## 🚀 Funcionalidades

- Consulta diária dos torneios e jogos de **Beach Tennis** da [ITF]((https://www.itftennis.com/en/itf-tours/beach-tennis-tour/).
- Envio automático das mensagens para um canal do telegram.
- Agendamento diário via **GitHub Actions** usando cron schedule.

## 🛠️ Tecnologias Utilizadas

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (Console App)
- [Telegram.Bot](https://github.com/TelegramBots/telegram.bot) (envio de mensagens)
- [GitHub Actions](https://docs.github.com/en/actions) (execução automática agendada)
- API pública da ITF para obtenção de dados de torneios

## ⏰ Agendamento (GitHub Actions)

O envio das mensagens é realizado diariamente às **9h da manhã** (horário configurado no cron)
