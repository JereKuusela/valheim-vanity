using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Vanity;
public class ChangeEquipment : MonoBehaviour
{
  private static Stack<string> UndoCommands = new();
  private static string Get(VanityEntry entry, VisSlot slot)
  {
    switch (slot)
    {
      case VisSlot.BackLeft:
        return entry.leftBack;
      case VisSlot.BackRight:
        return entry.rightBack;
      case VisSlot.Beard:
        return entry.beard;
      case VisSlot.Chest:
        return entry.chest;
      case VisSlot.Hair:
        return entry.hair;
      case VisSlot.HandLeft:
        return entry.leftHand;
      case VisSlot.HandRight:
        return entry.rightHand;
      case VisSlot.Helmet:
        return entry.helmet;
      case VisSlot.Legs:
        return entry.legs;
      case VisSlot.Shoulder:
        return entry.shoulder;
      case VisSlot.Utility:
        return entry.utility;
    }
    throw new NotImplementedException();
  }
  private static string GetColor(VanityEntry entry, VisSlot slot)
  {
    switch (slot)
    {
      case VisSlot.Hair:
        return entry.hairColor;
      case VisSlot.Legs:
        return entry.skinColor;
    }
    throw new NotImplementedException();
  }
  private static void Set(VanityEntry entry, VisSlot slot, string value)
  {
    switch (slot)
    {
      case VisSlot.BackLeft:
        entry.leftBack = value;
        return;
      case VisSlot.BackRight:
        entry.rightBack = value;
        return;
      case VisSlot.Beard:
        entry.beard = value;
        return;
      case VisSlot.Chest:
        entry.chest = value;
        return;
      case VisSlot.Hair:
        entry.hair = value;
        return;
      case VisSlot.HandLeft:
        entry.leftHand = value;
        return;
      case VisSlot.HandRight:
        entry.rightHand = value;
        return;
      case VisSlot.Helmet:
        entry.helmet = value;
        return;
      case VisSlot.Legs:
        entry.legs = value;
        return;
      case VisSlot.Shoulder:
        entry.shoulder = value;
        return;
      case VisSlot.Utility:
        entry.utility = value;
        return;
    }
    throw new NotImplementedException();
  }
  private static void SetColor(VanityEntry entry, VisSlot slot, string value)
  {
    switch (slot)
    {
      case VisSlot.Hair:
        entry.hairColor = value;
        return;
      case VisSlot.Legs:
        entry.skinColor = value;
        return;

    }
  }
  private static VanityEntry GetEntry(string id)
  {
    VanityEntry entry = new();
    if (VanityData.Data.TryGetValue(id, out var value))
      entry = value;
    else
      VanityData.Data[id] = entry;
    return entry;
  }
  private static string[] AddPlayerId(Terminal.ConsoleEventArgs args)
  {
    var name = args.Args.FirstOrDefault(s => s.StartsWith("player=", StringComparison.OrdinalIgnoreCase));
    if (string.IsNullOrEmpty(name)) return args.Args.Append("player=" + Helper.GetPlayerID()).ToArray();
    return args.Args;
  }
  private static string[] Parse(string[] args, out string id)
  {
    id = "";
    var name = args.FirstOrDefault(s => s.StartsWith("player=", StringComparison.OrdinalIgnoreCase));
    if (name == null) return args;
    var split = name.Split('=');
    var parsed = args.Where(s => !s.StartsWith("player=", StringComparison.OrdinalIgnoreCase)).ToArray();
    if (split.Length < 2) throw new InvalidOperationException("Missing player id");
    if (long.TryParse(split[1], out var playerId))
      id = playerId.ToString();
    else
      id = Helper.GetPlayerID(split[1]);
    return parsed;
  }
  private static void SetVisualValue(VisSlot slot, Terminal.ConsoleEventArgs args)
  {
    var argsWithId = AddPlayerId(args);
    var values = Parse(argsWithId, out var id);
    var value = string.Join(" ", values.Skip(1));
    var entry = GetEntry(id);
    var previous = Get(entry, slot);
    UndoCommands.Push(values[0] + " " + previous);
    if (Vanity.ConfigSync.IsSourceOfTruth)
    {
      Set(entry, slot, value);
      VanityData.ToFile();
    }
    else ServerExecution.Send(argsWithId);
  }
  private static void SetColorValue(VisSlot slot, Terminal.ConsoleEventArgs args)
  {
    var argsWithId = AddPlayerId(args);
    var values = Parse(argsWithId, out var id);
    var value = string.Join(" ", values.Skip(1));
    var entry = GetEntry(id);
    var previous = GetColor(entry, slot);
    UndoCommands.Push(values[0] + " " + previous);

    if (Vanity.ConfigSync.IsSourceOfTruth)
    {
      SetColor(entry, slot, value);
      VanityData.ToFile();
    }
    else ServerExecution.Send(argsWithId);
  }
  private static void SetDurationValue(Terminal.ConsoleEventArgs args)
  {
    var argsWithId = AddPlayerId(args);
    var values = Parse(argsWithId, out var id);
    var value = Helper.TryFloat(string.Join(" ", values.Skip(1)), 1f);
    var entry = GetEntry(id);
    UndoCommands.Push(values[0] + " " + entry.colorDuration);

    if (Vanity.ConfigSync.IsSourceOfTruth)
    {
      entry.colorDuration = value;
      VanityData.ToFile();
    }
    else ServerExecution.Send(argsWithId);
  }
  private static void SetIntervalValue(Terminal.ConsoleEventArgs args)
  {
    var argsWithId = AddPlayerId(args);
    var values = Parse(argsWithId, out var id);
    var value = Helper.TryFloat(string.Join(" ", values.Skip(1)), 1f);
    var entry = GetEntry(id);
    UndoCommands.Push(values[0] + " " + entry.updateInterval);

    if (Vanity.ConfigSync.IsSourceOfTruth)
    {
      entry.updateInterval = value;
      VanityData.ToFile();
    }
    else ServerExecution.Send(argsWithId);
  }
  private static string SlotToString(VisSlot slot)
  {
    switch (slot)
    {
      case VisSlot.BackLeft:
        return "back_left";
      case VisSlot.BackRight:
        return "back_right";
      case VisSlot.Beard:
        return "beard";
      case VisSlot.Chest:
        return "chest";
      case VisSlot.Hair:
        return "hair";
      case VisSlot.HandLeft:
        return "left";
      case VisSlot.HandRight:
        return "right";
      case VisSlot.Helmet:
        return "helmet";
      case VisSlot.Legs:
        return "legs";
      case VisSlot.Shoulder:
        return "shoulder";
      case VisSlot.Utility:
        return "utility";
    }
    throw new NotImplementedException();
  }
  private static void RegisterGearAutoComplete(string name)
  {
    CommandWrapper.Register(name, (int index) =>
    {
      if (index == 0) return ObjectData.Items;
      if (index == 1) return ObjectData.Items;
      if (index == 2) return CommandWrapper.Info("Item variant (number).");
      return new() { "player" };
    }, new() {
      { "player", (int index) => index == 0 ? Helper.Players() : null}
    });
  }
  private static void RegisterAutoComplete(string name, bool variant = true)
  {
    CommandWrapper.Register(name, (int index) =>
    {
      if (index == 0) return ObjectData.Items;
      if (index == 1 && variant) return CommandWrapper.Info("Item variant (number).");
      return new() { "player" };
    }, new() {
      { "player", (int index) => index == 0 ? Helper.Players() : null}
    });
  }
  private static void RegisterColorAutoComplete(string name)
  {
    CommandWrapper.Register(name, (int index) =>
    {
      return CommandWrapper.Info("Color (r,g,b)");
    }, new() {
      { "player", (int index) => index == 0 ? Helper.Players() : null}
    });
  }
  private static void CreateCommand(VisSlot slot)
  {
    RegisterAutoComplete("wear_" + SlotToString(slot));
    new Terminal.ConsoleCommand("wear_" + SlotToString(slot), "[item name] [variant = 0] - Changes visual equipment.", args =>
    {
      SetVisualValue(slot, args);
    }, optionsFetcher: () => ObjectData.Items);
  }
  public static void AddChangeEquipment()
  {
    new Terminal.ConsoleCommand("wear_info", "Prints information about visual equipment.", args =>
    {
      if (Player.m_localPlayer == null) return;
      var equipment = Player.m_localPlayer.GetComponent<VisEquipment>();
      if (equipment == null) return;
      args.Context.AddString("Helmet: " + equipment.m_helmetItem);
      args.Context.AddString("Shoulder: " + equipment.m_shoulderItem);
      args.Context.AddString("Chest: " + equipment.m_chestItem);
      args.Context.AddString("Leg: " + equipment.m_legItem);
      args.Context.AddString("Utility: " + equipment.m_utilityItem);
      args.Context.AddString("Left hand: " + equipment.m_leftItem);
      args.Context.AddString("Right hand: " + equipment.m_rightItem);
      args.Context.AddString("Left back: " + equipment.m_leftBackItem);
      args.Context.AddString("Right back: " + equipment.m_rightBackItem);
      args.Context.AddString("Hair: " + equipment.m_hairItem);
      args.Context.AddString("Beard: " + equipment.m_beardItem);
      args.Context.AddString("Skin: " + equipment.m_skinColor.ToString("F2"));
      args.Context.AddString("Hair color: " + equipment.m_hairColor.ToString("F2"));
    });
    new Terminal.ConsoleCommand("wear_reset", "Resets visual equipment.", args =>
    {
      var argsWithId = AddPlayerId(args);
      if (Vanity.ConfigSync.IsSourceOfTruth)
      {
        Parse(argsWithId, out var id);
        var entry = GetEntry(id);
        Set(entry, VisSlot.BackLeft, "");
        Set(entry, VisSlot.BackRight, "");
        Set(entry, VisSlot.Beard, "");
        Set(entry, VisSlot.Chest, "");
        Set(entry, VisSlot.Hair, "");
        Set(entry, VisSlot.HandLeft, "");
        Set(entry, VisSlot.Helmet, "");
        Set(entry, VisSlot.Helmet, "");
        Set(entry, VisSlot.Legs, "");
        Set(entry, VisSlot.Shoulder, "");
        Set(entry, VisSlot.Utility, "");
        Set(entry, VisSlot.Utility, "");
        SetColor(entry, VisSlot.Hair, "");
        SetColor(entry, VisSlot.Legs, "");
        VanityData.ToFile();
      }
      else ServerExecution.Send(argsWithId);
    });
    RegisterColorAutoComplete("wear_skin_color");
    new Terminal.ConsoleCommand("wear_skin_color", "[r1,g1,b1] [r2,g2,b2] ... - Changes skin color. Automatically cycles between multiple values.", args =>
    {
      SetColorValue(VisSlot.Legs, args);
    });
    RegisterColorAutoComplete("wear_hair_color");
    new Terminal.ConsoleCommand("wear_hair_color", "[r1,g1,b1] [r2,g2,b2] ... - Changes hair color. Automatically cycles between multiple values.", args =>
    {
      SetColorValue(VisSlot.Hair, args);
    });
    RegisterAutoComplete("wear_beard", false);
    new Terminal.ConsoleCommand("wear_beard", "[name] - Changes beard.", args =>
    {
      SetVisualValue(VisSlot.Beard, args);
    }, optionsFetcher: () => ObjectData.Beards);
    RegisterAutoComplete("wear_hair", false);
    new Terminal.ConsoleCommand("wear_hair", "[name] - Changes hair.", args =>
    {
      SetVisualValue(VisSlot.Hair, args);
    }, optionsFetcher: () => ObjectData.Hairs);
    RegisterGearAutoComplete("wear_gear");
    new Terminal.ConsoleCommand("wear_gear", "[item name] [visual name] [variant = 0] - Changes visual of a specific gear.", args =>
    {
      var argsWithId = AddPlayerId(args);
      var values = Parse(argsWithId, out var id);
      if (values.Length < 2) return;
      var entry = GetEntry(id);
      if (entry.gear.TryGetValue(values[1], out var previous))
        UndoCommands.Push(values[0] + " " + values[1] + " " + previous);
      else UndoCommands.Push(values[0] + " " + values[1]);

      if (Vanity.ConfigSync.IsSourceOfTruth)
      {
        if (values.Length == 2)
          entry.gear.Remove(values[1]);
        else
          entry.gear[args[1]] = string.Join(" ", values.Skip(2));
        VanityData.ToFile();
      }
      else ServerExecution.Send(argsWithId);
    }, optionsFetcher: () => ObjectData.Items);
    RegisterGearAutoComplete("wear_crafted");
    new Terminal.ConsoleCommand("wear_crafted", "[item name] [visual name] [variant = 0] - Changes visual of a specific gear.", args =>
    {
      var argsWithId = AddPlayerId(args);
      var values = Parse(argsWithId, out var id);
      var entry = GetEntry(id);
      if (entry.crafted.TryGetValue(values[1], out var previous))
        UndoCommands.Push(values[0] + " " + values[1] + " " + previous);
      else UndoCommands.Push(values[0] + " " + values[1]);

      if (Vanity.ConfigSync.IsSourceOfTruth)
      {
        entry.crafted[values[1]] = string.Join(" ", values.Skip(2));
        VanityData.ToFile();
      }
      else ServerExecution.Send(argsWithId);
    }, optionsFetcher: () => ObjectData.Items);
    new Terminal.ConsoleCommand("wear_undo", "Reverts wear commands.", args =>
    {
      if (UndoCommands.Count == 0)
      {
        args.Context.AddString("Nothing to undo.");
        return;
      }
      var command = UndoCommands.Pop();
      args.Context.TryRunCommand(command);
      // Removes the undo step caused by the undo.
      if (UndoCommands.Count > 0)
        UndoCommands.Pop();
    }, optionsFetcher: () => ObjectData.Items);
    CreateCommand(VisSlot.BackLeft);
    CreateCommand(VisSlot.BackRight);
    CreateCommand(VisSlot.Chest);
    CreateCommand(VisSlot.HandLeft);
    CreateCommand(VisSlot.HandRight);
    CreateCommand(VisSlot.Helmet);
    CreateCommand(VisSlot.Legs);
    CreateCommand(VisSlot.Shoulder);
    CreateCommand(VisSlot.Utility);
    new Terminal.ConsoleCommand("wear_color_interval", "Sets the color update interval.", SetIntervalValue);
    new Terminal.ConsoleCommand("wear_color_duration", "Sets the color duration.", SetDurationValue);
  }
}
