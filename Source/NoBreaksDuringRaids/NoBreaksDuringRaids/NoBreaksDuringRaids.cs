using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace NoBreaksDuringRaids
{
    public class NoBreaksDuringRaidsMod : Mod
    {
        public NoBreaksDuringRaidsMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("Mapc1.NoBreaksDuringRaidsMod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}