using System;
using System.Collections.Generic;
using System.Linq;
using Bus.Api;
using HarmonyLib;
using MetaLibrary.Collections.Generic;

namespace MetaLibrary;

public class Bindings
{
    public static Bindings Instance { get; } = new();

    private readonly HashSet<IBindingsProvider> _providers = ServiceLoad<IBindingsProvider>().ToHashSet();
    private readonly BiDictionary<string, IEventBus> _busCache = new(valueComparer: IdentityEqualityComparer<IEventBus>.Instance);

    private delegate T? SearchProvider<out T>(IBindingsProvider provider) where T : class;

    private static T? LookupBySearchingProviders<T>(SearchProvider<T> search) where T : class
    {
        foreach (var provider in Instance._providers) {
            var result = search(provider);
            if (result is not null) return result;
        }
        return null;
    }

    public static IEventBus LookupBus(string busIdentifier)
    {
        IEventBus? bus;
        if (Instance._busCache.TryGetValue(busIdentifier, out bus)) return bus;

        bus = LookupBySearchingProviders<IEventBus>(provider => provider.SearchForBus(busIdentifier));
        if (bus is null) throw new KeyNotFoundException($"No discovered IBindingsProvider offers an event bus for identifier '{busIdentifier}'");

        Instance._busCache.Add(busIdentifier, bus);
        return bus;
    }

    private static IEnumerable<T> ServiceLoad<T>()
    {
        var it = typeof(T);

        return AccessTools.AllTypes()
            .Where(type => type != it && !type.IsInterface)
            .Where(it.IsAssignableFrom)
            .Select(type => (T)Activator.CreateInstance(type));
    }
}
