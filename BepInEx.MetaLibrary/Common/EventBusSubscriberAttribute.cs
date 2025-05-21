using System;
using Bus.Api;
using BusIdentifier = MetaLibrary.Common.EventBusSubscriber.Bus;

namespace MetaLibrary.Common;

/// <summary>
/// Annotate a class which will be subscribed to an <see cref="IEventBus"/> when your plugin is initialised.
/// Defaults to subscribing to <see cref="MetaLibrary.EventBus"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EventBusSubscriberAttribute : Attribute
{
    private static readonly Side DefaultSide = Side.Client | Side.Server;

    public required string PluginGuid { get; set; }

    public BusIdentifier Bus { get; set; } = BusIdentifier.Game;

    public Side Side { get; set; } = DefaultSide;
}
