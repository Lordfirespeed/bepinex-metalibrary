using System;

namespace MetaLibrary;

/// <summary>
/// Extension methods providing functionality for <see cref="Side"/>.
/// </summary>
public static class SideExtensions
{
    /// <summary>
    /// Determines whether the <see cref="Side"/> represents a client.
    /// </summary>
    /// <param name="side">A <see cref="Side"/> to test.</param>
    /// <returns><see langword="true"/> if the <see cref="Side"/> includes client-sidedness. Otherwise, <see langword="false"/>.</returns>
    public static bool IsClient(this Side side) => side.HasFlag(Side.Client);

    /// <summary>
    /// Determines whether the <see cref="Side"/> represents a server.
    /// </summary>
    /// <param name="side"></param>
    /// <returns><see langword="true"/> if the <see cref="Side"/> includes server-sidedness. Otherwise, <see langword="false"/>.</returns>
    public static bool IsServer(this Side side) => side.HasFlag(Side.Server);

    /// <summary>
    /// Determines whether the <see cref="Side"/> represents a client host, also known as a 'listen server'.
    /// </summary>
    /// <param name="side"></param>
    /// <returns><see langword="true"/> if the <see cref="Side"/> includes both client- and server-sidedness. Otherwise, <see langword="false"/>.</returns>
    public static bool IsHost(this Side side) => side.HasFlag(Side.Client | Side.Server);
}

/// <summary>
/// A networking 'sidedness'.
/// </summary>
[Flags]
public enum Side
{
    /// <summary>
    /// The client-side.
    /// </summary>
    Client = 1 << 0,

    /// <summary>
    /// The server-side.
    /// </summary>
    Server = 1 << 1,
}
