using HarmonyLib;
using UnityEngine;

namespace Vanity;

[HarmonyPatch]
public class IconPatches
{
  static Sprite? OverrideItem(ItemDrop.ItemData item)
  {
    string name = "";
    int variant = 0;
    VanityManager.OverrideItem(item, ref name, ref variant);
    if (name == "") return null;
    var data = ObjectDB.instance.GetItemPrefab(name)?.GetComponent<ItemDrop>()?.m_itemData;
    if (data == null || data.m_shared.m_icons == null || data.m_shared.m_icons.Length <= variant) return null;
    return data.m_shared.m_icons[variant];
  }

  [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetIcon)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
  static Sprite GetIcon(Sprite result, ItemDrop.ItemData __instance)
  {
    return OverrideItem(__instance) ?? result;
  }

  [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.SetupUpgradeItem)), HarmonyPostfix, HarmonyPriority(Priority.Last)]

  static void SetupUpgradeItem(InventoryGui __instance, Recipe recipe, ItemDrop.ItemData item)
  {
    item ??= recipe.m_item.m_itemData;
    __instance.m_upgradeItemIcon.sprite = OverrideItem(item) ?? __instance.m_upgradeItemIcon.sprite;
  }
  [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateRecipe)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
  static void UpdateRecipe(InventoryGui __instance)
  {
    if (!__instance.m_selectedRecipe.Key) return;
    var item = __instance.m_selectedRecipe.Key.m_item.m_itemData;
    __instance.m_recipeIcon.sprite = OverrideItem(item) ?? __instance.m_recipeIcon.sprite;
  }
}
