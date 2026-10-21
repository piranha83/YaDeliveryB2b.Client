using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace YaDeliveryB2b.Client;

public interface IAuthenticationHeaderManger
{
    public Task<AuthenticationHeaderValue> GetAuthenticationHeader(CancellationToken cancellationToken);
}

///<inheritdoc/>
internal class SandboxManger(IOptions<YaDeliveryB2bSandboxOption> options) : IAuthenticationHeaderManger
{
    ///<inheritdoc/>
    public Task<AuthenticationHeaderValue> GetAuthenticationHeader(CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options.Value);
        // Sandbox mode:
        return Task.FromResult(new AuthenticationHeaderValue("Bearer", options.Value.Token));
    }
}