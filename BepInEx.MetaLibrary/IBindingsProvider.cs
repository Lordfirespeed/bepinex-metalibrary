using System;
using Bus.Api;

namespace MetaLibrary;

public interface IBindingsProvider
{
    public Func<IEventBus> SigurdBusSupplier { get; }
}
