using Bus.Api;

namespace MetaLibrary;

public static class MetaLibrary
{
    public static readonly IEventBus EventBus = new BusConfiguration {
        StartImmediately = false,
    }.Build();
}
