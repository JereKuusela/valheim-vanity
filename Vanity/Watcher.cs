

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace Service
{
  public class Watcher
  {
    static Dictionary<string, byte[]> hashes = new();
    private static byte[] GetHash(string path)
    {

      using (var md5 = MD5.Create())
      {
        using (var stream = File.OpenRead(path))
        {
          return md5.ComputeHash(stream);
        }
      }
    }
    public static void Setup(string folder, string pattern, Action action)
    {
      FileSystemWatcher watcher = new(folder, pattern);
      watcher.Changed += (s, e) =>
      {
        var hash = GetHash(e.FullPath);
        if (hashes.ContainsKey(e.FullPath) && hashes[e.FullPath].SequenceEqual(hash))
          return;
        hashes[e.FullPath] = hash;
        action();
      };
      watcher.IncludeSubdirectories = true;
      watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
      watcher.EnableRaisingEvents = true;
    }
    public static void Setup(string file, Action action)
    {
      var pattern = Path.GetFileName(file);
      Setup(Paths.ConfigPath, pattern, action);
    }
    public static void Setup(ConfigFile config, ManualLogSource logger)
    {
      var path = config.ConfigFilePath;
      var folder = Path.GetDirectoryName(path);
      var pattern = Path.GetFileName(path);
      Setup(folder, pattern, () =>
      {
        if (!File.Exists(config.ConfigFilePath)) return;
        try
        {
          logger.LogDebug("ReadConfigValues called");
          config.Reload();
        }
        catch
        {
          logger.LogError($"There was an issue loading your {config.ConfigFilePath}");
          logger.LogError("Please check your config entries for spelling and format!");
        }
      }
      );
    }
  }
}