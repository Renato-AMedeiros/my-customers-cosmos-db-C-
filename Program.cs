using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using my_customers_cosmos_db_C_.Services;
using Newtonsoft.Json;

namespace my_customers_cosmos_db_C_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurar o CosmosClient com a ConnectionString
            builder.Services.AddSingleton((s) =>
            {
                var connectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                var cosmosClientOptions = new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway // ou Direct
                };
                return new CosmosClient(connectionString, cosmosClientOptions);
            });

            // Add services to the container.
            builder.Services.AddControllers()
                        .AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        });

            builder.Services.AddTransient<CustomerService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Configura o cabeçalho personalizado
                c.OperationFilter<CustomHeaderOperationFilter>();
            });

            var app = builder.Build();

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
