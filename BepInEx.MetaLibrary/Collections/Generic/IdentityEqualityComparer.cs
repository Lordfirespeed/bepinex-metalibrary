using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MetaLibrary.Collections.Generic;

// https://stackoverflow.com/a/8946825
public sealed class IdentityEqualityComparer<T> : IEqualityComparer<T> where T : class
{
    public static IdentityEqualityComparer<T> Instance { get; } = new();

    private IdentityEqualityComparer() { }

    public int GetHashCode(T value) => RuntimeHelpers.GetHashCode(value);

    public bool Equals(T left, T right) => ReferenceEquals(left, right);
}
