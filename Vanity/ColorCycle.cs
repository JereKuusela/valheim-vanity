using System;
using HarmonyLib;
using UnityEngine;
namespace Vanity;
///<summary>Cycles the color between given values.</summary>
[HarmonyPatch(typeof(Player), nameof(Player.LateUpdate))]
public class ColorCycle {
  private const float TickLength = 0.02f;
  private static int Ticks = 0;
  private static int SecondsToTicks(float value) => (int)(value / TickLength);
  private static Color GetColor(Color[] colors) {
    if (colors.Length < 2) return colors[0];

    var ticksPerColor = SecondsToTicks(VanityManager.ColorDuration);
    if (ticksPerColor == 0) return colors[0];
    var leftOver = Ticks % (ticksPerColor * colors.Length);
    var progress = (float)leftOver / ticksPerColor;

    var index1 = (int)Math.Floor(progress);
    var color1 = colors[index1];
    var index2 = (int)Math.Ceiling(progress);
    if (index2 >= colors.Length) index2 = 0;
    var color2 = colors[index2];

    var percentage = progress - (float)Math.Truncate(progress);
    return Color.Lerp(color1, color2, percentage);
  }
  private static void UpdateSkinColor(Player obj) {
    var color = GetColor(VanityManager.GetSkinColors(obj));
    VanityManager.SkinColor = color;
    // Color doesn't matter as it gets overridden.
    obj.m_visEquipment.SetSkinColor(Vector3.zero);
  }
  private static void UpdateHairColor(Player obj) {
    var color = GetColor(VanityManager.GetHairColors(obj));
    VanityManager.HairColor = color;
    // Color doesn't matter as it gets overridden.
    obj.m_visEquipment.SetHairColor(Vector3.zero);
  }
  static void Postfix(Player __instance) {
    if (!Helper.IsLocalPlayer(__instance)) return;
    Ticks++;
    var granularity = SecondsToTicks(VanityManager.ColorUpdateInterval);
    if (Ticks % granularity != 0) return;
    UpdateSkinColor(__instance);
    UpdateHairColor(__instance);
  }
}
