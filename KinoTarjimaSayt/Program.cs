using DataAccess.Sayt;
using Kino.Repository.Services;
using KinoTarjimaSayt.Configurations;
using KinoTarjimaSayt.Middlewares;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace KinoTarjimaSayt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000);
            });


            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(dispose: true);


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IMovieRepository, MovieRepository>();
            builder.Services.AddScoped<MainContext>();
            builder.ConfigureDatabase();
            builder.Services.AddSingleton(new TelegramLogger("8058296814:AAGmLEzCcbukiRiqqqh7IW6Oh4YfXQ6YvkM", "-1002579719825"));
            var app = builder.Build();
            app.UseMiddleware<GlobalExeptionHandler>();
            // Error page va HTTPS redirect
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            // Swagger faqat Developmentda chiqadi
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
