
using Backend_GameDiscountNotifier.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Backend_GameDiscountNotifier
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpClient<BackgroundServiceAdquireJSON>();
            builder.Services.AddHostedService<BackgroundServiceAdquireJSON>();
            //builder.Services.AddDbContext<MariaDbContext>(options =>
            //    options.UseMySql(
            //        builder.Configuration.GetConnectionString("DefaultConnection"),
            //        ServerVersion.AutoDetect(
            //            builder.Configuration.GetConnectionString("DefaultConnection")
            //        )
            //    )
            //);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //using (var scope = app.Services.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<MariaDbContext>();

            //    await LaboratoriDb.Insert(context);
            //}

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
