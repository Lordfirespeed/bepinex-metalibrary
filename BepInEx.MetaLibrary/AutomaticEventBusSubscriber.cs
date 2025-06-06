using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Bootstrap;
using Bus;
using HarmonyLib;
using MetaLibrary.Collections.Generic;
using MetaLibrary.Common;
using Serilog;

namespace MetaLibrary;

/// <summary>
/// Automatic <see cref="EventBus"/> subscriber. Reads <see cref="EventBusSubscriberAttribute"/>
/// annotations and passes the annotated types to the <see cref="Bus"/> defined by the annotation.
/// </summary>
internal class AutomaticEventBusSubscriber(ILogger logger)
{
    private static readonly IEqualityComparer<Type> TypeComparer = IdentityEqualityComparer<Type>.Instance;

    private readonly ConcurrentDictionary<Type, byte> _autoSubscribedTypes = new(TypeComparer);

    public void Inject(PluginContainer plugin, Side context)
    {
        var eventBusSubscribers = AccessTools.GetTypesFromAssembly(plugin.Assembly)
            .Select(type => (Type: type, Attribute: type.GetCustomAttribute<EventBusSubscriberAttribute>()))
            .Where(pair => pair.Attribute is not null)
            .Where(pair => pair.Attribute.PluginGuid == plugin.Guid);

        foreach (var (registerType, subscriberAttribute) in eventBusSubscribers) {
            _autoSubscribedTypes.TryAdd(registerType, 0);
            if (!subscriberAttribute.Side.HasFlag(context)) continue;

            try {
                logger.Debug("Auto-subscribing {SubscriberType} to {Bus}", registerType, subscriberAttribute.Bus);

                var busIdentifier = subscriberAttribute.Bus;
                var bus = busIdentifier switch {
                    EventBusSubscriber.Bus.Game => MetaLibrary.EventBus,
                    EventBusSubscriber.Bus.Mod => plugin.EventBus,
                    _ => throw new ArgumentOutOfRangeException(nameof(busIdentifier), $"Unrecognised Bus value: {busIdentifier}")
                };
                bus.Register(registerType);
            }
            catch (Exception exc) {
                throw new InvalidOperationException($"Failed to auto-subscribe {registerType} to {subscriberAttribute.Bus}", exc);
            }
        }
    }

    public void WarnOfIgnoredSubscribers()
    {
        var ignoredSubscribers = AccessTools.AllTypes()
            .Select(type => (Type: type, Attribute: type.GetCustomAttribute<EventBusSubscriberAttribute>()))
            .Where(pair => pair.Attribute is not null)
            .Where(pair => !_autoSubscribedTypes.ContainsKey(pair.Type));

        foreach (var (registerType, subscriberAttribute) in ignoredSubscribers) {
            // plugin is not registered at all
            if (!Chainloader.PluginInfos.ContainsKey(subscriberAttribute.PluginGuid)) {
                logger.Warning(
                    "{SubscriberType} is annotated with {AutoSubscribeAttribute},\n" +
                    "but BepInPlugin with GUID {PluginGuid} is not recognised,\n" +
                    "so it has been ignored.",
                    registerType,
                    typeof(EventBusSubscriberAttribute),
                    subscriberAttribute.PluginGuid
                );
                return;
            }

            // plugin is registered, but the chainloader never attempted to load it
            var containerExists = PluginList.Instance.TryGetPluginContainerByGuid(subscriberAttribute.PluginGuid, out _);
            if (!containerExists) return;

            // plugin either loaded or failed to load
            logger.Warning(
                "{SubscriberType} is annotated with {AutoSubscribeAttribute},\n" +
                "but is not defined in the same assembly as BepInPlugin with GUID {PluginGuid},\n" +
                "so it has been ignored.",
                registerType,
                typeof(EventBusSubscriberAttribute),
                subscriberAttribute.PluginGuid
            );
        }
    }
}
