using System;
using System.Collections.Generic;
using uScript.API.Attributes;
using uScript.Unturned;
using Steamworks;
using SDG.Unturned;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace ProximityVoice
{
    [ScriptModule("proximityVoice")]
    public class Voice : ScriptModuleBase
    {
        [ScriptProperty("defaultRange")]
        public static uint DefaultRange { get; set; }

        private static Dictionary<CSteamID, uint> voicePlayers;
        private static string logPrefix = "uScript.Module.ProximityVoice";
        private Harmony harmony;

        protected override void OnModuleLoaded()
        {
            harmony = new Harmony("ProximityVoice");
            harmony.PatchAll();

            DefaultRange = 50;
            voicePlayers = new Dictionary<CSteamID, uint>();

            Provider.onEnemyConnected += onPlayerConnected;
            Provider.onEnemyDisconnected += onPlayerDisconnected;
            VoicePatch.onHandle += onHandleVoice;

            CommandWindow.Log($"[{logPrefix}] | INFO >> Loaded");
            CommandWindow.Log($"[{logPrefix}] | INFO >> Author Dudewithoutname#3129");
            CommandWindow.Log($"[{logPrefix}] | INFO >> Default Range is {DefaultRange}");
            CommandWindow.Log($"[{logPrefix}] | INFO >> Version {Assembly.GetExecutingAssembly().GetName().Version}");
        }

        [ScriptFunction("changeVoiceDistance")]
        public static void ChangeVoiceDistance(string playerId, uint radius)
        {
            var steamId = new CSteamID(Convert.ToUInt64(playerId));

            if (voicePlayers.ContainsKey(steamId))
                voicePlayers[steamId] = radius;
            else
                CommandWindow.LogError($"[{logPrefix}] | ERROR({nameof(ChangeVoiceDistance)}) >> Couldn't find player with SteamID {steamId}");
        }

        [ScriptFunction("getPlayerVoiceDistance")]
        public static uint GetPlayerVoiceDistance(string playerId)
        {
            var steamId = new CSteamID(Convert.ToUInt64(playerId));

            if (voicePlayers.ContainsKey(steamId))
                return voicePlayers[steamId];
            else
                CommandWindow.LogError($"[{logPrefix}] | ERROR({nameof(GetPlayerVoiceDistance)}) >> Couldn't find player with SteamID {steamId}");

            return 0;
        }


        #region Event Handling

        // doing inline methods is fun 
        private bool onHandleVoice(PlayerVoice speaker, PlayerVoice listener) => (Vector3.Distance(speaker.player.gameObject.transform.position, listener.player.gameObject.transform.position) <= voicePlayers[speaker.channel.owner.playerID.steamID]) ? true : false;

        private void onPlayerConnected(SteamPlayer player) 
        {
            var steamId = player.playerID.steamID;

            if (!voicePlayers.ContainsKey(steamId))
                voicePlayers.Add(steamId, DefaultRange);
            else
                CommandWindow.LogError($"[{logPrefix}] | ERROR({nameof(onPlayerConnected)}) >>  Player {steamId} is already in collection");
        }

        private void onPlayerDisconnected(SteamPlayer player)
        {
            var steamId = player.playerID.steamID;

            if (voicePlayers.ContainsKey(steamId)) 
                voicePlayers.Remove(steamId);
            else
                CommandWindow.LogError($"[{logPrefix}] | ERROR({nameof(onPlayerConnected)}) >> Couldn't find player with SteamID {steamId}");
        }

        #endregion
    }
}
