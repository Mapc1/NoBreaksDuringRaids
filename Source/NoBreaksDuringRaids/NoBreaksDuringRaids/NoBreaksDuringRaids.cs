using System.Reflection;
using HarmonyLib;
using Verse;

namespace NoBreaksDuringRaids
{
    [StaticConstructorOnStartup]
    public static class NoBreaksDuringRaidsMod
    {
        static NoBreaksDuringRaidsMod()
        {
            var harmony = new Harmony("NoBreaksDuringRaidsMod.Patch");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(nameof(Pawn.InMentalState), MethodType.Getter)]
    public static class PawnInMentalStatePatch
    {
        static bool Postfix(bool __result, Pawn __instance)
        {
            // FIXME: Change this if to check only for raiders ant make it more readable
            if (__instance.Map is null || !__instance.RaceProps.Humanlike || __instance.Map.mapPawns.AllPawnsSpawned.FindAll(p => p.RaceProps.Humanlike).FindAll(p => !p.IsColonist).Count == 0) 
                return __result;
            
            if (__instance.MentalState != null)
            {
                __instance.MentalState.RecoverFromState();
                Log.Message($"{__instance.Name} has recovered from it's mental state");
            }
            return false;
        }
    }
}