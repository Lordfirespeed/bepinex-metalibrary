/*
 * Copyright (c) 2024 Sigurd Team
 * The Sigurd Team licenses this file to you under the LGPL-3.0-OR-LATER license.
 */

using System;
using System.Reflection;
using BepInEx;
using Bus.Api;
using MetaLibrary.Event;

namespace MetaLibrary;

public class PluginContainer
{
    public PluginInfo Info { get; }

    public Assembly Assembly { get; }

    public string Guid => Info.Metadata.GUID;

    public string Namespace => Guid;

    public IEventBus EventBus { get; } = new BusConfiguration { }.Build();

    public PluginContainer(PluginInfo info, Assembly assembly)
    {
        Info = info;
        Assembly = assembly;
    }

    public void AcceptEvent<T>(T @event) where T : Bus.Api.Event, IPluginBusEvent
    {
        try {
            // logger.trace: firing event ...
            EventBus.Post(@event);
            // logger.trace: fired event ...
        }
        catch (Exception exc) {
            // logger.error: caught exception ...
            throw new InvalidOperationException($"An error occurred during event {typeof(T).Name} affecting plugin {Info}", exc);
        }
    }

    public void AcceptEvent<T>(EventPriority phase, T @event) where T : Bus.Api.Event, IPluginBusEvent
    {
        try {
            // logger.trace: firing event ...
            EventBus.Post(phase, @event);
            // logger.trace: fired event ...
        }
        catch (Exception exc) {
            // logger.error: caught exception ...
            throw new InvalidOperationException($"An error occurred during event {typeof(T).Name} affecting plugin {Info}", exc);
        }
    }

    /// <inheritdoc />
    public override string ToString() => $"PluginContainer[guid = {Guid}]";
}
