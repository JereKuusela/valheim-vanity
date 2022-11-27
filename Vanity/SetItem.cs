
using HarmonyLib;
using UnityEngine;
namespace Vanity;
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetLeftItem))]
public class SetLeftItem
{
  static void Prefix(VisEquipment __instance, ref string name, ref int variant)
  {
    VanityManager.OverrideItem(__instance, VisSlot.HandLeft, ref name, ref variant);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetRightItem))]
public class SetRightItem
{
  static void Prefix(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.HandRight, ref name);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetLeftBackItem))]
public class SetLeftBackItem
{
  static void Prefix(VisEquipment __instance, ref string name, ref int variant)
  {
    VanityManager.OverrideItem(__instance, VisSlot.BackLeft, ref name, ref variant);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetRightBackItem))]
public class SetRightBackItem
{
  static void Prefix(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.BackRight, ref name);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetChestItem))]
public class SetChestItem
{
  static void Prefix(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Chest, ref name);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetLegItem))]
public class SetLegItem
{
  static void Prefix(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Legs, ref name);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetHelmetItem))]
public class SetHelmetItem
{
  static void Prefix(VisEquipment __instance, ref string name)
  {
    if (Helper.IsLocalPlayer(__instance))
    {
      VanityManager.OverrideItem(__instance, VisSlot.Helmet, ref name);
      // Compatibility with Wearable Trophies.
      var trophy = Helper.GetEquippedTrophy(__instance.GetComponent<Player>()?.m_inventory);
      if (trophy != null) name = Helper.Name(trophy);
    }
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetShoulderItem))]
public class SetShoulderItem
{
  static void Prefix(VisEquipment __instance, ref string name, ref int variant)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Shoulder, ref name, ref variant);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetUtilityItem))]
public class SetUtilityItem
{
  static void Prefix(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Utility, ref name);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetBeardItem))]
public class SetBeardItem
{
  static void Prefix(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Beard, ref name);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetHairItem))]
public class SetHairItem
{
  static void Prefix(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Hair, ref name);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetSkinColor))]
public class SetSkinColor
{
  static void Prefix(VisEquipment __instance, ref Vector3 color)
  {
    VanityManager.OverrideSkinColor(__instance, ref color);
  }
}
[HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetHairColor))]
public class SetHairColor
{
  static void Prefix(VisEquipment __instance, ref Vector3 color)
  {
    VanityManager.OverrideHairColor(__instance, ref color);
  }
}

[HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), typeof(ItemDrop.ItemData), typeof(int), typeof(bool))]
public class Tooltip
{
  static string Postfix(string result, ItemDrop.ItemData item, bool crafting)
  {
    if (crafting) return result;
    string name = "";
    int variant = 0;
    VanityManager.OverrideItem(item, ref name, ref variant);
    if (variant != 0) name += " " + variant;
    if (name != "") return result + $"\nStyle: <color=orange>{name}</color>";
    return result;
  }
}