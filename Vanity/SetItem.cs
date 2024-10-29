
using HarmonyLib;
using UnityEngine;
namespace Vanity;

[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.SetupVisEquipment))]
public class SetupVisEquipment
{
  static void Prefix(Humanoid __instance) => SetEquipment.IsHidden = __instance.m_hiddenLeftItem != null || __instance.m_hiddenRightItem != null;

}
[HarmonyPatch(typeof(VisEquipment)), HarmonyPriority(Priority.Low)]
public class SetEquipment
{
  public static bool IsHidden = false;


  [HarmonyPatch(nameof(VisEquipment.SetRightItem)), HarmonyPrefix]
  static void SetRightItem(VisEquipment __instance, ref string name)
  {
    if (IsHidden) return;
    VanityManager.OverrideItem(__instance, VisSlot.HandRight, ref name);
  }
  [HarmonyPatch(nameof(VisEquipment.SetLeftItem)), HarmonyPrefix]
  static void SetLeftItem(VisEquipment __instance, ref string name, ref int variant)
  {
    if (IsHidden) return;
    VanityManager.OverrideItem(__instance, VisSlot.HandLeft, ref name, ref variant);
  }
  [HarmonyPatch(nameof(VisEquipment.SetRightBackItem)), HarmonyPrefix]
  static void SetRightBackItem(VisEquipment __instance, ref string name)
  {
    if (IsHidden)
      VanityManager.OverrideItem(__instance, VisSlot.HandRight, ref name);
    VanityManager.OverrideItem(__instance, VisSlot.BackRight, ref name);
  }
  [HarmonyPatch(nameof(VisEquipment.SetLeftBackItem)), HarmonyPrefix]
  static void SetLeftBackItem(VisEquipment __instance, ref string name, ref int variant)
  {
    if (IsHidden)
      VanityManager.OverrideItem(__instance, VisSlot.HandLeft, ref name, ref variant);
    VanityManager.OverrideItem(__instance, VisSlot.BackLeft, ref name, ref variant);
  }
  [HarmonyPatch(nameof(VisEquipment.SetChestItem)), HarmonyPrefix]
  static void SetChestItem(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Chest, ref name);
  }
  [HarmonyPatch(nameof(VisEquipment.SetLegItem)), HarmonyPrefix]
  static void SetLegItem(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Legs, ref name);
  }
  [HarmonyPatch(nameof(VisEquipment.SetHelmetItem)), HarmonyPrefix]
  static void SetHelmetItem(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Helmet, ref name);
  }
  [HarmonyPatch(nameof(VisEquipment.SetShoulderItem)), HarmonyPrefix]
  static void SetShoulderItem(VisEquipment __instance, ref string name, ref int variant)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Shoulder, ref name, ref variant);
  }
  [HarmonyPatch(nameof(VisEquipment.SetUtilityItem)), HarmonyPrefix]
  static void SetUtilityItem(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Utility, ref name);
  }
  [HarmonyPatch(nameof(VisEquipment.SetBeardItem)), HarmonyPrefix]
  static void SetBeardItem(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Beard, ref name);
  }
  [HarmonyPatch(nameof(VisEquipment.SetHairItem)), HarmonyPrefix]
  static void SetHairItem(VisEquipment __instance, ref string name)
  {
    VanityManager.OverrideItem(__instance, VisSlot.Hair, ref name);
  }
  [HarmonyPatch(nameof(VisEquipment.SetSkinColor)), HarmonyPrefix]
  static void SetSkinColor(VisEquipment __instance, ref Vector3 color)
  {
    VanityManager.OverrideSkinColor(__instance, ref color);
  }
  [HarmonyPatch(nameof(VisEquipment.SetHairColor)), HarmonyPrefix]
  static void SetHairColor(VisEquipment __instance, ref Vector3 color)
  {
    VanityManager.OverrideHairColor(__instance, ref color);
  }
}

[HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), typeof(ItemDrop.ItemData), typeof(int), typeof(bool), typeof(float), typeof(int))]
public class Tooltip
{
  static string Postfix(string result, ItemDrop.ItemData item, bool crafting)
  {
    if (item == null) return result;
    if (crafting) return result;
    string name = "";
    int variant = 0;
    VanityManager.OverrideItem(item, ref name, ref variant);
    if (variant != 0) name += " " + variant;
    if (name != "") return result + $"\nStyle: <color=orange>{name}</color>";
    return result;
  }
}
