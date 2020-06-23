using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app_nehmen_api.Middleware;
using app_nehmen_api.Models;
using app_nehmen_api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace app_nehmen_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cosmosConfig = Configuration.GetSection(CosmosConfig.CosmosDb).Get<CosmosConfig>();
            services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(cosmosConfig).GetAwaiter().GetResult());
            services.AddOptions<UserConfig>(UserConfig.User);
            services.AddControllers();
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Creates a Cosmos DB database and a container with the specified partition key. 
        /// </summary>
        /// <returns></returns>
        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(CosmosConfig cosmosConfig)
        {
            var clientBuilder = new Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder(cosmosConfig.Account, cosmosConfig.Key);
            var client = clientBuilder
                                .WithConnectionModeDirect()
                                .Build();
            var cosmosDbService = new CosmosDbService(client, cosmosConfig.DatabaseName, cosmosConfig.ContainerName);
            var database = await client.CreateDatabaseIfNotExistsAsync(cosmosConfig.DatabaseName);
            await database.Database.CreateContainerIfNotExistsAsync(cosmosConfig.ContainerName, "/pk");

            return cosmosDbService;
        }
    }
}
