using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YaDeliveryB2b.Client.Test;

[TestClass]
[TestCategory("Integration")]
public sealed class YaDeliveryB2bIntegrationTests
{
    ServiceProvider? sp;
    IServiceScope? scope;
    IYaDeliveryB2bClient? client;
    const string requestId = "d83ce0872fc64b25910b644c8809dabd-udp";

    [TestInitialize]
    public void Initialize()
    {
        var config = new ConfigurationBuilder();
        config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["YaDeliveryB2bApi:Url"] = "https://b2b.taxi.tst.yandex.net",
            ["YaDeliveryB2bApi:Token"] = "y2_AgAAAAD04omrAAAPeAAAAAACRpC94Qk6Z5rUTgOcTgYFECJllXYKFx8",
        })
        .AddJsonFile("appsettings.test.json", optional: true);

        var sc = new ServiceCollection();
        sc.AddYaDeliveryB2bClient(config.Build());

        sp = sc.BuildServiceProvider(true);
        scope = sp.CreateScope();

        client = scope.ServiceProvider.GetRequiredService<IYaDeliveryB2bClient>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        scope?.Dispose();
        sp?.Dispose();
    }

    [TestMethod]
    [DataRow("d83ce0872fc64b25910b644c8809dabd-udp")]
    public async Task StatusTest(string requestId)
    {
        // Act
        var response = await client!.InfoGET2Async(requestId).ConfigureAwait(false);

        // Assets
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Request_id == requestId);
        Assert.IsNotNull(response.State);
        Assert.IsTrue(response.State.Status == "CANCELLED");
    }

    [TestMethod]
    [DataRow("d83ce0872fc64b25910b644c8809dabd-udp")]
    public async Task HistoryTest(string requestId)
    {
        // Act
        var response = await client!.HistoryAsync(requestId).ConfigureAwait(false);

        // Assets
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.State_history);
        Assert.IsNotNull(response.State_history.SingleOrDefault(x => x.Status == "CREATED"));
    }
}
