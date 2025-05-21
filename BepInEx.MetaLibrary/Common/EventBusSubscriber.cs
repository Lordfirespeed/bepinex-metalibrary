namespace MetaLibrary.Common;

public static class EventBusSubscriber
{
    public enum Bus
    {
        /**
         * The main MetaLibrary event bus, used after the game has started up.
         */
        Game,
        /**
         * The mod-specific event bus, used during startup.
         */
        Mod,
    }
}
