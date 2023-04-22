using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
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
            // If pawn is not spawned in the map we don't do anything
            if (__instance.Map is null)
                return __result;
            
            var enemiesInMap = __instance.Map.mapPawns.AllPawnsSpawned
                .Any(p => p.RaceProps.Humanlike && p.Faction.HostileTo(Faction.OfPlayer));
            if (!enemiesInMap)
                return __result;
            
            // If the pawn is in a mental break snap them out of it
            __instance.MentalState?.RecoverFromState();
            return false;
        }
    }
}