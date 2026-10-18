using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace YaDeliveryB2b.Client;

///<inheritdoc/>
internal class YaDeliveryB2bSandboxHandler(IOptions<YaDeliveryB2bSandboxOption> options) : DelegatingHandler
{
    ///<inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options.Value);
        // Sandbox mode:
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.Token);
        return await base.SendAsync(request, cancellationToken);
    }
}