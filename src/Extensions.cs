using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace YaDeliveryB2b.Client;

/// <summary>
/// Расширения.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Добавить AddYaDeliveryB2b клиент в IServiceCollection.
    /// </summary>
    /// <param name="serviceCollection">Коллекция.</param>
    /// <param name="configuration">Настройки сервиса.</param>
    /// <returns></returns>
    public static IServiceCollection AddYaDeliveryB2bClient(
            this IServiceCollection serviceCollection,
            IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection);
        ArgumentNullException.ThrowIfNull(configuration);

        serviceCollection.AddTransient<YaDeliveryB2bSandboxHandler>().AddHttpClient<IYaDeliveryB2bClient, YaDeliveryB2bClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<YaDeliveryB2bSandboxOption>>();
            ArgumentNullException.ThrowIfNullOrWhiteSpace(options.Value.Url);

            client.BaseAddress = new Uri(options.Value.Url);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("YaDeliveryB2bApiClient");
        })
        .AddHttpMessageHandler<YaDeliveryB2bSandboxHandler>()
        .AddPolicyHandler(Create())
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
        .ConfigurePrimaryHttpMessageHandler(sp => Create(sp.GetRequiredService<IOptions<YaDeliveryB2bSandboxOption>>()));

        serviceCollection.Configure<YaDeliveryB2bSandboxOption>(configuration.GetSection("YaDeliveryB2bApi"));

        return serviceCollection;
    }

    private static IAsyncPolicy<HttpResponseMessage> Create()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static HttpMessageHandler Create(IOptions<YaDeliveryB2bSandboxOption> options)
    {
        return new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = options.Value.Settings.EnableMultipleConnections ?? true,
            MaxConnectionsPerServer = options.Value.Settings.MaxConnections ?? 100,
            PooledConnectionIdleTimeout = TimeSpan.FromSeconds(options.Value.Settings.PooledConnectionIdleSeconds ?? 120),
            PooledConnectionLifetime = TimeSpan.FromSeconds(options.Value.Settings.PooledConnectionSeconds ?? 300),
            ConnectTimeout = TimeSpan.FromSeconds(options.Value.Settings.ConnectTimeoutSeconds ?? 30),
            UseCookies = false,
            UseProxy = options.Value.Settings.Proxy != null,
            Proxy = options.Value.Settings.Proxy != null ? new WebProxy(options.Value.Settings.Proxy, true)
            {
                Credentials = options.Value.Settings.ProxyUserName != null && options.Value.Settings.ProxyPassword != null
                    ? new NetworkCredential(options.Value.Settings.ProxyUserName, options.Value.Settings.ProxyPassword)
                    : null,
            } : null,
        };
    }
}
