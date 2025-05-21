using System;
using Bus.Api;
using MetaLibrary.Event;

namespace MetaLibrary;

public static class PluginLoader
{
    public static void PostEvent<T>(T @event) where T : Bus.Api.Event, IPluginBusEvent
    {
        foreach (var phase in EventPriority.Values) {
            foreach (var container in PluginList.Instance.PluginContainersInOrder) {
                container.AcceptEvent(phase, @event);
            }
        }
    }

    public static void PostEventWithActiveContainerWrapInModOrder<T>(T @event) where T : Bus.Api.Event, IPluginBusEvent
    {
        PostEventWithWrapInModOrder(
            @event,
            (container, _) => PluginLoadingContext.Instance.ActiveContainer = container,
            (_, _) => PluginLoadingContext.Instance.ActiveContainer = null
        );
    }

    public static void PostEventWithWrapInModOrder<T>(
        T @event,
        Action<PluginContainer, T> preAction,
        Action<PluginContainer, T> postAction
    ) where T : Bus.Api.Event, IPluginBusEvent {
        foreach (var phase in EventPriority.Values) {
            foreach (var container in PluginList.Instance.PluginContainersInOrder) {
                preAction.Invoke(container, @event);
                container.AcceptEvent(phase, @event);
                postAction.Invoke(container, @event);
            }
        }
    }
}
