﻿using System;
using System.Data;
using DDDEfCore.Infrastructures.EfCore.Common.Migration;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.JsonConverters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using DDDEfCore.Core.Common.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        private readonly Checkpoint _checkpoint;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TestServer _testServer;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        protected readonly IFixture AutoFixture;

        public SharedFixture()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddJsonFile("appsettings.json");
                })
                .UseStartup<Startup>();

            this._testServer = new TestServer(webHostBuilder);
            this._serviceScopeFactory = this._testServer.Services.GetService<IServiceScopeFactory>();
            this._jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            this._jsonSerializerOptions.Converters.Add(new IdentityJsonConverterFactory());

            this._checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };
            this.AutoFixture = new Fixture();
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            await this.ResetCheckpoint();
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;

        #endregion

        public async Task SeedingData<T>(params T[] entities) where T : AggregateRoot
        {
            if (entities != null && entities.Any())
            {
                using var scope = this._serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<DbContext>();
                await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                try
                {
                    await dbContext.Set<T>().AddRangeAsync(entities);
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task DoTest(Func<HttpClient, JsonSerializerOptions, IServiceProvider, Task> doTestFnc)
        {
            using var client = this._testServer.CreateClient();
            using var scope = this._serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            await doTestFnc(client, this._jsonSerializerOptions, serviceProvider);
        }

        private async Task ResetCheckpoint()
        {
            using var serviceScope = this._serviceScopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<DbContext>();

            var databaseMigration = serviceScope.ServiceProvider.GetService<DatabaseMigration>();
            await databaseMigration.ApplyMigration();

            var dbConnection = dbContext.Database.GetDbConnection();
            await dbConnection.OpenAsync();
            await this._checkpoint.Reset(dbConnection);
        }
    }
}