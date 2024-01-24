using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace Identity.Marten.Tests;
public class IntegrationFicture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgeSqlContainer;

  

    public IntegrationFicture()
    {
        var randomPort = PortChecker.GetPort(35000, 45000);

        _postgeSqlContainer = new PostgreSqlBuilder()
        .WithDatabase("identity_matren_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithPortBinding(randomPort, 5432)
        .WithImage("postgres:15.1")
        .Build();


    }

    public HttpClient Client { get; private set; } = null!;
    public MockApp App { get; private set; } = null!;

    public async Task DisposeAsync()
    {
        await _postgeSqlContainer.StartAsync();
    }

    public async Task InitializeAsync()
    {
        await _postgeSqlContainer.StartAsync();

        App = new MockApp(_postgeSqlContainer.GetConnectionString());
        Client = App.CreateClient();
    }

    public class MockApp : WebApplicationFactory<Program>
    {
        private readonly string _postgresConnectionString;

        public MockApp(string postgresConnectionString)
        {
            _postgresConnectionString = postgresConnectionString;
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                services.ConfigureMarten(options =>
                {
                    options.Connection(_postgresConnectionString);
                });

            });


            builder.ConfigureTestServices(services =>
            {
                Console.WriteLine("ala");


            });
        }
    }
}


[CollectionDefinition(nameof(IntegrationFictureCollection))]
public class IntegrationFictureCollection : ICollectionFixture<IntegrationFicture>
{

}

[Collection(nameof(IntegrationFictureCollection))]
//public class IntegrationTest :  IClassFixture<IntegrationFicture> , IAsyncLifetime
public class IntegrationTest :   IAsyncLifetime
{
    public IntegrationTest(IntegrationFicture integrationFicture)
    {
        IntegrationFicture = integrationFicture;
    }

    public IntegrationFicture IntegrationFicture { get; }

    public HttpClient Client =>IntegrationFicture.Client;

    public IServiceScope Scope { get; private set; } = null!;
    public IServiceProvider Services => Scope.ServiceProvider;

    public Task DisposeAsync()
    {
        Scope.Dispose();
        return Task.CompletedTask;

    }

    public Task InitializeAsync()
    {
        Scope = IntegrationFicture.App.Services.CreateScope();
        return Task.CompletedTask;
    }
}


public static class PortChecker
{

    static List<int> portAddedList = new();

    public static int GetPort(int from = 40000, int to = 45000)
    {
        int randomPort = Random.Shared.Next(from, to);

        lock (portAddedList)
        {
            while (PortInUse(randomPort) ||  portAddedList.Any(p=>p == randomPort))
            {
                randomPort = Random.Shared.Next(from, to);
            }
            portAddedList.Add(randomPort);
        }

        return randomPort;

        static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();


            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }


            return inUse;
        }
    }

}