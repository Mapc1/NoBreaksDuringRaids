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

    [HarmonyPatch(typeof(MentalBreaker))]
    [HarmonyPatch(nameof(MentalBreaker.CanDoRandomMentalBreaks), MethodType.Getter)]
    public static class MentalBreaker_CanDoRandomMentalBreaks_Patch
    {
        static bool Postfix(bool __result, Pawn ___pawn)
        {
            if (!___pawn.Spawned || !___pawn.RaceProps.Humanlike)
                return __result;
            
            // If there are any pawns from an enemy faction on the same map then pawns cannot enter mental breaks
            // TODO: Still not sure if pawns should end their mental breaks when an enemy enters the map
            var factionsInMap = ___pawn.Map.mapPawns.AllPawnsSpawned
                    .FindAll(p => p.RaceProps.Humanlike)
                    .Select(p => p.Faction);
            var enemiesInMap = factionsInMap.Any(f => f.HostileTo(___pawn.Faction));
            
            return !enemiesInMap && __result;
        }
    }
    
}