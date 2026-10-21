using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace YaDeliveryB2b.Client;

///<inheritdoc/>
internal class YaDeliveryB2bHandler(IServiceProvider serviceProvider, IOptions<YaDeliveryB2bSandboxOption> options) : DelegatingHandler
{
    ///<inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options.Value);
        // Sandbox mode:
        var authenticationHeaderManger = serviceProvider.GetRequiredKeyedService<IAuthenticationHeaderManger>(options.Value.Type ?? nameof(SandboxManger));
        request.Headers.Authorization = await authenticationHeaderManger.GetAuthenticationHeader(cancellationToken);
        return await base.SendAsync(request, cancellationToken);
    }
}