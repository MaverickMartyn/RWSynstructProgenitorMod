using AlienRace;
using ArtificialBeings;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace SynstructProgenitorMod
{
    public class SynstructProgenitorMod : Mod
    {
        public SynstructProgenitorMod(ModContentPack content) : base(content)
        {
            Log.Message("Synstruct Progenitor Initialized");
        }

        public static bool IsBiological(Pawn pawn)
        {
            return !ABF_Utils.IsArtificial(pawn) && pawn.RaceProps.IsFlesh && !(pawn.def is ThingDef_AlienRace alienRace && !alienRace.alienRace.compatibility.IsFlesh);
        }

        public static bool IsArtificial(Pawn pawn)
        {
            return ABF_Utils.IsArtificial(pawn) || (pawn.def is ThingDef_AlienRace alienRace && !alienRace.alienRace.compatibility.IsFlesh);
        }

        internal static bool IsAnyArtificial(params Pawn[] pawns)
        {
            return pawns.Any(p => IsArtificial(p));
        }

        internal static bool IsAnyBiological(params Pawn[] pawns)
        {
            return pawns.Any(p => IsBiological(p));
        }
    }
}
