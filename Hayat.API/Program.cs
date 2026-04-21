using Hayat.API.Helper;
using Hayat.API.Infrastructure;
using Hayat.BLL.Helper;
using Hayat.DAL.Helper;
using System.Text.Json.Serialization;

namespace Hayat.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApiDependencies(builder.Configuration);
            builder.Services.AddBllDependencies(builder.Configuration);
            builder.Services.AddDalDependencies(builder.Configuration);

            builder.Services.AddControllers().AddJsonOptions(op=>op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //await app.ApplyDevelopmentDatabaseSetupAsync();
            }
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            await app.RunAsync();
        }
    }
}
