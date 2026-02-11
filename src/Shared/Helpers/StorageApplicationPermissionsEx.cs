using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace StrixMusic.Helpers;

/// <summary>
/// Provides access to FutureAccessList, using a polyfill on Linux where the platform implementation throws.
/// </summary>
public static class StorageApplicationPermissionsEx
{
#if __LINUX__
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.General) { WriteIndented = true };
    private static string? _storagePath;
    private static Dictionary<string, string>? _cache;
#endif

    /// <summary>
    /// Initializes the polyfill. Must be called before using FutureAccessList on Linux.
    /// </summary>
    public static void Initialize(string storagePath)
    {
#if __LINUX__
        _storagePath = storagePath;
        Directory.CreateDirectory(Path.GetDirectoryName(storagePath)!);
#endif
    }

    /// <summary>
    /// Gets the future access list.
    /// </summary>
    public static FutureAccessListEx FutureAccessList { get; } = new();

#if __LINUX__
    private static Dictionary<string, string> GetCache()
    {
        if (_storagePath is null)
            throw new InvalidOperationException("StorageApplicationPermissionsEx.Initialize must be called first.");

        if (_cache != null)
            return _cache;

        if (File.Exists(_storagePath))
        {
            try
            {
                var json = File.ReadAllText(_storagePath);
                _cache = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
            catch
            {
                _cache = new();
            }
        }
        else
        {
            _cache = new();
        }

        return _cache;
    }

    private static void SaveCache()
    {
        if (_storagePath is null)
            throw new InvalidOperationException("StorageApplicationPermissionsEx.Initialize must be called first.");

        var json = JsonSerializer.Serialize(_cache, _jsonOptions);
        File.WriteAllText(_storagePath, json);
    }
#endif

    public class FutureAccessListEx
    {
        public void AddOrReplace(string token, IStorageItem item)
        {
#if __LINUX__
            var cache = GetCache();
            cache[token] = item.Path;
            SaveCache();
#else
            StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, item);
#endif
        }

        public async Task<StorageFolder> GetFolderAsync(string token)
        {
#if __LINUX__
            var cache = GetCache();
            if (cache.TryGetValue(token, out var path) && Directory.Exists(path))
                return await StorageFolder.GetFolderFromPathAsync(path);

            throw new FileNotFoundException($"Token '{token}' not found or folder no longer exists.");
#else
            return await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
#endif
        }
    }
}
