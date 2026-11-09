using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SynstructProgenitorScenario
{
    public class ExtendedFactionUtility
    {
        public static int GetDisapprovedSlavesInFactionCount(Pawn pawn)
        {
            // TODO: Consider also handling HAR's alien races, since HAR doesn't patch the vanilla FactionUtility.GetDisapprovedSlavesInFactionCount method.
            // Alternatively, report this to HAR and see if they can patch it themselves, to avoid having to maintain a separate patch here.
            if (pawn == null)
            {
                return 0;
            }

            // If the pawn's ideoligion has a precept for artificial or biological slavery, do not count that slave type. They are handled by relevant precept instead.
            var hasBiologicalPrecept = false;
            var hasArtificialPrecept = false;
            foreach (var precept in pawn.Ideo?.PreceptsListForReading)
            {
                if (precept.def.issue == BiologicalBeingsDefOf.ABF_BiologicalSlavery)
                {
                    hasBiologicalPrecept = true;
                }
                if (precept.def.issue == ArtificialBeingsDefOf.ABF_ArtificialSlavery)
                {
                    hasArtificialPrecept = true;
                }
                if (hasBiologicalPrecept && hasArtificialPrecept)
                {
                    break;
                }
            }
            //if (hasArtificialPrecept || hasBiologicalPrecept)
            //{
            //    Log.Message($"SP: ABF slave precepts found for {pawn.Name}. Has Biological Precept: {hasBiologicalPrecept}, Has Artificial Precept: {hasArtificialPrecept}");
            //}
            //else
            //{
            //    Log.Message($"SP: No ABF slave precepts found for {pawn.Name}.");
            //}

            if (pawn.Faction == null)
            {
                return 0;
            }
            int num = 0;
            int ignoredArtificialSlaves = 0;
            int ignoredBiologicalSlaves = 0;
            foreach (Pawn item in PawnsFinder.AllMaps_SpawnedPawnsInFaction(pawn.Faction))
            {
                if (item.IsSlave)
                {
                    if (hasArtificialPrecept && SynstructProgenitorScenarioMod.IsArtificial(item))
                    {
                        ignoredArtificialSlaves++;
                        continue;
                    }
                    else if (hasBiologicalPrecept && SynstructProgenitorScenarioMod.IsBiological(item))
                    {
                        ignoredBiologicalSlaves++;
                        continue;
                    }
                    num++;
                }
            }
            //if (ignoredArtificialSlaves > 0 || ignoredBiologicalSlaves > 0)
            //{
            //    Log.Message($"SP: Ignored slaves found when checking thought for {pawn.Name}. Ignored Artificial Slaves: {ignoredArtificialSlaves}, Ignored Biological Slaves: {ignoredBiologicalSlaves}");
            //}
            //else
            //{
            //    Log.Message($"SP: No ignored slaves found when checking thought for {pawn.Name}.");
            //}
            return num;
        }
    }
}
