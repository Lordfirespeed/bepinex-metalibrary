using Bus.Api;

namespace MetaLibrary;

public class MetaLibrary
{
    public static readonly IEventBus EventBus = new BusConfiguration {
        StartImmediately = false,
    }.Build();
}
