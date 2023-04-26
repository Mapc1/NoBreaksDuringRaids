using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace NoBreaksDuringRaids
{
    public class NoBreaksDuringRaidsMod : Mod
    {
        public static NoBreaksDuringRaids_Settings Settings;

        public NoBreaksDuringRaidsMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("Mapc1.NoBreaksDuringRaidsMod");
            Settings = GetSettings<NoBreaksDuringRaids_Settings>();
            
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "NoBreaksDuringRaids".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard list = new();
            list.Begin(inRect);
            list.CheckboxLabeled("NBDR.EnableHome".Translate(), ref Settings.EnableHome);
            list.CheckboxLabeled("NBDR.PawnsNeverBreakOutside".Translate(), ref Settings.PawnsNeverBreakOutside);
            list.End();
            Settings.Write();
        }
    }

    
    public class NoBreaksDuringRaids_Settings : ModSettings
    {
        public bool EnableHome = true;
        public bool PawnsNeverBreakOutside = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnableHome, nameof(EnableHome), true);
            Scribe_Values.Look(ref PawnsNeverBreakOutside, nameof(PawnsNeverBreakOutside), false);
        }
    }
}