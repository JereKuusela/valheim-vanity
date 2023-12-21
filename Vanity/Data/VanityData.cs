
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using BepInEx;
using Service;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Vanity;

public class VanityEntry
{

  [DefaultValue("")]
  public string name = "";
  [DefaultValue("")]
  public string player = "";
  [DefaultValue("")]
  public string group = "";
  [DefaultValue("")]
  public string helmet = "";
  [DefaultValue("")]
  public string chest = "";
  [DefaultValue("")]
  public string shoulder = "";
  [DefaultValue("")]
  public string legs = "";
  [DefaultValue("")]
  public string utility = "";
  [DefaultValue("")]
  public string leftHand = "";
  [DefaultValue("")]
  public string rightHand = "";
  [DefaultValue("")]
  public string leftBack = "";
  [DefaultValue("")]
  public string rightBack = "";
  [DefaultValue("")]
  public string beard = "";
  [DefaultValue("")]
  public string hair = "";
  [DefaultValue("")]
  public string skinColor = "";
  [DefaultValue("")]
  public string hairColor = "";
  [DefaultValue(null)]
  public float? updateInterval = null;
  [DefaultValue(null)]
  public float? colorDuration = null;
  public Dictionary<string, string> gear = [];
  public Dictionary<string, string> crafted = [];
}

public class VanityInfo
{
  public Dictionary<string, Tuple<string, int>> gear = [];
  public Tuple<string, int>? helmet;
  public Tuple<string, int>? chest;
  public Tuple<string, int>? legs;
  public Tuple<string, int>? shoulder;
  public Tuple<string, int>? utility;
  public Tuple<string, int>? leftHand;
  public Tuple<string, int>? rightHand;
  public Tuple<string, int>? leftBack;
  public Tuple<string, int>? rightBack;
  public Tuple<string, int>? beard;
  public Tuple<string, int>? hair;
  public string skinColor = "";
  public string hairColor = "";
  public float? updateInterval;
  public float? colorDuration;
}

public class VanityData
{
  public static Dictionary<string, VanityEntry> Data = [];
  public static Dictionary<string, string> PlayerIds = [];
  public static string FileName = "vanity.yaml";
  public static string FilePath = Path.Combine(Paths.ConfigPath, FileName);



  public static void CreateFile()
  {
    if (File.Exists(FilePath)) return;
    File.WriteAllText(FilePath, "");
  }
  public static void ToFile()
  {
    if (!Vanity.ConfigSync.IsSourceOfTruth) return;
    var yaml = Serializer().Serialize(Data);
    File.WriteAllText(FilePath, yaml);
  }

  public static void FromFile()
  {
    if (!Vanity.ConfigSync.IsSourceOfTruth) return;
    Vanity.VanityValue.Value = File.ReadAllText(FilePath);
  }
  public static void FromValue(string value)
  {
    Data = Deserialize<Dictionary<string, VanityEntry>>(value, "Data") ?? [];
    PlayerIds.Clear();
    foreach (var kvp in Data)
    {
      if (!string.IsNullOrEmpty(kvp.Value.name))
        PlayerIds[kvp.Value.name] = kvp.Key;
    }
    VanityManager.Load();
    Vanity.Log.LogInfo($"Reloading {Data.Count} vanity data.");
  }
  public static void SetupWatcher()
  {
    Watcher.Setup(FileName, FromFile);
  }

  public static IDeserializer Deserializer() => new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
    .WithTypeConverter(new FloatConverter()).Build();
  public static IDeserializer DeserializerUnSafe() => new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
  .WithTypeConverter(new FloatConverter()).IgnoreUnmatchedProperties().Build();
  public static ISerializer Serializer() => new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).DisableAliases()
    .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults).WithTypeConverter(new FloatConverter()).Build();

  public static T Deserialize<T>(string raw, string fileName) where T : new()
  {
    try
    {
      return Deserializer().Deserialize<T>(raw);
    }
    catch (Exception ex1)
    {
      Vanity.Log.LogError($"{fileName}: {ex1.Message}");
      try
      {
        return DeserializerUnSafe().Deserialize<T>(raw);
      }
      catch (Exception)
      {
        return new();
      }
    }
  }

  public static Dictionary<long, T> Read<T>(string pattern)
  {
    Dictionary<long, T> ret = [];
    foreach (var name in Directory.GetFiles(Paths.ConfigPath, pattern))
    {
      var data = Deserialize<Dictionary<long, T>>(File.ReadAllText(name), name);
      if (data == null) continue;
      foreach (var kvp in data)
        ret[kvp.Key] = kvp.Value;
    }
    return ret;
  }
  private static bool UpdateName(string id, string name)
  {
    if (Data.TryGetValue(id, out var entry))
    {
      if (entry.name == name) return false;
      entry.name = name;
      return true;
    }
    VanityData.Data[id] = new() { name = name };
    return true;
  }
  private static bool UpdateNetworkId(string id, string networkId)
  {
    if (Data.TryGetValue(id, out var entry))
    {
      if (entry.player == networkId) return false;
      entry.player = networkId;
      return true;
    }
    VanityData.Data[id] = new() { player = networkId };
    return true;
  }
  public static void UpdatePlayerIds()
  {
    var zm = ZDOMan.instance;
    if (zm == null) return;
    if (!ZNet.instance || !ZNet.instance.IsServer()) return;
    var updated = false;
    updated |= UpdateName("Everyone", "Everyone");
    if (!ZNet.instance.IsDedicated())
    {
      updated |= UpdateName(Game.instance.GetPlayerProfile().GetPlayerID().ToString(), Game.instance.GetPlayerProfile().GetName());
      updated |= UpdateNetworkId(Game.instance.GetPlayerProfile().GetPlayerID().ToString(), Helper.GetNetworkId());
    }

    foreach (var peer in zm.m_peers)
    {
      var zdo = zm.GetZDO(peer.m_peer.m_characterID);
      if (zdo == null) return;
      var id = zdo.GetLong(ZDOVars.s_playerID);
      var name = zdo.GetString(ZDOVars.s_playerName);
      if (id == 0 || name == "") continue;
      updated |= UpdateName(id.ToString(), name);
      updated |= UpdateNetworkId(id.ToString(), peer.m_peer.m_rpc.GetSocket().GetHostName());
    }
    if (updated) ToFile();
  }
}
#nullable disable
public class FloatConverter : IYamlTypeConverter
{
  public bool Accepts(Type type) => type == typeof(float);

  public object ReadYaml(IParser parser, Type type)
  {
    var scalar = (YamlDotNet.Core.Events.Scalar)parser.Current;
    var number = float.Parse(scalar.Value, NumberStyles.Float, CultureInfo.InvariantCulture);
    parser.MoveNext();
    return number;
  }

  public void WriteYaml(IEmitter emitter, object value, Type type)
  {
    var number = (float)value;
    emitter.Emit(new YamlDotNet.Core.Events.Scalar(number.ToString("0.###", CultureInfo.InvariantCulture)));
  }
}

#nullable enable
