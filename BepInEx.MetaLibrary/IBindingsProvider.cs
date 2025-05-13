using Bus.Api;

namespace MetaLibrary;

public interface IBindingsProvider
{
    /// <summary>
    /// Event bus search function. Search functions are expected to take one argument, being the bus name - usually the
    /// BepInEx plugin GUID of a MetaPlugin - and return an <see cref="IEventBus"/> instance.
    /// In case a search function cannot find a given event bus, it should return <see langword="null"/>.
    /// </summary>
    public IEventBus? SearchForBus(string busIdentifier) => null;
}
