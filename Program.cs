using HallOfFame_Test.Models;
using Serilog;

namespace HallOfFame_Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            
            builder.Services.AddSwaggerGen();
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .MinimumLevel.Debug()
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