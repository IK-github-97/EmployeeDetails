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
            var builder = WebApplication.CreateBuilder(args);
            /*builder.Services.AddDbContext<EmployeeDetailsContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDetailsContext") ?? throw new InvalidOperationException("Connection string 'EmployeeDetailsContext' not found.")));
*/
            // Retrieve the secret from Azure Key Vault
            var keyVaultName = "ik-key-vault-store"; // Replace with your actual Key Vault name
            var secretName = "EmployeesDbConnection"; // Replace with your secret name
            var client = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
            var secretValue = client.GetSecret(secretName).Value.Value;

            // Use secretValue as your connection string
            //var connectionString = string.Format(builder.Configuration.GetConnectionString("EmployeeDetailsContext"), secretValue);
            var connectionString = secretValue;

            builder.Services.AddDbContext<EmployeeDetailsContext>(options =>
    options.UseSqlServer(connectionString));
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
