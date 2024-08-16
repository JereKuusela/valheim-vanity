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
    var networkId = Helper.GetNetworkId();
    Load(Info, "Everyone");
    Load(Info, "0");
    Load(Info, networkId);
    Load(Info, id);
    if (Game.instance && Game.instance.m_shuttingDown) return;
    if (FejdStartup.m_instance && FejdStartup.m_instance.m_playerInstance)
      FejdStartup.m_instance.m_playerInstance.GetComponent<Player>()?.SetupEquipment();
    if (Player.m_localPlayer)
      Player.m_localPlayer.SetupEquipment();
  }
  public static VanityInfo Info = new();
  public static Dictionary<string, Dictionary<string, Tuple<string, int>>> Crafted = [];
  public static Color? SkinColor;
  public static Color? HairColor;
  public static float ColorUpdateInterval => Info.updateInterval ?? 0.1f;
  public static float ColorDuration => Info.colorDuration ?? 1f;
  private static void LoadCrafted()
  {
    Crafted = [];
    foreach (var entry in VanityData.Data)
    {
      if (!Crafted.ContainsKey(entry.Key))
        Crafted[entry.Key] = [];
      foreach (var crafted in entry.Value.crafted)
        Crafted[entry.Key][crafted.Key] = Helper.Parse(crafted.Value);

    }
  }
  private static void Load(VanityInfo info, string id)
  {
    if (!VanityData.Data.TryGetValue(id, out var data)) return;
    if (!string.IsNullOrEmpty(data.group))
    {
      var groups = Helper.ParseGroups(data.group);
      foreach (var group in groups)
        Load(info, group);
    }
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
    return slot switch
    {
      VisSlot.BackLeft => Info.leftBack,
      VisSlot.BackRight => Info.rightBack,
      VisSlot.Beard => Info.beard,
      VisSlot.Chest => Info.chest,
      VisSlot.Hair => Info.hair,
      VisSlot.HandLeft => Info.leftHand,
      VisSlot.HandRight => Info.rightHand,
      VisSlot.Helmet => Info.helmet,
      VisSlot.Legs => Info.legs,
      VisSlot.Shoulder => Info.shoulder,
      VisSlot.Utility => Info.utility,
      _ => null,
    };
  }

  public static Color[] GetSkinColors(Player obj) => Helper.ParseColors(Info.skinColor, obj.m_skinColor);
  public static Color[] GetHairColors(Player obj) => Helper.ParseColors(Info.hairColor, obj.m_hairColor);
  private static ItemDrop.ItemData? GetEquipment(Player player, VisSlot slot)
  {
    return slot switch
    {
      VisSlot.BackLeft => player.m_leftItem,
      VisSlot.BackRight => player.m_rightItem,
      VisSlot.Beard => null,
      VisSlot.Chest => player.m_chestItem,
      VisSlot.Hair => null,
      VisSlot.HandLeft => player.m_leftItem,
      VisSlot.HandRight => player.m_rightItem,
      VisSlot.Helmet => player.m_helmetItem,
      VisSlot.Legs => player.m_legItem,
      VisSlot.Shoulder => player.m_shoulderItem,
      VisSlot.Utility => player.m_utilityItem,
      _ => throw new NotImplementedException(),
    };
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
    if (item != null && Crafted.TryGetValue(item.m_crafterID.ToString(), out var craftedGear))
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
    return item.m_shared.m_itemType switch
    {
      ItemDrop.ItemData.ItemType.OneHandedWeapon => (VisSlot?)VisSlot.HandRight,
      ItemDrop.ItemData.ItemType.Bow => (VisSlot?)VisSlot.HandLeft,
      ItemDrop.ItemData.ItemType.Chest => (VisSlot?)VisSlot.Chest,
      ItemDrop.ItemData.ItemType.Helmet => (VisSlot?)VisSlot.Helmet,
      ItemDrop.ItemData.ItemType.Legs => (VisSlot?)VisSlot.Legs,
      ItemDrop.ItemData.ItemType.Shield => (VisSlot?)VisSlot.HandLeft,
      ItemDrop.ItemData.ItemType.Tool => (VisSlot?)VisSlot.HandRight,
      ItemDrop.ItemData.ItemType.Shoulder => (VisSlot?)VisSlot.Shoulder,
      ItemDrop.ItemData.ItemType.Torch => (VisSlot?)VisSlot.HandRight,
      ItemDrop.ItemData.ItemType.TwoHandedWeapon => (VisSlot?)VisSlot.HandRight,
      ItemDrop.ItemData.ItemType.Utility => (VisSlot?)VisSlot.Utility,
      _ => null,
    };
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
    if (Crafted.TryGetValue(item.m_crafterID.ToString(), out var craftedGear))
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
