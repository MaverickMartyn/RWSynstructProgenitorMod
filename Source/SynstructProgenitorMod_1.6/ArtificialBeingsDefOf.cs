using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SynstructProgenitorMod
{
    [DefOf]
    public static class ArtificialBeingsDefOf
    {
        //public static ThoughtDef ABF_BiologicalSupremacyVsArtificial;

        //public static ThingCategoryDef ABF_ArtificialCorpseCategory;
        
        //[MayRequireIdeology]
        //public static HistoryEventDef ABF_ButcheredArtificial;

        [MayRequireIdeology]
        public static HistoryEventDef ABF_ArtificialDating_Dating;

        [MayRequireIdeology]
        public static HistoryEventDef ABF_ArtificialDating_BeginRomance;

        [MayRequireIdeology]
        public static HistoryEventDef ABF_ArtificialDating_SharedBed;

        [MayRequireIdeology]
        public static HistoryEventDef ABF_Artificial_SoldSlave;

        [MayRequireIdeology]
        public static IssueDef ABF_ArtificialSlavery;

        #region Vanilla

        //public static ThoughtDef ButcheredHumanlikeCorpse;
        //public static ThoughtDef KnowButcheredHumanlikeCorpse;

        public static NeedDef Mood;

        #endregion
    }
}
