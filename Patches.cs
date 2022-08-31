using HarmonyLib;
using UnityEngine;


namespace BlazeDiscordRPC {
    public static class Patches {
        public static class AMKONCFCDOD_Patches {
            [HarmonyPatch(typeof(AMKONCFCDOD), "HMJDLJKCEPK")]
            [HarmonyPrefix]

            static void GetLeaderboardID_Patch(string __0) {
                //if (__0 != "cheaters") SeasonFetcher.season.LeaderboardName = __0;
            }
        }
    }
}
