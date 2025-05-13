/*
 * https://github.com/BepInEx/BepInEx/blob/6c5766d5abc230a4c9427ebb14acfca05255efb8/BepInEx.Preloader/Preloader.cs#L140C3-L222C4
 * Copyright (c) 2018 Bepis
 * Bepis licenses the basis of this file to Lordfirespeed under the MIT license.
 * Lordfirespeed licenses this file to you under the LGPL-3.0-OR-LATER license.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Preloader;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;
using static System.Reflection.Assembly;

namespace MetaLibrary.Patcher;

[UsedImplicitly]
public static class MetaLibraryPatcher
{
    private const string MetaLibraryAssemblyName = "dev.lordfirespeed.metalibrary.dll";

    private static string ThisAssemblyPath {
        get {
            var path = GetExecutingAssembly().Location;
            // check for UNC path: https://stackoverflow.com/a/18907418/11045433
            if (path.StartsWith(new string(Path.DirectorySeparatorChar, 2)))
                throw new InvalidOperationException("Patcher plugin location should be a local filepath, not UNC");
            return path;
        }
    }

    private static IList<string> MetaLibraryRelativePathParts {
        get {
            var relativePath = Path.GetRelativePath(Paths.PatcherPluginPath, ThisAssemblyPath);
            if (relativePath == ThisAssemblyPath)
                throw new InvalidOperationException("Patcher plugin should be in the BepInEx patcher plugins directory");
            var relativePathParts = relativePath.Split(Path.DirectorySeparatorChar).ToList();
            relativePathParts.RemoveAt(relativePathParts.Count - 1);
            return relativePathParts;
        }
    }

    private static IEnumerable<string> MetaLibraryAssemblySearchPath => [
        // can't hardcode the package identifier because it varies: e.g. Lordfirecompany-B..., LordfireREPO-B...
        Path.Combine([Paths.BepInExAssemblyDirectory, ..MetaLibraryRelativePathParts]),
        Path.Combine(Paths.BepInExAssemblyDirectory, "MetaLibrary"),
        Paths.BepInExAssemblyDirectory,
    ];

    private static string? _metaLibraryAssemblyPath;
    private static string? MetaLibraryAssemblyPath => _metaLibraryAssemblyPath ??= MetaLibraryAssemblySearchPath
        .Select(directoryPath => Path.Combine(directoryPath, MetaLibraryAssemblyName))
        .FirstOrDefault(File.Exists);

    [UsedImplicitly]
    public static IEnumerable<string> TargetDLLs => [ Preloader.ConfigEntrypointAssembly.Value ];

    /// <summary>
    /// Inserts MetaLibrary's entrypoint just before BepInEx's Chainloader.
	/// </summary>
	/// <param name="assembly">The assembly that the <see cref="MetaLibraryPatcher"/> will attempt to patch.</param>
	[UsedImplicitly]
	public static void Patch(AssemblyDefinition assembly)
	{
        if (MetaLibraryAssemblyPath is null)
            throw new InvalidOperationException("BepInEx.MetaLibrary assembly could not be found. It should be in the BepInEx/core/ directory.");

		string entrypointType = Preloader.ConfigEntrypointType.Value;
		string entrypointMethod = Preloader.ConfigEntrypointMethod.Value;

		bool isCctor = entrypointMethod.IsNullOrWhiteSpace() || entrypointMethod == ".cctor";

		var entryType = assembly.MainModule.Types.FirstOrDefault(x => x.Name == entrypointType);

        if (entryType is null)
            return; // fail silently because BepInEx will throw an error anyway

        using var injected = AssemblyDefinition.ReadAssembly(MetaLibraryAssemblyPath);
        var originalStartMethod = injected.MainModule.Types.First(x => x.Name == "Entrypoint").Methods
            .First(x => x.Name == "Start");

        var startMethod = assembly.MainModule.ImportReference(originalStartMethod);

        var methods = new List<MethodDefinition>();

        if (isCctor)
        {
            var cctor = entryType.Methods.FirstOrDefault(m => m.IsConstructor && m.IsStatic);

            if (cctor == null)
            {
                cctor = new MethodDefinition(".cctor",
                    MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig
                    | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    assembly.MainModule.ImportReference(typeof(void)));

                entryType.Methods.Add(cctor);
                var il = cctor.Body.GetILProcessor();
                il.Append(il.Create(OpCodes.Ret));
            }

            methods.Add(cctor);
        }
        else
        {
            methods.AddRange(entryType.Methods.Where(x => x.Name == entrypointMethod));
        }

        if (!methods.Any())
            throw new Exception("The entrypoint method is invalid! Please check your config.ini");

        foreach (var method in methods)
        {
            var il = method.Body.GetILProcessor();

            var ins = il.Body.Instructions.First();

            il.InsertBefore(ins, il.Create(OpCodes.Call, startMethod));
        }
    }
}
