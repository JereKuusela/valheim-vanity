using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
namespace Vanity;
[HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
public class ObjectDB_Awake
{
  static void Postfix(ObjectDB __instance)
  {
    ObjectData.Beards = __instance.GetAllItems(ItemDrop.ItemData.ItemType.Customization, "Beard").Select(item => item.name).ToList();
    ObjectData.Hairs = __instance.GetAllItems(ItemDrop.ItemData.ItemType.Customization, "Hair").Select(item => item.name).ToList();
    ObjectData.Items = __instance.m_items.Select(item => item.name).ToList();
  }
}

public class ObjectData
{
  public static List<string> Beards = [];
  public static List<string> Hairs = [];
  public static List<string> Items = [];
}
