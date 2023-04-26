using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace NoBreaksDuringRaids
{
    [HarmonyPatch(typeof(MentalBreaker))]
    [HarmonyPatch(nameof(MentalBreaker.CanDoRandomMentalBreaks), MethodType.Getter)]
    public static class MentalBreaker_CanDoRandomMentalBreaks_Patch
    {
        public static bool Postfix(bool __result, Pawn ___pawn)
        {
            if (!___pawn.Spawned || !___pawn.RaceProps.Humanlike || ___pawn.Downed)
                return __result;
            
            if (!NoBreaksDuringRaidsMod.Settings.EnemyPawnsAffected && !___pawn.Faction.IsPlayer)
                return __result;
            
            // If the ParentFaction is null then we are at a neutral map tile so we are not home
            bool inHomeTile = ___pawn.Map.ParentFaction?.IsPlayer ?? false;
            
            // If the mod is disabled in player settlements and the pawn is at a player settlement skip this function
            if (!NoBreaksDuringRaidsMod.Settings.EnableHome && inHomeTile)
                return __result;
            // If the PawnsNeverBreakOutside is enabled and the pawn is outside a home settlement then they cannot break
            if (NoBreaksDuringRaidsMod.Settings.PawnsNeverBreakOutside && !inHomeTile)
                return false;
            
            // If there are any pawns from an enemy faction on the same map then pawns cannot enter mental breaks
            // TODO: Still not sure if pawns should end their mental breaks when an enemy enters the map
            var factionsInMap = ___pawn.Map.mapPawns.AllPawnsSpawned
                    .FindAll(p => p.RaceProps.Humanlike && !p.Downed)
                    .Select(p => p.Faction);
            var enemiesInMap = factionsInMap.Any(f => f.HostileTo(___pawn.Faction));
            
            return !enemiesInMap && __result;
        }
    }
}