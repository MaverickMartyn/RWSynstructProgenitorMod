using AlienRace;
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
    public class ThoughtWorker_Precept_BiologicalBeings : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            Lord lord = p.GetLord();
            if (lord != null)
                if (lord.ownedPawns.Any(c => SynstructProgenitorScenarioMod.IsBiological(c)))
                    return true;

            Caravan car = p.GetCaravan();
            if (car != null)
            {
                if (car.PawnsListForReading.Any(c => SynstructProgenitorScenarioMod.IsBiological(c)))
                    return true;
            }

            Map map = p.MapHeld;
            if (map != null)
            {
                Faction fac = p.Faction;
                if (fac != null)
                {
                    if (map.mapPawns.SpawnedPawnsInFaction(fac).Any(c => SynstructProgenitorScenarioMod.IsBiological(c)))
                        return true;
                }
                else if (map.mapPawns.AllPawnsSpawned.Any(c => SynstructProgenitorScenarioMod.IsBiological(c) && !p.HostileTo(c)))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ThoughtWorker_Precept_BiologicalBeings_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn) =>
            SynstructProgenitorScenarioMod.IsBiological(otherPawn);
    }

    public class ThoughtWorker_Precept_ArtificialBeings : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            Lord lord = p.GetLord();
            if (lord != null)
                if (lord.ownedPawns.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c)))
                    return true;

            Caravan car = p.GetCaravan();
            if (car != null)
            {
                if (car.PawnsListForReading.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c)))
                    return true;
            }

            Map map = p.MapHeld;
            if (map != null)
            {
                Faction fac = p.Faction;
                if (fac != null)
                {
                    if (map.mapPawns.SpawnedPawnsInFaction(fac).Any(c => SynstructProgenitorScenarioMod.IsArtificial(c)))
                        return true;
                }
                else if (map.mapPawns.AllPawnsSpawned.Any(c => SynstructProgenitorScenarioMod.IsArtificial(c) && !p.HostileTo(c)))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ThoughtWorker_Precept_ArtificialBeings_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn) =>
            SynstructProgenitorScenarioMod.IsArtificial(otherPawn);
    }
}
