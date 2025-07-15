namespace Mango.AuthService.Domain.Events
{
  public class EventBridgeEnvelope<T>
  {
    public string DetailType { get; set; } = default!;
    public string Source { get; set; } = default!;
    public T Detail { get; set; } = default!;
  }
}
