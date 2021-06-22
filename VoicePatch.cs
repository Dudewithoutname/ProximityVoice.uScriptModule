using HarmonyLib;
using SDG.Unturned;

namespace ProximityVoice
{
    [HarmonyPatch(typeof(PlayerVoice), "handleRelayVoiceCulling_Proximity")]
    internal class VoicePatch
    {
        public static Handle onHandle;

        [HarmonyPrefix]
        private static bool Handler(PlayerVoice speaker, PlayerVoice listener) => onHandle.Invoke(speaker, listener);

        public delegate bool Handle(PlayerVoice speaker, PlayerVoice listener);
    }
}
