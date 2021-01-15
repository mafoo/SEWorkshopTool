using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Microsoft.Win32;

namespace Phoenix.WorkshopTool
{
    public static class AssemblyHelper
    {
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public static string FindSteamGameRoot(string gameFolderName)
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam"))
            {
                var steamPath = key?.GetValue("InstallPath") as string ??
                                throw new PlatformNotSupportedException(
                                    "Failed to locate Steam being installed by checking the registry");
                var steamConfig =
                    VdfConvert.Deserialize(File.ReadAllText(Path.Combine(steamPath, "config", "config.vdf")));
                var libraries = (steamConfig.Value["Software"]?["Valve"]?["Steam"].ToJson()
                        .ToObject<Dictionary<string, object>>() ?? new Dictionary<string, object>())
                    .Where(x => x.Key.StartsWith("BaseInstallFolder_")).Select(x => x.Value.ToString())
                    .Concat(new List<string> {steamPath})
                    .Select(x => Path.Combine(x, "steamapps", "common", gameFolderName)).ToList();
                var path = libraries.FirstOrDefault(Directory.Exists);
                if (string.IsNullOrWhiteSpace(path))
                    throw new PlatformNotSupportedException(
                        $"Failed to locate {gameFolderName} in any of the Steam Libraries, see inner exceptions",
                        new AggregateException(libraries.Select(x => new DirectoryNotFoundException(x))));

                return path;
            }
        }

        private static string ResolvePath(string assemblyName, string ext, string root)
        {
            var assemblyPath = Path.Combine(root, assemblyName + ext);

            if (!File.Exists(assemblyPath))
                assemblyPath = Path.Combine(root, "Bin64", assemblyName + ext);

            if (!File.Exists(assemblyPath))
                assemblyPath = Path.Combine(root, "Bin64", "x64", assemblyName + ext);

            var subLength = assemblyName.LastIndexOf('.');

            if (subLength == -1)
                subLength = assemblyName.Length;

            if (!File.Exists(assemblyPath))
                assemblyPath = Path.Combine(root, assemblyName.Substring(0, subLength) + ext);

            if (!File.Exists(assemblyPath))
                assemblyPath = Path.Combine(root, "Bin64", assemblyName.Substring(0, subLength) + ext);

            return assemblyPath;
        }

        public static string ResolvePathWithRoot(string assemblyName, string ext, string root = null)
        {
            string assemblyPath = null;
            if (!string.IsNullOrWhiteSpace(root))
                assemblyPath = ResolvePath(assemblyName, ext, root);
            if (assemblyPath == null || !File.Exists(assemblyPath))
                assemblyPath = ResolvePath(assemblyName, ext, Environment.CurrentDirectory);
            return assemblyPath;
        }
    }
}