using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Vanity;
public static class Helper {
  public static List<string> Players() {
    return VanityData.PlayerIds.Keys.Select(s => s.Replace(" ", "_")).OrderBy(s => s).ToList();
  }
  private static string Normalize(string name) => name.ToLower().Replace(" ", "_");
  public static string GetPlayerID(string name) {
    name = name.ToLower().Replace(" ", "_");
    var ids = VanityData.PlayerIds.ToDictionary(kvp => Normalize(kvp.Key), kvp => kvp.Value);
    if (ids.TryGetValue(name, out var id)) return id;
    var key = ids.Keys.FirstOrDefault(s => s.StartsWith(name, StringComparison.OrdinalIgnoreCase));
    if (!string.IsNullOrEmpty(key)) return ids[key];
    key = ids.Keys.FirstOrDefault(s => s.Contains(name));
    if (!string.IsNullOrEmpty(key)) return ids[key];
    throw new InvalidOperationException("Unable to find the player.");
  }
  public static bool IsLocalPlayer(VisEquipment obj) => obj && obj.m_isPlayer && IsLocalPlayer(obj.GetComponent<Player>());
  public static bool IsLocalPlayer(Player obj) => obj && (obj == Player.m_localPlayer || obj.GetZDOID() == ZDOID.None);
  public static bool IsTrophy(ItemDrop.ItemData item) => item != null && Helper.Name(item).ToLower().Contains("trophy");
  public static string Name(ItemDrop.ItemData item) => item?.m_dropPrefab?.name ?? "";
  public static ItemDrop.ItemData? GetEquippedTrophy(Inventory? inventory) {
    if (inventory == null) return null;
    var items = inventory.m_inventory;
    if (items == null) return null;
    return items.FirstOrDefault(item => Helper.IsTrophy(item) && item.m_equipped);
  }
  public static float TryFloat(string arg, float defaultValue = 1) {
    if (!float.TryParse(arg, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
      return defaultValue;
    return result;
  }
  public static float TryFloat(string[] args, int index, float defaultValue = 1) {
    if (index >= args.Length) return defaultValue;
    return TryFloat(args[index], defaultValue);
  }
  public static int TryInt(string arg, int defaultValue = 1) {
    if (!int.TryParse(arg, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
      return defaultValue;
    return result;
  }
  public static int TryInt(string[] args, int index, int defaultValue = 1) {
    if (index >= args.Length) return defaultValue;
    return TryInt(args[index], defaultValue);
  }

  public static Tuple<string, int> Parse(string value) {
    var separators = new char[] { ',', ' ' };
    var split = value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToArray();
    var name = split[0];
    var variant = Helper.TryInt(split, 1, 0);
    return new(name, variant);
  }
  public static string[] ParseGroups(string value) {
    var separators = new char[] { ',', ' ' };
    return value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToArray();
  }
  public static Color ParseColor(string value, Color baseColor) {
    var split = value.Split(',');
    Color color = baseColor;
    color.r = Helper.TryFloat(split, 0, color.r);
    color.g = Helper.TryFloat(split, 1, color.g);
    color.b = Helper.TryFloat(split, 2, color.b);
    return color;
  }
  public static Color[] ParseColors(string value, Vector3 baseColor) {
    var values = value.Split(' ');
    Color color = new(baseColor.x, baseColor.y, baseColor.z);
    return values.Select(value => ParseColor(value, color)).ToArray();
  }

  public static string GetPlayerID() => Player.m_localPlayer?.GetPlayerID().ToString() ?? CharacterPreview.Id;
  public static string GetNetworkId() => PrivilegeManager.privilegeData == null ? "0" : PrivilegeManager.PlatformUserId.ToString();

}



[HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.SetupCharacterPreview))]
public class CharacterPreview {
  public static string Id = "";
  static void Prefix(PlayerProfile profile) {
    if (profile == null)
      Id = "";
    else
      Id = profile.m_playerID.ToString();
    VanityManager.Load();
  }
}

[HarmonyPatch(typeof(Player), nameof(Player.Awake))]
public class LoadVanity {
  static void Postfix() {
    VanityManager.Load();
  }
}
