using HallOfFame_Test.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace HallOfFame_Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            
            builder.Services.AddSwaggerGen();
            builder.Services.AddSerilog();
            
            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationContext>(opt =>
                opt.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));
            
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            var path = config.GetValue<string>("Logging:LogFilePath");
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(path)
                .CreateLogger();
            
            var app = builder.Build();

            app.UseAuthorization();

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Run();
        }
    }
}