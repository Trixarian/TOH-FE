﻿using Hazel;

namespace TOHFE;

public static class NameNotifyManager
{
    public static readonly Dictionary<byte, (string, long)> Notice = [];
    public static void Reset() => Notice.Clear();
    public static bool Notifying(this PlayerControl pc) => Notice.ContainsKey(pc.PlayerId);
    public static void Notify(this PlayerControl pc, string text, float time = 4f, bool sendInLog = true)
    {
        if (!AmongUsClient.Instance.AmHost || pc == null) return;
        if (!GameStates.IsInTask) return;

        if (!text.Contains("<color=#")) text = Utils.ColorString(Utils.GetRoleColor(pc.GetCustomRole()), text);
        
        Notice.Remove(pc.PlayerId);
        Notice.Add(pc.PlayerId, new(text, Utils.GetTimeStamp() + (long)time));
        
        SendRPC(pc.PlayerId);
        Utils.NotifyRoles(SpecifySeer: pc, ForceLoop: false);

        if (sendInLog) Logger.Info($"New name notify for {pc.GetNameWithRole().RemoveHtmlTags()}: {text} ({time}s)", "Name Notify");
    }
    public static void OnFixedUpdate(PlayerControl player)
    {
        if (!GameStates.IsInTask)
        {
            if (Notice.Any()) Notice.Clear();
            return;
        }
        if (Notice.ContainsKey(player.PlayerId) && Notice[player.PlayerId].Item2 < Utils.GetTimeStamp())
        {
            Notice.Remove(player.PlayerId);
            Utils.NotifyRoles(SpecifySeer: player, ForceLoop: false);
        }
    }
    public static bool GetNameNotify(PlayerControl player, out string name)
    {
        name = string.Empty;
        if (!Notice.ContainsKey(player.PlayerId)) return false;
        name = Notice[player.PlayerId].Item1;
        return true;
    }
    private static void SendRPC(byte playerId)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncNameNotify, SendOption.Reliable, -1);
        writer.Write(playerId);
        if (Notice.ContainsKey(playerId))
        {
            writer.Write(true);
            writer.Write(Notice[playerId].Item1);
            writer.Write(Notice[playerId].Item2 - Utils.GetTimeStamp());
        }
        else writer.Write(false);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
    public static void ReceiveRPC(MessageReader reader)
    {
        byte PlayerId = reader.ReadByte();
        Notice.Remove(PlayerId);
        long now = Utils.GetTimeStamp();
        if (reader.ReadBoolean())
            Notice.Add(PlayerId, new(reader.ReadString(), now + (long)reader.ReadSingle()));
        Logger.Info($"New name notify for {Main.AllPlayerNames[PlayerId]}: {Notice[PlayerId].Item1} ({Notice[PlayerId].Item2 - now}s)", "Name Notify");
    }
}