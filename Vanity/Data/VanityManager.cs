using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vanity;

public class VanityManager
{
  public static void Load()
  {
    Info = new();
    LoadCrafted();
    var id = Helper.GetPlayerID();
    Load(Info, 0);
    Load(Info, id);
    FejdStartup.m_instance?.m_playerInstance?.GetComponent<Player>()?.SetupEquipment();
    Player.m_localPlayer?.SetupEquipment();
  }
  public static VanityInfo Info = new();
  public static Dictionary<long, Dictionary<string, Tuple<string, int>>> Crafted = new();
  public static Color? SkinColor;
  public static Color? HairColor;
  public static float ColorUpdateInterval => Info.updateInterval ?? 0.1f;
  public static float ColorDuration => Info.colorDuration ?? 1f;
  private static void LoadCrafted()
  {
    Crafted = new();
    foreach (var entry in VanityData.Data)
    {
      if (!Crafted.ContainsKey(entry.Key))
        Crafted[entry.Key] = new();
      foreach (var crafted in entry.Value.crafted)
        Crafted[entry.Key][crafted.Key] = Helper.Parse(crafted.Value);

    }
  }
  private static void Load(VanityInfo info, long id)
  {
    if (!VanityData.Data.TryGetValue(id, out var data)) return;
    if (!string.IsNullOrEmpty(data.beard)) info.beard = Helper.Parse(data.beard);
    if (!string.IsNullOrEmpty(data.chest)) info.chest = Helper.Parse(data.chest);
    if (data.colorDuration.HasValue) info.colorDuration = data.colorDuration;
    if (!string.IsNullOrEmpty(data.hair)) info.hair = Helper.Parse(data.hair);
    if (!string.IsNullOrEmpty(data.hairColor)) info.hairColor = data.hairColor;
    if (!string.IsNullOrEmpty(data.helmet)) info.helmet = Helper.Parse(data.helmet);
    if (!string.IsNullOrEmpty(data.leftBack)) info.leftBack = Helper.Parse(data.leftBack);
    if (!string.IsNullOrEmpty(data.leftHand)) info.leftHand = Helper.Parse(data.leftHand);
    if (!string.IsNullOrEmpty(data.legs)) info.legs = Helper.Parse(data.legs);
    if (!string.IsNullOrEmpty(data.rightBack)) info.rightBack = Helper.Parse(data.rightBack);
    if (!string.IsNullOrEmpty(data.rightHand)) info.rightHand = Helper.Parse(data.rightHand);
    if (!string.IsNullOrEmpty(data.shoulder)) info.shoulder = Helper.Parse(data.shoulder);
    if (!string.IsNullOrEmpty(data.skinColor)) info.skinColor = data.skinColor;
    if (data.updateInterval.HasValue) info.updateInterval = data.updateInterval;
    if (!string.IsNullOrEmpty(data.utility)) info.utility = Helper.Parse(data.utility);
    foreach (var gear in data.gear)
      info.gear[gear.Key] = Helper.Parse(gear.Value);
  }

  public static Tuple<string, int>? GetVisualBySlot(VisSlot slot)
  {
    switch (slot)
    {
      case VisSlot.BackLeft:
        return Info.leftBack;
      case VisSlot.BackRight:
        return Info.rightBack;
      case VisSlot.Beard:
        return Info.beard;
      case VisSlot.Chest:
        return Info.chest;
      case VisSlot.Hair:
        return Info.hair;
      case VisSlot.HandLeft:
        return Info.leftHand;
      case VisSlot.HandRight:
        return Info.rightHand;
      case VisSlot.Helmet:
        return Info.helmet;
      case VisSlot.Legs:
        return Info.legs;
      case VisSlot.Shoulder:
        return Info.shoulder;
      case VisSlot.Utility:
        return Info.utility;
    }
    return null;
  }

  public static Color[] GetSkinColors(Player obj) => Helper.ParseColors(Info.skinColor, obj.m_skinColor);
  public static Color[] GetHairColors(Player obj) => Helper.ParseColors(Info.hairColor, obj.m_hairColor);
  private static ItemDrop.ItemData? GetEquipment(Player player, VisSlot slot)
  {
    switch (slot)
    {
      case VisSlot.BackLeft:
        return player.m_leftItem;
      case VisSlot.BackRight:
        return player.m_rightItem;
      case VisSlot.Beard:
        return null;
      case VisSlot.Chest:
        return player.m_chestItem;
      case VisSlot.Hair:
        return null;
      case VisSlot.HandLeft:
        return player.m_leftItem;
      case VisSlot.HandRight:
        return player.m_rightItem;
      case VisSlot.Helmet:
        return player.m_helmetItem;
      case VisSlot.Legs:
        return player.m_legItem;
      case VisSlot.Shoulder:
        return player.m_shoulderItem;
      case VisSlot.Utility:
        return player.m_utilityItem;
    }
    throw new NotImplementedException();
  }
  public static void OverrideItem(VisEquipment vis, VisSlot slot, ref string name, ref int variant)
  {
    if (!Helper.IsLocalPlayer(vis)) return;
    var player = vis.GetComponent<Player>();
    var item = GetEquipment(player, slot);
    var visual = GetVisualBySlot(slot);
    if (visual != null)
    {
      name = visual.Item1;
      variant = visual.Item2;
      return;
    }
    if (item != null && Crafted.TryGetValue(item.m_crafterID, out var craftedGear))
    {
      if (craftedGear.TryGetValue(name, out var crafted))
      {
        name = crafted.Item1;
        variant = crafted.Item2;
        return;
      }
    }
    if (Info.gear.TryGetValue(name, out var gear))
    {
      name = gear.Item1;
      variant = gear.Item2;
      return;
    }
  }
  private static VisSlot? GetSlot(ItemDrop.ItemData item)
  {
    switch (item.m_shared.m_itemType)
    {
      case ItemDrop.ItemData.ItemType.OneHandedWeapon:
        return VisSlot.HandRight;
      case ItemDrop.ItemData.ItemType.Bow:
        return VisSlot.HandLeft;
      case ItemDrop.ItemData.ItemType.Chest:
        return VisSlot.Chest;
      case ItemDrop.ItemData.ItemType.Helmet:
        return VisSlot.Helmet;
      case ItemDrop.ItemData.ItemType.Legs:
        return VisSlot.Legs;
      case ItemDrop.ItemData.ItemType.Shield:
        return VisSlot.HandLeft;
      case ItemDrop.ItemData.ItemType.Tool:
        return VisSlot.HandRight;
      case ItemDrop.ItemData.ItemType.Shoulder:
        return VisSlot.Shoulder;
      case ItemDrop.ItemData.ItemType.Torch:
        return VisSlot.HandRight;
      case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
        return VisSlot.HandRight;
      case ItemDrop.ItemData.ItemType.Utility:
        return VisSlot.Utility;
    }
    return null;
  }
  public static void OverrideItem(ItemDrop.ItemData item, ref string name, ref int variant)
  {
    var slot = GetSlot(item);
    if (slot == null) return;
    var visual = GetVisualBySlot(slot.Value);
    if (visual != null)
    {
      name = visual.Item1;
      variant = visual.Item2;
      return;
    }
    if (Crafted.TryGetValue(item.m_crafterID, out var craftedGear))
    {
      if (item.m_dropPrefab && craftedGear.TryGetValue(item.m_dropPrefab.name, out var crafted))
      {
        name = crafted.Item1;
        variant = crafted.Item2;
        return;
      }
    }
    if (item.m_dropPrefab && Info.gear.TryGetValue(item.m_dropPrefab.name, out var gear))
    {
      name = gear.Item1;
      variant = gear.Item2;
      return;
    }
  }

  public static void OverrideItem(VisEquipment vis, VisSlot slot, ref string name)
  {
    int variant = 0;
    OverrideItem(vis, slot, ref name, ref variant);
  }

  public static void OverrideSkinColor(VisEquipment vis, ref Vector3 color)
  {
    if (!Helper.IsLocalPlayer(vis)) return;
    if (SkinColor == null) return;
    color.x = SkinColor.Value.r;
    color.y = SkinColor.Value.g;
    color.z = SkinColor.Value.b;
  }
  public static void OverrideHairColor(VisEquipment vis, ref Vector3 color)
  {
    if (!Helper.IsLocalPlayer(vis)) return;
    if (HairColor == null) return;
    color.x = HairColor.Value.r;
    color.y = HairColor.Value.g;
    color.z = HairColor.Value.b;
  }
}
