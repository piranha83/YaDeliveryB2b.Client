namespace YaDeliveryB2b.Client;

/// <summary>
/// Настройки.
/// </summary>
internal class YaDeliveryB2bOption
{
    /// <summary>
    /// Сервис.
    /// </summary>
    public string Url { get; set; } = default!;

    /// <summary>
    /// Сетевые настройки.
    /// </summary>
    public YaDeliveryB2bSettings Settings { get; set; } = new();
}

internal sealed class YaDeliveryB2bSandboxOption : YaDeliveryB2bOption
{
    /// <summary>
    /// Токен(sandbox).
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Откуда забрать заказ(sandbox).
    /// </summary>
    public string? SourceStationId { get; set; }

    /// <summary>
    /// Куда доставить заказ(sandbox).
    /// </summary>
    public string? DestanationStationId { get; set; }
}