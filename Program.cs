using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EmployeeDetails.Data;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace EmployeeDetails
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var keyVaultName = "ik-key-vault-store";
            var secretName = "EmployeesDbConnection";

            var client = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
            var EmployeeDetailsContext = client.GetSecret(secretName).Value.Value; //secret value

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<EmployeeDetailsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString(EmployeeDetailsContext) ?? throw new InvalidOperationException("Connection string 'EmployeeDetailsContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
