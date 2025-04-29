using System.Net;
using System.Text.Json;
using Kino.Repository.Services;

namespace KinoTarjimaSayt.Middlewares;

public class GlobalExeptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExeptionHandler> _logger;
    private readonly TelegramLogger telegramLogger;

    public GlobalExeptionHandler(RequestDelegate next, TelegramLogger telegramLogger, ILogger<GlobalExeptionHandler> logger)
    {
        _next = next;
        this.telegramLogger = telegramLogger;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Controller yoki keyingi middlewarega berish
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred."); // To'liq Exception objectni log qil
            await telegramLogger.LogAsync(ex.ToString()); // Faqat ex.Message emas, to'liq Exception

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = context.Response.StatusCode,
                message = "Internal Server Error", // Foydalanuvchiga juda ko'p error tafsilot bermaymiz
                detail = ex.Message // yoki dev/prod modega qarab ko'rsatish mumkin
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
