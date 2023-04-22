using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

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

    [HarmonyPatch(typeof(MentalBreaker))]
    [HarmonyPatch(nameof(MentalBreaker.CanDoRandomMentalBreaks), MethodType.Getter)]
    public static class MentalBreakerCanDoRandomMentalBreaksPatch
    {
        static bool Postfix(bool __result, Thing ___pawn)
        {
            if (___pawn.Map is null)
                return __result;
            
            var enemiesInMap = ___pawn.Map.mapPawns.AllPawnsSpawned
                .Any(p => p.RaceProps.Humanlike && p.Faction.HostileTo(Faction.OfPlayer));
            return !enemiesInMap && __result;
        }
    }
    
}