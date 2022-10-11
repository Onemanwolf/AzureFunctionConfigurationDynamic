using System.Collections.Immutable;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

using System;

[assembly: FunctionsStartup(typeof(Mvp.Function.Startup))]

namespace Mvp.Function
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string cs = Environment.GetEnvironmentVariable("ConnectionString");
            string vault = Environment.GetEnvironmentVariable("KeyVault");
            builder.ConfigurationBuilder.AddAzureAppConfiguration(cs).AddAzureKeyVault(
            new Uri(vault),
            new DefaultAzureCredential(),

            new AzureKeyVaultConfigurationOptions
            {
               // Manager = new PrefixKeyVaultSecretManager(secretPrefix),
               ReloadInterval = TimeSpan.FromMinutes(5)
            });

            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
        {
            Delay= TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
         }
            };
            var client = new SecretClient(new Uri(vault), new DefaultAzureCredential(), options);

            KeyVaultSecret secret = client.GetSecret("Key");

            string secretValue = secret.Value;
            Console.WriteLine(secretValue);
        }


        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<IMyService>((s) =>
            {
                return new MyService();
            });


        }
    }
}