using DemoApi.Models;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace DemoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true).AddEnvironmentVariables().Build();

            var apiConfiguration = builder.Configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>();
            builder.Services.AddSingleton(apiConfiguration);
            builder.Services.AddControllers().AddNewtonsoftJson();
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(apiConfiguration.ApiVersion, new OpenApiInfo { 
                    Title = apiConfiguration.ApiName, 
                    Description = apiConfiguration.ApiDescription,
                    Version = apiConfiguration.ApiVersion
                });
                
                string xmlFilePath = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilePath);
                c.IncludeXmlComments(xmlPath);
            })
            .AddSwaggerGenNewtonsoftSupport();

            WebApplication app = builder.Build();

            if (builder.Environment.EnvironmentName.Equals("Development", StringComparison.InvariantCultureIgnoreCase)
                || builder.Environment.EnvironmentName.Equals("QA", StringComparison.InvariantCultureIgnoreCase))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{apiConfiguration.ApiBaseUrl}/swagger/v1/swagger.json", 
                        apiConfiguration.ApiName + " " + builder.Environment.EnvironmentName);
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}