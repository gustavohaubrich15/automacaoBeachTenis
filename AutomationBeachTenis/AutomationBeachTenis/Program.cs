using AutomationBeachTenis.Services.GenericApiService;
using AutomationBeachTenis.Services.MatchDayBeachTenisService;
using AutomationBeachTenis.Services.TelegramService;
using AutomationBeachTenis.Services.TournamentService;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddHttpClient();

builder.Services.AddScoped<IGenericApiService, GenericApiService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IMatchDayBeachTenisService, MatchDayBeachTenisService>();
builder.Services.AddScoped<ITelegramService, TelegramService>();

var app = builder.Build();


if (args.Contains("run-api-service-github-actions"))
{
    using var scope = app.Services.CreateScope();
    var matchDayBeachTenisService = scope.ServiceProvider.GetRequiredService<IMatchDayBeachTenisService>();
    await matchDayBeachTenisService.SendMatchListOfDayToTelegramChanel();
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
