using System;
using System.Linq;
using BepInEx;

namespace MetaLibrary;

/// <summary>
/// Annotate a class which will be subscribed to an <see cref="EventBusIdentifier"/> when your plugin is initialised.
/// Defaults to subscribing to <c>SigurdLib.EventBus</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EventBusSubscriberAttribute : Attribute
{
    private static readonly Side DefaultSide = Side.Client | Side.Server;

    internal string PluginGuid { get; }

    internal Side Side { get; }

    internal string EventBusIdentifier { get; }

    /// <param name="pluginGuid">The <see cref="BepInPlugin.GUID"/> of the <see cref="BepInPlugin"/> this
    /// event listener should be associated with.</param>
    /// <param name="busIdentifier">The bus identifier to register this event listener with, usually the
    /// <see cref="BepInPlugin.GUID"/> of a MetaPlugin.
    /// </param>
    /// <param name="sides"></param>
    public EventBusSubscriberAttribute(string pluginGuid, string busIdentifier, params Side[] sides)
    {
        PluginGuid = pluginGuid;
        EventBusIdentifier = busIdentifier;
        Side = sides.Length > 0 ? sides.Aggregate((accumulator, next) => accumulator | next) : DefaultSide;
    }
}
