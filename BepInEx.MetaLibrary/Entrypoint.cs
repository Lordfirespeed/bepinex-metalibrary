using System;
using BepInEx.Bootstrap;
using HarmonyLib;
using JetBrains.Annotations;
using MetaLibrary.Event.Lifecycle;
using MetaLibrary.Extensions;
using Serilog;

namespace MetaLibrary;

internal static class Entrypoint
{
    private static ILogger? _logger;
    internal static ILogger Logger => _logger ??= CreateLogger();

    private static ILogger CreateLogger() => new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.BepInExLogger(ProductInfo.PRODUCT_NAME)
        .CreateLogger();

    [UsedImplicitly]
    static void Start()
    {
        var harmony = new Harmony(ProductInfo.PRODUCT_GUID);
        try {
            harmony.PatchAllNestedTypes(typeof(ChainloaderHooks));
        }
        catch (Exception exc) {
            Logger.Fatal(exc, "Failed to patch {MethodName}", nameof(Chainloader.Start));
        }

        ConfigureAutomaticEventBusSubscriber();
    }

    static void ConfigureAutomaticEventBusSubscriber()
    {
        var autoSubscriber = new AutomaticEventBusSubscriber(Logger);

        ChainloaderHooks.Plugin.OnPostLoad += (_, args) => {
            autoSubscriber.Inject(args.PluginContainer, Side.Client);
            args.PluginContainer.AcceptEvent(new MetaLibraryConstructPluginEvent { Container = args.PluginContainer });
        };

        ChainloaderHooks.OnComplete += (_, args) => {
            autoSubscriber.WarnOfIgnoredSubscribers();
        };
    }
}
