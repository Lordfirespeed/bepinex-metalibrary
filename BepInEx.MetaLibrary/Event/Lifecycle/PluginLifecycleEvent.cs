namespace MetaLibrary.Event.Lifecycle;

public abstract class PluginLifecycleEvent : Bus.Api.Event, IPluginBusEvent
{
    public required PluginContainer Container { get; init; }
}
