using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SynstructProgenitorScenario
{
    [DefOf]
    public static class BiologicalBeingsDefOf
    {
        //public static ThoughtDef ABF_ArtificialSupremacyVsBiological;

        //public static ThingCategoryDef ABF_BiologicalCorpseCategory;

        //[MayRequireIdeology]
        //public static HistoryEventDef ABF_ButcheredBiological;

        [MayRequireIdeology]
        public static HistoryEventDef ABF_BiologicalDating_Dating;

        [MayRequireIdeology]
        public static HistoryEventDef ABF_BiologicalDating_BeginRomance;

        [MayRequireIdeology]
        public static HistoryEventDef ABF_BiologicalDating_SharedBed;

        [MayRequireIdeology]
        public static HistoryEventDef ABF_Biological_SoldSlave;

        [MayRequireIdeology]
        public static IssueDef ABF_BiologicalSlavery;

        #region Vanilla
        //public static ThoughtDef ButcheredHumanlikeCorpse;
        //public static ThoughtDef KnowButcheredHumanlikeCorpse;

        public static NeedDef Mood;

        #endregion
    }
}
